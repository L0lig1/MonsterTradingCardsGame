using System.Collections.Concurrent;
using MonsterTradingCardsGame.DbConn.Tables;
using Npgsql;
using user;


namespace MonsterTradingCardsGame.Battle
{
    public class BattleLobby
    {

        // A flag to track when both players have joined
        public bool OnePlayerJoined = false;

        // A lock to synchronize access to the flag
        private readonly object _lockFlag = new();
        private readonly DbUsers _dbUser = new();
        private readonly DbStack _dbStack = new();
        private readonly List<string> _users = new();
        private NpgsqlConnection _conn = null!;
        private dynamic? _celo1 = 0, _celo2 = 0;
        private (string, int, int) _battleLog;

        private readonly ConcurrentDictionary<string, string> _gameResults = new();
        //private readonly NpgsqlConnection _conn = new();

        private void PlayGame()
        {
            Console.WriteLine("Game starting!");
            var battle = new Battle(
                new User(_users[0], _dbStack.GetDeck(_users[0], _conn), _celo1),
                new User(_users[1], _dbStack.GetDeck(_users[1], _conn), _celo2)
            );
            _battleLog = battle.Fight();
            Console.WriteLine(_battleLog.Item1 + Environment.NewLine + _battleLog.Item2 + Environment.NewLine + _battleLog.Item3);
        }

        private void BattleResult(string username, int elo)
        {
            _dbUser.UpdateUserStats(username, elo, _conn);
            _gameResults.TryAdd(username, _battleLog.Item1);
        }


        public string StartLobby(NpgsqlConnection conn, string username)
        {
            _conn = conn;
            try
            {
                lock (_lockFlag)
                {
                    _users.Add(username);
                    if (!OnePlayerJoined)
                    {
                        _celo1 = int.Parse(_dbUser.UserStats(username, _conn).Body?.Data);
                        OnePlayerJoined = true;
                        Monitor.Wait(_lockFlag);
                        if (_gameResults.ContainsKey(_users[0]))
                            return _gameResults[_users[0]];
                        // check in dict if user has battlelog, wait until username has battlelog
                    }
                    else
                    {
                        // Both players have joined, start the game!

                        _celo2 = int.Parse(_dbUser.UserStats(username, _conn).Body?.Data);

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
