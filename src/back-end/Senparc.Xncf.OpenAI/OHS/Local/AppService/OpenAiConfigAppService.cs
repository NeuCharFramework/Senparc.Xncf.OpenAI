using Senparc.Ncf.Core.AppServices;
using Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel.Dto;
using Senparc.Xncf.OpenAI.Domain.Services;
using Senparc.Xncf.OpenAI.OHS.Local.PL;
using System;
using System.Threading.Tasks;

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
        /// 获取全部配置信息
        /// </summary>
        /// <returns></returns>
        [FunctionRender("获取全部配置信息", "获取 ApiPlatform，ApiKey 等信息", typeof(Register))]
        public async Task<AppResponseBase<SenparcAiConfigDto>> GetSettings()
        {
            return await this.GetResponseAsync<AppResponseBase<SenparcAiConfigDto>, SenparcAiConfigDto>(async (response, logger) =>
            {
                var result = await _openAiConfigService.GetSenparcAiConfigDtoAsync(true);
                return result;
            });
        }

        /// <summary>
        /// 切换AI平台
        /// </summary>
        /// <returns></returns>
        [FunctionRender("切换API平台", "切换API平台", typeof(Register))]
        public async Task<StringAppResponse> SwitchApiPlatform(string platformName)
        {
            return await this.GetResponseAsync<StringAppResponse, string>(async (response, logger) =>
            {
                try
                {
                    await _openAiConfigService.SwitchApiPlatform(platformName);
                }
                catch (ArgumentException ex)
                {
                    return $"{platformName}为无效值，有效值为'OpenAI'，'AzureOpenAI'，'NeuCharOpenAI'";
                }
                return $"当前使用AI平台已切换为{platformName}";
            });
        }

        /// <summary>
        /// 更新 OpenAI 配置信息
        /// </summary>
        /// <returns></returns>
        [FunctionRender("更新 OpenAI 配置信息", "配置 OpenAI 的 AppKey、Organization 等信息", typeof(Register))]
        public async Task<StringAppResponse> UpdateOpenAi(OpenAiConfigRequest_Update request)
        {
            return await this.GetResponseAsync<StringAppResponse, string>(async (response, logger) =>
            {
                await _openAiConfigService.UpdateOpenAi(request.ApiKey, request.OrganizationId);
                return "更新完成。";
            });
        }

        /// <summary>
        /// 更新 AzureOpenAI 配置信息
        /// </summary>
        /// <returns></returns>
        [FunctionRender("更新 AzureOpenAI 配置信息", "配置 OpenAI 的 AppKey、Organization 等信息", typeof(Register))]
        public async Task<StringAppResponse> UpdateAzureOpenAi(AzureOpenAiConfigRequest_Update request)
        {
            return await this.GetResponseAsync<StringAppResponse, string>(async (response, logger) =>
            {
                await _openAiConfigService.UpdateAzureOpenAi(request.ApiKey, request.Endpoint,request.ApiVersion);
                return "更新完成。";
            });
        }

        /// <summary>
        /// 更新 NeuCharOpenAI 配置信息
        /// </summary>
        /// <returns></returns>
        [FunctionRender("更新 NeuCharOpenAI 配置信息", "配置 OpenAI 的 AppKey、Organization 等信息", typeof(Register))]
        public async Task<StringAppResponse> UpdateNeuCharOpenAi(NeuCharOpenAiConfigRequest_Update request)
        {
            return await this.GetResponseAsync<StringAppResponse, string>(async (response, logger) =>
            {
                await _openAiConfigService.UpdateNeuCharOpenAi(request.ApiKey, request.Endpoint,request.ApiVersion);
                return "更新完成。";
            });
        }
    }
}
