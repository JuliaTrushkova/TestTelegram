using MimeKit;
using System;
using MailKit.Net.Smtp;
using MailKit.Net.Imap;
using MailKit;
using MailKit.Search;


namespace TestTelegram
{
    internal class EmailService
    {
        //Отправка сообщений от Телеграм бота на почту
        public static async Task SendMessageAsync(string? textOfMessage)
        {
            EmailService emailService = new EmailService();
            if (textOfMessage == null)
                textOfMessage = "Message is empty";

            await emailService.SendMailAsync("test@yandex.ru", "test of telegram", textOfMessage);

        }
        //Проверка писем
        public static async Task CheckMessageAsync()
        {
            EmailService emailService = new EmailService();
            
            await emailService.RetrieveMailAsync();
        }
        //Отправка письме по SMTP 
        public async Task SendMailAsync(string email, string subjectOfEmail, string bodyOfMessage)
        {
            using MimeMessage emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Test", "Test@yandex.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subjectOfEmail;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = bodyOfMessage
            };

            using (SmtpClient smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync("smtp.yandex.ru", 465, true);
                await smtpClient.AuthenticateAsync("Test@yandex.ru", System.IO.File.ReadAllText("C:\\yandex_smtp.txt"));
                await smtpClient.SendAsync(emailMessage);
                await smtpClient.DisconnectAsync(true);
            }
        }

        //Подключение к почте через IMAP и получение списка писем
        public async Task RetrieveMailAsync()
        {
            Console.WriteLine("start retrieve");
            using (ImapClient imapClient = new ImapClient())
            {
                await imapClient.ConnectAsync("imap.yandex.ru", 465, true);
                await imapClient.AuthenticateAsync("Test@yandex.ru", System.IO.File.ReadAllText("C:\\yandex_imap.txt"));
                IMailFolder inboxFolder = imapClient.Inbox;
                inboxFolder.Open(FolderAccess.ReadOnly);

                Console.WriteLine($"Name {inboxFolder.Name}");
                Console.WriteLine($"Count ALL  {inboxFolder.Count}");
                Console.WriteLine($"Unread {inboxFolder.Unread}");
                Console.WriteLine($"Recent {inboxFolder.Recent}");

                for (int i = 0; i < inboxFolder.Count; i++)
                {
                    MimeMessage message = inboxFolder.GetMessage(i);
                    Console.WriteLine(message.From.ToString() + " " + message.MessageId.ToString() + " " + message.Subject.ToString());
                }

                Console.WriteLine("start filter");
                var query = SearchQuery.FromContains("yandex.ru").And(SearchQuery.NotSeen);

                foreach (UniqueId uid in inboxFolder.Search(query))
                {
                    var message = inboxFolder.GetMessage(uid);
                    Console.WriteLine("[match] {0}: {1}", uid, message.Subject);
                    //await telegramBotTest.SendTextMessageAsync(messageChat, message.Subject);
                    int y = 0;
                }               

                await imapClient.DisconnectAsync(true);

            }
            Console.WriteLine("end retrieve");
        }
    }
}
