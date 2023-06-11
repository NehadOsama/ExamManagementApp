using CemexExamApp.DBCore;
using CemexExamApp.Models;
using CemexExamApp.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace CemexExamApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            this.httpContextAccessor = httpContextAccessor;
        }

        //[CustomPrivilege]
        public IActionResult Index()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {

            //var exceptionDetails = httpContextAccessor.HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            //ExceptionLog exceptionLog = new ExceptionLog()
            //{
            //    ExceptionMessage = exceptionDetails.Error.Source + " , Method Name:" + exceptionDetails.Error.Message,
            //    InnerException = exceptionDetails.Error.InnerException == null ? "" : exceptionDetails.Error.InnerException.Message,
            //    PageURL = exceptionDetails.Path + " , Method Name:" + exceptionDetails.Error.TargetSite.Name,
            //    StackTrace = exceptionDetails.Error.StackTrace,
            //    LogDate = DateTime.Now,
            //    LogUserId = int.Parse(httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value),
            //    LogUserName = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value,
            //    IPAddress = "Trace Identifier: " + Activity.Current?.Id ?? HttpContext.TraceIdentifier

            //};
            //AdminHelper.LogException(exceptionLog);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}