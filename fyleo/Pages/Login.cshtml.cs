using System.Linq;
using System.Threading.Tasks;
using fyleo.EventLog;
using fyleo.Repository.Models;
using Microsoft.AspNetCore.Mvc;

namespace fyleo.Pages
{
    public class LoginModel : BasePageModel
    {
        private readonly IAuthRepository authRepo;
        private readonly IAccountRepository accountRepo;

        public LoginModel(IAuthRepository authRepo, IEventLog eventLog, IAccountRepository accountRepo, ISiteConfigRepository siteConfigRepo, ITranslations translations) 
            : base(eventLog, siteConfigRepo, translations)
        {
            this.accountRepo = accountRepo;
            this.authRepo = authRepo;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string email, string password)
        {
            // 1. validate input
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                Response.Cookies.Delete("authCookie");
                return RedirectToPage($"/Login");
            }            

            // 2. get account
            Account userAccount = accountRepo.GetByMail(email);
            if (userAccount == null)
            {
                Response.Cookies.Delete("authCookie");
                await LogEvent(email, "failed, invalid email");
                return RedirectToPage($"/Login");
            }

            // 3. check password
            if (!password.Equals(userAccount.Password))
            {
                Response.Cookies.Delete("authCookie");
                await LogEvent(email, "failed, invalid password");
                return RedirectToPage($"/Login");
            }

            // 4. success - provide cookie
            var authConfig = authRepo.Get();
            Response.Cookies.Append("authCookie", "user:" + email + ",token:" + authConfig.Token);
            await LogEvent(email, "ok");

            string redirectUrl;
            var redirectQuery = Request.QueryString.Value.Split("RedirectUrl=");
            if (redirectQuery.Count() == 2)
            {
                redirectUrl = redirectQuery[1];
                return Redirect(redirectUrl);
            }
            else
            {
                return RedirectToPage($"/Index");
            }
        }

        private async Task LogEvent(string user, string message)
        {
            await EventLog.Write(user, Actions.LOGIN, message);
        }
    }
}
