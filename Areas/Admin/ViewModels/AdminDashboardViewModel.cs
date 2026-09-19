using BilalPortfolio.Models;

namespace BilalPortfolio.Areas.Admin.ViewModels;

public sealed class AdminDashboardViewModel
{
    public required IReadOnlyList<ContactMessage> Messages { get; init; }
    public required string Filter { get; init; }
    public required string Search { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalMessages { get; init; }
    public int UnreadMessages { get; init; }
    public int MessagesToday { get; init; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalItems / (double)PageSize));
}
