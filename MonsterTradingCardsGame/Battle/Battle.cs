using MonsterTradingCardsGame.CardNamespace;
using MonsterTradingCardsGame.LogsManagement;
using MonsterTradingCardsGame.Users;


namespace MonsterTradingCardsGame.Battle
{
    public class Battle
    {
        private int _round = 1;
        private readonly User _user1;
        private readonly User _user2;
        private readonly Calculator _calc = new();
        private Card _user1CurrentCard = null!;
        private Card _user2CurrentCard = null!;
        private readonly LogFileManagement _log = new();


        public Battle(User user1, User user2)
        {
            if (user1 == null || user2 == null) throw new Exception("Users invalid");
            _user1 = user1;
            _user2 = user2;
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
                $"{_user1.Name}: {_user1CurrentCard?.PrintCard()} vs " +
                $"{_user2.Name}: {_user2CurrentCard?.PrintCard()} => " + 
                $"{_user1CurrentCard?.Damage} VS {_user2CurrentCard?.Damage}"
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

        public void CompareDamage(int card1dmg, int card2dmg)
        {
            if (card1dmg > card2dmg)
            {
                FightResult(_user1, _user1CurrentCard, card1dmg, _user2, _user2CurrentCard, card2dmg, false);
            }
            else if (card1dmg < card2dmg)
            {
                FightResult(_user2, _user2CurrentCard, card2dmg, _user1, _user1CurrentCard, card1dmg, false);
            }
            else
            {
                _log.Log($" => Draw!{Environment.NewLine}");
            }
        }

        public (string, int, int) Fight()
        {
            Console.WriteLine($"{Environment.NewLine}THE FIGHT BEGINS!");
            int c1Damage, c2Damage;
            PrintDecks("before");

            while (_user1.Deck.Count > 0 && _user2.Deck.Count > 0 && _round <= 100)
            {
                SetRandomCards();
                PrintCurrentCards();
                c1Damage = _calc.CalculateDamage(_user1CurrentCard ?? throw new InvalidOperationException(), _user2CurrentCard ?? throw new InvalidOperationException());
                c2Damage = _calc.CalculateDamage(_user2CurrentCard ?? throw new InvalidOperationException(), _user1CurrentCard ?? throw new InvalidOperationException());
                CompareDamage(c1Damage, c2Damage);
                _round++;
            }

            _round--;
            PrintDecks("after");

            GameEnd();
            _log.WriteLog(_log.GetLog());
            var U1Elo = _calc.CalculateEloRating(_user1.Elo, _user2.Elo, _user1.Deck.Count == 0 ? 0 : (_user2.Deck.Count == 0 ? 1 : 0.5));
            var U2Elo = _calc.CalculateEloRating(_user2.Elo, _user1.Elo, _user2.Deck.Count == 0 ? 0 : (_user1.Deck.Count == 0 ? 1 : 0.5));
            return (_log.GetLog(), U1Elo, U2Elo);
        }

    }

}
