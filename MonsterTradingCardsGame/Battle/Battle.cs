using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame;
using MonsterTradingCardsGame.StoreNamespace;
using MonsterTradingCardsGame.CardNamespace;
using MonsterTradingCardsGame.LogsManagement;
using user;

namespace MonsterTradingCardsGame.Battle
{
    
    class Battle
    {
        private int _round = 0;
        private readonly User _user1;
        private readonly User _user2;
        private Card _user1CurrentCard;
        private Card _user2CurrentCard;
        private readonly LogFileManagement _log = new();

        public Battle(User user1, User user2)
        {
            _user1 = user1;
            _user2 = user2;
        }

        public static int CalculateDmgElementType(Card attackingCard, Card defendingCard)
        {
            switch (attackingCard.ElementType)
            {
                case "Fire"   when defendingCard.ElementType == "Water":
                case "Normal" when defendingCard.ElementType == "Fire":
                case "Water"  when defendingCard.ElementType == "Normal":
                    return attackingCard.Damage / 2;
                case "Water"  when defendingCard.ElementType == "Fire":
                case "Fire"   when defendingCard.ElementType == "Normal":
                case "Normal" when defendingCard.ElementType == "Water":
                    return attackingCard.Damage * 2;
                default:
                    return attackingCard.Damage;
            }
        }

        public static int CalculateEloRating(int playerARating, int playerBRating, double score)
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
                < 2100 => 32,
                < 2400 => 24,
                _ => 16
            };
        }

        public bool KnightVsWater()
        {
            //if (_user1CurrentCard.Name == "Knight" && _user2CurrentCard.ElementType == "water")
            //{
            //    FightResult(_user2, _user2CurrentCard, 0, _user1, _user1CurrentCard, 0, true);
            //    return true;
            //}
            //if (_user2CurrentCard.Name == "Knight" && _user1CurrentCard.ElementType == "water")
            //{
            //    FightResult(_user1, _user1CurrentCard, 0, _user2, _user2CurrentCard, 0, true);
            //    return true;
            //}

            return false;
        }

        public static int CalculateDamage(Card attackingCard, Card defendingCard)
        {
            if
            (
                (attackingCard.CardType == "Monster" && defendingCard.CardType == "Monster") ||
                (attackingCard.Name == "Goblin"  && defendingCard.Name == "Dragon") || 
                (attackingCard.Name == "Ork"     && defendingCard.Name == "Wizzard") ||
                (defendingCard.Name == "Kraken"  && attackingCard.CardType == "Spell") ||
                (attackingCard.Name == "FireElf" && defendingCard.Name == "Dragon")
            )
            {
                return attackingCard.Damage;
            }

            return CalculateDmgElementType(attackingCard, defendingCard);
        }

        public void GameEnd()
        {
            if (_user1.Deck.Count == 0 || _user2.Deck.Count == 0)
            {
                _log.Log($"{Environment.NewLine}{(_user1.Deck.Count == 0 ? _user2.Name : _user1.Name)} WINS!" +
                         $"{Environment.NewLine}Rounds played: {_round}{Environment.NewLine}");
            }
            else
            {
                _log.Log($"{Environment.NewLine}Game ended after {_round} rounds. It's a draw!");
            }
        }

        public void FightResult(User winnerUser, Card winnerCard, int user1TotalDmg, User loserUser, Card loserCard, int user2TotalDmg, bool knightDrown)
        {
            loserUser.Deck.Remove(loserCard);
            winnerUser.Deck.Add(loserCard);
            _log.Log(
                knightDrown
                ? $" => Knight drowned from in the water (spell){Environment.NewLine}"
                : $" -> {user1TotalDmg} VS {user2TotalDmg} => {winnerCard.Name} defeats {loserCard.Name}{Environment.NewLine}"
            );
        }

        public void PrintCurrentCards()
        {
            _log.Log(
                $"Round {_round}{Environment.NewLine}" +
                $"{_user1.Name}: {_user1CurrentCard.PrintCard()} vs " +
                $"{_user2.Name}: {_user2CurrentCard.PrintCard()} => " + 
                $"{_user1CurrentCard.Damage} VS {_user2CurrentCard.Damage}"
            );
        }

        public void PrintDecks(string deckBeforeOrAfterFight)
        {
            _log.Log($"{Environment.NewLine}Decks {deckBeforeOrAfterFight} the fight{Environment.NewLine}" +
                     $"{Environment.NewLine}{_user1.Name}'s deck:{Environment.NewLine}{_user1.PrintDeck()}" +
                     $"{Environment.NewLine}{_user2.Name}'s deck:{Environment.NewLine}{_user2.PrintDeck()}");
        }

        public void SetRandomCards()
        {
            _user1CurrentCard = _user1.ReturnRandomCardFromDeck();
            _user2CurrentCard = _user2.ReturnRandomCardFromDeck();
        }

        public (string, int, int) Fight()
        {
            Console.WriteLine($"{Environment.NewLine}THE FIGHT BEGINS!");
            int c1Damage, c2Damage;
            PrintDecks("before");

            while (_user1.Deck.Count > 0 && _user2.Deck.Count > 0 && _round <= 100)
            {
                _round++;
                SetRandomCards();
                PrintCurrentCards();

                if (KnightVsWater())
                {
                    continue;
                }
                c1Damage = CalculateDamage(_user1CurrentCard, _user2CurrentCard);
                c2Damage = CalculateDamage(_user2CurrentCard, _user1CurrentCard);
                if (c1Damage > c2Damage) 
                {
                    FightResult(_user1, _user1CurrentCard, c1Damage, _user2, _user2CurrentCard, c2Damage, false);
                }
                else if (c1Damage < c2Damage)
                {
                    FightResult(_user2, _user2CurrentCard, c2Damage, _user1, _user1CurrentCard, c1Damage, false);
                }
                else
                {
                    _log.Log($" => Draw!{Environment.NewLine}");
                }

            }

            PrintDecks("after");

            GameEnd();
            _log.WriteLog(_log.GetLog());
            var U1Elo = CalculateEloRating(_user1.Elo, _user2.Elo, _user1.Deck.Count == 0 ? 0 : (_user2.Deck.Count == 0 ? 1 : 0.5));
            var U2Elo = CalculateEloRating(_user2.Elo, _user1.Elo, _user2.Deck.Count == 0 ? 0 : (_user1.Deck.Count == 0 ? 1 : 0.5));
            return _log.GetLog();
        }
    }

}
