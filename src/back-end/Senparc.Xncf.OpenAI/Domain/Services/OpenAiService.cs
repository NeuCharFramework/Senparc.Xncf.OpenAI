using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels.ResponseModels;
using Senparc.CO2NET.Extensions;
using Senparc.Ncf.Core.Exceptions;
using Senparc.Ncf.Repository;
using Senparc.Ncf.Service;
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

        public OpenAiService(IRepositoryBase<OpenAiConfig> repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
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
        /// <param name="fromSystem"></param>
        /// <param name="fromUser"></param>
        /// <param name="maxTokens"></param>
        /// <returns></returns>
        public async Task<ChatCompletionCreateResponse> GetChatGPTResultAsync(string fromSystem, string fromUser, int maxTokens = 50)
        {
            var openAiService = await this.GetOpenAiServiceAsync();

            var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
                            {
                                ChatMessage.FromSystem("You are a helpful assistant."),
                                ChatMessage.FromUser("Who won the world series in 2020?"),
                                ChatMessage.FromAssistance("The Los Angeles Dodgers won the World Series in 2020."),
                                ChatMessage.FromUser("Where was it played?")
                            },
                Model = global::OpenAI.GPT3.ObjectModels.Models.ChatGpt3_5Turbo,
                MaxTokens = 50//optional
            });

            if (completionResult.Successful)
            {
                Console.WriteLine(completionResult.Choices.First().Message.Content);
            }

            return completionResult;
        }
    }
}
