using fyleo.EventLog;
using fyleo.Repository.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fyleo.Pages
{
    public class BasePageModel : PageModel
    {
        protected readonly IEventLog EventLog;
        protected readonly ISiteConfigRepository SiteConfigRepo;
        public ITranslations Translations { get; private set; }

        public BasePageModel(IEventLog eventLog, ISiteConfigRepository siteConfigRepo, ITranslations translations)
        {
            SiteConfigRepo = siteConfigRepo;
            EventLog = eventLog;
            Translations = translations;
        }

        public string SiteName => SiteConfigRepo.SiteName;
        public string SiteNameLong => SiteConfigRepo.SiteNameLong;
    }
}
