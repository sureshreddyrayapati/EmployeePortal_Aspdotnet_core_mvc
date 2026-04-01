using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticeAspcoreMVC.Controllers;
using PracticeAspcoreMVC.Models;
using Xunit;

namespace PracticeAspcoreMVC.Tests;

public class AccountControllerTests
{
    [Fact]
    public void Login_Get_WhenUserAlreadyInSession_RedirectsToHome()
    {
        var controller = CreateController();
        controller.HttpContext.Session.SetString("LoggedInUser", "admin");

        var result = controller.Login();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
    }

    [Fact]
    public void Login_Post_WithValidStaticCredentials_RedirectsToHomeAndStoresSession()
    {
        var controller = CreateController();
        var model = new LoginViewModel
        {
            Username = "admin",
            Password = "admin"
        };

        var result = controller.Login(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
        Assert.Equal("admin", controller.HttpContext.Session.GetString("LoggedInUser"));
    }

    [Fact]
    public void Login_Post_WithInvalidCredentials_ReturnsViewWithModelError()
    {
        var controller = CreateController();
        var model = new LoginViewModel
        {
            Username = "wrong",
            Password = "user"
        };

        var result = controller.Login(model);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(model, view.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.Contains(controller.ModelState[string.Empty]!.Errors, error => error.ErrorMessage == "Invalid username or password.");
    }

    [Fact]
    public void Logout_ClearsSessionAndRedirectsToLogin()
    {
        var controller = CreateController();
        controller.HttpContext.Session.SetString("LoggedInUser", "admin");

        var result = controller.Logout();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Null(controller.HttpContext.Session.GetString("LoggedInUser"));
    }

    private static AccountController CreateController()
    {
        var httpContext = new DefaultHttpContext
        {
            Session = new TestSession()
        };

        return new AccountController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }
}
