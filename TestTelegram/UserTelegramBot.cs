using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Text.Json;
using Telegram.Bot.Polling;

namespace TestTelegram
{
    internal class UserTelegramBot
    {
        static ITelegramBotClient telegramBot = new TelegramBotClient(System.IO.File.ReadAllText("C:\\Users\\Julie\\Documents\\Ju\\programming\\Pyxis\\telegram.txt"));
        static Chat messageChat;

        //Обработка полученных от пользователя сообщений и отправка ответа
        static async Task HandleUpdateAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(JsonSerializer.Serialize(update) + "\n");

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                Message? message = update.Message;
                messageChat = message.Chat;
                if (message?.Text?.ToLower() == "/start")
                {
                    await telegramBotClient.SendTextMessageAsync(messageChat, "I am ready to start, Julia!");
                    return;
                }
                await telegramBotClient.SendTextMessageAsync(messageChat, "Helloooo1");

                if (message?.Sticker != null)
                {
                    await telegramBotClient.SendTextMessageAsync(messageChat, "You sent me a sticker, Julia!");
                    return;
                }

                await telegramBotClient.SendTextMessageAsync(messageChat, "Helloooo2");

                await EmailService.SendMessageAsync(message?.Text);
            }

        }

        //Обработка ошибок
        static async Task HandleErrorAsync(ITelegramBotClient telegramBotClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error: {Newtonsoft.Json.JsonConvert.SerializeObject(exception)}");
        }

        //запуск телеграмм бота 
        public static void RunTelegramBot()
        {
            Console.WriteLine($"Bot {telegramBot.GetMeAsync().Result.FirstName} of {telegramBot.GetMeAsync().Result.Username} is running");

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            ReceiverOptions receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = { }
            };
            
            telegramBot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
        }

    }
}
