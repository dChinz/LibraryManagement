using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class LanguageController : Controller
    {
        public IActionResult SetLanguage(
            string culture,
            string returnUrl = "/")
        {
            if (culture != "vi-VN" && culture != "en-US")
            {
                culture = "vi-VN";
            }

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(culture)));

            return LocalRedirect(returnUrl);
        }
    }
}