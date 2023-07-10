﻿using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Senparc.AI.Interfaces;
using Senparc.AI.Kernel;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels.ResponseModels;
using Senparc.CO2NET.Cache;
using Senparc.CO2NET.Extensions;
using Senparc.Ncf.Core.Exceptions;
using Senparc.Ncf.Repository;
using Senparc.Ncf.Service;
using Senparc.Xncf.OpenAI.Domain.Models.CacheModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senparc.AI.Kernel.Helpers;
using Senparc.AI;
using Senparc.AI.Entities;
using Senparc.AI.Kernel.Handlers;

namespace Senparc.Xncf.OpenAI.Domain.Services
{
    public class OpenAiService : ServiceBase<OpenAiConfig>
    {
        SemanticAiHandler _semanticAiHandler;

        private OpenAIService _openAiService;

        private readonly IBaseObjectCacheStrategy _cache;

        public OpenAiService(IRepositoryBase<OpenAiConfig> repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
            _cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
        }

        /// <summary>
        /// 获取 SemanticAiHandler 对象
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NcfExceptionBase"></exception>
        public async Task<SemanticAiHandler> GetSemanticAiHandlerAsync()
        {
            if (_semanticAiHandler == null)
            {
                var config = await base.GetObjectAsync(z => true, z => z.Id, Ncf.Core.Enums.OrderingType.Descending);
                if (config == null || config.ApiKey.IsNullOrEmpty())
                {
                    throw new NcfExceptionBase("请先设置 API Key！");
                }

                SenparcAiSetting aiSetting = new SenparcAiSetting();
                aiSetting.AiPlatform = AI.AiPlatform.OpenAI;
                aiSetting.OpenAIKeys = new OpenAIKeys
                {
                    ApiKey = config.GetOriginalAppKey(),
                    OrgaizationId = config.OrganizationID
                };

                _semanticAiHandler = new SemanticAiHandler(new SemanticKernelHelper(aiSetting));
            }
            return _semanticAiHandler;
        }

        /// <summary>
        /// 获取 OpenAIService 对象
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NcfExceptionBase"></exception>
        public async Task<OpenAIService> GetOpenAiServiceAsync()
        {
            if (_openAiService == null)
            {
                var config = await base.GetObjectAsync(z => true, z => z.Id, Ncf.Core.Enums.OrderingType.Descending);
                if (config == null || config.ApiKey.IsNullOrEmpty())
                {
                    throw new NcfExceptionBase("请先设置 API Key！");
                }

                _openAiService = new OpenAIService(new OpenAiOptions()
                {
                    ApiKey = config.GetOriginalAppKey(),
                    Organization = config.OrganizationID?.Trim().Length == 0 ? null : config.OrganizationID
                });


            }
            return _openAiService;
        }

        /// <summary>
        /// 获取 ChatGPT 结果
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="prompt"></param>
        /// <param name="startNewConversation"></param>
        /// <param name="maxTokens"></param>
        /// <returns></returns>
        public async Task<string> GetChatGPTResultAsync(string userId, string prompt, bool startNewConversation = false, int maxTokens = 50)
        {
            var cacheKey = ChatHistory.GetCacheKey(userId);
            ChatHistory history = await _cache.GetAsync<ChatHistory>(cacheKey);
            history ??= new ChatHistory(userId);

            var promptParameter = new PromptConfigParameter()
            {
                MaxTokens = maxTokens,
                Temperature = 0.7,
                TopP = 0.5,
            };

            var semanticAiHandler = await GetSemanticAiHandlerAsync();
            var iWantToRun = semanticAiHandler
                .IWantTo()
                .ConfigModel(ConfigModel.TextCompletion, userId, "text-davinci-003")
                .BuildKernel()
                .RegisterSemanticFunction("ChatBot","Chat", promptParameter)
                .iWantToRun;

            var chatRequest = iWantToRun.CreateRequest(true);

            chatRequest.SetStoredContext("history",history.Content);
            chatRequest.SetStoredContext("human_input", prompt);

            string finalMessage;
            try
            {
                var completionResult = await iWantToRun.RunAsync(chatRequest);
                if (completionResult.LastException == null)
                {
                    finalMessage = completionResult.Output;
                    history.AppendHistory($"\nHuman: {prompt}\nBot: {finalMessage}");
                    await _cache.SetAsync(cacheKey, history, TimeSpan.FromHours(1));
                }
                else
                {
                    finalMessage = $"OpenAI GPT-3 服务出错：{completionResult.LastException.Message}";
                }
            }
            catch (Exception ex)
            {
                finalMessage = $"OpenAI GPT-3 服务出错：{ex.Message}";
            }
            finally
            {

            }
            return finalMessage;
        }

        /// <summary>
        /// 清空消息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task CleanChatGPT(string userId)
        {
            var cacheKey = ChatGPTMessages.GetCacheKey(userId);
            ChatGPTMessages messages = await _cache.GetAsync<ChatGPTMessages>(cacheKey);
            if (messages != null)
            {
                await _cache.RemoveFromCacheAsync(cacheKey);
            }
        }

        /// <summary>
        /// 清除上一条消息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task CleanLastChatGPT(string userId, int chatNumber = 1)
        {
            var cacheKey = ChatGPTMessages.GetCacheKey(userId);
            ChatGPTMessages messages = await _cache.GetAsync<ChatGPTMessages>(cacheKey);
            if (messages != null)
            {
                if (chatNumber == 0)
                {
                    messages.CleanMessage();
                }
                else
                {
                    for (int i = 0; i < chatNumber; i++)
                    {
                        //清除一次对话（两次）
                        if (messages.Messages.Count >= 1)
                        {
                            //删除上一条系统回复
                            messages.Messages.RemoveAt(messages.Messages.Count - 1);
                        }
                        if (messages.Messages.Count >= 1)
                        {
                            //删除上一条用户对话
                            messages.Messages.RemoveAt(messages.Messages.Count - 1);
                        }
                    }
                }

                await _cache.SetAsync(cacheKey, messages, TimeSpan.FromHours(1));
            }
        }

        /// <summary>
        /// 使用 Dall·E 接口绘图
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="user"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<(List<Stream> streamList, string errorMessage)> GetDallEResult(string prompt, string user, string size = "512x512")
        {
            var openAiService = await this.GetOpenAiServiceAsync();
            
            var imageResult = await _openAiService.Image.CreateImage(new ImageCreateRequest
            {
                Prompt = prompt,
                N = 2,
                Size = size,
                ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
                User = user
            });

            List<Stream> streamResult = new List<Stream>();
            string errorMessage = String.Empty;
            if (imageResult.Successful)
            {
                var urls = imageResult.Results.Select(r => r.Url);
                foreach (var url in urls)
                {
                    try
                    {
                        var memoryStream = new MemoryStream();

                        await Senparc.CO2NET.HttpUtility.Get.DownloadAsync(base._serviceProvider, url, memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        streamResult.Add(memoryStream);
                    }
                    catch
                    {
                        //TODO:显示错误信息
                    }
                }
            }
            else
            {
                if (imageResult.Error == null)
                {
                    //TODO: OpenApi Exception
                    new NcfExceptionBase("OpenAI Error: Unknown Error");
                }
                else
                {
                    errorMessage = $"{imageResult.Error.Code} {imageResult.Error.Message}";
                    new NcfExceptionBase($"OpenAI Error: {errorMessage}");
                }
            }
            return (streamResult, errorMessage);
        }
    }
}
