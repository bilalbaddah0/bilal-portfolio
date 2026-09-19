using System.Diagnostics;
using BilalPortfolio.Models;
using BilalPortfolio.Options;
using BilalPortfolio.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BilalPortfolio.Controllers;

public sealed class HomeController(
    IOptions<PortfolioOptions> portfolioOptions,
    IWebHostEnvironment environment) : Controller
{
    [HttpGet("/")]
    public IActionResult Index() =>
        View(new HomeViewModel { Portfolio = portfolioOptions.Value });

    [HttpGet("/cv")]
    public IActionResult DownloadCv()
    {
        var path = Path.Combine(environment.WebRootPath, "files", "Bilal-Baddah-CV.pdf");
        if (!System.IO.File.Exists(path))
        {
            TempData["CvMessage"] = "The CV will be available soon.";
            return Redirect("/#home");
        }

        return PhysicalFile(path, "application/pdf", "Bilal-Baddah-CV.pdf");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
