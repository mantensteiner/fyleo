using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace fyleo.Repository.Models
{
    public class Translations : ITranslations
    {
        public static string DATA_DIR = Directory.GetCurrentDirectory() + "/translations";
        private readonly LanguageConfig langConfig;

        public Translations(LanguageConfig langConfig) 
        {
            this.langConfig = langConfig;

            if(!Directory.Exists(DATA_DIR))
                Directory.CreateDirectory(DATA_DIR);
            
            var filePath = GetLangFilePath();
            if(!File.Exists(filePath))
            {
                using (StreamWriter sw = File.CreateText(filePath)) 
                {
                    sw.WriteLine($"{{\n\"{LanguageConfig.KEY_RECYCLE_BIN}\": \"{LanguageConfig.KEY_RECYCLE_BIN}\"\n}}");
                }	
            }
        }

        public string ByKey(string key)
        {
            var filePath = GetLangFilePath();
            var content = File.ReadAllText(filePath);
            var translations = JsonSerializer.Deserialize<IDictionary<string, string>>(content);
            return translations[key];
        }

        public string Trash
        {
            get
            {
                var trashFolderName = ByKey(LanguageConfig.KEY_RECYCLE_BIN);

                if(!new Regex("[a-zA-Z0-9]").IsMatch(trashFolderName))
                    throw new InvalidDataException("Translation for RECYCLE_BIN must only contain letters and numbers");

                return trashFolderName;
            }
        }

        private string GetLangFilePath()
        {
            return DATA_DIR + "/" + langConfig.LangCode.ToUpper() + ".json";
        }
    }
}