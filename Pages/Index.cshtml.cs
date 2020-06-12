using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Roshart.Pages
{
    public class IndexModel : PageModel
    {
        public string? Title { get; set; }
        public string? HostName { get; set; }
        public string? GoogleAnalyticsTrackingCode { get; set; }
        public string? HostCss { get; set; }

        public IActionResult OnGet()
        {
            Title = "Test";

            HostName = HttpContext.Request.Host.Host.ToLowerInvariant();

            switch (HostName) {
                case "catoverflow.com":
                    Title = "Cat Overflow";
                    HostCss = "cat.css";
                    GoogleAnalyticsTrackingCode = "UA-2679312-4";
                    break;
                case "dogoverflow.com":
                    Title = "Dog Overflow";
                    HostCss = "dog.css";
                    GoogleAnalyticsTrackingCode = "UA-2679312-5";
                    break;
                case "otteroverflow.com":
                    Title = "Otter Overflow";
                    HostCss = "otter.css";
                    GoogleAnalyticsTrackingCode = "UA-2679312-7";
                    break;
                default:
                    return Redirect($"{HttpContext.Request.Scheme}://catoverflow.com:{HttpContext.Request.Host.Port}");
            }

            return Page();
        }
    }
}
