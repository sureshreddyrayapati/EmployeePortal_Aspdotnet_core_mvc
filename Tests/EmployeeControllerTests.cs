using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using PracticeAspcoreMVC.Controllers;
using PracticeAspcoreMVC.Models;
using Xunit;

namespace PracticeAspcoreMVC.Tests;

public class EmployeeControllerTests
{
    [Fact]
    public void OnActionExecuting_WithoutSessionUser_RedirectsToLogin()
    {
        var controller = CreateController(isLoggedIn: false);
        var context = CreateActionExecutingContext(controller);

        controller.OnActionExecuting(context);

        var redirect = Assert.IsType<RedirectToActionResult>(context.Result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Equal("Account", redirect.ControllerName);
    }

    [Fact]
    public void Index_WithSearchTerm_FiltersEmployeesAndStoresCurrentFilter()
    {
        ResetEmployeeList();
        var controller = CreateController(isLoggedIn: true);

        var result = controller.Index("Hyderabad");

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<List<Employee>>(view.Model);
        Assert.Single(model);
        Assert.Equal("Priya Sharma", model[0].EmployeeName);
        Assert.Equal("Hyderabad", controller.ViewData["CurrentFilter"]);
    }

    [Fact]
    public void Create_WithValidEmployee_AddsEmployeeAndRedirects()
    {
        ResetEmployeeList();
        var controller = CreateController(isLoggedIn: true);
        var employee = new Employee
        {
            EmployeeId = 0,
            EmployeeName = "Anitha Rao",
            EmployeeAge = 30,
            JoingDate = new DateTime(2026, 4, 1),
            Location = "Vizag"
        };

        var result = controller.Create(employee);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Contains(EmployeeController.employeList, e => e.EmployeeName == "Anitha Rao" && e.Location == "Vizag");
    }

    private static EmployeeController CreateController(bool isLoggedIn)
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

        return new EmployeeController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }

    private static ActionExecutingContext CreateActionExecutingContext(Controller controller)
    {
        return new ActionExecutingContext(
            new ActionContext(controller.HttpContext, new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller);
    }

    private static void ResetEmployeeList()
    {
        EmployeeController.employeList = new List<Employee>
        {
            new() { EmployeeId = 1, EmployeeName = "Suresh Reddy", EmployeeAge = 24, JoingDate = new DateTime(2025, 7, 27), Location = "Bengaluru" },
            new() { EmployeeId = 2, EmployeeName = "Priya Sharma", EmployeeAge = 29, JoingDate = new DateTime(2024, 12, 10), Location = "Hyderabad" },
            new() { EmployeeId = 3, EmployeeName = "Kiran Kumar", EmployeeAge = 31, JoingDate = new DateTime(2023, 3, 15), Location = "Chennai" }
        };
    }
}
