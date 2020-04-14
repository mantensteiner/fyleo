using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using fyleo.EventLog;
using fyleo.Repository.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fyleo.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : BasePageModel
    {
        public string RequestId { get; set; }
        public string Message { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public ErrorModel(IEventLog eventLog, ISiteConfigRepository siteConfigRepo, ITranslations translations) : 
            base(eventLog, siteConfigRepo, translations)
        {
            
        }
        
        public void OnGet(string message)
        {
            Message = message;
        }
    }
}
