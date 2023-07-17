using Microsoft.Extensions.DependencyInjection;
using Senparc.Ncf.XncfBase.FunctionRenders;
using Senparc.Xncf.OpenAI.Domain.Services;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Senparc.Xncf.OpenAI.OHS.Local.PL
{
    public class SwitchAiPlatform_Update : FunctionAppRequestBase
    {
        [Required]
        [MaxLength(50)]
        [Description("AiPlatform||当前要使用的AI平台，可选值有'OpenAI','AzureOpenAI'以及'NeuCharOpenAI'")]
        public string AiPlatform { get; set; }

        public SwitchAiPlatform_Update()
        {

        }

        public override async Task LoadData(IServiceProvider serviceProvider)
        {
            var openAiConfigService = serviceProvider.GetService<OpenAiConfigService>();
            var result = await openAiConfigService.GetSenparcAiConfigDtoAsync();
            if (result != null)
            {
                AiPlatform = result.AiPlatform;
            }
            await base.LoadData(serviceProvider);
        }
    }
}
