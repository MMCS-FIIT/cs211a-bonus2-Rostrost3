using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace Telegram_bot
{
    public class Game
    {
        public static Random r = new Random();

        public static int c = 0; //баллы игрока

        public static Dictionary<int, string[]> FootballPlayers = new Dictionary<int, string[]>(); //int для того, чтобы вопросы выпадали рандомно. Массив состоит из фото и вариантов

        public static List<string> CorrectAnswers = new List<string>();

        public static List<int> ListOfRandoms = new List<int>(); //Чтобы вопросы не повторялись, потому что я кидаю их рандомно

        public static List<string> AnswId = new List<string>();

        public static async Task Update(ITelegramBotClient botclient, Update update, CancellationToken token)
        {
            try
            {
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
                                            await MainGame(botclient, message);
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

        private static async Task MainGame(ITelegramBotClient botclient, Message message)
        {
            AnswId.Add(message.MessageId.ToString());
            c = 0; //новая игра => баллы обнуляются
            await botclient.SendTextMessageAsync(message.Chat.Id, "Давай поиграем!\nТебе нужно будет угадать игрока\nЗа каждый правильный ответ 1 балл\nУдачи!");
            Thread.Sleep(2000);

            while (ListOfRandoms.Count() != FootballPlayers.Count())
            {
                int y = r.Next(0, FootballPlayers.Count());
                while (ListOfRandoms.Contains(y))
                {
                    y = r.Next(0, FootballPlayers.Count());
                }
                ListOfRandoms.Add(y);

                try
                {
                    await botclient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri(FootballPlayers[y][0]));
                }
                catch (Exception)
                {
                    continue;
                }

                var a = new List<InlineKeyboardButton[]>();
                var inlineKeyboard1 = new InlineKeyboardMarkup(a);
                for (int i = 1; i < FootballPlayers[y].Length; i++)
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
        }

        public static Task Error(ITelegramBotClient botclient, Exception exception, CancellationToken token)
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
