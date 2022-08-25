using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace HomeWork9._4_VKBot
{
    class VkApi
    {
        private readonly WebClient wc;

        private Uri uri;
        private string url;

        private readonly string startUrl;
        private string methodName;
        private readonly string v;
        private readonly string token;

        public VkApi()
        {
            wc = new WebClient() { Encoding = Encoding.UTF8 };

            startUrl = $@"https://api.vk.com/method/";
            token = File.ReadAllText(@"token.txt");
            v = "5.131";
        }

        /// <summary>
        /// Возвращает список бесед
        /// </summary>
        /// <returns></returns>
        public string MessagesGetConversations()
        {
            methodName = "messages.getConversations";
            url = $"{startUrl}{methodName}?extended=1&access_token={token}&v={v}";

            uri = new Uri(url);

            var r = wc.DownloadString(uri);

            return r;
        }

        /// <summary>
        /// Возвращает историю сообщений конкретного пользоватя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Id пользователя у которого надо прочитать сообщение</returns>
        public JToken[] MessagesGetHistory(string userId)
        {
            methodName = "messages.getHistory";
            url = $"{startUrl}{methodName}?user_id={userId}&count=1&access_token={token}&v={v}";

            uri = new Uri(url);

            var r = wc.DownloadString(uri);

            var msgs = JObject.Parse(r)["response"]["items"].ToArray();

            return msgs;
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sendText"></param>
        public void MessagesSend(string userId, string sendText)
        {
            methodName = "messages.send";

            url = $"{startUrl}{methodName}?user_id={userId}&random_id=0&message={sendText}&access_token={token}&v={v}";
            uri = new Uri(url);
            wc.DownloadString(uri);

            Thread.Sleep(500);
        }

        /// <summary>
        /// Возвращает информацию об пользователе
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Id возвращаемого пользователя</returns>
        public JToken[] UsersGet(string userId)
        {
            methodName = "users.get";
            
            url = $"{startUrl}{methodName}?user_id={userId}&fields=about, sex, bdate, city, country, domain&access_token={token}&v={v}";
            uri = new Uri(url);

            var r = wc.DownloadString(uri);

            var msgs = JObject.Parse(r)["response"].ToArray();

            return msgs;
        }
    }
}
