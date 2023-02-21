using Senparc.CO2NET.Extensions;
using Senparc.Ncf.Core.Exceptions;
using Senparc.Ncf.Repository;
using Senparc.Ncf.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.Xncf.OpenAI.Domain.Services
{
    public class OpenAiService : ServiceBase<OpenAiConfig>
    {
        public OpenAiService(IRepositoryBase<OpenAiConfig> repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
        }

        public async Task<OpenAiService> GetOpenAiServiceAsync()
        {
            var config = await base.GetObjectAsync(z => true, z => z.Id, Ncf.Core.Enums.OrderingType.Descending);
            if (config == null || config.ApiKey.IsNullOrEmpty())
            {
                throw new NcfExceptionBase("请先设置 API Key！");
            }

            var openAiService = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = config.ApiKey,
                Organization = config.OrganizationID
            });
        }
    }
}
