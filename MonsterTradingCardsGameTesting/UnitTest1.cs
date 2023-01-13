using System;
using System.Net;
using NUnit.Framework;
using Newtonsoft.Json;
using MonsterTradingCardsGame.Battle;
using MonsterTradingCardsGame.DbConn;
using MonsterTradingCardsGame.Server;
using MonsterTradingCardsGame.DbConn.Tables;
using MonsterTradingCardsGame.Authorization;
using MonsterTradingCardsGame.CardNamespace;
using MonsterTradingCardsGame.ClientServer.Http;
using MonsterTradingCardsGame.ClientServer.Http.Request;
using Authorization = MonsterTradingCardsGame.Authorization.Authorization;
using HttpRequestHeader = MonsterTradingCardsGame.ClientServer.Http.Request.HttpRequestHeader;


namespace MonsterTradingCardsGameTesting
{
    
    public class Tests
    {
        readonly string nl = Environment.NewLine;

        [Test]
        public void TestBattleCalculateElo()
        {
            // Arrange
            const int playerARating = 100;
            const int playerBRating = 100;

            // Act
            var playerANewRating = new Calculator().CalculateEloRating(playerARating, playerBRating, 1);
            var playerBNewRating = new Calculator().CalculateEloRating(playerBRating, playerARating, 0);

            // Assert
            Assert.Greater(playerANewRating, playerBNewRating);
        }

        [Test]
        public void TestBattleCalculatorSpellVsSpell()
        {
            // Arrange
            var c1 = new Card("id", "WaterSpell", 100, "Water", "spell");
            var c2 = new Card("id", "FireSpell", 100, "Fire", "spell");

            //Act
            var dmgc1 = new Calculator().CalculateDamage(c1,c2);
            var dmgc2 = new Calculator().CalculateDamage(c2,c1);

            //Assert
            Assert.Greater(dmgc1, dmgc2);
        }

        [Test]
        public void TestBattleCalculatorSpellVsMonster()
        {
            // Arrange
            var c1 = new Card("id", "FireElf", 100, "Fire", "monster");
            var c2 = new Card("id", "RegularSpell", 100, "Normal", "spell");

            //Act
            var dmgc1 = new Calculator().CalculateDamage(c1,c2);
            var dmgc2 = new Calculator().CalculateDamage(c2,c1);

            //Assert
            Assert.Greater(dmgc1, dmgc2);
        }
        
        [Test]
        public void TestBattleCalculatorMonsterVsMonster()
        {
            // Arrange
            var c1 = new Card("id", "Dragon", 100, "Fire", "monster");
            var c2 = new Card("id", "Knight", 100, "Normal", "monster");

            //Act
            var dmgc1 = new Calculator().CalculateDamage(c1,c2);
            var dmgc2 = new Calculator().CalculateDamage(c2,c1);

            //Assert
            Assert.AreEqual(dmgc1, dmgc2);
        }

        [Test]
        public void TestBattleDragonVsFireElf()
        {
            // Arrange
            var fireElf = new Card("id", "FireElf", 100, "Fire", "monster");
            var dragon = new Card("id", "Dragon", 100, "Fire", "monster");

            //Act
            var dragonAttackingFireElf = new Calculator().CalculateDamage(dragon,fireElf);
            
            //Assert
            /* The FireElves know Dragons since they were little and can evade their attacks. */
            Assert.AreEqual(0, dragonAttackingFireElf);
        }

        [Test]
        public void TestBattleCalculatorGoblinVsDragon()
        {
            // Arrange
            var goblin = new Card("id", "FireGoblin", 100, "Fire", "monster");
            var dragon = new Card("id", "Dragon", 100, "Fire", "monster");

            //Act
            var goblinAttackingDragon = new Calculator().CalculateDamage(goblin, dragon);

            //Assert
            /* Goblins are too afraid of Dragons to attack (0 Damage) */
            Assert.AreEqual(0, goblinAttackingDragon);
        }

        [Test]
        public void TestBattleCalculatorKnightVsWaterSpell()
        {
            // Arrange
            var knight = new Card("id", "Knight", 100, "Normal", "monster");
            var waterSpell = new Card("id", "WaterSpell", 100, "Water", "spell");

            //Act
            var knightAttackWaterSpell = new Calculator().CalculateDamage(knight, waterSpell);

            //Assert
            /* The armor of Knights is so heavy that WaterSpells make them drown them instantly. (0 Damage) */
            Assert.AreEqual(0, knightAttackWaterSpell);
        }
        
        [Test]
        public void TestBattleCalculatorSpellsVsKraken()
        {
            // Arrange
            var kraken = new Card("id", "Kraken", 100, "Normal", "monster");
            var fireSpell = new Card("id", "FireSpell", 100, "Fire", "spell");

            //Act
            var spellAttackingKraken = new Calculator().CalculateDamage(fireSpell, kraken);

            //Assert
            /* The Kraken is immune against spells. (0 Damage) */
            Assert.AreEqual(0, spellAttackingKraken);
        }        
        
        [Test]
        public void TestBattleCalculatorWizzardVsOrk()
        {
            // Arrange
            var wizzard = new Card("id", "Wizzard", 100, "Fire", "monster");
            var ork = new Card("id", "Ork", 100, "Normal", "monster");
            
            //Act
            var orkAttackingWizzard = new Calculator().CalculateDamage(ork, wizzard);

            //Assert
            /* Wizzard can control Orks so they are not able to damage them. (0 Damage) */
            Assert.AreEqual(0, orkAttackingWizzard);
        }

