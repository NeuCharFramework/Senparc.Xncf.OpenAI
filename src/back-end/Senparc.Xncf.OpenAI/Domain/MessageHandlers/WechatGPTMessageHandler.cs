using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Senparc.CO2NET.Cache;
using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.MessageQueue;
using Senparc.Ncf.Core.Exceptions;
using Senparc.NeuChar.App.AppStore;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Entities.Request;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageContexts;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Xncf.OpenAI.Domain.Models.CacheModels;
using Senparc.Xncf.OpenAI.Domain.Services;
using Senparc.Xncf.WeixinManager;
using Senparc.Xncf.WeixinManager.Models;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Senparc.Xncf.OpenAI.Domain.MessageHandlers
{
    [MpMessageHandler("WechatGPTMessageHandler")]
    public class WechatGPTMessageHandler : MessageHandler<DefaultMpMessageContext>
    {
        const string DEFAULT_MESSAGE = @"欢迎使用 Senparc.Weixin SDK！
===命令===
请输入命令进行互动：
s - 清空上下文，开始聊天
q - 退出聊天

===开源地址===
Xncf.OpenAI模块：https://github.com/NeuCharFramework/Senparc.Xncf.OpenAI
微信SDK：https://github.com/JeffreySu/WeiXinMPSDK";

        private readonly OpenAiService _openAiService;
        private readonly IBaseObjectCacheStrategy _cache;
        private readonly MpAccountDto _mpAccountDto;
        private readonly string _appId;

        public WechatGPTMessageHandler(MpAccountDto mpAccountDto, Stream inputStream, PostModel postModel, int maxRecordCount, IServiceProvider serviceProvider) : base(inputStream, postModel, maxRecordCount, serviceProvider: serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            _openAiService = scope.ServiceProvider.GetService<OpenAiService>();

            _cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();

            _mpAccountDto = mpAccountDto;
            _appId = mpAccountDto.AppId;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var response = this.CreateResponseMessage<ResponseMessageText>();
            response.Content = DEFAULT_MESSAGE;
            return response;
        }

        public override async Task<IResponseMessageBase> OnTextRequestAsync(RequestMessageText requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            var messageContext = await base.GetCurrentMessageContext();

            //正式对话
            var cacheKey = ChatHistory.GetCacheKey(OpenId);
            ChatHistory history = await _cache.GetAsync<ChatHistory>(cacheKey);
            history ??= new ChatHistory(OpenId);

            var requestHandler = await requestMessage.StartHandler()
                .Keyword("s", () =>
                {
                    var result = Task.Factory.StartNew(async () =>
                    {
                        messageContext.StorageData = "Chatting";
                        GlobalMessageContext.UpdateMessageContext(messageContext);//储存到缓存

                        //清除上下文
                        history.CleanHistory();
                        await _cache.SetAsync(cacheKey, history, TimeSpan.FromHours(1));
                        responseMessage.Content = "ChatGPT 准备就绪，请开始对话！";
                        return responseMessage;
                    });
                    return result.Unwrap().GetAwaiter().GetResult() as IResponseMessageBase;
                })
                .Keyword("q", () =>
                {
                    messageContext.StorageData = null;
                    GlobalMessageContext.UpdateMessageContext(messageContext);//储存到缓存
                    responseMessage.Content = "ChatGPT 对话状态已退出，后续聊天不会消耗额度。";
                    return responseMessage;
                })
                .Regex(@"(?<=[img|画图|绘图]\s)(?<prompt>([\.\s\S\w\W]+))", () =>
                {
                    //使用队列处理
                    var smq = new SenparcMessageQueue();
                    var smqKey = $"ChatGPT-{OpenId}-{SystemTime.NowTicks}";
                    smq.Add(smqKey, async () =>
                    {
                        var keyword = Regex.Match(requestMessage.Content, @"(?<=[img|画图|绘图]\s)(?<prompt>([\.\s\S\w\W]+))").Groups["prompt"].Value;

                        _ = CustomApi.SendTextAsync(_appId, OpenId, $"已收到请求，请等待约 1 分钟，期间您可以正常进行其他对话，不会打乱我的思路 :)");

                        var streamResult = await _openAiService.GetDallEResult(keyword, OpenId, 512, 512 , 1);
                        var streamList = streamResult.streamList;
                        //TODO:拼接图片

                        if (streamList.Count == 0)
                        {
                            await CustomApi.SendTextAsync(_appId, OpenId, $"发生错误，图片生成失败：{streamResult.errorMessage}");
                        }
                        else
                        {
                            for (int i = 0; i < streamList.Count; i++)
                            {
                                _ = CustomApi.SendTextAsync(_appId, OpenId, $"正在生成第 {i + 1}/{streamList.Count} 张图片，请等待...");

                                try
                                {
                                    //缓存在本地
                                    var fileDir = Senparc.CO2NET.Utilities.ServerUtility.ContentRootMapPath($"~/App_Data/OpenAiImageTemp");
                                    //确认文件夹存在
                                    Senparc.CO2NET.Helpers.FileHelper.TryCreateDirectory(fileDir);

                                    var file = Path.Combine(fileDir, $"{SystemTime.Now.ToString("yyyyMMdd-HH-mm-ss")}_{OpenId}_{SystemTime.NowTicks}.jpg");

                                    using (var fs = new FileStream(file, FileMode.OpenOrCreate))
                                    {
                                        var ms = streamList[i];
                                        await ms.CopyToAsync(fs);
                                        await fs.FlushAsync();
                                    }

                                    //创建一个副本，防止被占用
                                    var newFile = file + ".jpg";
                                    File.Copy(file, newFile);

                                    //上传到微信素材库
                                    var uploadResult = await MediaApi.UploadTemporaryMediaAsync(_appId, Weixin.MP.UploadMediaFileType.image, newFile);

                                    await CustomApi.SendImageAsync(_appId, OpenId, uploadResult.media_id);

                                    //删除临时文件
                                    File.Delete(newFile);
                                }
                                catch (Exception ex)
                                {
                                    new NcfExceptionBase(ex.Message, ex);
                                    await CustomApi.SendTextAsync(_appId, OpenId, $"第 {i + 1}/{streamList.Count} 张图片生成失败！");
                                }

                            }
                            _ = CustomApi.SendTextAsync(_appId, OpenId, $"生成完毕");
                        }
                    });

                    return base.CreateResponseMessage<ResponseMessageNoResponse>();

                })
                .Default(async () =>
                {
                    if (messageContext.StorageData as string == "Chatting")
                    {
                        //使用消息队列处理
                        var smq = new SenparcMessageQueue();
                        var smqKey = $"ChatGPT-{OpenId}-{SystemTime.NowTicks}";
                        smq.Add(smqKey, async () =>
                        {
                            var result = await _openAiService.GetChatGPTResultAsync(OpenId, requestMessage.Content, false, 500);

                            if (result.IsNullOrEmpty())
                            {
                                result = "未返回有效信息，请重试。";
                            }

                            //使用客服消息
                            await Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendTextAsync(_appId, OpenId, result.Trim());
                        });

                        return base.CreateResponseMessage<ResponseMessageNoResponse>();
                    }
                    else
                    {
                        responseMessage.Content = DEFAULT_MESSAGE;
                        return responseMessage;
                    }
                });
            return requestHandler.GetResponseMessage() as IResponseMessageBase;
        }

        /// <summary>
        /// 语音信息
        /// <para>提示：需要启用微信公众号后台“接收语音识别结果”功能</para>
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnVoiceRequestAsync(RequestMessageVoice requestMessage)
        {
            var text = requestMessage.Recognition;
            if (text.IsNullOrEmpty())
            {
                var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content = "没有接收到语音识别结果，请重试。";
                return responseMessage;
            }
            else
            {
                var textRequest = new RequestMessageText()
                {
                    Content = text,
                    FromUserName = requestMessage.FromUserName,
                    ToUserName = requestMessage.ToUserName,
                    MsgId = requestMessage.MsgId + 1,
                    CreateTime = requestMessage.CreateTime,
                };

                //判断是否存在特殊内容
                textRequest.Content = text switch
                {
                    var t when t == "启动机器人。" || t == "机器人启动。" => "S",
                    var t when t == "关闭机器人。" || t == "机器人关闭。" => "Q",
                    var t when t.StartsWith("绘制图片") || t.EndsWith("绘制图片。") || t.StartsWith("生成图片") || t.EndsWith("生成图片。") => $"img {text}",
                    _ => text
                };

                return await OnTextRequestAsync(textRequest);
            }

        }

    }
}
