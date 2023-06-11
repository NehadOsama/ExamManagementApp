using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Reflection;

namespace CemexExamApp.Repository
{
    public class ViewRenderService : IViewRenderService 
    {
        private readonly IRazorViewEngine razorViewEngine;
        private readonly ITempDataProvider tempDataProvider;
        private readonly IServiceProvider serviceProvider;
        private readonly IHttpContextAccessor contextAccessor;

        public ViewRenderService(IRazorViewEngine razorViewEngine
                                 ,ITempDataProvider tempDataProvider 
                                 ,IServiceProvider serviceProvider
            , IHttpContextAccessor contextAccessor)
        {
            this.razorViewEngine = razorViewEngine;
            this.tempDataProvider = tempDataProvider;
            this.serviceProvider = serviceProvider;
            this.contextAccessor = contextAccessor;
        }
        public async Task<string> RenderToStringAsync(string ViewName, object model)
        {
           // var httpContext = GetActionContext();

            var actionContext = GetActionContext();

            using (var sw = new StringWriter()) 
            {
                var ViewResult = FindView(actionContext, ViewName);
                if (ViewResult == null)
                {
                    throw new ArgumentNullException($"{ViewName} doesn't match any available view");
                }

                var ViewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(),new ModelStateDictionary())
                {
                    Model = model
                };

                var ViewContext = new ViewContext(actionContext,ViewResult,ViewDictionary,new TempDataDictionary(actionContext.HttpContext,tempDataProvider),sw,new HtmlHelperOptions());

                await ViewResult.RenderAsync(ViewContext);
                return sw.ToString();
            
            }
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };
            return new ActionContext(contextAccessor.HttpContext, contextAccessor.HttpContext.GetRouteData(), new ActionDescriptor());
        }


        private IView FindView(ActionContext actionContext,string partialName)
        {
            var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            dir = Path.Combine(dir, "Templates");
            var GetPartialResult = razorViewEngine.GetView(null, partialName,true);
            if (GetPartialResult.Success)
            {
                return GetPartialResult.View;
            }
            var FindPartialResult = razorViewEngine.FindView(actionContext, partialName, true);
            if (FindPartialResult.Success)
            {
                return GetPartialResult.View;
            }

            var searchedLocations = GetPartialResult.SearchedLocations.Concat(FindPartialResult.SearchedLocations);
            var errorMessage = string.Join(Environment.NewLine, new[] { $"Unable to find partial'{partialName}'" }.Concat(searchedLocations));        
        throw new InvalidOperationException(errorMessage);
                }
    }
}
