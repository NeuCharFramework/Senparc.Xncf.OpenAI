using Microsoft.Extensions.DependencyInjection;
using Senparc.Ncf.XncfBase.FunctionRenders;
using Senparc.Xncf.OpenAI.Domain.Services;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Senparc.Xncf.OpenAI.OHS.Local.PL
{
    public class NeuCharOpenAiConfigRequest_Update : FunctionAppRequestBase
    {
        [Required]
        [MaxLength(100)]
        [Description("AppKey||OpenAI 的 AppKey，可以从 https://platform.openai.com/account/api-keys 获取。出于安全考虑，AppKey 不会明文返回，每次需要重新设置")]
        public string ApiKey { get; set; }
        [Required]
        [MaxLength(200)]
        [Description("Endpoint||")]
        public string Endpoint { get; set; }
        [Required]
        [MaxLength(50)]
        [Description("ApiVersion||")]
        public string ApiVersion { get; set; }

        public NeuCharOpenAiConfigRequest_Update()
        {

        }

        public override async Task LoadData(IServiceProvider serviceProvider)
        {
            var openAiConfigService = serviceProvider.GetService<OpenAiConfigService>();
            var result = await openAiConfigService.GetSenparcAiConfigDtoAsync();
            if (result != null)
            {
                ApiKey = "";//始终留空
                Endpoint = result.NeuCharOpenAiEndpoint;
                ApiVersion = result.NeuCharOpenAiApiVersion;
            }

            await base.LoadData(serviceProvider);
        }
    }
}
