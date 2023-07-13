using Senparc.Ncf.Core.Models;

namespace Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel.Dto
{
    public class AzureOpenAiConfigDto:DtoBase
    {
        public string ApiKey { get; set; }
        public string AzureEndpoint { get; set; }
        public string AzureOpenAIApiVersion { get; set; }
    }
}
