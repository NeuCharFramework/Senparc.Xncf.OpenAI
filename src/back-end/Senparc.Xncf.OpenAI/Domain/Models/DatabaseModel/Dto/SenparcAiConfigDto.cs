using Senparc.AI;
using Senparc.AI.Interfaces;
using Senparc.CO2NET.Extensions;
using Senparc.Ncf.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel.Dto
{
    public class SenparcAiConfigDto : DtoBase
    {
        public bool IsDebug { get; set; }

        public string AiPlatform { get; set; }

        public string OpenAiApiKey { get; set; }

        public string OpenAiOrganizationId { get; set; }

        public string AzureOpenAiApiKey { get; set; }

        public string AzureOpenAiEndpoint { get; set; }

        public string AzureOpenAiApiVersion { get; set; }

        public string NeuCharOpenAiApiKey { get; set; }

        public string NeuCharOpenAiEndpoint { get; set; }

        public string NeuCharOpenAiApiVersion { get; set; }
    }
}
