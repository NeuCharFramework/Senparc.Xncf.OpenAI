using Microsoft.CodeAnalysis;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels.ResponseModels;
using OpenAI.GPT3.ObjectModels.SharedModels;
using Senparc.CO2NET;
using Senparc.Ncf.Core.AppServices;
using Senparc.Ncf.Core.Exceptions;
using Senparc.Xncf.OpenAI.Domain.Services;
using Senparc.Xncf.OpenAI.OHS.Local.PL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.Xncf.OpenAI.OHS.Local.AppService
{
    [ApiBind]
    public class GPT3AppService : AppServiceBase
    {
        readonly OpenAiService _openAiService;

        public GPT3AppService(IServiceProvider serviceProvider, OpenAiService openAiService) : base(serviceProvider)
        {
            _openAiService = openAiService;
        }

        [ApiBind(ApiRequestMethod = CO2NET.WebApi.ApiRequestMethod.Post)]
        public async Task<AppResponseBase<List<ChoiceResponse>>> TextDavinciV3Async(string prompt)
        {
            return await this.GetResponseAsync<AppResponseBase<List<ChoiceResponse>>, List<ChoiceResponse>>(async (response, logger) =>
            {
                var dt1 = SystemTime.Now;//开始计时

                var openAIService = await _openAiService.GetOpenAiServiceAsync();

                openAIService.SetDefaultModelId(global::OpenAI.GPT3.ObjectModels.Models.TextDavinciV3);
                var completionResult = await openAIService.Completions.CreateCompletion(new CompletionCreateRequest()
                {
                    Prompt = prompt,
                    Model = global::OpenAI.GPT3.ObjectModels.Models.TextDavinciV3
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

        /// <summary>
        /// 运行 TextDavinciV3 模型
        /// </summary>
        /// <param name="prompt">prompt 提示内容</param>
        /// <returns></returns>
        /// <exception cref="NcfExceptionBase"></exception>
        [ApiBind(ApiRequestMethod = CO2NET.WebApi.ApiRequestMethod.Post)]
        public async Task<AppResponseBase<List<ChoiceResponse>>> TextDavinciV3StreamAsync(string prompt)
        {
            return await this.GetResponseAsync<AppResponseBase<List<ChoiceResponse>>, List<ChoiceResponse>>(async (response, logger) =>
            {
                var dt1 = SystemTime.Now;//开始计时

                var openAIService = await _openAiService.GetOpenAiServiceAsync();

                openAIService.SetDefaultModelId(global::OpenAI.GPT3.ObjectModels.Models.TextDavinciV3);
                var completionResult = openAIService.Completions.CreateCompletionAsStream(new CompletionCreateRequest()
                {
                    Prompt = prompt,
                    Model = global::OpenAI.GPT3.ObjectModels.Models.TextDavinciV3
                });

                List<ChoiceResponse> result = new List<ChoiceResponse>();
                //var sb = new StringBuilder();

                await foreach (var completion in completionResult)
                {
                    if (completion.Successful)
                    {
                        if (completion.Choices.FirstOrDefault() is ChoiceResponse first && first is not null)
                        {
                            result.Append(first);
                        }
                        //sb.Append(completion.Choices.FirstOrDefault()?.Text);
                    }
                    else
                    {
                        if (completion.Error == null)
                        {
                            throw new NcfExceptionBase("Unknown Error");
                        }
                        new NcfExceptionBase($"{completion.Error.Code}: {completion.Error.Message}");
                    }
                }

                return result;
            });
        }
    }
}
