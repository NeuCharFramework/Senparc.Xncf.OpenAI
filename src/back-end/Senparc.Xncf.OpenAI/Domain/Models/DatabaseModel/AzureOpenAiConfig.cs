using Senparc.CO2NET.Extensions;
using Senparc.Ncf.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Senparc.Xncf.OpenAI
{
    [Table(Register.DATABASE_PREFIX + nameof(AzureOpenAiConfig))]
    [Serializable]
    public class AzureOpenAiConfig : EntityBase<int>
    {
        [Required]
        [MaxLength(100)]
        public string ApiKey { get; private set; }

        [MaxLength(100)]
        public string AzureEndpoint { get; private set; }

        public string AzureOpenAIApiVersion { get; private set; } = "2022-12-01";

        private AzureOpenAiConfig() { }

        public AzureOpenAiConfig(string apiKey, string azureEndpoint)
        {
            Update(apiKey, azureEndpoint);
        }

        public void Update(string apiKey, string azureEndpoint)
        {
            if (!apiKey.IsNullOrEmpty())
            {
                ApiKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(apiKey));
            }
            AzureEndpoint = azureEndpoint;
        }

        public string GetOriginalApiKey()
        {
            return Encoding.Default.GetString(Convert.FromBase64String(ApiKey));
        }
    }
}

