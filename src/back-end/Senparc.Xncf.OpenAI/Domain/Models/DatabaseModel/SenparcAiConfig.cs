using Senparc.CO2NET.Extensions;
using Senparc.Ncf.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel
{
    /// <summary>
    /// OpenAiConfig 实体类
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.TableAttribute(Register.DATABASE_PREFIX + nameof(SenparcAiConfig))]//必须添加前缀，防止全系统中发生冲突
    [Serializable]
    public class SenparcAiConfig : EntityBase<int>
    {
        public bool IsDebug { get; set; }
        [MaxLength(50)]
        public string AiPlatform { get; set; }


        [MaxLength(100)]
        public string OpenAiApiKey { get; set; }

        [MaxLength(100)]
        public string OpenAiOrganizationId { get; set; }



        [MaxLength(100)]
        public string AzureOpenAiApiKey { get; set; }

        [MaxLength(200)]
        public string AzureOpenAiEndpoint { get; set; }

        [MaxLength(50)]
        public string AzureOpenAiApiVersion { get; set; }



        [MaxLength(100)]
        public string NeuCharOpenAiApiKey { get; set; }

        [MaxLength(200)]
        public string NeuCharOpenAiEndpoint { get; set; }
        [MaxLength(50)]
        public string NeuCharOpenAiApiVersion { get; set; }

        [NotMapped]
        public bool IsApiKeySetted
        { 
            get
            {
                return !(string.IsNullOrEmpty(OpenAiApiKey) && string.IsNullOrEmpty(AzureOpenAiApiKey) && string.IsNullOrEmpty(NeuCharOpenAiApiKey));
            }
        }

        public void UpdateOpenAi(string apiKey, string organizationId)
        {
            OpenAiApiKey = EncryptApikey(apiKey);
            OpenAiOrganizationId = organizationId;
        }

        public void UpdateNeuCharOpenAi(string apiKey, string endpoint, string apiVersion)
        {
            NeuCharOpenAiApiKey = EncryptApikey(apiKey);
            NeuCharOpenAiEndpoint = endpoint;
            NeuCharOpenAiApiVersion = apiVersion;
        }

        public void UpdateAzureOpenAi(string apiKey, string endpoint, string apiVersion)
        {
            AzureOpenAiApiKey = EncryptApikey(apiKey);
            AzureOpenAiEndpoint = endpoint;
            AzureOpenAiApiVersion = apiVersion;
        }

        private string EncryptApikey(string apiKey)
        {
            if(!apiKey.IsNullOrEmpty())
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(apiKey));
            return null;
        }

        public string DecryptApiKey(string apiKey)
        {
            if (!apiKey.IsNullOrEmpty())
                return Encoding.Default.GetString(Convert.FromBase64String(apiKey));
            return null;
        }
    }
}
