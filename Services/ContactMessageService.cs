using BilalPortfolio.Data;
using BilalPortfolio.DTOs;
using BilalPortfolio.Models;
using BilalPortfolio.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BilalPortfolio.Services;

public sealed class ContactMessageService(ApplicationDbContext dbContext) : IContactMessageService
{
    public async Task<int> CreateAsync(ContactMessageDto request, CancellationToken cancellationToken = default)
    {
        var message = new ContactMessage
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            Subject = request.Subject.Trim(),
            Message = request.Message.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            IsRead = false
        };

        dbContext.ContactMessages.Add(message);
        await dbContext.SaveChangesAsync(cancellationToken);
        return message.Id;
    }

    public Task<ContactMessage?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.ContactMessages.AsNoTracking().SingleOrDefaultAsync(message => message.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<ContactMessage> Items, int Total)> GetPageAsync(
        string? search,
        string? filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.ContactMessages.AsNoTracking();

        if (string.Equals(filter, "unread", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(message => !message.IsRead);
        }
        else if (string.Equals(filter, "read", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(message => message.IsRead);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(message =>
                message.FullName.Contains(term) ||
                message.Email.Contains(term) ||
                message.Subject.Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(message => message.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<(int Total, int Unread, int Today)> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var startOfTodayUtc = DateTime.UtcNow.Date;
        var total = await dbContext.ContactMessages.CountAsync(cancellationToken);
        var unread = await dbContext.ContactMessages.CountAsync(message => !message.IsRead, cancellationToken);
        var today = await dbContext.ContactMessages.CountAsync(message => message.CreatedAtUtc >= startOfTodayUtc, cancellationToken);
        return (total, unread, today);
    }

    public async Task<bool> SetReadStatusAsync(int id, bool isRead, CancellationToken cancellationToken = default)
    {
        var message = await dbContext.ContactMessages.FindAsync([id], cancellationToken);
        if (message is null)
        {
            return false;
        }

        message.IsRead = isRead;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var message = await dbContext.ContactMessages.FindAsync([id], cancellationToken);
        if (message is null)
        {
            return false;
        }

        dbContext.ContactMessages.Remove(message);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
