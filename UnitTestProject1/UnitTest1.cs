using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using StroyModern;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void addUser()
        {
            var login = "Хуа";
            var password = "эмин";

            bool expected = false;
            bool actual;

            TestData testData = new TestData();
            testData.AddUsers(login, password);
        }

        [TestMethod]
        public void CheckUserTest()
        {
            bool expected = false;
            var login = "Waguteru";
            var password = "admin20";

            TestData testData = new TestData();
                 testData.Authorization(login, password);
        }

        [TestMethod]
        public void Update()
        {
            bool expected = false;
            var acticle = "179209625303230";

            TestData testData = new TestData();

            testData.UPDATEitem(acticle);
        }

        [TestMethod]
        public void Delete()
        {
            bool expected = false;
            var name_item = "Арка";

            TestData testData = new TestData();
            testData.Deleteitem(name_item);
        }
    }
}
