using System;
using MonsterTradingCardsGame.Authorization;
using NUnit.Framework;
using MonsterTradingCardsGame.Battle;
using MonsterTradingCardsGame.CardNamespace;
using Authorization = MonsterTradingCardsGame.Authorization.Authorization;


namespace MonsterTradingCardsGameTesting
{

    public interface INpgsqlConnection
    {
        int Execute(string command, object[] parameters);
    }

    public class Tests
    {


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
        public void Test1()
        {
            

        }

        // check if card can be used when it's actually locked for trade
        // try login 3 times get banned

        
    }
}
        /*
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
        }*/