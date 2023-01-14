using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.Authorization;
using NUnit.Framework;

namespace MonsterTradingCardsGameTesting.Unit_Tests
{
    internal class AuthTests
    {

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

    }
}
