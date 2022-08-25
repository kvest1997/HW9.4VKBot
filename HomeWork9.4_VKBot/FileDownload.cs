using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace HomeWork9._4_VKBot
{
    class FileDownload
    {

        public string Title { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }

        public string MessageId { get; set; }

        public string UserId { get; set; }

        public string Ext { get; set; }

        public FileDownload(string title, string  type, string url, string messageId, string userId,string ext)
        {
            this.Title = title;
            this.Type = type;
            this.Url = url;
            this.MessageId = messageId;
            this.UserId = userId;
            this.Ext = ext;
        }

        /// <summary>
        /// Загружает выбранный файл из диалога
        /// </summary>
        public async void LoadFile()
        {
            byte[] data;

            using(var client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(Url))
                {
                    using (HttpContent content = response.Content)
                    {
                        data = await content.ReadAsByteArrayAsync();
                        using (FileStream fs = File.Create($"FileSend\\{Title}{Ext}"))
                        {
                            fs.Write(data, 0, data.Length);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Показывает какие файлы были добавлены для скачивания
        /// </summary>
        public string ShowFile()
        {
            return $"{Title} {Type} {Ext}";
        }
    }
}
