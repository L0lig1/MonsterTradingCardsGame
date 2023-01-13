using System.Net;
using MonsterTradingCardsGame.Authorization;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using MonsterTradingCardsGame.DbConn;
using MonsterTradingCardsGame.Server;


namespace MonsterTradingCardsGame.ClientServer
{
    internal class RequestHandler
    {
        private readonly Router _router = new ();
        public HttpResponse Response = new();
        private AuthorizationHandler _authHandler = new();

        public HttpResponse HandleRequest(Http.Request.HttpRequest? request, AuthorizationHandler authorization)
        {
            _authHandler = authorization;
            try
            {
                if (request == null)
                {
                    return _router.CreateHttpResponse(HttpStatusCode.BadRequest, "Invalid request");
                }

                var splitUrl = request.Header.Url.Split('/');

                if (_authHandler.AuthorizationNeeded(splitUrl, request))
                {
                    if (!_authHandler.IsAuthorized(request.Header.User))
                    {
                        return _router.CreateHttpResponse(HttpStatusCode.Unauthorized, "Unauthorized");
                    }
                }
                Response = _router.Route(splitUrl[1], request, _authHandler);

                return Response;
            }
            catch (Exception e)
            {
                return _router.CreateHttpResponse(HttpStatusCode.Conflict, $"Error: {e.Message}");
            }
        }
    }
}
