using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame;


namespace MonsterTradingCardsGame.Card
{
    internal class Card
    {
        public Card()
        {

        }



        Random _rnd = new Random();
        public string name;
        private int _damage;
        private int _elementType;
        private int _cardType;



        public string Name   // property
        {
            get => name;         // get method
            set => name = value; // set method
        }
        
        public int ElementType   // property
        {
            get => _elementType;         // get method
            set
            {
                switch (value)
                {
                    case 1: // Feuer
                        _elementType = 1;
                        break;
                    case 2: // Wasser
                        _elementType = 2;
                        break;
                    case 3: // Normal
                        _elementType = 3;
                        break;
                    default:
                        Console.WriteLine("Ungueltig");
                        break;
                }
            } 
        }

        public int Damage
        { get => _damage; set => _damage = value; }
    }
}
