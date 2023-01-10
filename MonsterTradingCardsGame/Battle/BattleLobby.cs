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
        private dynamic? _celo1 = 0, _celo2 = 0;

        private ConcurrentDictionary<string, string> _gameResults = new();
        //private readonly NpgsqlConnection _conn = new();


        public string PlayGame(NpgsqlConnection conn, string username)
        {
            try
            {
                /* dictionary (thread safe) username battlelog, für jeweilige user reinschreiben */
                (string, int, int) battleLog = default;
                // Wait for the other player to join
                lock (_lockFlag)
                {
                    _users.Add(username);
                    if (!OnePlayerJoined)
                    {
                        _celo1 = int.Parse(_dbUser.UserStats(username, conn).Body?.Data);
                        OnePlayerJoined = true;
                        Monitor.Wait(_lockFlag);
                        if (_gameResults.ContainsKey(_users[0]))
                            return _gameResults[_users[0]];
                        // check in dict if user has battlelog, wait until username has battlelog
                    }
                    else
                    {
                        _celo2 = int.Parse(_dbUser.UserStats(username, conn).Body?.Data);

                        // change decks
                        // Both players have joined, start the game!
                        Console.WriteLine("Game starting!");
                        // Code for the game goes here

                        var battle = new Battle(new User(_users[0], _dbStack.GetDeck(_users[0], conn), _celo1),
                            new User(_users[1], _dbStack.GetDeck(_users[1], conn), _celo2));
                        battleLog = battle.Fight();
                        Console.WriteLine(battleLog.Item1 + Environment.NewLine + battleLog.Item2 + Environment.NewLine + battleLog.Item3);
                        _dbUser.UpdateUserStats(_users[0], battleLog.Item2, conn);
                        _dbUser.UpdateUserStats(_users[1], battleLog.Item3, conn);
                        _gameResults.TryAdd(_users[0], battleLog.Item1);
                        _gameResults.TryAdd(_users[1], battleLog.Item1);



                        Monitor.Pulse(_lockFlag);
                    }
                }

                if (battleLog.Item1 != null) return battleLog.Item1;
                throw new Exception("Error while battling");
                /*
                 * Battle:
                 * 2 User class
                 * Get Decks 
                 * Fill Deck Class with Cards
                 *
                 */
                // when battle over, give result to each thread
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }

    }
}
