using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.CardNamespace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonsterTradingCardsGame.Stack
{
    internal class Stack
    {
        private List<Card> _stack = new List<Card>();

        public void CreateRandomStack()
        {
            var jsonFileIn = "C:\\Users\\Nahash\\source\\repos\\MonsterTradingCardsGame\\MonsterTradingCardsGame\\CardNamespace\\CardData.json";
            dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText(jsonFileIn));
            for (int i = 0; i < 5; i++)
            {
                var rand = new Random();
                var cardType = rand.Next((int)ElementTypeEnum.Fire, (int)ElementTypeEnum.Water);
                var card = jsonFile[cardType.ToString()][rand.Next(0, 4)];
                _stack.Add(new Card((string)card[0]["name"], (int)card[0]["damage"], cardType));
            }
        }

        public void PrintStack()
        {
            foreach (var card in _stack)
            {
                card.PrintCard();
            }
        }

        public void AddToStack(Card card)
        {
            _stack.Add(card);
        }

        public void RemoveFromStack(Card card)
        {
            _stack.Remove(card);
        }

        public Card ReturnCard(string name)
        {
            var index = _stack.FindIndex(x => x.Name == name);
            return _stack[index];
        }



    }
}
