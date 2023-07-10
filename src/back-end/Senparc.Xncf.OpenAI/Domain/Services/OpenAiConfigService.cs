using Senparc.CO2NET;
using Senparc.Ncf.Core.Extensions;
using Senparc.Ncf.Repository;
using Senparc.Ncf.Service;
using Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel.Dto;
using System;
using System.Threading.Tasks;

namespace Senparc.Xncf.OpenAI.Domain.Services
{
    public class OpenAiConfigService : ServiceBase<OpenAiConfig>
    {
        public OpenAiConfigService(IRepositoryBase<OpenAiConfig> repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
        }


        private async Task<OpenAiConfig> GetObjectAsync()
        {
            var config = await base.GetObjectAsync(z => true, z => z.Id, Ncf.Core.Enums.OrderingType.Ascending);
            return config;
        }

        /// <summary>
        /// 获取 OpenAiConfig 配置信息。其中 AppKey 为部分信息
        /// </summary>
        /// <param name="hideApiKey">是否隐藏 ApiKey</param>
        /// <returns></returns>
        [ApiBind]
        public async Task<OpenAiConfigDto> GetOpenAiConfigDtoAsync(bool hideApiKey = true)
        {
            var config = await GetObjectAsync();
            if (config == null)
            {
                return new OpenAiConfigDto();
            }

            var dto = base.Mapper.Map<OpenAiConfigDto>(config);
            var apiKey = config.GetOriginalAppKey();
            dto.ApiKey = hideApiKey ? apiKey.SubString(0, 5) + "..." : apiKey;
            dto.OrganizationID = config.OrganizationID;
            return dto;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="organizationID"></param>
        /// <returns></returns>
        [ApiBind(ApiRequestMethod = CO2NET.WebApi.ApiRequestMethod.Post)]
        public async Task Update(string appKey, string organizationID)
        {
            var config = await GetObjectAsync();
            if (config == null)
            {
                config = new OpenAiConfig(appKey, organizationID);
            }
            else
            {
                config.Update(appKey, organizationID);
            }

            await base.SaveObjectAsync(config);
        }

    }
}
