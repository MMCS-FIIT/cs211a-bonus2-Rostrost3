using System;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram_bot
{
    internal class Program
    {
        public static int c = 0; //баллы игрока
        static void Main(string[] args)
        {
            var client = new TelegramBotClient("6668334841:AAFfb4xkbEGL-HQ_d7AEdABDL2JgH30CH7Y");
            client.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        static async Task Update(ITelegramBotClient botclient, Update update, CancellationToken token)
        {
            try
            {
                switch(update.Type)
                {
                    case UpdateType.Message:
                    {
                       var message = update.Message;
                       switch (message.Type) 
                       {
                           case MessageType.Text:
                                {
                                    if (message.Text.ToLower().Contains("привет") || message.Text.ToLower().Contains("/start"))
                                    {
                                        await botclient.SendTextMessageAsync(message.Chat.Id, "Привет! :)");
                                        return;
                                    }
                                    if (message.Text.ToLower().Contains("закончить"))
                                    {
                                            await botclient.SendTextMessageAsync(message.Chat.Id, "Пока ;)");
                                            
                                            return;
                                    }
                                    if (message.Text.ToLower().Contains("играть"))
                                    {
                                            c = 0; //новая игра => баллы обнуляются
                                            await botclient.SendTextMessageAsync(message.Chat.Id, "Давай поиграем!\nТебе нужно будет угадать игрока\nЗа каждый правильный ответ 1 балл\nУдачи!");

                                            await botclient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri("https://s-cdn.sportbox.ru/images/styles/upload/fp_fotos/bd/b0/95dd9d6c42d283d22a6ef81a48f850d2657879e3a203e325196686.jpg"));
                                            var inlineKeyboard1 = new InlineKeyboardMarkup(
                                            new List<InlineKeyboardButton[]>()
                                            {
                                                new InlineKeyboardButton[]
                                                {
                                                    InlineKeyboardButton.WithCallbackData("Cristiano Ronaldo,Al-Nassr"),
                                                    InlineKeyboardButton.WithCallbackData("Lionel Messi,Inter Miami")
                                                }
                                            }
                                            );
                                            await botclient.SendTextMessageAsync(message.Chat.Id, "Кто же это?", replyMarkup: inlineKeyboard1);

                                            await botclient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri("https://www.thetimes.co.uk/imageserver/image/%2Fmethode%2Ftimes%2Fprod%2Fweb%2Fbin%2Fd060a46a-97ad-48ba-9299-adce61729dc8.jpg?crop=5000%2C2812%2C0%2C58&resize=1200"));
                                            var inlineKeyboard2 = new InlineKeyboardMarkup(
                                            new List<InlineKeyboardButton[]>()
                                            {
                                                new InlineKeyboardButton[]
                                                {
                                                    InlineKeyboardButton.WithCallbackData("Jude Bellingham,Real Madrid"),
                                                    InlineKeyboardButton.WithCallbackData("Erling Haaland,Manchester City")
                                                },
                                                new InlineKeyboardButton[]
                                                {
                                                    InlineKeyboardButton.WithCallbackData("Rodri,Manchester City"),
                                                }
                                            }
                                            );
                                            await botclient.SendTextMessageAsync(message.Chat.Id, "Кто же это?", replyMarkup: inlineKeyboard2);
                                            return;
                                    }
                                    return;
                           }
                       }
                            return;
                    }
                    case UpdateType.CallbackQuery:
                        {
                            var message = update.CallbackQuery;
                            switch (message.Data)
                            {
                                case "Cristiano Ronaldo,Al-Nassr":
                                    {
                                        c++;
                                        await botclient.SendTextMessageAsync(message.Message.Chat.Id, $"Поздравляю!\nЭто правильный ответ!\nУ вас {c} баллов");
                                        return;
                                    }
                                case "Lionel Messi,Inter Miami":
                                    {
                                        await botclient.SendTextMessageAsync(message.Message.Chat.Id, $"Не расстраивайтесь\nЭто неправильный ответ!\nУ вас {c} баллов");
                                        return;
                                    }

                                case "Jude Bellingham,Real Madrid":
                                    {
                                        await botclient.SendTextMessageAsync(message.Message.Chat.Id, $"Не расстраивайтесь\nЭто неправильный ответ!\nУ вас {c} баллов");
                                        return;
                                    }
                                case "Erling Haaland,Manchester City":
                                    {
                                        c++;
                                        await botclient.SendTextMessageAsync(message.Message.Chat.Id, $"Поздравляю!\nЭто правильный ответ!\nУ вас {c} баллов");
                                        return;
                                    }
                                case "Rodri,Manchester City":
                                    {
                                        await botclient.SendTextMessageAsync(message.Message.Chat.Id, $"Не расстраивайтесь\nЭто неправильный ответ!\nУ вас {c} баллов");
                                        return;
                                    }
                            }
                            return;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        static Task Error(ITelegramBotClient botclient, Exception exception, CancellationToken token)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
