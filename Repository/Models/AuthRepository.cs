using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace fyleo.Repository.Models
{
    public class AuthRepository : IAuthRepository
    {
        public static string DATA_DIR = Directory.GetCurrentDirectory() + "/data";
        public const string DATA_FILE = "auth.json";

        public AuthRepository() 
        {
            if(!Directory.Exists(DATA_DIR))
                Directory.CreateDirectory(DATA_DIR);
            
            var filePath = DATA_DIR + "/" + DATA_FILE;
            if(!File.Exists(filePath))
            {
                using (StreamWriter sw = File.CreateText(filePath)) 
                {
                    sw.WriteLine(@"{
    ""Token"": ""TOKEN_VALUE""
}"
                    .Replace("TOKEN_VALUE", RandomString(20)));
                }	
            }               
        }

        public AuthConfig Get()
        {
            var filePath = DATA_DIR + "/" + DATA_FILE;
            var content = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<AuthConfig>(content);
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}