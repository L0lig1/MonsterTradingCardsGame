using MonsterTradingCardsGame.CardNamespace;


namespace MonsterTradingCardsGame.Battle
{
    public class Calculator
    {
        public static int CalculateDmgElementType(Card attackingCard, Card defendingCard)
        {
            switch (attackingCard.ElementType)
            {
                case "Fire" when defendingCard.ElementType == "Water":
                case "Normal" when defendingCard.ElementType == "Fire":
                case "Water" when defendingCard.ElementType == "Normal":
                    return attackingCard.Damage / 2;
                case "Water" when defendingCard.ElementType == "Fire":
                case "Fire" when defendingCard.ElementType == "Normal":
                case "Normal" when defendingCard.ElementType == "Water":
                    return attackingCard.Damage * 2;
                default:
                    return attackingCard.Damage;
            }
        }

        public int CalculateEloRating(int playerARating, int playerBRating, double score)
        {
            // Calculate the expected score for player A
            double expectedScore = CalculateExpectedScore(playerARating, playerBRating);

            // Determine the K factor to use based on the rating of player A
            int kFactor = DetermineKFactor(playerARating);

            // Calculate the rating change for player A
            int ratingChange = (int)Math.Round(kFactor * (score - expectedScore));

            // Return the updated rating for player A
            return playerARating + ratingChange;
        }

        private static double CalculateExpectedScore(int playerARating, int playerBRating)
        {
            double denominator = 1 + Math.Pow(10, (playerBRating - playerARating) / 400.0);
            return 1 / denominator;
        }

        private static int DetermineKFactor(int playerRating)
        {
            return playerRating switch
            {
                < 200 => 32,
                < 300 => 24,
                _ => 16
            };
        }

        public (bool, int) CalculateDamageCardType(Card attackingCard, Card defendingCard)
        {
            if
            (
                (attackingCard.Name.Contains("Goblin") && defendingCard.Name == "Dragon") ||
                (attackingCard.Name == "Ork" && defendingCard.Name == "Wizzard") ||
                (defendingCard.Name == "Kraken" && attackingCard.CardType == "spell") ||
                (attackingCard.Name == "Dragon" && defendingCard.Name == "FireElf") ||
                (attackingCard.Name == "Knight" && defendingCard.Name == "WaterSpell")
            )
            {
                return (true, 0);
            }
            if (attackingCard.CardType == "monster" && defendingCard.CardType == "monster")
            {
                return (true, attackingCard.Damage);
            }
            return (false, defendingCard.Damage);
        }

        public int CalculateDamage(Card attackingCard, Card defendingCard)
        {
            var cardtypedmg = CalculateDamageCardType(attackingCard, defendingCard);
            return cardtypedmg.Item1
                ? cardtypedmg.Item2
                : CalculateDmgElementType(attackingCard, defendingCard);
        }

    }
}
