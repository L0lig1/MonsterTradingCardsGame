using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.CardNamespace;

namespace MonsterTradingCardsGame.CardNamespace
{
    internal class Stack : IEnumerable
    {
        private List<Card> _stack = new();
        public void PrintStack()
        {
            var counter = 1;
            foreach (var card in _stack)
            {
                Console.Write($"Card {counter++}: ");
                Console.WriteLine(card.PrintCard());
            }
        }

        public List<Card> ReturnDeck()
        {
            return _stack;
        }

        public int StackLength()
        {
            return _stack.Count;
        }

        public void AddToStack(Card card)
        {
            _stack.Add(card);
        }

        public void RemoveFromStack(Card card)
        {
            _stack.Remove(card);
        }
        public void RemoveFromStackAtIndex(int cardAtIndex)
        {
            _stack.RemoveAt(cardAtIndex);
        }


        public Card ReturnCard(int index)
        {
            if (index >= 0 || index < _stack.Count)
            {
                return _stack[index];
            }

            return null;
        }


        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
