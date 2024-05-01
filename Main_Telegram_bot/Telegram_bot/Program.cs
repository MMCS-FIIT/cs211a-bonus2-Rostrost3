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

        static void Main(string[] args)
        {
            //FootballPlayers[0] = new string[] { "https://s-cdn.sportbox.ru/images/styles/upload/fp_fotos/bd/b0/95dd9d6c42d283d22a6ef81a48f850d2657879e3a203e325196686.jpg","Cristiano Ronaldo", "Lionel Messi" };
            //FootballPlayers[1] = new string[] { "https://www.thetimes.co.uk/imageserver/image/%2Fmethode%2Ftimes%2Fprod%2Fweb%2Fbin%2Fd060a46a-97ad-48ba-9299-adce61729dc8.jpg?crop=5000%2C2812%2C0%2C58&resize=1200","Jude Bellingham", "Erling Haaland", "Rodri", "Cristiano Ronaldo" };
            //FootballPlayers[2] = new string[] { "https://cdn.iz.ru/sites/default/files/styles/900x506/public/news-2019-08/2019-08-29T173343Z_1926237047_RC1615819390_RTRMADP_3_SOCCER-CHAMPIONS-DRAW.jpg?itok=ravgNCj1","Erling Haaland", "Virgil van Dijk", "Vinícius Júnior", "Karim Benzema" };
            
            //CorrectAnswers.Add("Cristiano Ronaldo");
            //CorrectAnswers.Add("Erling Haaland");
            //CorrectAnswers.Add("Virgil van Dijk");

            Parsing.Parssing(); //Файл с футболистами
            Parsing.ParssingStickers(); //Файлы со стикерами

            var client = new TelegramBotClient("6668334841:AAFfb4xkbEGL-HQ_d7AEdABDL2JgH30CH7Y");

            client.StartReceiving(Game.Update, Game.Error);
            Console.ReadLine();
        }
    }
}
