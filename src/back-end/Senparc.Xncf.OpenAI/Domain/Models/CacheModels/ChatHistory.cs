namespace Senparc.Xncf.OpenAI.Domain.Models.CacheModels
{
    /// <summary>
    /// ChatGPT 消息
    /// </summary>
    public class ChatHistory
    {

        /// <summary>
        /// 用户 ID
        /// </summary>
        public string UserId { get; private set; }
        /// <summary>
        /// 消息列表
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// 获取缓存 Key
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetCacheKey(string userId)
        {
            return $"ChatGPT-{userId}";
        }

        public ChatHistory(string userId, string content = null)
        {
            UserId = userId;
            Content = content ?? "";
        }

        public void AppendHistory(string content)
        {
            Content += content;
        }

        public void CleanHistory()
        {
            Content = "";
        }
    }
}
