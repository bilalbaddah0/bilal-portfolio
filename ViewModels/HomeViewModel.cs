using BilalPortfolio.DTOs;
using BilalPortfolio.Options;

namespace BilalPortfolio.ViewModels;

public sealed class HomeViewModel
{
    public required PortfolioOptions Portfolio { get; init; }
    public ContactMessageDto Contact { get; init; } = new();
}
