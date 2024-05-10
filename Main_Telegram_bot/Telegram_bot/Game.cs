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
using Telegram.Bot.Types.Passport;

namespace Telegram_bot
{
    public class Game
    {
        public static Random r = new Random();

        public static int c = 0; //баллы игрока

        public static Dictionary<int, string[]> FootballPlayers = new Dictionary<int, string[]>(); //int для того, чтобы вопросы выпадали рандомно. Массив состоит из фото и вариантов

        public static List<string> CorrectAnswers = new List<string>(); //Правильные ответы на определённый комплект из Parsing(фото + 4 футболиста)

        public static List<int> ListOfRandoms = new List<int>(); //Чтобы вопросы не повторялись, потому что я кидаю их рандомно

        public static List<string> AnswId = new List<string>(); //Запоминаем id сообщений

        public static List<string> StickersIdCats = new List<string>(); //Стикеры котиков

        public static List<string> StickersIdPeopleMemes = new List<string>(); //Стикеры мемов

        public static Dictionary<string,int> Record = new Dictionary<string, int>(); //Рекорд: id - макс за все раунды

        public static Dictionary<string, (string, int, string)> Data_Base = new Dictionary<string, (string, int, string)>(); //База данных: id - имя,рекорд,время последнего посещения

        public static async Task Update(ITelegramBotClient botclient, Update update, CancellationToken token)
        {
            try
            {

                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            var message = update.Message;
                            var user = message.Chat.FirstName;
                            switch (message.Type)
                            {
                                case MessageType.Text:
                                    {

                                        if (message.Text.ToLower().Contains("привет") || message.Text.ToLower().Contains("/start"))
                                        {
                                            var replyKeyboard = new ReplyKeyboardMarkup(
                                                new List<KeyboardButton[]>()
                                                {
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("Привет(/start)"),
                                                    new KeyboardButton("Пока(/end)"),
                                                },
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("Играть(/play)"),
                                                    new KeyboardButton("Рекорд(/max)")
                                                }
                                                })
                                            {
                                                ResizeKeyboard = true,
                                            };

                                            if (!Data_Base.ContainsKey(message.Chat.Id.ToString()))
                                            {
                                                await botclient.SendTextMessageAsync(message.Chat.Id, $"Привет, {user}! :)\nТы тут первый раз, приятно познакомиться\nНапиши Играть(/play) чтобы начать игру!",replyMarkup: replyKeyboard);
                                                Record[message.Chat.Id.ToString()] = 0;
                                                UpdateDataBase(message, user);
                                            }
                                            else
                                            {
                                                await botclient.SendTextMessageAsync(message.Chat.Id, $"Привет, {user}! :)\nТы был тут последний раз {Data_Base[message.Chat.Id.ToString()].Item3}\nНапиши Играть(/play) чтобы начать игру!",replyMarkup: replyKeyboard);
                                                UpdateDataBase(message, user);
                                            }

                                            await botclient.SendStickerAsync(message.Chat.Id, InputFile.FromString(StickersIdCats[r.Next(StickersIdCats.Count())]));
                                            return;
                                        }
                                        if (message.Text.ToLower().Contains("пока") || message.Text.ToLower().Contains("/end"))
                                        {
                                            await botclient.SendTextMessageAsync(message.Chat.Id, $"Ваши баллы: {c}\nПока, {user} ;)");
                                            await botclient.SendStickerAsync(message.Chat.Id, InputFile.FromString("CAACAgIAAxkBAAEMDjRmN2Bm8O57fdZcQV-zyfaGXwhT9wACoBkAApvQeUh5IuR9qD2hKTUE"));
                                            UpdateDataBase(message, user);
                                            return;
                                        }
                                        if (message.Text.ToLower().Contains("рекорд") || message.Text.ToLower().Contains("/max"))
                                        {
                                            var maxx = int.MinValue;
                                            var usermax = "";
                                            foreach(var x in Data_Base.Keys)
                                            {
                                                if (Data_Base[x].Item2 > maxx)
                                                {
                                                    maxx = Data_Base[x].Item2;
                                                    usermax = Data_Base[x].Item1 + " ";
                                                }
                                                else if (Data_Base[x].Item2 == maxx)
                                                {
                                                    maxx = Data_Base[x].Item2;
                                                    usermax += Data_Base[x].Item1 + " ";
                                                }
                                            }
                                            await botclient.SendTextMessageAsync(message.Chat.Id, $"Рекорд среди всех пользователей обладает(ют) игрок(и): {usermax} = {maxx}");
                                            await botclient.SendStickerAsync(message.Chat.Id, InputFile.FromString("CAACAgIAAxkBAAEMDktmN2ZTSWSxhO3ym5hJeBdRleqX0QACwBsAAskEMUrgWHe0nAUiGzUE"));
                                            UpdateDataBase(message, user);
                                            return;
                                        }
                                        if (message.Text.ToLower().Contains("играть") || message.Text.ToLower().Contains("/play"))
                                        {
                                            await MainGame(botclient, message);
                                            return;
                                        }
                                        return;
                                    }
                                case MessageType.Sticker:
                                    {
                                        UpdateDataBase(message, user);
                                        await botclient.SendStickerAsync(message.Chat.Id, InputFile.FromString(StickersIdCats[r.Next(StickersIdCats.Count())]));
                                        return;
                                    }
                                case MessageType.Photo:
                                    {
                                        UpdateDataBase(message, user);
                                        await botclient.SendTextMessageAsync(message.Chat.Id, "Интересная фотография, но давай сыграем!(/play)");
                                        await botclient.SendStickerAsync(message.Chat.Id, InputFile.FromString(StickersIdCats[r.Next(StickersIdCats.Count())]));
                                        return;
                                    }
                                case MessageType.Voice:
                                    {
                                        UpdateDataBase(message, user);
                                        await botclient.SendTextMessageAsync(message.Chat.Id, "Мои способности не позволяют мне слушать это голосовое. Ты наверняка предлагаешь сыграть?)(/play)");
                                        await botclient.SendStickerAsync(message.Chat.Id, InputFile.FromString(StickersIdPeopleMemes[r.Next(StickersIdPeopleMemes.Count())]));
                                        return;
                                    }
                                case MessageType.Video:
                                    {
                                        UpdateDataBase(message, user);
                                        await botclient.SendTextMessageAsync(message.Chat.Id, "Я не могу посмотреть это увлекательное видео, поэтому давай поиграем!(/play)");
                                        await botclient.SendStickerAsync(message.Chat.Id, InputFile.FromString(StickersIdPeopleMemes[r.Next(StickersIdPeopleMemes.Count())]));
                                        return;
                                    }
                                case MessageType.Audio:
                                    {
                                        UpdateDataBase(message, user);
                                        await botclient.SendTextMessageAsync(message.Chat.Id, "Мои способности не позволяют мне слушать это аудио. Ты наверняка предлагаешь сыграть?)(/play)");
                                        await botclient.SendStickerAsync(message.Chat.Id, InputFile.FromString(StickersIdPeopleMemes[r.Next(StickersIdPeopleMemes.Count())]));
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

        private static async Task MainGame(ITelegramBotClient botclient, Message message)
        {
            //Чтобы он не брал предыдущий ответ(ниже обосновано)
            AnswId.Add(message.MessageId.ToString());
            c = 0; //новая игра => баллы обнуляются
            await botclient.SendTextMessageAsync(message.Chat.Id, "Давай поиграем!\nТебе нужно будет угадать игрока\nЗа каждый правильный ответ 1 балл\nУдачи!");
            Thread.Sleep(2000);

            //Играем, пока не закончится список
            while (ListOfRandoms.Count() != FootballPlayers.Count())
            {
                //Берём рандомный комплект, который мы распределяли в Parsing, из словаря(фото + 4 футболиста)
                int y = r.Next(0, FootballPlayers.Count());
                //Можем брать, если раньше не было такого комплекта(фото + 4 футболиста)
                while (ListOfRandoms.Contains(y))
                {
                    y = r.Next(0, FootballPlayers.Count());
                }
                ListOfRandoms.Add(y);

                //Некоторые картинки в файле не открываются, поэтому, если бот не может отправить фотку, то скипаем его
                try
                {
                    await botclient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri(FootballPlayers[y][0]));
                }
                catch (Exception)
                {
                    continue;
                }

                //Выводим варианты ответов
                var a = new List<InlineKeyboardButton[]>();
                var inlineKeyboard1 = new InlineKeyboardMarkup(a);
                for (int i = 1; i < FootballPlayers[y].Length; i++)
                {
                    a.Add(new InlineKeyboardButton[] { FootballPlayers[y][i] });
                }
                await botclient.SendTextMessageAsync(message.Chat.Id, "Кто же это?", replyMarkup: inlineKeyboard1);

                //Стикеры для разнообразия, иногда отправляет, иногда нет
                if(y % 3 == 0)
                {
                    await botclient.SendStickerAsync(message.Chat.Id, InputFile.FromString(StickersIdPeopleMemes[r.Next(0,StickersIdPeopleMemes.Count)]));
                }
                else if(y % 3 == 1)
                {
                    await botclient.SendStickerAsync(message.Chat.Id, InputFile.FromString(StickersIdCats[r.Next(0, StickersIdCats.Count)]));
                }

                //Ожидание ответа от пользователя
                Update selectedUpdate = null;
                while (selectedUpdate == null)
                {
                    var updates = await botclient.GetUpdatesAsync();
                    selectedUpdate = updates.FirstOrDefault(u => u.Type == UpdateType.CallbackQuery && u.CallbackQuery.Message.Chat.Id == message.Chat.Id && !AnswId.Contains(u.CallbackQuery.Id.ToString())); //Бот может взять предыдущий ответ, а не ждать нового. Поэтому тут надо это учитывать
                    //Если пришла другая команда, то надо выйти и посмотреть это. Иначе будет бесконечная игра
                    if (updates.FirstOrDefault(u => u.Type == UpdateType.Message && !AnswId.Contains(u.Message.MessageId.ToString())) != null)
                    {
                        Data_Base[message.Chat.Id.ToString()] = (Data_Base[message.Chat.Id.ToString()].Item1, Math.Max(Record[message.Chat.Id.ToString()], Data_Base[message.Chat.Id.ToString()].Item2), DateTime.Now.ToString());
                        Parsing.LoadingDataBase();
                        return;
                    }
                    // Пауза перед повторной проверкой
                    await Task.Delay(500);
                }

                //Обработка выбранного ответа
                var selectedPlayer = selectedUpdate.CallbackQuery.Data; //Фиксируем вариант ответа пользователя
                if (CorrectAnswers[y] == selectedPlayer)
                {
                    c++;
                    Record[message.Chat.Id.ToString()] = Math.Max(c, Record[message.Chat.Id.ToString()]);
                    await botclient.SendTextMessageAsync(message.Chat.Id, $"Поздравляю!\n{selectedPlayer} - правильный ответ!\nУ вас {c} балл(ов)");
                }
                else
                {
                    await botclient.SendTextMessageAsync(message.Chat.Id, $"Не расстраивайтесь\n{selectedPlayer} - неправильный ответ!\nУ вас {c} балл(ов)"); //\nПравильный ответ:\n{CorrectAnswers[y]} Добавлю, как разберусь как сделать спойлер
                }
                //Также запоминаем id, чтобы бот не брал предыдущий ответ
                AnswId.Add(selectedUpdate.CallbackQuery.Id.ToString());
            }
            if(ListOfRandoms.Count == FootballPlayers.Count)
            {
                Data_Base[message.Chat.Id.ToString()] = (Data_Base[message.Chat.Id.ToString()].Item1, Math.Max(Record[message.Chat.Id.ToString()], Data_Base[message.Chat.Id.ToString()].Item2), DateTime.Now.ToString());
                Parsing.LoadingDataBase();
                await botclient.SendTextMessageAsync(message.Chat.Id, "Ты доиграл до конца, поздравляю!\nМожешь начать заново(/play)");
            }
            return;
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

        private static void UpdateDataBase(Message message,string user)
        {
            Data_Base[message.Chat.Id.ToString()] = ($"{user}", Record[message.Chat.Id.ToString()], DateTime.Now.ToString());
            Parsing.LoadingDataBase();
        }
    }
}
