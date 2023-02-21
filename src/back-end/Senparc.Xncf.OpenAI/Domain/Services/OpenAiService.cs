using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
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
        private OpenAIService _openAiService;
        
        public OpenAiService(IRepositoryBase<OpenAiConfig> repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
        }

        /// <summary>
        /// 获取 OpenAIService 对象
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NcfExceptionBase"></exception>
        public async Task<OpenAIService> GetOpenAiServiceAsync()
        {
            if (_openAiService==null)
            {
                var config = await base.GetObjectAsync(z => true, z => z.Id, Ncf.Core.Enums.OrderingType.Descending);
                if (config == null || config.ApiKey.IsNullOrEmpty())
                {
                    throw new NcfExceptionBase("请先设置 API Key！");
                }

                _openAiService = new OpenAIService(new OpenAiOptions()
                {
                    ApiKey = config.GetOriginalAppKey(),
                    Organization = config.OrganizationID
                });

                
            }
            return _openAiService;
        }
    }
}
