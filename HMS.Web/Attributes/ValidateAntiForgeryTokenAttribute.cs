using Microsoft.AspNetCore.Mvc.Filters;

namespace HMS.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateAntiForgeryTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // ASP.NET Core handles this automatically
            base.OnActionExecuting(context);
        }
    }
}
