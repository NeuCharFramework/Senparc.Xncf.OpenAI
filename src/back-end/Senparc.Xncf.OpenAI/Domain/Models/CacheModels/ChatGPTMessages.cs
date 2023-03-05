using OpenAI.GPT3.ObjectModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.Xncf.OpenAI.Domain.Models.CacheModels
{
    /// <summary>
    /// ChatGPT 消息
    /// </summary>
    public class ChatGPTMessages
    {

        /// <summary>
        /// 用户 ID
        /// </summary>
        public string UserId { get; private set; }
        /// <summary>
        /// 消息列表
        /// </summary>
        public List<ChatMessage> Messages { get; private set; }

        /// <summary>
        /// 获取缓存 Key
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetCacheKey(string userId)
        {
            return $"ChatGPT-{userId}";
        }

        public ChatGPTMessages(string userId, List<ChatMessage> messages = null)
        {
            UserId = userId;
            Messages = messages ?? new List<ChatMessage>();
        }

        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(ChatMessage message)
        {
            Messages.Add(message);
        }
    }
}
