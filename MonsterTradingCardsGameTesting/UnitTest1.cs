using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using MonsterTradingCardsGame;
using MonsterTradingCardsGame.CardNamespace;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using MonsterTradingCardsGame.DbConn;
using MonsterTradingCardsGame.DbConn.Tables;
using Moq;
using Npgsql;
using user;


namespace MonsterTradingCardsGameTesting
{

    public interface INpgsqlConnection
    {
        int Execute(string command, object[] parameters);
    }

    public class Tests
    {

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
            //using var mock = AutoMock.GetLoose();
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
            var mockConnection = new Mock<INpgsqlConnection>();
            mockConnection.Setup(conn => conn.Execute(It.IsAny<string>(), It.IsAny<object[]>()))
                          .Throws(new Exception("23505: duplicate key value violates unique constraint"));
            //Act
            var response = new DbUsers().RegisterUser(username, password, db.Conn);
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

        // check if card can be used when it's actually locked for trade
        // 

        
    }
}