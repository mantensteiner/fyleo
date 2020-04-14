using fyleo.EventLog;
using fyleo.Repository.Models;

namespace fyleo.Pages
{
    public class PrivacyModel : BasePageModel
    {
        public PrivacyModel(IEventLog eventLog, ISiteConfigRepository siteConfigRepo, ITranslations translations)
            : base(eventLog, siteConfigRepo, translations)
        {
        }

        public string TermsContent =>  SiteConfigRepo.PrivacyTermsContent;

        public void OnGet()
        {
        }
    }
}