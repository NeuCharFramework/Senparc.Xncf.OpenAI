﻿using Senparc.CO2NET;
using Senparc.Ncf.Core.AppServices;
using Senparc.Xncf.OpenAI.Domain.Services;
using System;
using System.Threading.Tasks;

namespace Senparc.Xncf.OpenAI.OHS.Local.AppService
{
    //[BackendJwtAuthorize]
    [ApiBind]
    public class GPT3AppService : AppServiceBase
    {
        readonly OpenAiService _openAiService;

        public GPT3AppService(IServiceProvider serviceProvider, OpenAiService openAiService) : base(serviceProvider)
        {
            _openAiService = openAiService;
        }
        /**
        /// <summary>
        /// 使用不同模型运行 OpenAI
        /// </summary>
        /// <param name="prompt">prompt 提示信息</param>
        /// <param name="model">选用模型，如果留空则默认使用 text-davinci-v3</param>
        /// <param name="maxTokens">最大消费 Token 数量。默认为 50</param>
        /// <returns></returns>
        /// <exception cref="NcfExceptionBase"></exception>
        [ApiBind(ApiRequestMethod = CO2NET.WebApi.ApiRequestMethod.Post)]
        public async Task<AppResponseBase<List<ChoiceResponse>>> CreateCompletionAsync(string prompt, string model = null, int maxTokens = 50)
        {
            return await this.GetResponseAsync<AppResponseBase<List<ChoiceResponse>>, List<ChoiceResponse>>(async (response, logger) =>
            {
                var dt1 = SystemTime.Now;//开始计时

                var openAIService = await _openAiService.GetOpenAiServiceAsync();

                if (model.IsNullOrEmpty())
                {
                    model = global::OpenAI.GPT3.ObjectModels.Models.CodeDavinciV2;
                }

                //openAIService.SetDefaultModelId(global::OpenAI.GPT3.ObjectModels.Models.TextDavinciV3);
                var completionResult = await openAIService.Completions.CreateCompletion(new CompletionCreateRequest()
                {
                    Prompt = prompt,
                    Model = model,
                    MaxTokens = maxTokens,

                });

                if (completionResult.Successful)
                {
                    return completionResult.Choices;//.FirstOrDefault();
                }
                else //handle errors
                {
                    if (completionResult.Error == null)
                    {
                        throw new NcfExceptionBase("Unknown Error");
                    }
                    else
                    {
                        throw new NcfExceptionBase($"Error：{completionResult.Error.Message}");
                    }
                }

            });
        }
        **/

        /// <summary>
        /// 使用不同模型运行 OpenAI（使用 Stream 方式）
        /// </summary>
        /// <param name="prompt">prompt 提示内容</param>
        /// <param name="model">选用模型，如果留空则默认使用 text-davinci-v3</param>
        /// <param name="maxTokens">最大消费 Token 数量。默认为 50</param>
        /// <returns></returns>
        [ApiBind(ApiRequestMethod = CO2NET.WebApi.ApiRequestMethod.Post)]
        public async Task<AppResponseBase<string>> CreateCompletionStreamAsync(string prompt, string model = null, int maxTokens = 50)
        {
            return await this.GetResponseAsync<AppResponseBase<string>, string>(async (response, logger) =>
            {
                if (string.IsNullOrEmpty(model))
                {
                    model = "text-davinci-003";
                }
                var result = await _openAiService.GetChatGPTResultAsync("", prompt, true, maxTokens, model);
                return result;
            });
        }

        /// <summary>
        /// ChatGPT 接口
        /// </summary>
        /// <param name="prompt">prompt 提示信息</param>
        /// <param name="maxTokens">最大消费 Token 数量。默认为 50</param>
        /// <returns></returns>
        [ApiBind(ApiRequestMethod = CO2NET.WebApi.ApiRequestMethod.Post)]
        public async Task<AppResponseBase<string>> ChatGPTAsync(string prompt, int maxTokens = 50)
        {
            return await this.GetResponseAsync<AppResponseBase<string>, string>(async (response, logger) =>
            {

                var result = await _openAiService.GetChatGPTResultAsync("DefaultUser", prompt);

                return result;
            });
        }

        /// <summary>
        /// ChatGPT 接口
        /// </summary>
        /// <returns></returns>
        [ApiBind(ApiRequestMethod = CO2NET.WebApi.ApiRequestMethod.Post)]
        public async Task<AppResponseBase<string>> CleanLastChatGPTAsync()
        {
            return await this.GetResponseAsync<AppResponseBase<string>, string>(async (response, logger) =>
            {
                await _openAiService.CleanChatGPT("DefaultUser");
                return "OK";
            });
        }
    }
}
