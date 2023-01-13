namespace MonsterTradingCardsGame.CardNamespace
{
    public class Card
    {
        public Card(string id, string name, int damage, string elementType, string cardType)
        {
            Id = id;
            Name = name;
            Damage = damage;
            ElementType = elementType;
            CardType = cardType;
        }

        public string Name { get; set; }
        public string Id { get; set; }
        private int _damage;
        private string _elementType = string.Empty,
                       _cardType    = string.Empty;
        public string[] ValidElementTypes = new CardTypes().ValidElementTypes;
        public string[] ValidCardTypes    = new CardTypes().ValidCardTypes;

        public string CardType   
        {
            get => _cardType;         
            set
            {
                if (ValidCardTypes.Any(value.Contains))
                {
                    _cardType = value;
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
            return $"{Name} ({Damage}) ({CardType}) ({ElementType})";
        }

    }
}
