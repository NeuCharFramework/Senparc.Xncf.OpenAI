using Senparc.CO2NET;
using Senparc.Ncf.Repository;
using Senparc.Ncf.Service;
using Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel;
using Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel.Dto;
using System;
using System.Threading.Tasks;

namespace Senparc.Xncf.OpenAI.Domain.Services
{
    public class NeuCharOpenAiConfigService : ServiceBase<NeuCharOpenAiConfig>
    {
        public NeuCharOpenAiConfigService(IRepositoryBase<NeuCharOpenAiConfig> repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
        }

        private async Task<NeuCharOpenAiConfig> GetObjectAsync()
        {
            var config = await base.GetObjectAsync(z => true, z => z.Id, Ncf.Core.Enums.OrderingType.Ascending);
            return config;
        }

        [ApiBind]
        public async Task<NeuCharOpenAiConfigDto> GetNeuCharOpenAiConfigDtoAsync(bool hideApiKey = true)
        {
            var config = await GetObjectAsync();
            if (config == null)
            {
                return new NeuCharOpenAiConfigDto();
            }

            var dto = base.Mapper.Map<NeuCharOpenAiConfigDto>(config);
            var apiKey = config.GetOriginalApiKey();
            dto.ApiKey = hideApiKey ? apiKey.Substring(0, 5) + "..." : apiKey;
            dto.NeuCharEndpoint = config.NeuCharEndpoint;
            return dto;
        }

        [ApiBind(ApiRequestMethod = CO2NET.WebApi.ApiRequestMethod.Post)]
        public async Task Update(string apiKey, string neuCharEndpoint)
        {
            var config = await GetObjectAsync();
            if (config == null)
            {
                config = new NeuCharOpenAiConfig(apiKey, neuCharEndpoint);
            }
            else
            {
                config.Update(apiKey, neuCharEndpoint);
            }

            await base.SaveObjectAsync(config);
        }
    }

}
