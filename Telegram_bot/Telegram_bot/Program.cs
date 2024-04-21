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
        public static Random r = new Random();

        public static int c,i = 0; //баллы игрока,индекс правильного ответа для определённого вопроса

        public static Dictionary<int, string[]>? FootballPlayers; //int для того, чтобы вопросы выпадали рандомно. Массив состоит из фото и вариантов

        public static List<string>? CorrectAnswers;

        public static List<int>? ListOfRandoms; //Чтобы вопросы не повторялись, потому что я кидаю их рандомно1

        static void Main(string[] args)
        {
            FootballPlayers = new Dictionary<int, string[]>();
            //FootballPlayers[0] = new string[] { "https://s-cdn.sportbox.ru/images/styles/upload/fp_fotos/bd/b0/95dd9d6c42d283d22a6ef81a48f850d2657879e3a203e325196686.jpg","Cristiano Ronaldo", "Lionel Messi" };
            //FootballPlayers[1] = new string[] { "https://www.thetimes.co.uk/imageserver/image/%2Fmethode%2Ftimes%2Fprod%2Fweb%2Fbin%2Fd060a46a-97ad-48ba-9299-adce61729dc8.jpg?crop=5000%2C2812%2C0%2C58&resize=1200","Jude Bellingham", "Erling Haaland", "Rodri", "Cristiano Ronaldo" };
            //FootballPlayers[2] = new string[] { "https://cdn.iz.ru/sites/default/files/styles/900x506/public/news-2019-08/2019-08-29T173343Z_1926237047_RC1615819390_RTRMADP_3_SOCCER-CHAMPIONS-DRAW.jpg?itok=ravgNCj1","Erling Haaland", "Virgil van Dijk", "Vinícius Júnior", "Karim Benzema" };
            
            CorrectAnswers = new List<string>();
            //CorrectAnswers.Add("Cristiano Ronaldo");
            //CorrectAnswers.Add("Erling Haaland");
            //CorrectAnswers.Add("Virgil van Dijk");

            Parsing.Parssing();
            var client = new TelegramBotClient("6668334841:AAFfb4xkbEGL-HQ_d7AEdABDL2JgH30CH7Y");
            client.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        static async Task Update(ITelegramBotClient botclient, Update update, CancellationToken token)
        {
            try
            {
                List<string> AnswId = new List<string>();
                List<int> ListOfRandoms = new List<int>();
                switch (update.Type)
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
                                            AnswId.Add(message.MessageId.ToString());
                                            c = 0; //новая игра => баллы обнуляются
                                            await botclient.SendTextMessageAsync(message.Chat.Id, "Давай поиграем!\nТебе нужно будет угадать игрока\nЗа каждый правильный ответ 1 балл\nУдачи!");
                                            Thread.Sleep(2000);

                                            while(ListOfRandoms.Count() != FootballPlayers.Count())
                                            {
                                                int y = r.Next(0, FootballPlayers.Count());
                                                while(ListOfRandoms.Contains(y))
                                                {
                                                    y = r.Next(0, FootballPlayers.Count());
                                                }
                                                ListOfRandoms.Add(y);

                                                await botclient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri(FootballPlayers[y][0]));
                                                var a = new List<InlineKeyboardButton[]>();
                                                var inlineKeyboard1 = new InlineKeyboardMarkup(a);
                                                for(int i = 1; i < FootballPlayers[y].Length; i++)
                                                {
                                                    a.Add(new InlineKeyboardButton[] { FootballPlayers[y][i] });
                                                }
                                                Message b = await botclient.SendTextMessageAsync(message.Chat.Id, "Кто же это?", replyMarkup: inlineKeyboard1);

                                                // Ожидание ответа от пользователя
                                                Update selectedUpdate = null;
                                                while (selectedUpdate == null)
                                                {
                                                    var updates = await botclient.GetUpdatesAsync();
                                                    selectedUpdate = updates.FirstOrDefault(u => u.Type == UpdateType.CallbackQuery && u.CallbackQuery.Message.Chat.Id == message.Chat.Id && !AnswId.Contains(u.CallbackQuery.Id.ToString())); //Чтобы он не брал предыдущий ответ
                                                    if (updates.FirstOrDefault(u => u.Type == UpdateType.Message && !AnswId.Contains(u.Message.MessageId.ToString())) != null)
                                                    {
                                                        return;
                                                    }
                                                    // Пауза перед повторной проверкой
                                                    await Task.Delay(500);
                                                }

                                                // Обработка выбранного ответа
                                                var selectedPlayer = selectedUpdate.CallbackQuery.Data;
                                                if (CorrectAnswers[y] == selectedPlayer)
                                                {
                                                    c++;
                                                    await botclient.SendTextMessageAsync(message.Chat.Id, $"Поздравляю!\n{selectedPlayer} - правильный ответ!\nУ вас {c} балл(ов)");
                                                }
                                                else
                                                {
                                                    await botclient.SendTextMessageAsync(message.Chat.Id, $"Не расстраивайтесь\n{selectedPlayer} - неправильный ответ!\nУ вас {c} балл(ов)");
                                                }
                                                AnswId.Add(selectedUpdate.CallbackQuery.Id.ToString());
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
