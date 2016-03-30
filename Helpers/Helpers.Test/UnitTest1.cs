using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Helpers.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_Password_Encrypt_Hashing()
        {
            // Act
            string result = Helpers.Password.Encrypt("test") as string;

            // Assert
            Assert.AreEqual("kEK9k3Xy+B4=", result);
        }

        
        [TestMethod]
        public void Test_Password_Decrypt_Hashing()
        {
            // Act
            string result = Helpers.Password.Decrypt("kEK9k3Xy+B4=") as string;

            // Assert
            Assert.AreEqual("test", result);
        }
    }
}
