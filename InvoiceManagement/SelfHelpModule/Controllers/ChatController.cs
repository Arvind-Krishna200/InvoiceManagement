using Microsoft.AspNetCore.Mvc;
using InvoiceManagement.SelfHelpModule.Models;
namespace InvoiceManagement.SelfHelpModule.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        [HttpPost("reply")]
        public IActionResult GetBotReply([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Message cannot be empty.");

            var lower = request.Message.ToLower();
            string reply;

            // static bot logic
            if (lower.Contains("reset"))
                reply = "To reset your password, go to the 'Forgot Password' page.";
            else if (lower.Contains("account"))
                reply = "You can view account settings from the Profile section.";
            else
                reply = "Thanks for your message! We'll get back to you shortly.";

            return Ok(new { reply });
        }
    }
}
