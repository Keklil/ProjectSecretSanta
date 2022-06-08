using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSanta_Backend.Services;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class PhoneNumberHelperTest
    {
        [TestMethod]
        public void TestPhoneDbFormat()
        {
            //Arrange
            List<string> phoneNumbers = new List<string>()
            {
                "+79630377005",
                "+7 (909) 008-05-22",
                "8965 520- 26-46",
                "8(999)3365480",
                "+7(854)2221548",
                "79630255006"
            };
            List<string> expected = new List<string>()
            {
                "9630377005",
                "9090080522",
                "9655202646",
                "9993365480",
                "8542221548",
                "9630255006"
            };

            //Act
            List<string> test = new List<string>();
            foreach (string phoneNumber in phoneNumbers)
            {
                test.Add(phoneNumber.PhoneDbFormat());
            }

            //Assert
            Assert.AreEqual(expected.Count, test.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], test[i]);
            }
        }

        [TestMethod]
        public void TestPhoneViewFormat()
        {
            //Arrange
            List<string> phoneNumbers = new List<string>()
            {
                "9630377005",
                "9090080522",
                "9655202646",
                "9993365480",
                "8542221548",
                "9630255006"
            };
            List<string> expected = new List<string>()
            {

                "+7 (963) 037-70-05",
                "+7 (909) 008-05-22",
                "+7 (965) 520-26-46",
                "+7 (999) 336-54-80",
                "+7 (854) 222-15-48",
                "+7 (963) 025-50-06"
            };

            //Act
            List<string> test = new List<string>();
            foreach (string phoneNumber in phoneNumbers)
            {
                test.Add(phoneNumber.PhoneViewFormat());
            }

            //Assert
            Assert.AreEqual(expected.Count, test.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], test[i]);
            }
        }
    }
}