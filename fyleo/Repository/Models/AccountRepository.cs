using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace fyleo.Repository.Models
{
    public class AccountRepository : IAccountRepository
    {
        public static string DATA_DIR = Directory.GetCurrentDirectory() + "/data";
        public const string DATA_FILE = "accounts.json";

        public AccountRepository() 
        {
            if(!Directory.Exists(DATA_DIR))
                Directory.CreateDirectory(DATA_DIR);
            
            var filePath = DATA_DIR + "/" + DATA_FILE;
            if(!File.Exists(filePath))
            {
                using (StreamWriter sw = File.CreateText(filePath)) 
                {
                    sw.WriteLine(@"[
    {
        ""Email"": ""admin@fyleo.site"",
        ""Password"": ""admin"",
        ""Name"": """",
        ""Role"": ""admin""
    }
]"
                    );
                }	
            }
        }

        public IEnumerable<Account> Get()
        {
            return ReadAccounts();

        }

        public Account GetByMail(string email)
        {
            return ReadAccounts().SingleOrDefault(x=>x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<Account> ReadAccounts()
        {
            var filePath = DATA_DIR + "/" + DATA_FILE;
            var content = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<IEnumerable<Account>>(content);
        }
    }
}