using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.CardNamespace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonsterTradingCardsGame.ParseJSON
{
    internal class JSON
    {
        public static string jsonFileIn = "C:\\Users\\Nahash\\source\\repos\\MonsterTradingCardsGame\\MonsterTradingCardsGame\\CardNamespace\\CardData.json";
        public dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText(jsonFileIn));


        public static int MonsterCardCount = 0;
        public static int SpellCardCount = 0;

        public JSON()
        {

        }

        public dynamic getJson()
        {
            return JsonConvert.DeserializeObject(File.ReadAllText(jsonFileIn));
        }

        public List<Card> jsonGetCardList()
        {
            var allCards = new List<Card>();
            for (int i = 1; i <= 2; i++)
            {
                foreach (var item in jsonFile[i.ToString()])
                {
                    //allCards.Add(new Card((string)item.name, (int)item.damage, (int)item.type, i));
                }
            }

            Console.WriteLine(allCards);

            return allCards;
        }


        public Card GetRandomCard()
        {
            var rand = new Random();
            var cardType = rand.Next(1, 3);
            var item = jsonFile[cardType.ToString()][(cardType == 1 ? rand.Next(0,6) : rand.Next(0, 15))];
            return new Card("id", (string)item.name, (int)item.damage, "(int)item.type", "cardType");
        }
    }
}
