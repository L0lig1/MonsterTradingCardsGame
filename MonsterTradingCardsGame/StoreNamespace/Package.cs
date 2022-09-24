using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.CardNamespace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonsterTradingCardsGame.StoreNamespace
{
    internal class Package
    {
        private int _price;
        public List<Card> package = new List<Card>();

        public int Price { 
            get => _price;
            set => _price = value;
        }

        public List<Card> PackageProperty
        {
            get;
        }

        public void PrintPackage()
        {
            foreach (var card in package)
            {
                card.PrintCard();
            }
        }
        public Package()
        {
            const string jsonFileIn = "C:\\Users\\Nahash\\source\\repos\\MonsterTradingCardsGame\\MonsterTradingCardsGame\\CardNamespace\\CardData.json";
            dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText(jsonFileIn)) ?? throw new InvalidOperationException();
            for (var i = 0; i < 5; i++)
            {
                var rand = new Random();
                var cardType = rand.Next((int)ElementTypeEnum.Fire, (int)ElementTypeEnum.Water + 1);
                var card = jsonFile[cardType.ToString()][rand.Next(0, 4)];
                package.Add(new Card((string)card[0]["name"], (int)card[0]["damage"], cardType));
            }

            Price = 5;
        }
    }
}
