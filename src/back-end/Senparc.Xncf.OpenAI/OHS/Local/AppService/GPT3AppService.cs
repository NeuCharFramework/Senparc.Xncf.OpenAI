using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
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

        public async Task<AppResponseBase<ChoiceResponse>> TextDavinciV3Async(string prompt)
        {
            return await this.GetResponseAsync<AppResponseBase<ChoiceResponse>, ChoiceResponse>(async (response, logger) =>
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
                    return completionResult.Choices.FirstOrDefault();
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
    }
}
