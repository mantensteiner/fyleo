using System.Linq;
using System.Threading.Tasks;
using fyleo.EventLog;
using fyleo.Repository.Models;
using Microsoft.AspNetCore.Mvc;

namespace fyleo.Pages
{
    public class LogoutModel : BasePageModel
    {
        public LogoutModel(IEventLog eventLog, ISiteConfigRepository siteConfigRepo, ITranslations translations) 
            : base(eventLog, siteConfigRepo, translations)
        {
        }

        public async Task<IActionResult> OnGet()
        {
            var authCookie = Request.Cookies["authCookie"];
            if (!string.IsNullOrEmpty(authCookie))
            {
                var user = authCookie.Split(',').Single(x => x.Contains("user:")).Split(':')[1];
                var token = authCookie.Split(',').Single(x => x.Contains("token:")).Split(':')[1];
                await LogEvent(user, "");
            }

            Response.Cookies.Delete("authCookie");
            return RedirectToPage($"/Login");
        }

        private async Task LogEvent(string user, string message)
        {
            await EventLog.Write(user, Actions.LOGOUT, message);
        }
    }
}
