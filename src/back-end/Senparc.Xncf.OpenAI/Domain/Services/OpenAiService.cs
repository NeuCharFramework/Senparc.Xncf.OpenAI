using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.Xncf.OpenAI.Domain.Services
{
    public class OpenAiService : ServiceBase<OpenAiConfig>
    {
        private OpenAIService _openAiService;

        private readonly IBaseObjectCacheStrategy _cache;

        public OpenAiService(IRepositoryBase<OpenAiConfig> repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
            _cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
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

            var cacheKey = ChatGPTMessages.GetCacheKey(userId);
            ChatGPTMessages messages = await _cache.GetAsync<ChatGPTMessages>(cacheKey);
            messages ??= new ChatGPTMessages(userId);

            var chatRequest = new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>(),
                Model = global::OpenAI.GPT3.ObjectModels.Models.ChatGpt3_5Turbo,
                MaxTokens = 200//optional
            };

            foreach (var msg in messages.Messages)
            {
                chatRequest.Messages.Add(msg);
            }

            chatRequest.Messages.Add(ChatMessage.FromUser(prompt));

            var openAiService = await this.GetOpenAiServiceAsync();
            var completionResult = await openAiService.ChatCompletion.CreateCompletion(chatRequest);

            string finalMessage = null;

            if (completionResult.Successful)
            {
                finalMessage = completionResult.Choices.First().Message.Content;
                messages.Messages.Add(ChatMessage.FromAssistance(finalMessage));
                await _cache.SetAsync(cacheKey, messages, TimeSpan.FromHours(1));
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
        public async Task CleanLastChatGPT(string userId)
        {
            var cacheKey = ChatGPTMessages.GetCacheKey(userId);
            ChatGPTMessages messages = await _cache.GetAsync<ChatGPTMessages>(cacheKey);
            if (messages != null)
            {
                //清除一次对话（两次）
                if (messages.Messages.Count >= 1)
                {
                    messages.Messages.RemoveAt(messages.Messages.Count - 1);
                }
                if (messages.Messages.Count >= 1)
                {
                    messages.Messages.RemoveAt(messages.Messages.Count - 1);
                }
                
                await _cache.SetAsync(cacheKey, messages, TimeSpan.FromHours(1));
            }
        }
    }
}
