using Senparc.Ncf.Core.Models;
using Senparc.Xncf.OpenAI.Models.DatabaseModel.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Senparc.Xncf.OpenAI
{
    /// <summary>
    /// OpenAiConfig 实体类
    /// </summary>
    [Table(Register.DATABASE_PREFIX + nameof(OpenAiConfig))]//必须添加前缀，防止全系统中发生冲突
    [Serializable]
    public class OpenAiConfig : EntityBase<int>
    {

        [Required]
        [MaxLength(100)]
        public string ApiKey { get; private set; }
        [MaxLength(100)]
        public string OrganizationID { get; private set; }
        
        public OpenAiConfig(string apiKey, string organizationID)
        {
            ApiKey = apiKey;
            OrganizationID = organizationID;
        }

        /// <summary>
        /// 更新信息
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="organizationID"></param>
        public void Update(string apiKey, string organizationID)
        {
            ApiKey = apiKey;
            OrganizationID = organizationID;
        }
    }
}
