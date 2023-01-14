using System;
using NUnit.Framework;
using MonsterTradingCardsGame.ClientServer.Http;

namespace MonsterTradingCardsGameTesting.Unit_Tests
{
    internal class HttpParserTests
    {
        string nl = Environment.NewLine;

        [Test]
        public void TestParserValidRequest()
        {
            // Arrange
            var req = $"POST /users HTTP/1.1{nl}content-length: 20{nl}content-type: application/json{nl}{nl}" + "{\"Id\":1}";
            var parser = new HttpParser();

            // Act
            var parsedReq = parser.ParseHttpData(req);

            // Assert
            Assert.AreEqual("POST", parsedReq.Header.Method);
            Assert.AreEqual("1.1", parsedReq.Header.HttpVersion);
            Assert.AreEqual("/users", parsedReq.Header.Url);
            Assert.AreEqual(1, (int)parsedReq.Body?.Data?.Id.Value);
            Assert.AreEqual(true, parsedReq.IsValid);
        }

        [Test]
        public void TestParserInvalidRequest()
        {
            // Arrange
            var req = $"POST /users HTTP/1.1{nl}content-length: 20{nl}content-type: application/json" + "{\"Id\":1}";
            var parser = new HttpParser();

            // Act
            var parsedReq = parser.ParseHttpData(req);

            // Assert
            Assert.AreEqual(false, parsedReq.IsValid);

        }
    }
}
