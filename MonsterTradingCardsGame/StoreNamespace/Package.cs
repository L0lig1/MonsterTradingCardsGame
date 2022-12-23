using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.CardNamespace;
using MonsterTradingCardsGame.ParseJSON;


namespace MonsterTradingCardsGame.StoreNamespace
{
    public class Package

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
                Console.WriteLine(card.PrintCard());
            }
        }
        public Package()
        {
            var cardDataJson = new JSON();
            for (var i = 0; i < 5; i++)
            {
                package.Add(cardDataJson.GetRandomCard());
            }

            Price = 5;
        }
    }
}
