using System.Collections.Generic;

namespace fyleo.Repository.Models
{
    public interface ISiteConfigRepository
    {
        string SiteName {get;}
        string SiteNameLong {get;}
        string PrivacyTermsContent {get;}
    }
}