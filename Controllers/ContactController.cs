using BilalPortfolio.DTOs;
using BilalPortfolio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BilalPortfolio.Controllers;

[ApiController]
public sealed class ContactController(
    IContactMessageService messageService,
    ILogger<ContactController> logger) : ControllerBase
{
    [HttpPost("/contact")]
    [EnableRateLimiting("contact")]
    [RequestSizeLimit(32_768)]
    public async Task<IActionResult> Create(
        [FromForm] ContactMessageDto request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Website))
        {
            return Ok(new { success = true, message = "Thank you. Your message was received." });
        }

        try
        {
            await messageService.CreateAsync(request, cancellationToken);
            return Ok(new { success = true, message = "Thank you. Your message has been sent." });
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to save a contact message.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                success = false,
                message = "The message service is temporarily unavailable. Please try again shortly."
            });
        }
    }
}
