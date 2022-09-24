using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardInterface;

namespace user
{
    class User
    {
        public string name; // field
        public Card[] Stack;

        public string Name   // property
        {
            get => name; 
            set => name = value; 
        }
    }
}


