using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonsterTradingCardsGame.CardNamespace
{
    internal class Deck
    {
        private List<Card> _deck = new();



        public void CreateRandomDeck()
        {
            var jsonFileIn = "C:\\Users\\Nahash\\source\\repos\\MonsterTradingCardsGame\\MonsterTradingCardsGame\\CardNamespace\\CardData.json";
            dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText(jsonFileIn));
            for (int i = 0; i < 4; i++)
            {
                var rand = new Random();
                var cardType = rand.Next((int)ElementTypeEnum.Fire, (int)ElementTypeEnum.Water + 1);
                var card = jsonFile[cardType.ToString()][rand.Next(0, 4)];
                _deck.Add(new Card((string)card[0]["name"], (int)card[0]["damage"], cardType));
            }
            Console.WriteLine("Deck created!");
        }

        public void CreateCustomDeck(List<Card> stack)
        {
            //var jsonFileIn = "C:\\Users\\Nahash\\source\\repos\\MonsterTradingCardsGame\\MonsterTradingCardsGame\\CardNamespace\\CardData.json";
            //dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText(jsonFileIn));
            //List<Card> cards = new();
            //var cardCount = 1;
            //for (int i = 1; i <= 3; i++)
            //{
            //    for (int j = 0; j < 5; j++)
            //    {
            //        var card = jsonFile[i.ToString()][j];
            //        cards.Add(new Card((string)card[0]["name"], (int)card[0]["damage"], i));
            //        Console.WriteLine($"Card {cardCount++}: {card[0]["name"]}, i, {card[0]["damage"]}");
            //    }
            //}

            List<Card> cards = stack;

            while (_deck.Count != 4)
            {
                var count = 1;
                foreach (var card in cards)
                {
                    Console.WriteLine($"Card {count++}:");
                    card.PrintCard();
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
                card.PrintCard();
            }
        }

        public void AddToDeck(Card card)
        {
            _deck.Add(card);
        }

        public void RemoveFromDeck(Card card)
        {
            _deck.Remove(card);
        }

    }
}
