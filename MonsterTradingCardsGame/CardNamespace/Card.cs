using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame;
using MonsterTradingCardsGame.CardNamespace;


namespace MonsterTradingCardsGame.CardNamespace
{
    public class Card
    {
        //Random _rnd = new();
        private int _damage;
        private int _elementType;
        private int _cardType;

        public Card(string name, int damage, int elementType, int cardType)
        {
            Name = name;
            Damage = damage;
            ElementType = elementType;
            CardType = cardType;
        }

        public string PrintCard()
        {
            return $"{Name} ({EnumToStringEt()} type) ({Damage} damage)";
        }

        public string Name { get; set; }

        public string EnumToStringCt()
        {
            return CardType switch
            {
                (int)CardTypeEnum.Monster => "Monster",
                (int)CardTypeEnum.Spell => "Spell",
                _ => ""
            };
        }
        public string EnumToStringEt()
        {
            return ElementType switch
            {
                (int)ElementTypeEnum.Fire => "Fire",
                (int)ElementTypeEnum.Normal => "Normal",
                (int)ElementTypeEnum.Water => "Water",
                _ => ""
            };
        }
        
        public int CardType   
        {
            get => _cardType;         
            set
            {
                switch (value)
                {
                    case (int)CardTypeEnum.Monster:
                        _cardType = (int)CardTypeEnum.Monster;
                        break;
                    case (int)CardTypeEnum.Spell:
                        _cardType = (int)CardTypeEnum.Spell;
                        break;
                    default:
                        Console.WriteLine("Ungueltig");
                        break;
                }
            } 
        }
        
        public int ElementType   
        {
            get => _elementType;         
            set
            {
                switch (value)
                {
                    case (int)ElementTypeEnum.Fire: 
                        _elementType = (int)ElementTypeEnum.Fire;
                        break;
                    case (int)ElementTypeEnum.Normal: 
                        _elementType = (int)ElementTypeEnum.Normal;
                        break;
                    case (int)ElementTypeEnum.Water: 
                        _elementType = (int)ElementTypeEnum.Water;
                        break;
                    default:
                        Console.WriteLine("Ungueltig");
                        break;
                }
            } 
        }

        public int Damage
        {
            get => _damage;
            set
            {
                if (value <= 200 && value > 0)
                    _damage = value;
                else
                    Console.WriteLine("Ungueltig");
            }
        }
    }
}
