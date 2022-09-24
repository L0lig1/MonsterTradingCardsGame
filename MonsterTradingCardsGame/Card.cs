using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CardInterface
{
    internal class Card
    {

        public string name;
        private int _damage;
        private int elementType;
        private int cardType;



        public string Name   // property
        {
            get => name;         // get method
            set => name = value; // set method
        }
        
        public int ElementType   // property
        {
            get => elementType;         // get method
            set
            {
                switch (value)
                {
                    case 1: // Feuer
                        elementType = 1;
                        break;
                    case 2: // Wasser
                        elementType = 2;
                        break;
                    case 3: // Normal
                        elementType = 3;
                        break;
                    default:
                        Console.WriteLine("Ungueltig");
                        break;
                }
            } 
        }
    }
}
