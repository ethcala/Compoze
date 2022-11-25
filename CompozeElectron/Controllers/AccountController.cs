using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CompozeElectron.Models;
using Microsoft.AspNetCore.Authentication;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using ElectronNET.API;
using ElectronNET.API.Entities;

public class AccountController : Controller
{
    public async Task Login(string returnUrl = "UserInfo/Dashboard?searchMode=false&search=none") 
    {
        var authProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(returnUrl)
            .Build();
        ChangeElectronMenu(true);
        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authProperties);
    }

    [Authorize]
    public async Task Logout(String returnUrl = "/") 
    {
        var authProperties = new LogoutAuthenticationPropertiesBuilder()
            .WithRedirectUri(returnUrl)
            .Build();
        ChangeElectronMenu(false);
        await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authProperties);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
    public void ChangeElectronMenu(bool boolEnabled)
    {
        var fileMenu = new MenuItem[]
        {
            new MenuItem { Label = "New Project",
                                    Click = () => Electron.WindowManager.BrowserWindows.First().LoadURL($"http://localhost:{BridgeSettings.WebPort}/UserInfo/NewProject"),
                                    Enabled = boolEnabled},
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Label = "Home", 
                                    Click = () => Electron.WindowManager.BrowserWindows.First().LoadURL($"http://localhost:{BridgeSettings.WebPort}/") },
            new MenuItem { Label = "Privacy", 
                                    Click = () => Electron.WindowManager.BrowserWindows.First().LoadURL($"http://localhost:{BridgeSettings.WebPort}/UserInfo/Privacy") },
            new MenuItem { Label = "Help", 
                                    Click = () => Electron.WindowManager.BrowserWindows.First().LoadURL($"http://localhost:{BridgeSettings.WebPort}/UserInfo/Help") },
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Role = MenuRole.quit }
        };

        var viewMenu = new MenuItem[]
        {
            new MenuItem { Role = MenuRole.reload },
            new MenuItem { Role = MenuRole.forcereload },
            new MenuItem { Role = MenuRole.toggledevtools },
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Role = MenuRole.resetzoom },
            new MenuItem { Role = MenuRole.zoomin },
            new MenuItem { Role = MenuRole.zoomout },
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Role = MenuRole.togglefullscreen }
        };

        var menu = new MenuItem[] 
        {
            new MenuItem { Label = "File", Type = MenuType.submenu, Submenu = fileMenu },
            new MenuItem { Label = "View", Type = MenuType.submenu, Submenu = viewMenu }
        };

        Electron.Menu.SetApplicationMenu(menu);
    }
}