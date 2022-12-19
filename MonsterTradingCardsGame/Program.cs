﻿using MonsterTradingCardsGame.ClientServer;
using MonsterTradingCardsGame.DBconn;
using MonsterTradingCardsGame.StoreNamespace;
using user;


// for automatic game (deck creation, battle etc), change AUTOMATIC var in Store and User

namespace MonsterTradingCardsGame
{
    class Program
    {
        public static void Main(String[] args)
        {
            //var server = new Server();
            //server.Listen();
            var db = new DB();
            db.Connect();
            db.AddCardToStack("Marcin", "2", db.Conn);
            db.AddCardToStack("Marcin", "1", db.Conn);

            //db.Scoreboard();
            //db.UpdateUserStats("Marcin", -100);
            //db.UserStats("Marcin");

            //db.CreateCard("845f0dc7-37d0-426e-994e-43fc3ac83c08", "WaterGoblin", 10, "Monster", "Water");

            //var user1 = new User("User 1");
            //var user2 = new User("User 2");
            //Package package = null;
            //var store = new Store();
            //var choice = "A";
            //
            //Console.WriteLine("Welcome to MonsterTradingCardGame!");
            //while (choice != "X")
            //{
            //    Console.WriteLine("(B)attle, (T)rade, (S)tore, e(X)it");
            //    choice = Console.ReadLine();
            //    switch (choice)
            //    {
            //        case "B":
            //            Battle.Battle battle = new(user1, user2);
            //            battle.CreateDecks();
            //            battle.Fight();
            //            break;
            //        case "S":
            //            package = store.BuyPackage(user1.Coins);
            //            if (package != null)
            //            {
            //                user1.AddPackageToStack(package);
            //            }
            //            user1.PrintStack();
            //            package = store.BuyPackage(user2.Coins);
            //            if (package != null)
            //            {
            //                user2.AddPackageToStack(package);
            //            }
            //            user2.PrintStack();
            //            break;
            //        case "T":
            //            break;
            //        case "X":
            //            Console.WriteLine("Thanks for playing MonsterTradingCardGame!");
            //            break;
            //        default: 
            //            Console.WriteLine("Wrong Input");
            //            break;
            //    }
            //}
        }
    }
}

