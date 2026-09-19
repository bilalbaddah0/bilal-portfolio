using BilalPortfolio.DTOs;
using BilalPortfolio.Models;

namespace BilalPortfolio.Services.Interfaces;

public interface IContactMessageService
{
    Task<int> CreateAsync(ContactMessageDto request, CancellationToken cancellationToken = default);
    Task<ContactMessage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<ContactMessage> Items, int Total)> GetPageAsync(
        string? search,
        string? filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<(int Total, int Unread, int Today)> GetStatisticsAsync(CancellationToken cancellationToken = default);
    Task<bool> SetReadStatusAsync(int id, bool isRead, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
