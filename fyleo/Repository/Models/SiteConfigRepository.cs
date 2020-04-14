using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace fyleo.Repository.Models
{
    public class SiteConfigRepository : ISiteConfigRepository
    {
        public static string DATA_DIR = Directory.GetCurrentDirectory() + "/data";
        public const string DATA_FILE = "site_config.json";
        public string SiteName => GetSiteConfig().SiteName;
        public string SiteNameLong => GetSiteConfig().SiteNameLong;
        public string PrivacyTermsContent => GetSiteConfig().PrivacyTermsContent;

        public SiteConfigRepository() 
        {
            if(!Directory.Exists(DATA_DIR))
                Directory.CreateDirectory(DATA_DIR);
            
            var filePath = DATA_DIR + "/" + DATA_FILE;
            if(!File.Exists(filePath))
            {
                using (StreamWriter sw = File.CreateText(filePath)) 
                {
                    sw.WriteLine(@"{
    ""SiteName"": ""fyleo"",
    ""SiteNameLong"": ""fyleo simple file manager"",
    ""PrivacyTermsContent"" : ""your terms of service...""
}"
                    );
                }	
            }   
        }

        private static SiteConfig GetSiteConfig()
        {
            var filePath = DATA_DIR + "/" + DATA_FILE;
            var content = File.ReadAllText(filePath);
            var siteConfig = JsonSerializer.Deserialize<SiteConfig>(content);
            return siteConfig;
        }
    }
}