        [Test]
        public void TestAuthHandlerCheckIfBannedUserAuthorized()
        {
            // Arrange
            var ah = new AuthorizationHandler
            {
                _authorization =
                {
                    ["user"] = new Authorization(DateTime.Now.AddMinutes(10), 12)
                }
            };

            //Act
            bool isAuthorized;
            try
            {

                isAuthorized = ah.IsAuthorized("user");
                //Assert
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isAuthorized = false;
            }    
            /* User is banned (12 tries) */
            Assert.AreEqual(false, isAuthorized);
        }

        [Test]
        public void TestRoutingRequestWrongData()
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
        public void TestRoutingRequestWrongMethod()
        {
            // Arrange
            dynamic data = new System.Dynamic.ExpandoObject();
            var router = new Router();
            var db = new DbHandler();
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
            var nl = Environment.NewLine;
            var req = $"POST /users HTTP/1.1{nl}content-length: 20{nl}content-type: application/json" + "{\"Id\":1}";
            var parser = new HttpParser();

            // Act
            var parsedReq = parser.ParseHttpData(req);

            // Assert
            Assert.AreEqual(false, parsedReq.IsValid);

        }

        private Db _db = new();
        private readonly DbHandler _dH = new();
        private DbUsers _dbUser = new();
        private readonly DbStack _dbStack = new();
        private DbCards _dbCard = new();
        private DbPackages _dbPackages = new();
        
        [Test]
        public void TestDbUseAllCoins()
        {
            // Arrange
            _db = new Db();
            _dbUser = new DbUsers();

            // Act
            _db.TruncateAll();
            _dbUser.RegisterUser("testuser", "test");
            _dbUser.UseCoins("testuser", 5);
            _dbUser.UseCoins("testuser", 5);
            _dbUser.UseCoins("testuser", 5);
            _dbUser.UseCoins("testuser", 5);
            var res = string.Empty;
            try
            {
                _dbUser.HasEnoughCoins("testuser");
            }
            catch (Exception e)
            {
                res = e.Message;
            }

            // Assert
            Assert.AreEqual("Not enough coins! You have 0, but should at least have 5", res);
        }
        
        [Test]
        public void TestDbCreateDuplicateUser()
        {
            // Arrange
            _db = new Db();
            _dbUser = new DbUsers();

            // Act
            _db.TruncateAll();
            _dbUser.RegisterUser("testuser", "test");
            var res = _dbUser.RegisterUser("testuser", "test");

            // Assert
            Assert.AreEqual(HttpStatusCode.Conflict, res.Header.StatusCode);
            Assert.AreEqual("User with that username already exists!", res.Body?.Data);
        }  

        [Test]
        public void TestDbCreateCard()
        {
            // Arrange
            _db = new Db();
            _dbCard = new DbCards();

            // Act
            _db.TruncateAll();
            var createCard = _dbCard.CreateCard("cardid", "WaterSpell", 50);

            // Assert
            Assert.AreEqual(true, createCard);
        }

        [Test]
        public void TestDbCreateCardAndAddToStack()
        {
            // Arrange
            _db.TruncateAll();
            _dbUser.RegisterUser("testuser", "test");
            _dbCard.CreateCard("cardid", "WaterSpell", 50);

            // Act
            var addCardToStack = _dbStack.AddCardToStack("testuser", "cardid");
            var getStack = _dbStack.ShowStack("testuser");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, addCardToStack.Header.StatusCode);
            Assert.AreEqual("WaterSpell", getStack.Body?.Data);
        }

        [Test]
        public void TestDbCreateTradingDealAndCheckIfCardIsLocked()
        {
            // Arrange
            dynamic cardList = JsonConvert.DeserializeObject("[\"1\", \"2\", \"3\", \"4\"]") ?? throw new InvalidOperationException();
            _db.TruncateAll();
            _dbUser.RegisterUser("testuser", "test");
            _dbCard.CreateCard("1", "WaterSpell", 50);
            _dbCard.CreateCard("2", "WaterSpell", 50);
            _dbCard.CreateCard("3", "WaterSpell", 50);
            _dbCard.CreateCard("4", "WaterSpell", 50);
            _dbStack.AddCardToStack("testuser", "1");
            _dbStack.AddCardToStack("testuser", "2");
            _dbStack.AddCardToStack("testuser", "3");
            _dbStack.AddCardToStack("testuser", "4");
            _dbStack.ShowStack("testuser");
            _dH.ExecNonQuery(_dH.Sql.Commands["LockCardForTrade"], new [,] { { "user", "testuser" }, { "card", "1" } });

            // Act
            var configDeck = _dbStack.ConfigureDeck("testuser", cardList);

            // Assert
            Assert.AreEqual(HttpStatusCode.Conflict, configDeck.Header.StatusCode); // can't get card in deck (locked)
        }        
        
        [Test]
        public void TestDbInsufficientCardsForDeckConfig()
        {
            // Arrange
            dynamic cardList = JsonConvert.DeserializeObject("[\"1\", \"2\", \"3\"]") ?? throw new InvalidOperationException();
            _db.TruncateAll();
            _dbUser.RegisterUser("testuser", "test");
            _dbCard.CreateCard("1", "WaterSpell", 50);
            _dbCard.CreateCard("2", "WaterSpell", 50);
            _dbCard.CreateCard("3", "WaterSpell", 50);
            _dbCard.CreateCard("4", "WaterSpell", 50);
            _dbStack.AddCardToStack("testuser", "1");
            _dbStack.AddCardToStack("testuser", "2");
            _dbStack.AddCardToStack("testuser", "3");
            _dbStack.AddCardToStack("testuser", "4");

            // Act
            var configDeck = _dbStack.ConfigureDeck("testuser", cardList);

            // Assert
            Assert.AreEqual(HttpStatusCode.Conflict, configDeck.Header.StatusCode); // only 3 cards provided
        }

    }
}
 