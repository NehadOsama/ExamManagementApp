using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CemexExamApp.ContextData;
using CemexExamApp.Controllers;
using CemexExamApp.DBCore;
using TheInventory.Models;
using CemexExamApp.Models;
using Microsoft.AspNetCore.Authentication;
using System.Net;

namespace CemexExamApp.DBCore
{
//    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class CustomPrivilege : AuthorizeAttribute , IAuthorizationFilter
    {
        ExamManagmentAppContext dc = new ExamManagmentAppContext();

        public IHttpContextAccessor ContextHttpAccessor { get; private set; }

        
        protected void HandleUnauthorizedRequest(AuthorizationFilterContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
               {
                    { "controller", "Home" },
                    { "action", "UnAuthorized" }
               });
        }

         void IAuthorizationFilter.OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                AdminUser UserPermissionObj = new AdminUser();
                string IsURLCont = context.HttpContext.Request.RouteValues.GetValueOrDefault("controller").ToString();
                string IsURLAct = context.HttpContext.Request.RouteValues.GetValueOrDefault("action").ToString();
                //set up a dictionary
                var claims = context.HttpContext.User.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);

                //access as follows
                var CurrentRoleId = claims["CurrentRoleId"];

                var currentUserId = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                //var CurrentUserGrb = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value;

                bool chkMenu = false;
                //var SessionObj = new SessionController(ContextHttpAccessor);
                var PerControllActionList = UserPermissionObj.FindControllerActionByGroupId(int.Parse(CurrentRoleId));

                foreach (RoleRouting Per in PerControllActionList)
                {
                    chkMenu = (IsURLCont.Contains(Per.ControllerAction.ControllerName) && IsURLAct.Contains(Per.ControllerAction.ActionName));
                    if (chkMenu) break;

                }
                if (!chkMenu)
                {   
                  //  context.Result = new ForbidResult();
                    context.HttpContext.SignOutAsync();
                    context.HttpContext.Response.Redirect("/Account/Login", true);

                    // context.HttpContext.Response.StatusCode = 401;
                }
            }
            catch (Exception ex)
            {
                context.HttpContext.SignOutAsync();
                context.HttpContext.Response.Redirect("/Account/Login", true);
                //var response = context.HttpContext.Response.StatusCode = 401;

                //if (response.StatusCode == (int)HttpStatusCode.Unauthorized ||
                //    response.StatusCode == (int)HttpStatusCode.Forbidden)
                //{
                //    response.Redirect("/Account/Login");
                //}
            }
           
        }
    }


}

