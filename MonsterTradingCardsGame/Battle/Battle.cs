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
        private int round;
        private readonly User _user1;
        private readonly User _user2;
        private Card _user1CurrentCard;
        private Card _user2CurrentCard;
        private LogFileManagement _log = new();

        public Battle(User user1, User user2)
        {
            this._user1 = user1;
            this._user2 = user2;
            this.round = 0;
        }

        public void CreateDecks()
        {
            _user1.CustomOrRandomDeck();
            Console.WriteLine($"Congrats {_user1.Name}, you created a Deck!");
            _user1.PrintDeck();
            _user2.CustomOrRandomDeck();
            Console.WriteLine($"Congrats {_user2.Name}, you created a Deck!");
            _user2.PrintDeck();
        }

        public static int CalculateDmgElementType(Card attackingCard, Card defendingCard)
        {
            //switch (attackingCard.ElementType)
            //{
            //    case (int)ElementTypeEnum.Fire   when defendingCard.ElementType == (int)ElementTypeEnum.Water:
            //    case (int)ElementTypeEnum.Normal when defendingCard.ElementType == (int)ElementTypeEnum.Fire:
            //    case (int)ElementTypeEnum.Water  when defendingCard.ElementType == (int)ElementTypeEnum.Normal:
            //        return attackingCard.Damage / 2;
            //    case (int)ElementTypeEnum.Water  when defendingCard.ElementType == (int)ElementTypeEnum.Fire:
            //    case (int)ElementTypeEnum.Fire   when defendingCard.ElementType == (int)ElementTypeEnum.Normal:
            //    case (int)ElementTypeEnum.Normal when defendingCard.ElementType == (int)ElementTypeEnum.Water:
            //        return attackingCard.Damage * 2;
            //    default:
                    return attackingCard.Damage;
            //}
        }

        public bool KnightVsWater()
        {
            //if (_user1CurrentCard.Name == "Knight" && _user2CurrentCard.ElementType == (int)ElementTypeEnum.Water)
            //{
            //    FightResult(_user2, _user2CurrentCard, 0, _user1, _user1CurrentCard, 0, true);
            //    return true;
            //}
            //if (_user2CurrentCard.Name == "Knight" && _user1CurrentCard.ElementType == (int)ElementTypeEnum.Water)
            //{
            //    FightResult(_user1, _user1CurrentCard, 0, _user2, _user2CurrentCard, 0, true);
            //    return true;
            //}

            return false;
        }

        public int CalculateDamage(Card attackingCard, Card defendingCard)
        {
            //if
            //(
            //    (attackingCard.CardType == (int)CardTypeEnum.Monster && defendingCard.CardType == (int)CardTypeEnum.Monster) ||
            //    (attackingCard.Name == "Goblin"  && defendingCard.Name == "Dragon") || 
            //    (attackingCard.Name == "Ork"     && defendingCard.Name == "Wizzard") ||
            //    (defendingCard.Name == "Kraken"  && attackingCard.CardType == (int)CardTypeEnum.Spell) ||
            //    (attackingCard.Name == "FireElf" && defendingCard.Name == "Dragon")
            //)
            //{
            //    return attackingCard.Damage;
            //}

            return CalculateDmgElementType(attackingCard, defendingCard);
        }

        public void GameEnd()
        {
            if (_user2._deck.Length() == 0)
            {
                _log.WriteLog($"\nUser 1 WINS!\nRounds played: {round}");
            }
            else if (_user1._deck.Length() == 0)
            {
                _log.WriteLog($"\nUser 2 WINS!\nRounds played: {round}");
            }
            else
            {
                _log.WriteLog($"\nGame ended after {round} rounds. It's a draw!");
            }
        }

        public void FightResult(User winnerUser, Card winnerCard, int wDmg, User loserUser, Card loserCard, int lDmg, bool knightDrown)
        {
            loserUser._deck.RemoveFromDeck(loserCard);
            winnerUser._deck.AddToDeck(loserCard);
            if(winnerUser == _user1 && !knightDrown)
                _log.WriteLog($" -> {wDmg} VS {lDmg} => {winnerCard.Name} defeats {loserCard.Name}\n");            
            else if(winnerUser == _user2 && !knightDrown)
                _log.WriteLog($" -> {lDmg} VS {wDmg} => {winnerCard.Name} defeats {loserCard.Name}\n");
            else
                _log.WriteLog(" => Knight drowned from in the water (spell)\n");
        }

        public void PrintCurrentCards()
        {
            _log.WriteLog(
                (round == 1 ? "\n" : "") + 
                $"Round {round}\n" +
                _user1.Name + ": " + _user1CurrentCard.PrintCard() + " vs " +
                _user2.Name + ": " + _user2CurrentCard.PrintCard() + " => " + 
                _user1CurrentCard.Damage + " VS " + _user2CurrentCard.Damage
            );
        }

        public void PrintDecks()
        {
            string deck = _user1._deck.Length() > 0 && _user2._deck.Length() > 0 
                          ? $"\nBattle between {_user1.Name} and {_user2.Name}\nDecks before the fight" 
                          : "\nDecks after the fight";
            
            deck += $"\n{_user1.Name}'s deck:\n";
            for (int i = 0; i < _user1._deck.Length(); i++)
            {
                deck += _user1._deck.ReturnDeckAtIndex(i).PrintCard() + "\n";
            }           
                        
            deck += $"\n{_user2.Name}'s deck:\n";
            for (int i = 0; i < _user2._deck.Length(); i++)
            {           
                deck += _user2._deck.ReturnDeckAtIndex(i).PrintCard() + "\n";
            }

            _log.WriteLog(deck);
            
            //Console.WriteLine("\nUser 1 Deck");
            //_user1.PrintDeck();
            //Console.WriteLine("\nUser 2 Deck");
            //_user2.PrintDeck();
        }

        public void Fight()
        {
            Console.WriteLine("\nTHE FIGHT BEGINS!");
            int c1Damage, c2Damage;
            PrintDecks();

            while (_user1._deck.Length() > 0 && _user2._deck.Length() > 0 && round <= 100)
            {
                round++;
                _user1CurrentCard = _user1.ReturnRandomCardFromDeck();
                _user2CurrentCard = _user2.ReturnRandomCardFromDeck();

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
                    _log.WriteLog(" => Draw!\n");
                }

            }

            PrintDecks();

            GameEnd();
            _log.ReadLog();
        }
    }

}
