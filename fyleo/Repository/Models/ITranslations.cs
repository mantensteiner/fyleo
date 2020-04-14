using System.Collections.Generic;

namespace fyleo.Repository.Models
{
    public class LanguageConfig
    {
        public const string KEY_RECYCLE_BIN = "RECYCLE_BIN";
        
        public string LangCode { get; set; }
    }

    public interface ITranslations
    {
        string Trash { get; }
        string ByKey(string key);
    }
}