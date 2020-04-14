using System.Collections.Generic;

namespace fyleo.Repository.Models
{
    public interface IAccountRepository
    {
        Account GetByMail(string email);
        IEnumerable<Account> Get();
    }
}