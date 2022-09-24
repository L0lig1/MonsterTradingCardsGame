using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.CardNamespace;
using MonsterTradingCardsGame.Stack;

namespace user
{
    class User
    {
        public string name; // field
        public Stack stack;

        public string Name   // property
        {
            get => name; 
            set => name = value; 
        }
    }
}


