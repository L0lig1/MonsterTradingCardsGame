using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.CardNamespace;
using MonsterTradingCardsGame.StoreNamespace;

namespace user
{
    public class User
    {
        private string name;
        private int _coins = 20;
        public Deck _deck = new();
        private readonly Stack _stack = new();
        public bool AUTOMATICGAME = true;

        public User(string name)
        {
            this.Name = name;
        }

        public string Name   // property
        {
            get => name;
            set => name = value.Length < 25 ? value : "AutoNameCzNameTooLong";
        }

        public Card ReturnRandomCardFromDeck()
        {
            var rand = new Random();
            return _deck.ReturnDeckAtIndex(rand.Next(0,_deck.Length()));
        }

        public void CustomOrRandomDeck()
        {
            var choice = "";
            while (choice is not ("C" or "R"))
            {
                Console.WriteLine("Dou you want to create a (C)ustom deck yourself or get a (R)andom one?");
                choice = AUTOMATICGAME ? "R" : Console.ReadLine();
                //choice = "R";
            }

            switch (choice)
            {
                case "C":
                    _deck.CreateCustomDeck(_stack.ReturnDeck());
                    break;
                case "R":
                    _deck.CreateRandomDeck();
                    break;
                default:
                    Console.WriteLine("BRUH");
                    break;
            }
        }

        public int Coins   // property
        {
            get => _coins; 
            set => _coins = value; 
        }

        public void AddPackageToStack(Package cardInPackage)
        {
            foreach (var card in cardInPackage.package)
            {
                _stack.AddToStack(card);
            }
        }

        public void PrintStack()
        {
            _stack.PrintStack();
        }
        public void PrintDeck()
        {
            _deck.PrintDeck();
        }

        public Card TradeCard()
        {
            PrintStack();
            var option = 8;
            while (option > _stack.StackLength() || option < 0)
            {
                Console.WriteLine($"Which card do you want to trade? (1-{_stack.StackLength()})");
                option = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
            }
            
            return _stack.ReturnCard(option);
        }

        public void ManageStack()
        {
            PrintStack();
            Console.WriteLine("Do you want to change anything in your deck?\n" +
                              "(R)emove, (T)rade,");
        }

    }
}


