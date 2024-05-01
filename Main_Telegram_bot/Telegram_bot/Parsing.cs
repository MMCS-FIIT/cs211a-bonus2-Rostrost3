using System.Text.RegularExpressions;

namespace Telegram_bot
{
    internal class Parsing
    {
        public static void Parssing()
        {
            var c = 0;
            var p = File.ReadAllLines("Files/Fifa23.csv");
            for (int i = 1; i < p.Length-3; i += 3)
            {
                var a = new string[] { p[i], p[i + 1], p[i + 2], p[i + 3] };
                var b = new List<string[]>(); //Сплит всех игроков
                for(int x = 0;x < a.Length;x++)
                {
                    b.Add(a[x].Split(new char[] { ',' }));
                }

                var y = Game.r.Next(0, a.Length); //Рандомный игрок
                var z = Regex.Match(string.Join('\n', b[y]),@"\bhttps.*\.png"); //Фотография
                Game.CorrectAnswers.Add(b[y][1]); //Правильный ответ
                Game.FootballPlayers[c] = new string[] { z.ToString(), b[0][1], b[1][1], b[2][1], b[3][1] }; //Варианты ответа
                c++;
            }
        }

        public static void ParssingStickers()
        {
            Game.StickersIdCats = File.ReadAllLines("Files/StickersIdCats.txt").ToList();
            Game.StickersIdPeopleMemes = File.ReadAllLines("Files/StickersIdPeopleMemes.txt").ToList();
        }
    }
}
