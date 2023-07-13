﻿using Senparc.Ncf.Core.Models;

namespace Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel.Dto
{
    public class OpenAiConfigDto : DtoBase
    {
        public string ApiKey { get; set; }
        public string OrganizationID { get; set; }
    }
}
