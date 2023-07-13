using Senparc.CO2NET;
using Senparc.Ncf.Repository;
using Senparc.Ncf.Service;
using Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel;
using Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel.Dto;
using System;
using System.Threading.Tasks;

namespace Senparc.Xncf.OpenAI.Domain.Services
{
    public class AzureOpenAiConfigService : ServiceBase<AzureOpenAiConfig>
    {
        public AzureOpenAiConfigService(IRepositoryBase<AzureOpenAiConfig> repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
        }

        private async Task<AzureOpenAiConfig> GetObjectAsync()
        {
            var config = await base.GetObjectAsync(z => true, z => z.Id, Ncf.Core.Enums.OrderingType.Ascending);
            return config;
        }

        [ApiBind]
        public async Task<AzureOpenAiConfigDto> GetAzureOpenAiConfigDtoAsync(bool hideApiKey = true)
        {
            var config = await GetObjectAsync();
            if (config == null)
            {
                return new AzureOpenAiConfigDto();
            }

            var dto = base.Mapper.Map<AzureOpenAiConfigDto>(config);
            var apiKey = config.GetOriginalApiKey();
            dto.ApiKey = hideApiKey ? apiKey.Substring(0, 5) + "..." : apiKey;
            dto.AzureEndpoint = config.AzureEndpoint;
            return dto;
        }

        [ApiBind(ApiRequestMethod = CO2NET.WebApi.ApiRequestMethod.Post)]
        public async Task Update(string apiKey, string azureEndpoint)
        {
            var config = await GetObjectAsync();
            if (config == null)
            {
                config = new AzureOpenAiConfig(apiKey, azureEndpoint);
            }
            else
            {
                config.Update(apiKey, azureEndpoint);
            }

            await base.SaveObjectAsync(config);
        }
    }

}
