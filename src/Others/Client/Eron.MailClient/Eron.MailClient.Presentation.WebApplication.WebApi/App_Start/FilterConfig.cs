using System.Web;
using System.Web.Mvc;

namespace Eron.MailClient.Presentation.WebApplication.WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
