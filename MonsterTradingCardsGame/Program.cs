// See https://aka.ms/new-console-template for more information

using System.Reflection.Metadata;
using System;
using System.Net;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MonsterTradingCardsGame.StoreNamespace;
using MonsterTradingCardsGame;
using MonsterTradingCardsGame.Game;
using user;

namespace HelloWorld
{
    class Program
    {
        public static void Main(String[] args)
        {
            var user1 = new User();
            var store = new Store();
            var package = store.BuyPackage(user1.Coins);
            if (package != null)
            {
                user1.AddPackageToStack(package);
            }
            user1.PrintStack();
            var user2 = new User();
            package = store.BuyPackage(user2.Coins);
            if (package != null)
            {
                user2.AddPackageToStack(package);
            }
            user1.PrintStack();
            Battle battle = new(user1, user2);
            battle.createDecks();
        }
        
    }
}

