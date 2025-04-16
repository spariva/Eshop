using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Eshop.Filters
{
    public class AuthorizeUserAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context) {
            var user = context.HttpContext.User;

            string controller = context.RouteData.Values["controller"].ToString();
            string action = context.RouteData.Values["action"].ToString();
            var routeValues = context.RouteData.Values;

            var id = routeValues.ContainsKey("id") ? routeValues["id"] : null;

            ITempDataProvider provider = context.HttpContext.RequestServices.GetService<ITempDataProvider>();
            var TempData = provider.LoadTempData(context.HttpContext);


            TempData["controller"] = controller;
            TempData["action"] = action;
            TempData["id"] = id; 

            provider.SaveTempData(context.HttpContext, TempData);

            if (!user.Identity.IsAuthenticated) {
                context.Result = this.GetRoute("Users", "Login");
            }
        }

        private RedirectToRouteResult GetRoute(string controller, string action) {
            RouteValueDictionary ruta = new RouteValueDictionary(new { controller = controller, action = action });
            RedirectToRouteResult result = new RedirectToRouteResult(ruta);
            return result;
        }
    }
}