using System;
using NUnit.Framework;
using MonsterTradingCardsGame.Battle;
using MonsterTradingCardsGame.CardNamespace;

namespace MonsterTradingCardsGameTesting.Unit_Tests
{
    internal class BattleTests
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
            var dmgc1 = new Calculator().CalculateDamage(c1, c2);
            var dmgc2 = new Calculator().CalculateDamage(c2, c1);

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
            var dmgc1 = new Calculator().CalculateDamage(c1, c2);
            var dmgc2 = new Calculator().CalculateDamage(c2, c1);

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
            var dmgc1 = new Calculator().CalculateDamage(c1, c2);
            var dmgc2 = new Calculator().CalculateDamage(c2, c1);

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
            var dragonAttackingFireElf = new Calculator().CalculateDamage(dragon, fireElf);

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
    }
}
