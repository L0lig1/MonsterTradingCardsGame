using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame;
using MonsterTradingCardsGame.StoreNamespace;
using MonsterTradingCardsGame.CardNamespace;
using user;

namespace MonsterTradingCardsGame.Game
{
    internal class Battle
    {
        public Battle(User user1, User user2)
        {
            this.user1 = user1;
            this.user2 = user2;
        }
        private User user1;
        private User user2;
        

        public void createDecks()
        {
            
            user1.CustomOrRandomDeck();
            Console.WriteLine($"Congrats {user1.Name}, you created a Deck!");
            user1.PrintDeck();
            user2.CustomOrRandomDeck();
            Console.WriteLine($"Congrats {user2.Name}, you created a Deck!");
            user2.PrintDeck();
        }


        public void fight()
        {

        }        

    }
}
