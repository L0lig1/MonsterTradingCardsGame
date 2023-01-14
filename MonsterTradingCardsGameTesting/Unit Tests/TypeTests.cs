using MonsterTradingCardsGame.DbConn.Tables;
using NUnit.Framework;
using System.Linq;


namespace MonsterTradingCardsGameTesting.Unit_Tests
{
    internal class TypeTests
    {
        private readonly DbCards _dbCards = new();

        [Test]
        public void GetElemTypeFromName_RequestRegularX_ReturnsNormal()
        {
            // Arrange
            var cardname = "afsdgagagRegulardhsh";
            var expected = "Normal";

            // Act
            var resp = _dbCards.GetElemTypeFromName(cardname);

            // Assert
            Assert.AreEqual(expected, resp);
        }
        [Test]

        public void GetElemTypeFromName_RequestWaterX_ReturnsWater()
        {
            // Arrange
            var cardname = "aghafd1had2f+hWateralkjdng.af";
            var expected = "Water";

            // Act
            var resp = _dbCards.GetElemTypeFromName(cardname);

            // Assert
            Assert.AreEqual(expected, resp);
        }

        [Test]
        public void GetElemTypeFromName_RequestInvalid_ReturnsRand()
        {
            // Arrange
            var cardname = "asdghsfh.aasfh6+41f";
            string[] types = { "Water", "Fire", "Normal" };

            // Act
            var resp = types.Contains(_dbCards.GetElemTypeFromName(cardname));

            // Assert
            Assert.IsTrue(resp);
        }

        [Test]
        public void GetCardTypeFromName_RequestSpellX_ReturnsSpell()
        {
            // Arrange
            var cardname = "aghafd1had2f+hSpellalkjdng.af";
            var expected = "spell";

            // Act
            var resp = _dbCards.GetCardTypeFromName(cardname);

            // Assert
            Assert.AreEqual(expected, resp);
        }

    }
}
