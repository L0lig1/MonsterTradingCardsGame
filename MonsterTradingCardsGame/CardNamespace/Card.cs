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
        private string _id;
        private string _elementType;
        private string _cardType;

        public Card(string id, string name, int damage, string elementType, string cardType)
        {
            Id = id;
            Name = name;
            Damage = damage;
            var rnd = new Random();
            ElementType = ValidElementTypes[rnd.Next(0, 3)];
            CardType = ValidElementTypes[rnd.Next(0, 2)];
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public string[] ValidElementTypes = { "Fire", "Water", "Normal" };
        public string[] ValidCardTypes = { "Monster", "Spell" };

        public string CardType   
        {
            get => _cardType;         
            set
            {
                if (ValidCardTypes.Any(value.Contains))
                {
                    _elementType = value;
                } 
                else
                {
                    Console.WriteLine($"Invalid element type. Valid element types: {ValidCardTypes}");
                }
            } 
        }
        
        public string ElementType   
        {
            get => _elementType;         
            set
            {
                if (ValidElementTypes.Any(value.Contains))
                {
                    _elementType = value;
                }
                else
                {
                    Console.WriteLine($"Invalid element type. Valid element types: {ValidElementTypes}");
                }
            } 
        }

        public int Damage
        {
            get => _damage;
            set
            {
                if (value is <= 200 and > 0)
                    _damage = value;
                else
                    Console.WriteLine("Ungueltig");
            }
        }

        public string PrintCard()
        {
            return $"{Name} ({ElementType} type) ({Damage} damage)";
        }
    }
}
