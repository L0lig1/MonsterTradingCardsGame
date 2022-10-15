using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.ParseJSON;

namespace MonsterTradingCardsGame.CardNamespace
{
    public class Deck
    {
        private List<Card> _deck = new();

        public Card ReturnDeckAtIndex(int index)
        {
            if (index <= _deck.Count && index >= 0 && _deck.Count > 0)
            {
                return _deck[index];
            }
            return null;
        }

        public void CreateRandomDeck()
        {
            var json = new JSON();
            for (int i = 0; i < 4; i++)
            {
                _deck.Add(json.GetRandomCard());
            }
            Console.WriteLine("Deck created!");
        }

        public void CreateCustomDeck(List<Card> stack)
        {

            List<Card> cards = stack;

            while (_deck.Count != 4)
            {
                var count = 1;
                foreach (var card in cards)
                {
                    Console.WriteLine($"Card {count++}:");
                    Console.WriteLine(card.PrintCard());
                }
                var choice = 200;
                while (choice <= 0 || choice > cards.Count)
                {
                    Console.WriteLine($"Choose a card (1-{cards.Count})");
                    choice = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
                }

                choice -= 1;
                Console.WriteLine($"You chose {cards[choice].Name}, {cards[choice].ElementType}, {cards[choice].Damage}\n");

                var confirm = "";
                while (confirm is not ("Y" or "N"))
                {
                    Console.WriteLine("Confirm choice: (Y|N)\n");
                    confirm = Console.ReadLine();
                }
                switch (confirm)
                {
                    case "Y":
                        Console.WriteLine("Card added to deck successfully!\n");
                        _deck.Add(cards[choice]);
                        break;
                    case "N":
                        Console.WriteLine("Choice canceled!\n");
                        break;
                    default:
                        Console.WriteLine("Unexpected error\n");
                        break;
                }

                cards.Remove(cards[choice]);
            }
            
        }

        public void PrintDeck()
        {
            foreach (var card in _deck)
            {
                Console.WriteLine(card.PrintCard());
            }
        }

        // sind diese Funktionen notwendig oder deck returnen und diese Sachen ausführen?
        public void AddToDeck(Card card)
        {
            _deck.Add(card);
        }

        public int Length()
        {
            return _deck.Count;
        }
        
        public void RemoveFromDeck(Card card)
        {
            _deck.Remove(card);
        }

    }
}
