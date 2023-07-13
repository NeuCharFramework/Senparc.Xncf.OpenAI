using Senparc.Ncf.Core.Models;

namespace Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel.Dto
{
    public class NeuCharOpenAiConfigDto:DtoBase
    {
        public string ApiKey { get; set; }
        public string NeuCharEndpoint { get; set; }
        public string NeuCharOpenAIApiVersion { get; set; }
    }
}
