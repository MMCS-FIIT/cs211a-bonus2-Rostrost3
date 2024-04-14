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
        public static int c,i = 0; //баллы игрока,индекс правильного ответа для определённого вопроса
        public static Dictionary<string, string[]> FootballPlayers;
        public static List<string> CorrectAnswers;

        static void Main(string[] args)
        {
            FootballPlayers = new Dictionary<string, string[]>();
            FootballPlayers["https://s-cdn.sportbox.ru/images/styles/upload/fp_fotos/bd/b0/95dd9d6c42d283d22a6ef81a48f850d2657879e3a203e325196686.jpg"] = new string[] { "Cristiano Ronaldo,Al-Nassr", "Lionel Messi,Inter Miami" };
            FootballPlayers["https://www.thetimes.co.uk/imageserver/image/%2Fmethode%2Ftimes%2Fprod%2Fweb%2Fbin%2Fd060a46a-97ad-48ba-9299-adce61729dc8.jpg?crop=5000%2C2812%2C0%2C58&resize=1200"] = new string[] { "Jude Bellingham,Real Madrid", "Erling Haaland,Manchester City", "Rodri,Manchester City", "Cristiano Ronaldo,Al-Nassr" };
            FootballPlayers["https://cdn.iz.ru/sites/default/files/styles/900x506/public/news-2019-08/2019-08-29T173343Z_1926237047_RC1615819390_RTRMADP_3_SOCCER-CHAMPIONS-DRAW.jpg?itok=ravgNCj1"] = new string[] { "Erling Haaland,Manchester City", "Virgil van Dijk", "Vinícius Júnior", "Karim Benzema" };
            //Сделать рандомно, то есть ключ - цифра и цифру берём рандомно, а затем запоминаем её во множестве, чтобы не повторялось
            CorrectAnswers = new List<string>();
            CorrectAnswers.Add("Cristiano Ronaldo,Al-Nassr");
            CorrectAnswers.Add("Erling Haaland,Manchester City");
            CorrectAnswers.Add("Virgil van Dijk");
            var client = new TelegramBotClient("6668334841:AAFfb4xkbEGL-HQ_d7AEdABDL2JgH30CH7Y");
            client.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        static async Task Update(ITelegramBotClient botclient, Update update, CancellationToken token)
        {
            try
            {
                List<string> AnswId = new List<string>();
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
                                    if (message.Text.ToLower().Contains("закончить") || message.Text.ToLower().Contains("/end"))
                                    {
                                            await botclient.SendTextMessageAsync(message.Chat.Id, $"Ваши баллы: {c}\nПока ;)");
                                            
                                            return;
                                    }
                                    if (message.Text.ToLower().Contains("играть") || message.Text.ToLower().Contains("/play"))
                                    {
                                            i = 0;
                                            c = 0; //новая игра => баллы обнуляются
                                            await botclient.SendTextMessageAsync(message.Chat.Id, "Давай поиграем!\nТебе нужно будет угадать игрока\nЗа каждый правильный ответ 1 балл\nУдачи!");
                                            Thread.Sleep(2000);

                                            foreach (var x in FootballPlayers)
                                            {
                                                await botclient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri(x.Key));
                                                var a = new List<InlineKeyboardButton[]>();
                                                var inlineKeyboard1 = new InlineKeyboardMarkup(a);
                                                for(int i = 0; i < x.Value.Length; i++)
                                                {
                                                    a.Add(new InlineKeyboardButton[] { FootballPlayers[x.Key][i] });
                                                }
                                                await botclient.SendTextMessageAsync(message.Chat.Id, "Кто же это?", replyMarkup: inlineKeyboard1);

                                                // Ожидание ответа от пользователя
                                                Update selectedUpdate = null;
                                                while (selectedUpdate == null)
                                                {
                                                    //Сделать так, чтобы работали тут команды выше, всё из-за фореача?
                                                    var updates = await botclient.GetUpdatesAsync();
                                                    selectedUpdate = updates.FirstOrDefault(u => u.Type == UpdateType.CallbackQuery && u.CallbackQuery.Message.Chat.Id == message.Chat.Id && !AnswId.Contains(u.CallbackQuery.Id.ToString())); //Чтобы он не брал предыдущий ответ
                                                    // Пауза перед повторной проверкой
                                                    await Task.Delay(500);
                                                }

                                                // Обработка выбранного ответа
                                                var selectedPlayer = selectedUpdate.CallbackQuery.Data;
                                                if (CorrectAnswers[i] == selectedPlayer)
                                                {
                                                    c++;
                                                    await botclient.SendTextMessageAsync(message.Chat.Id, $"Поздравляю!\n{selectedPlayer} - правильный ответ!\nУ вас {c} балл(ов)");
                                                }
                                                else
                                                {
                                                    await botclient.SendTextMessageAsync(message.Chat.Id, $"Не расстраивайтесь\n{selectedPlayer} - неправильный ответ!\nУ вас {c} балл(ов)");
                                                }
                                                AnswId.Add(selectedUpdate.CallbackQuery.Id.ToString());
                                                i++;
                                            }
                                            return;
                                    }
                                    return;
                           }
                       }
                            return;
                    }
                    //case UpdateType.CallbackQuery:
                    //    {
                    //        var message = update.CallbackQuery;
                    //        foreach (var x in CorrectAnswers)
                    //        {
                    //            if (message.Data == x)
                    //            {
                    //                c++;
                    //                await botclient.SendTextMessageAsync(message.Message.Chat.Id, $"Поздравляю!\n{message.Data} - правильный ответ!\nУ вас {c} балл(ов)");
                    //                return;
                    //            }
                    //        }
                    //        await botclient.SendTextMessageAsync(message.Message.Chat.Id, $"Не расстраивайтесь\n{message.Data} - неправильный ответ!\nУ вас {c} балл(ов)");
                    //        return;
                    //    }
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
