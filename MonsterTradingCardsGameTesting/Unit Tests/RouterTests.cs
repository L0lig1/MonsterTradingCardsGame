using System.Net;
using NUnit.Framework;
using MonsterTradingCardsGame.Server;
using MonsterTradingCardsGame.Authorization;
using MonsterTradingCardsGame.ClientServer.Http.Request;
using HttpRequestHeader = MonsterTradingCardsGame.ClientServer.Http.Request.HttpRequestHeader;


namespace MonsterTradingCardsGameTesting.Unit_Tests
{
    internal class RouterTests
    {

        [Test]
        public void UsersRoute_RequestWrongData_ReturnsInvalidRequest()
        {
            // Arrange
            dynamic data = new System.Dynamic.ExpandoObject();
            var router = new Router();
            var req = new HttpRequest
            {
                Header = new HttpRequestHeader("1.1", "GET", "/users", "Basic", "kienboec-mtcg", "kienboec"),
                Body = new HttpRequestBody(data)
            };

            // Act
            var resp = router.Route("users", req, new AuthorizationHandler());

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, resp.Header.StatusCode);
            Assert.AreEqual("Error: Invalid request!", resp.Body?.Data);
        }

        [Test]
        public void UsersRoute_RequestWrongMethod_ReturnsInvalidRequest()
        {
            // Arrange
            dynamic data = new System.Dynamic.ExpandoObject();
            var router = new Router();
            var req = new HttpRequest
            {
                Header = new HttpRequestHeader("1.1", "DELETE", "/users", "Basic", "kienboec-mtcg", "kienboec"),
                Body = new HttpRequestBody(data)
            };

            // Act
            var resp = router.Route("users", req, new AuthorizationHandler());

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, resp.Header.StatusCode);
            Assert.AreEqual("Error: Invalid request!", resp.Body?.Data);
        }

        [Test]
        public void TradingRoute_RequestWrongMethod_ReturnsInvalidRequest()
        {
            // Arrange
            dynamic data = new System.Dynamic.ExpandoObject();
            var router = new Router();
            var req = new HttpRequest
            {
                Header = new HttpRequestHeader("1.1", "PUT", "/tradings", "Basic", "kienboec-mtcg", "kienboec"),
                Body = new HttpRequestBody(data)
            };

            // Act
            var resp = router.Route("tradings", req, new AuthorizationHandler());

            // Assert
            Assert.AreEqual(HttpStatusCode.Conflict, resp.Header.StatusCode);
            Assert.AreEqual("Error occured: Invalid request!", resp.Body?.Data);
        }

    }
}
