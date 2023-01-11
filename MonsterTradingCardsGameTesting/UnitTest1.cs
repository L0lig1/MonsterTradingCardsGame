using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using MonsterTradingCardsGame;
using MonsterTradingCardsGame.CardNamespace;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using MonsterTradingCardsGame.DbConn;
using MonsterTradingCardsGame.DbConn.Tables;
using Npgsql;
using user;


namespace MonsterTradingCardsGameTesting
{
    public class Tests
    {
        Mock<>

        [SetUp]
        public void Setup()
        {

        }

        [SetUp]
        public void RegisterUser_ValidInput_ReturnsSuccess()
        {
            // Arrange
            var username = "testuser";
            var password = "password";
            var db = new Router();
            db.Connect();

            // Act
            var response = new DbUsers().RegisterUser(username, password, db.Conn);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.Header.StatusCode);
            Assert.AreEqual("User has been registered", response.Body?.Data);
        }

        [SetUp]
        public void RegisterUser_DuplicateUsername_ReturnsConflict()
        {
            // Arrange
            var username = "testuser";
            var password = "password";
            var db = new Router();
            db.Connect();
            //Act
            var response = new DbUsers().RegisterUser(username, password, db.Conn);
            response = new DbUsers().RegisterUser(username, password, db.Conn);
            //Assert
            Assert.AreEqual(HttpStatusCode.Conflict, response.Header.StatusCode);
            Assert.AreEqual("User with that username already exists!", response.Body?.Data);
        }


        [Test]
        public void Test1()
        {
            RegisterUser_ValidInput_ReturnsSuccess();
            RegisterUser_DuplicateUsername_ReturnsConflict();

        }

        
    }
}