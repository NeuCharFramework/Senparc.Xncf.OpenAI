using Senparc.CO2NET.Extensions;
using Senparc.Ncf.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Senparc.Xncf.OpenAI
{
    [Table(Register.DATABASE_PREFIX + nameof(NeuCharOpenAiConfig))]
    [Serializable]
    public class NeuCharOpenAiConfig : EntityBase<int>
    {
        [Required]
        [MaxLength(100)]
        public string ApiKey { get; private set; }

        [MaxLength(100)]
        public string NeuCharEndpoint { get; private set; }

        public string NeuCharOpenAIApiVersion { get; private set; } = "2022-12-01";

        private NeuCharOpenAiConfig() { }

        public NeuCharOpenAiConfig(string apiKey, string neuCharEndpoint)
        {
            Update(apiKey, neuCharEndpoint);
        }

        public void Update(string apiKey, string neuCharEndpoint)
        {
            if (!apiKey.IsNullOrEmpty())
            {
                ApiKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(apiKey));
            }
            NeuCharEndpoint = neuCharEndpoint;
        }

        public string GetOriginalApiKey()
        {
            return Encoding.Default.GetString(Convert.FromBase64String(ApiKey));
        }
    }
}
