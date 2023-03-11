using Senparc.Ncf.XncfBase.FunctionRenders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senparc.Xncf.OpenAI.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Senparc.Xncf.OpenAI.OHS.Local.PL
{
    public class OpenAiConfigRequest_Update : FunctionAppRequestBase
    {
        [Required]
        [MaxLength(100)]
        [Description("AppKey||OpenAI 的 AppKey，可以从 https://platform.openai.com/account/api-keys 获取。出于安全考虑，AppKey 不会明文返回，每次需要重新设置")]
        public string AppKey { get; set; }
        [MaxLength(100)]
        [Description("OrganizationID||OpenAI 的 OrganizationID，可以从 https://platform.openai.com/account/org-settings 获取")]
        public string OrganizationID { get; set; }

        public OpenAiConfigRequest_Update()
        {

        }

        public override async Task LoadData(IServiceProvider serviceProvider)
        {
            var openAiConfigService = serviceProvider.GetService<OpenAiConfigService>();
            var result = await openAiConfigService.GetOpenAiConfigDtoAsync();
            if (result != null)
            {
                AppKey = "";//始终留空
                OrganizationID = result.OrganizationID;
            }

            await base.LoadData(serviceProvider);
        }
    }
}
