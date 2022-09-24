using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame;
using MonsterTradingCardsGame.CardNamespace;


namespace MonsterTradingCardsGame.CardNamespace
{
    internal class Card
    {
        public Card(string name, int damage, int elementType)
        {
            Name = name;
            Damage = damage;
            ElementType = elementType;
        }



        Random _rnd = new Random();
        public string name;
        private int _damage;
        private int _elementType;
        private int _cardType;


        public void PrintCard()
        {
            Console.WriteLine($"{Name}, {ElementType}, {Damage}");
        }
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
                    case 1: 
                        _elementType = (int)ElementTypeEnum.Fire;
                        break;
                    case 2: 
                        _elementType = (int)ElementTypeEnum.Normal;
                        break;
                    case 3: 
                        _elementType = (int)ElementTypeEnum.Water;
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
