using Microsoft.EntityFrameworkCore.Update.Internal;
using Senparc.CO2NET;
using Senparc.Ncf.Core.Exceptions;
using Senparc.Ncf.Core.Extensions;
using Senparc.Ncf.Repository;
using Senparc.Ncf.Service;
using Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <returns></returns>
        [ApiBind]
        public async Task<OpenAiConfigDto> GetOpenAiConfigDtoAsync()
        {
            var config = await GetObjectAsync();
            if (config == null)
            {
                return new OpenAiConfigDto();
            }

            var dto = base.Mapper.Map<OpenAiConfigDto>(config);
            dto.ApiKey = config.GetOriginalAppKey().SubString(0, 5) + "...";
            return dto;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="organizaionID"></param>
        /// <returns></returns>
        [ApiBind(ApiRequestMethod = CO2NET.WebApi.ApiRequestMethod.Post)]
        public async Task Update(string appKey, string organizaionID)
        {
            var config = await GetObjectAsync();
            if (config == null)
            {
                config = new OpenAiConfig(appKey, organizaionID);
            }
            else
            {
                config.Update(appKey, organizaionID);
            }

            await base.SaveObjectAsync(config);
        }

    }
}
