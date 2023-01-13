using MonsterTradingCardsGame.ClientServer.Http.Request;


namespace MonsterTradingCardsGame.Authorization
{
    public class Authorization
    {
        public DateTime LoggedInUntil;
        public int Tries;

        public Authorization(DateTime loggedInUntil, int tries)
        {
            LoggedInUntil = loggedInUntil;
            Tries = tries;
        }
    }

    public class AuthorizationHandler
    {
        public Dictionary<string, Authorization> _authorization = new();

        private bool IsLoggedIn(string username)
        {
            if (_authorization.ContainsKey(username) && _authorization[username].LoggedInUntil > DateTime.Now)
                return true;
            throw new Exception("User is not Logged in!");
        }

        public bool IsBanned(string username)
        {
            if (_authorization.ContainsKey(username) && _authorization[username].Tries < 3)
                return false;
            throw new Exception("User is permanently banned!");
        }

        public bool AuthorizationNeeded(string[] url, HttpRequest request)
        {
            return url[1] != "sessions" && !(url[1] == "users" && request.Header.Method == "POST" && url.Length == 2);
        }

        public bool IsAuthorized(string? username)
        {
            try
            {
                if (username == null)
                    throw new Exception("No username was provided!");
                if (IsLoggedIn(username) && !IsBanned(username))
                    return true;

                _authorization.Remove(username);
                return false;
            }
            catch (Exception e)
            {
                throw new Exception($"Authorization failed: {e.Message}");
            }
        }

    }
}
