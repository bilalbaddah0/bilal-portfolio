using BilalPortfolio.Services;
using BilalPortfolio.Services.Interfaces;
using BilalPortfolio.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilalPortfolio.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = AdminSeeder.AdminRole)]
[Route("admin")]
public sealed class AdminController(IContactMessageService messageService) : Controller
{
    private const int PageSize = 12;

    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        string? filter,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        filter = NormalizeFilter(filter);
        search = search?.Trim() ?? string.Empty;

        var (messages, total) = await messageService.GetPageAsync(
            search, filter, page, PageSize, cancellationToken);
        var stats = await messageService.GetStatisticsAsync(cancellationToken);

        return View(new AdminDashboardViewModel
        {
            Messages = messages,
            Filter = filter,
            Search = search,
            Page = page,
            PageSize = PageSize,
            TotalItems = total,
            TotalMessages = stats.Total,
            UnreadMessages = stats.Unread,
            MessagesToday = stats.Today
        });
    }

    [HttpGet("messages/{id:int}")]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var message = await messageService.GetByIdAsync(id, cancellationToken);
        return message is null ? NotFound() : View(message);
    }

    [HttpPost("messages/{id:int}/status")]
    public async Task<IActionResult> SetStatus(
        int id,
        bool isRead,
        string? returnUrl,
        CancellationToken cancellationToken)
    {
        if (!await messageService.SetReadStatusAsync(id, isRead, cancellationToken))
        {
            return NotFound();
        }

        TempData["AdminMessage"] = isRead ? "Message marked as read." : "Message marked as unread.";
        return RedirectToLocal(returnUrl);
    }

    [HttpPost("messages/{id:int}/delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!await messageService.DeleteAsync(id, cancellationToken))
        {
            return NotFound();
        }

        TempData["AdminMessage"] = "Message deleted.";
        return RedirectToAction(nameof(Index));
    }

    private IActionResult RedirectToLocal(string? returnUrl) =>
        !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToAction(nameof(Index));

    private static string NormalizeFilter(string? filter) =>
        filter?.ToLowerInvariant() is "read" or "unread" ? filter.ToLowerInvariant() : "all";
}
