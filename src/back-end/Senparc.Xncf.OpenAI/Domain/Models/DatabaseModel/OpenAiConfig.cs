using Senparc.Ncf.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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

        private OpenAiConfig() { }

        public OpenAiConfig(string apiKey, string organizationID)
        {
            Update(apiKey, organizationID);

        }

        /// <summary>
        /// 更新信息
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="organizationID"></param>
        public void Update(string apiKey, string organizationID)
        {
            ApiKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(apiKey));
            OrganizationID = organizationID;
        }

        /// <summary>
        /// 获取原始的 AppId
        /// </summary>
        /// <returns></returns>
        public string GetOriginalAppKey()
        {
            return Encoding.Default.GetString(Convert.FromBase64String(ApiKey));

        }
    }
}
