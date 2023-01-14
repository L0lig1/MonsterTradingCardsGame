using System;
using Moq;
using System.Net;
using NUnit.Framework;
using System.Collections.Generic;
using MonsterTradingCardsGame.DbConn;
using MonsterTradingCardsGame.DbConn.Tables;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using Newtonsoft.Json;
using HttpResponseHeader = MonsterTradingCardsGame.ClientServer.Http.Response.HttpResponseHeader;


namespace MonsterTradingCardsGameTesting.Unit_Tests
{
    internal class MoqTests
    {
        private Mock<Db> _dbMock = null!;
        private Mock<DbHandler> _dbHandlerMock = null!;
        private Db _db = new();
        private DbUsers _dbUsers = new();
        private DbHandler _dbHandler = new();
        private Dictionary<string, string> Commands = new DbCommands().Commands;
        private string[,] vals = null!;

        [SetUp]
        public void Setup()
        {
            _dbHandlerMock = new Mock<DbHandler>();
            /*_dbHandlerMock.SetupSequence(x => x.UseCoins(It.IsAny<string>(), 5))
                        .Returns(true)
                        .Returns(true)
                        .Returns(true)
                        .Returns(true)
                        .Returns(false);
            _dbHandlerMock.SetupSequence(x => x.LoginUser("testuser", "testpwd"))
                        .Returns(new HttpResponse
                        {
                            Body = new HttpResponseBody("Login success"),
                            Header = new HttpResponseHeader(HttpStatusCode.OK, "text/plain", 8)
                        });*/

            _dbMock = new Mock<Db>();
        }

        [Test]
        public void RegisterUser_ValidInput_ReturnsCreated()
        {
            // Arrange
            vals = new[,] { { "user", "testuser" }, { "pw", "testpw" } };
            _dbHandlerMock.Setup(x => x.ExecNonQuery(Commands["RegisterUser"], vals))
                          .Returns(true);

            // Act
            var result = _dbUsers.RegisterUser("testuser", "testpw", _dbHandlerMock.Object);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, result.Header.StatusCode);
            Assert.AreEqual("User has been registered", result.Body?.Data);
        }

        [Test]
        public void RegisterUser_ExecNonQueryFails_ReturnsInternalServerError()
        {
            // Arrange
            vals = new[,] { { "user", "testuser" }, { "pw", "testpw" } };
            _dbHandlerMock.Setup(x => x.ExecNonQuery(Commands["RegisterUser"], vals)).Returns(false);


            // Act
            var result = _dbUsers.RegisterUser("testuser", "testpw", _dbHandlerMock.Object);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.Header.StatusCode);
            Assert.AreEqual("Problem occurred adding user!", result.Body?.Data);
        }

        [Test]
        public void RegisterUser_DuplicateUsername_ReturnsConflict()
        {
            // Arrange
            vals = new[,] { { "user", "testuser" }, { "pw", "testpw" } };
            _dbHandlerMock.Setup(x => x.ExecNonQuery(Commands["RegisterUser"], vals)).Throws(new Exception("23505: Unique constraint violated"));

            // Act
            var result = _dbUsers.RegisterUser("testuser", "testpw", _dbHandlerMock.Object);

            // Assert
            Assert.AreEqual(HttpStatusCode.Conflict, result.Header.StatusCode);
            Assert.AreEqual("User with that username already exists!", result.Body?.Data);
        }

        [Test]
        public void HasEnoughCoins_HasEnoughCoins_ReturnsTrue()
        {
            // Arrange
            vals = new[,] { { "user", "testuser" } };
            _dbHandlerMock.Setup(x => x.ExecQuery(Commands["HasEnoughCoins"], 1, new[] { 0 }, vals, true))
                          .Returns((true, "5"));

            // Act
            var result = _dbUsers.HasEnoughCoins("testuser", _dbHandlerMock.Object);

            // Assert
            Assert.AreEqual(true, result);
        }
      
        [Test]
        public void Trade_NotMatchingCardTypes_ThrowsException()
        {
            // Arrange
            vals = new[,] { { "user", "testuser" } };
            _dbHandlerMock.Setup(x => x.ExecQuery(Commands["HasEnoughCoins"], 1, new[] { 0 }, vals, true))
                          .Returns((false, "2"));
            var result = string.Empty;

            // Act
            try
            {
                _dbUsers.HasEnoughCoins("testuser", _dbHandlerMock.Object);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = e.Message;
            }

            // Assert
            Assert.AreEqual($"Not enough coins! You have 2, but should at least have 5", result);
        }
      
              
        [Test]
        public void HasEnoughCoins_NotEnoughCoins_ThrowsException()
        {
            // Arrange
            vals = new[,] { { "user", "testuser" } };
            _dbHandlerMock.Setup(x => x.ExecQuery(Commands["HasEnoughCoins"], 1, new[] { 0 }, vals, true))
                          .Returns((false, "2"));
            var result = string.Empty;

            // Act
            try
            {
                _dbUsers.HasEnoughCoins("testuser", _dbHandlerMock.Object);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = e.Message;
            }

            // Assert
            Assert.AreEqual($"Not enough coins! You have 2, but should at least have 5", result);
        }
      


    }
}
