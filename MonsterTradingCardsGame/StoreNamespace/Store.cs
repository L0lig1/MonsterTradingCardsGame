using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.StoreNamespace;
using MonsterTradingCardsGame.CardNamespace;
using user;



namespace MonsterTradingCardsGame.StoreNamespace
{
    internal class Store
    {
        private List<Package> _packages = new List<Package>();
        public bool AUTOMATICGAME = true;

        public Store()
        {
            for (var i = 0; i < 5; i++)
            {
                _packages.Add(new Package());
            }

        }

        public void Trade(User user1, User user2)
        {
            Card user1CardToTrade = user1.TradeCard();
            Console.WriteLine("What is your requirement for the trade?");
            var requirement = Console.ReadLine();
            Console.WriteLine($"Player {user1.Name}: adds {user1CardToTrade.Name} ({user1CardToTrade.Damage} damage) in the store and wants {requirement}");
            // TODO
        }

        
        public Package BuyPackage(int coins)
        {
            Console.WriteLine("\nWelcome to the store!\nHere are some packages you could buy");
            if (coins >= 5)
            {
                PrintPackages();
                var rand = new Random();
                var packageChoice = 8;
                while (packageChoice is > 5 or < 1)
                {
                    Console.WriteLine("Choose a package");
                    packageChoice = AUTOMATICGAME ? rand.Next(1,5) : int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
                    //packageChoice = rand.Next(1,5);
                }

                Console.WriteLine($"\nYou chose {packageChoice}");
                Console.WriteLine("Purchase successful!");
                return _packages[packageChoice-1];
                
            }
            Console.WriteLine($"Not enough coins! You have {coins} and you need to have 5");
            return null;
        }


        public void PrintPackages()
        {
            var count = 1;
            foreach (var package in _packages)
            {
                Console.WriteLine($"\nPackage nr {count++}");
                package.PrintPackage();
            }
        }
    }
}
