using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace HomeWork9._4_VKBot
{
    class BotWorker
    {
        private readonly VkApi vkApi;

        private readonly List<FileDownload> fileDownloads;
        private List<UserInfo> users;

        private string UserMessage;
        private int currentMessageId;

        private dynamic items;
        private bool onExit = false;

        public BotWorker()
        {
            vkApi = new VkApi();
            fileDownloads = new List<FileDownload>();
            GetUser();
        }


        /// <summary>
        /// Функция старта бота
        /// </summary>
        public void Start()
        {
            UpdateBot();
            if (onExit)
            {
                foreach (var item in users)
                {
                    currentMessageId = Convert.ToInt32(item.MessageId) - 1;
                    foreach (dynamic message in vkApi.MessagesGetHistory(item.UserID))
                    {
                        UserMessage = message.text;

                        int lastIdMessage = message.id;


                        switch (UserMessage.ToLower())
                        {
                            case "/start":
                                if (currentMessageId < lastIdMessage)
                                {
                                    vkApi.MessagesSend(item.UserID, $"Приветсвую тебя, {item.FirstName} {item.LastName}" +
                                        $"\r\nБот умеет:" +
                                        $"\r\n/about Показать инфромацию о пользователе" +
                                        $"\r\n/listdownload Список файлов для загрузки" +
                                        $"\r\n/download Что бы скачать файл");
                                    lastIdMessage++;
                                }
                                break;

                            case "/about":
                                vkApi.MessagesSend(item.UserID, $"Имя: {item.FirstName}" +
                                    $"\r\nФамилия: {item.LastName}" +
                                    $"\r\nID: {item.UserID}" +
                                    $"\r\nДень рождение: {item.BDay}" +
                                    $"\r\nПол: {item.Sex}" +
                                    $"\r\nКороткий ID: {item.ShortName}" +
                                    $"\r\nОбо мне: {item.About}" +
                                    $"\r\nСтрана: {item.Country}" +
                                    $"\r\nГород: {item.City}");
                                break;

                            case "/listdownload":
                                vkApi.MessagesSend(item.UserID, "Файлы достпуные для скачивания:");
                                foreach (var file in fileDownloads)
                                {
                                    vkApi.MessagesSend(item.UserID, $"{file.MessageId} {file.ShowFile()}");
                                }

                                vkApi.MessagesSend(item.UserID, "Скачанные файлы");

                                foreach (var fileDown in ShowDownloadFile())
                                    vkApi.MessagesSend(item.UserID, fileDown);

                                break;

                            case "/download":

                                vkApi.MessagesSend(item.UserID, "Напишите ID файла для скачивания(на запрос есть 10 секунд)");

                                Thread.Sleep(10000);

                                foreach (dynamic oneMes in vkApi.MessagesGetHistory(item.UserID))
                                    UserMessage = oneMes.text;

                                bool isExit = false;

                                foreach (var file in fileDownloads)
                                {
                                    if (UserMessage == file.MessageId)
                                    {
                                        vkApi.MessagesSend(item.UserID, "Файл скачен");
                                        file.LoadFile();
                                        isExit = true;
                                        return;
                                    }
                                }

                                if (isExit)
                                    vkApi.MessagesSend(item.UserID, "Неправильный запрос или закончилось время ожидания!!!");

                                break;

                            default:
                                if (currentMessageId < lastIdMessage && message.from_id == item.UserID)
                                {
                                    vkApi.MessagesSend(item.UserID, $"{message.text}");
                                    lastIdMessage++;
                                }
                                break;
                        }
                    }
                }
            }

            onExit = false;
        }

        /// <summary>
        /// Обновление бота по всем диалогам, для поиска нового сообщения
        /// </summary>
        private void UpdateBot()
        {
            items = JObject.Parse(vkApi.MessagesGetConversations())["response"]["items"].ToArray();

            int inRead;
            int lastMessageId;

            foreach (dynamic item in items)
            {
                inRead = Convert.ToInt32(item.conversation.in_read);
                lastMessageId = Convert.ToInt32(item.conversation.last_message_id);


                if (inRead < lastMessageId)
                {
                    GetUser();
                    onExit = true;
                }


                var attachments = item.last_message.attachments.HasValues;

                string fromId = item.last_message.from_id;

                if (attachments != false)
                    GetFile(fromId);
            }

        }

        /// <summary>
        /// Получение файлов из диалога по Id
        /// </summary>
        /// <param name="userId">Id пользователя у которого надо получить файлы</param>
        private void GetFile(string userId)
        {
            dynamic lastMessage = vkApi.MessagesGetHistory(userId);

            string typeFile = lastMessage[0].attachments[0].type;
            string title;
            string url;
            string messageId = lastMessage[0].id;
            string UserID = userId;
            string ext;

            switch (typeFile)
            {
                case "audio_message":
                    title = $"{typeFile}_{userId}_{messageId}";
                    url = lastMessage[0].attachments[0].audio_message.link_mp3;

                    fileDownloads.Add(new FileDownload(title, typeFile, url, messageId, UserID, ".mp3"));
                    break;

                case "photo":
                    title = $"{typeFile}_{userId}_{messageId}";
                    ext = ".jpg";
                    url = lastMessage[0].attachments[0].photo.sizes[9].url;

                    fileDownloads.Add(new FileDownload(title, typeFile, url, messageId, UserID, ext));
                    break;

                case "video":
                    vkApi.MessagesSend(UserID, "Что бы я мог скачать видео, отправьте его документом");
                    break;

                case "audio":
                    title = $"{lastMessage[0].attachments[0].audio.artist} {lastMessage[0].attachments[0].audio.title}";
                    ext = ".mp3";
                    url = lastMessage[0].attachments[0].audio.url;

                    fileDownloads.Add(new FileDownload(title, typeFile, url, messageId, UserID, ext));
                    break;

                case "doc":
                    title = lastMessage[0].attachments[0].doc.title;
                    ext = $".{lastMessage[0].attachments[0].doc.ext}";
                    url = lastMessage[0].attachments[0].doc.url;

                    fileDownloads.Add(new FileDownload(title, typeFile, url, messageId, UserID, ext));
                    break;
            }

            vkApi.MessagesSend(UserID, "Добавил в лист загрузки");
        }


        /// <summary>
        /// Обновление данных пользователя которые нам написали
        /// </summary>
        private void GetUser()
        {
            users = new List<UserInfo>();

            items = JObject.Parse(vkApi.MessagesGetConversations())["response"]["items"].ToArray();

            dynamic profiles = JObject.Parse(vkApi.MessagesGetConversations())["response"]["profiles"].ToArray();
            dynamic user;

            string profilId;
            string firstName;
            string lastName;
            string messageId;
            string bDay;
            string sex;
            string shortName;
            string about;
            string city;
            string country;
            int i = 0;


            foreach (dynamic item in items)
            {
                profilId = profiles[i].id;
                messageId = item.conversation.last_message_id;
                firstName = profiles[i].first_name;
                lastName = profiles[i].last_name;
                user = vkApi.UsersGet(profilId);

                bDay = user[0].bdate;
                sex = user[0].sex;
                shortName = user[0].domain;
                about = user[0].about;

                if (user[0].city != null)
                    city = user[0].city.title;
                else
                    city = null;

                if (user[0].country != null)
                    country = user[0].country.title;
                else
                    country = null;

                i++;
                users.Add(new UserInfo(
                            profilId,
                            firstName,
                            lastName,
                            messageId,
                            bDay,
                            sex,
                            shortName,
                            about,
                            city,
                            country
                            ));

            }
        }


        private string[] ShowDownloadFile()
        {
            string[] fileName = Directory.GetFiles(@"FileSend");

            return fileName;
        }
    }
}
