using System.Collections.Generic;
using fyleo.Repository.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fyleo.test
{
    public class AccountRepoMock : IAccountRepository
    {
        public IEnumerable<Account> Get()
        {
            return new[] {
                new Account() 
                {
                    Email = "TestUser@Test.test",
                    Name = "TestUser"
                }};
        }

        public Account GetByMail(string email)
        {
            string expedtedMail = "TestUser@Test.test";
            if(email.Equals(expedtedMail))
                return  new Account() 
                {
                    Email = "TestUser@Test.test",
                    Name = "TestUser"
                };
            throw new KeyNotFoundException(email);
        }
    }

    [TestClass]
    public class AccountTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var accountRepo = new AccountRepoMock();
            var account = accountRepo.GetByMail("TestUser@Test.test");

            Assert.AreEqual("TestUser", account.Name);
        }
    }
}
