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

            if (coins >= 5)
            {
                PrintPackages();
                var packageChoice = 8;
                while (packageChoice is > 4 or < 0)
                {
                    Console.WriteLine("Choose a package\n");
                    packageChoice = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
                }

                Console.WriteLine($"You chose {packageChoice}\n");

                var choice = "";
                while (choice is not ("Y" or "N"))
                {
                    Console.WriteLine("You have enough coins. Confirm purchase: (Y|N)\n");
                    choice = Console.ReadLine();
                }
                switch (choice)
                {
                    case "Y":
                        Console.WriteLine("Purchase successful!\n");
                        return _packages[packageChoice];
                    case "N":
                        Console.WriteLine("Purchase canceled!\n");
                        return null;
                    default:
                        Console.WriteLine("Unexpected error\n");
                        return null;
                }
            }
            Console.WriteLine($"Not enough coins! You have {coins} and you need to have 5");
            return null;
        }


        public void PrintPackages()
        {
            var count = 0;
            foreach (var package in _packages)
            {
                Console.WriteLine($"\nPackage nr {count++}");
                package.PrintPackage();
            }
            
        }
    }
}
