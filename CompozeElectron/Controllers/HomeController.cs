using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CompozeElectron.Models;
using CompozeData.Services;
using System.Security.Claims;

namespace CompozeElectron.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly CompozeService dal;
    public HomeController(ILogger<HomeController> logger, CompozeService dal)
    {
        _logger = logger;
        this.dal = dal;
    }

    public IActionResult Index()
    {
        ViewBag.Dal = dal;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
