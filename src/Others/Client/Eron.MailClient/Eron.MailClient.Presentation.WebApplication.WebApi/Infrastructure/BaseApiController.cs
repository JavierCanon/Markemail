using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Eron.MailClient.Presentation.WebApplication.WebApi.Infrastructure
{
    public class BaseApiController : ApiController
    {
        protected string CurrentUserId;
        protected string CurrentUserName;

        public BaseApiController()
        {
            CurrentUserId = User.Identity.GetUserId();
            CurrentUserName = User.Identity.GetUserName();
        }
    }
}