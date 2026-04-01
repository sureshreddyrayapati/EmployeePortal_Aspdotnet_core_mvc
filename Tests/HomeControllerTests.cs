using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using PracticeAspcoreMVC.Controllers;
using Xunit;

namespace PracticeAspcoreMVC.Tests;

public class HomeControllerTests
{
    [Fact]
    public void OnActionExecuting_WhenSessionMissingOnNonErrorAction_RedirectsToLogin()
    {
        var controller = CreateController(isLoggedIn: false);
        var context = CreateActionExecutingContext(controller, "Index");

        controller.OnActionExecuting(context);

        var redirect = Assert.IsType<RedirectToActionResult>(context.Result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Equal("Account", redirect.ControllerName);
    }

    [Fact]
    public void OnActionExecuting_WhenErrorAction_AllowsExecutionWithoutSession()
    {
        var controller = CreateController(isLoggedIn: false);
        var context = CreateActionExecutingContext(controller, "Error");

        controller.OnActionExecuting(context);

        Assert.Null(context.Result);
    }

    private static HomeController CreateController(bool isLoggedIn)
    {
        var session = new TestSession();
        if (isLoggedIn)
        {
            session.SetString("LoggedInUser", "admin");
        }

        var httpContext = new DefaultHttpContext
        {
            Session = session
        };

        return new HomeController(NullLogger<HomeController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }

    private static ActionExecutingContext CreateActionExecutingContext(Controller controller, string actionName)
    {
        var routeData = new RouteData();
        routeData.Values["action"] = actionName;

        return new ActionExecutingContext(
            new ActionContext(controller.HttpContext, routeData, new ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller);
    }
}
