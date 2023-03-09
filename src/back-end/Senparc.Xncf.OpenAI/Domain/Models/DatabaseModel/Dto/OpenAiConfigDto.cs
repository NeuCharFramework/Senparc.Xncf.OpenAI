using Senparc.Ncf.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.Xncf.OpenAI.Domain.Models.DatabaseModel.Dto
{
    public class OpenAiConfigDto : DtoBase
    {
        public string ApiKey { get; set; }
        public string OrganizationID { get; set; }
    }
}
