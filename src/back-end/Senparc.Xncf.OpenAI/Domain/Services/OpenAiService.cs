using Microsoft.SemanticKernel.AI.ImageGeneration;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using Senparc.AI;
using Senparc.AI.Entities;
using Senparc.AI.Kernel;
using Senparc.AI.Kernel.Handlers;
using Senparc.AI.Kernel.Helpers;
using Senparc.CO2NET.Cache;
using Senparc.CO2NET.Extensions;
using Senparc.Ncf.Core.Exceptions;
using Senparc.Ncf.Repository;
using Senparc.Ncf.Service;
using Senparc.Xncf.OpenAI.Domain.Models.CacheModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
            var cacheKey = ChatHistory.GetCacheKey(userId);
            ChatHistory history = await _cache.GetAsync<ChatHistory>(cacheKey);
            if (history != null)
            {
                await _cache.RemoveFromCacheAsync(cacheKey);
            }
        }

        /// <summary>
        /// 使用 Dall·E 接口绘图
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="userId"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="imageCount"></param>
        /// <param name="user"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<(List<Stream> streamList, string errorMessage)> GetDallEResult(string prompt, string userId, int width = 512, int height = 512, int imageCount = 1)
        {
            var semanticAiHandler = await GetSemanticAiHandlerAsync();
            var iWantTo = semanticAiHandler
                .IWantTo()
                .ConfigModel(ConfigModel.ImageGeneration, userId, "image-generation")
                .BuildKernel();

            var dallE = iWantTo.GetService<IImageGeneration>();
            List<Stream> streamList = new List<Stream>();
            string errorMessage = String.Empty;

            for (int i = 0; i < imageCount; i++)
            {
                try
                {
                    var memoryStream = new MemoryStream();
                    var url = await dallE.GenerateImageAsync(prompt, width, height);

                    await Senparc.CO2NET.HttpUtility.Get.DownloadAsync(base._serviceProvider, url, memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    streamList.Add(memoryStream);
                }
                catch (Exception ex)
                {
                    errorMessage = $"OpenAI Error: {ex.Message}";
                    new NcfExceptionBase($"OpenAI Error: {errorMessage}");
                }
            }
            return (streamList, errorMessage);
        }
    }
}
