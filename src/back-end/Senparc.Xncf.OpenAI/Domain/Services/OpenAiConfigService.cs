using Microsoft.OpenApi.Extensions;
using Senparc.AI;
using Senparc.CO2NET;
using Senparc.Ncf.Core.Extensions;
using Senparc.Ncf.Repository;
using Senparc.Ncf.Service;
using Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel;
using Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel.Dto;
using System;
using System.Threading.Tasks;

namespace Senparc.Xncf.OpenAI.Domain.Services
{
    public class OpenAiConfigService : ServiceBase<SenparcAiConfig>
    {
        public OpenAiConfigService(IRepositoryBase<SenparcAiConfig> repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
        }


        private async Task<SenparcAiConfig> GetObjectAsync()
        {
            var config = await base.GetObjectAsync(z => true, z => z.Id, Ncf.Core.Enums.OrderingType.Ascending);
            return config;
        }

        /// <summary>
        /// 获取所有配置信息。其中 ApiKey 为部分信息
        /// </summary>
        /// <param name="hideApiKey">是否隐藏 ApiKey</param>
        /// <returns></returns>
        [ApiBind]
        public async Task<SenparcAiConfigDto> GetSenparcAiConfigDtoAsync(bool hideApiKey = true)
        {
            var config = await GetObjectAsync();
            if (config == null)
            {
                return new SenparcAiConfigDto();
            }

            var dto = base.Mapper.Map<SenparcAiConfigDto>(config);

            if (hideApiKey)
            {
                dto.OpenAiApiKey = config.GetOriginalApiKey(config.OpenAiApiKey).SubString(0, 5) + "...";
                dto.AzureOpenAiApiKey = config.GetOriginalApiKey(config.AzureOpenAiApiKey).SubString(0, 5) + "...";
                dto.NeuCharOpenAiApiKey = config.GetOriginalApiKey(config.NeuCharOpenAiApiKey).SubString(0, 5) + "...";
            }

            return dto;
        }

        /// <summary>
        /// 切换AI平台
        /// </summary>
        /// <param name="platformName"></param>
        /// <returns></returns>
        [ApiBind(ApiRequestMethod = CO2NET.WebApi.ApiRequestMethod.Post)]
        public async Task SwitchApiPlatform(string platformName)
        {
            if (!Enum.TryParse(platformName, out AiPlatform aiPlatform))
            {
                throw new ArgumentException("Invalid platform name.", nameof(platformName));
            }
            var config = await GetObjectAsync();
            config ??= new SenparcAiConfig();
            config.AiPlatform = aiPlatform.GetDisplayName();
            await base.SaveObjectAsync(config);
        }

        /// <summary>
        /// 更新 OpenAi配置
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [ApiBind(ApiRequestMethod = CO2NET.WebApi.ApiRequestMethod.Post)]
        public async Task UpdateOpenAi(string apiKey, string organizationId)
        {
            var config = await GetObjectAsync();
            config ??= new SenparcAiConfig();
            config.UpdateOpenAi(apiKey, organizationId);
            await base.SaveObjectAsync(config);
        }

        /// <summary>
        /// 更新 AzureOpenAi配置
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="endpoint"></param>
        /// <param name="apiVersion"></param>
        /// <returns></returns>
        [ApiBind(ApiRequestMethod = CO2NET.WebApi.ApiRequestMethod.Post)]
        public async Task UpdateAzureOpenAi(string apiKey, string endpoint, string apiVersion)
        {
            var config = await GetObjectAsync();
            config ??= new SenparcAiConfig();
            config.UpdateAzureOpenAi(apiKey,endpoint, apiVersion);
            await base.SaveObjectAsync(config);
        }

        /// <summary>
        /// 更新 NeuCharOpenAi配置
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        [ApiBind(ApiRequestMethod = CO2NET.WebApi.ApiRequestMethod.Post)]
        public async Task UpdateNeuCharOpenAi(string apiKey, string endpoint, string apiVersion)
        {
            var config = await GetObjectAsync();
            config ??= new SenparcAiConfig();
            config.UpdateNeuCharOpenAi(apiKey, endpoint, apiVersion);
            await base.SaveObjectAsync(config);
        }

    }
}
