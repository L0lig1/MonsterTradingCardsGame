using System.Collections.Concurrent;
using MonsterTradingCardsGame.DbConn;
using MonsterTradingCardsGame.DbConn.Tables;
using MonsterTradingCardsGame.Users;


namespace MonsterTradingCardsGame.Battle
{
    public class BattleLobby
    {
        public bool OnePlayerJoined = false;
        private readonly object _lockFlag = new();
        private readonly DbUsers _dbUser = new();
        private readonly DbStack _dbStack = new();
        private readonly List<string> _users = new();
        private dynamic? _celo1 = 0, _celo2 = 0;
        private (string, int, int) _battleLog;
        private readonly ConcurrentDictionary<string, string> _gameResults = new();
        private readonly DbHandler _handler = new();

        private void PlayGame()
        {
            Console.WriteLine("Game starting!");
            var battle = new Battle(
                new User(_users[0], _dbStack.GetDeck(_users[0], _handler), _celo1),
                new User(_users[1], _dbStack.GetDeck(_users[1], _handler), _celo2)
            );
            _battleLog = battle.Fight();
            Console.WriteLine(_battleLog.Item1 + Environment.NewLine + _battleLog.Item2 + Environment.NewLine + _battleLog.Item3);
        }

        private void BattleResult(string username, int elo)
        {
            _dbUser.UpdateUserStats(username, elo, _handler);
            _gameResults.TryAdd(username, _battleLog.Item1);
        }

        public string StartLobby(string username)
        {
            try
            {
                lock (_lockFlag)
                {
                    _users.Add(username);
                    if (!OnePlayerJoined)
                    {
                        _celo1 = int.Parse(_dbUser.UserStats(username, _handler).Body?.Data);
                        OnePlayerJoined = true;
                        Monitor.Wait(_lockFlag);
                        if (_gameResults.ContainsKey(_users[0]))
                            return _gameResults[_users[0]];
                        // check in dict if user has battlelog, wait until username has battlelog
                    }
                    else
                    {
                        // Both players have joined, start the game!

                        _celo2 = int.Parse(_dbUser.UserStats(username, _handler).Body?.Data);

                        PlayGame();
                        BattleResult(_users[0], _battleLog.Item2);
                        BattleResult(_users[1], _battleLog.Item3);
                        
                        Monitor.Pulse(_lockFlag);
                    }
                }

                return _battleLog.Item1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }

    }
}
