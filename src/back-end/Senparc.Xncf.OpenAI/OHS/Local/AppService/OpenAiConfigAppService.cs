using Senparc.CO2NET.WebApi;
using Senparc.CO2NET;
using Senparc.Ncf.Core.AppServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senparc.Xncf.OpenAI.Domain.Services;
using Senparc.Xncf.OpenAI.OHS.Local.PL;
using Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel.Dto;

namespace Senparc.Xncf.OpenAI.OHS.Local.AppService
{
    public class OpenAiConfigAppService : AppServiceBase
    {
        public OpenAiConfigService _openAiConfigService { get; }

        public OpenAiConfigAppService(IServiceProvider serviceProvider, OpenAiConfigService openAiConfigService) : base(serviceProvider)
        {
            _openAiConfigService = openAiConfigService;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [FunctionRender("更新 OpenAI 配置信息", "配置 OpenAI 的 AppKey、Organization 等信息", typeof(Register))]
        public async Task<StringAppResponse> Update(OpenAiConfigRequest_Update request)
        {
            return await this.GetResponseAsync<StringAppResponse, string>(async (response, logger) =>
            {
                await _openAiConfigService.Update(request.AppKey, request.OrganizationID);
                return "更新完成。";
            });
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [FunctionRender("获取 OpenAI 配置信息", "获取 OpenAI 的 AppKey、Organization 等信息", typeof(Register))]
        public async Task<AppResponseBase<OpenAiConfigDto>> Get(OpenAiConfigRequest_Update request)
        {
            return await this.GetResponseAsync<AppResponseBase<OpenAiConfigDto>, OpenAiConfigDto >(async (response, logger) =>
            {
                var result = await _openAiConfigService.GetOpenAiConfigDtoAsync(true);
                return result;
            });
        }
    }
}
