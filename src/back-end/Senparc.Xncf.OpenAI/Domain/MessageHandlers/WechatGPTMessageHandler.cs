using Microsoft.Extensions.DependencyInjection;
using Senparc.CO2NET.Cache;
using Senparc.NeuChar.App.AppStore;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Entities.Request;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageContexts;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Xncf.OpenAI.Domain.Models.CacheModels;
using Senparc.Xncf.OpenAI.Domain.Services;
using Senparc.Xncf.WeixinManager;
using System;
using System.IO;
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

        public WechatGPTMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0, bool onlyAllowEncryptMessage = false, DeveloperInfo developerInfo = null, IServiceProvider serviceProvider = null) : base(inputStream, postModel, maxRecordCount, onlyAllowEncryptMessage, developerInfo, serviceProvider)
        {
            _openAiService = serviceProvider.GetService<OpenAiService>();
            _cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
        }

        public override async Task<IResponseMessageBase> OnTextRequestAsync(RequestMessageText requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            var messageContext = await base.GetCurrentMessageContext();

            //正式对话
            var cacheKey = ChatGPTMessages.GetCacheKey(OpenId);
            ChatGPTMessages messages = await _cache.GetAsync<ChatGPTMessages>(cacheKey);
            messages ??= new ChatGPTMessages(OpenId);

            var requestHandler = await requestMessage.StartHandler()
                .Keyword("s", () =>
                {
                    var result = Task.Factory.StartNew(async () =>
                    {
                        messageContext.StorageData = "Chatting";
                        GlobalMessageContext.UpdateMessageContext(messageContext);//储存到缓存

                        //清除上下文
                        messages.CleanMessage();
                        await _cache.SetAsync(cacheKey, messages, TimeSpan.FromHours(1));
                        responseMessage.Content = "ChatGPT 准备就绪，请开始对话！";
                        return responseMessage;
                    });
                    return result.GetAwaiter().GetResult() as IResponseMessageBase;
                })
                .Keyword("q", () =>
                {
                    messageContext.StorageData = null;
                    GlobalMessageContext.UpdateMessageContext(messageContext);//储存到缓存
                    responseMessage.Content = "ChatGPT 对话状态已退出，后续聊天不会消耗额度。";
                    return responseMessage;
                })
                .Default(async () =>
                {
                    if (messageContext.StorageData as string == "Chatting")
                    {
                        var result = await _openAiService.GetChatGPTResultAsync(OpenId, requestMessage.Content, false, 200);
                        responseMessage.Content = result;
                        return responseMessage;
                    }
                    else
                    {
                        responseMessage.Content = DEFAULT_MESSAGE;
                        return responseMessage;
                    }
                });
            return requestHandler.GetResponseMessage() as IResponseMessageBase;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var response = this.CreateResponseMessage<ResponseMessageText>();
            response.Content = DEFAULT_MESSAGE;
            return response;
        }
    }
}
