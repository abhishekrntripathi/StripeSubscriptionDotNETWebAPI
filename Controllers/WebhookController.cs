using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace StripeUsingDotNETWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public WebhookController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("stripe")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"],
                    _configuration["Stripe:WebhookSecret"]);

                if (stripeEvent.Type == Events.InvoicePaymentSucceeded)
                {
                    var invoice = stripeEvent.Data.Object as Invoice;
                    // Handle successful payment
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionCreated)
                {
                    var subscription = stripeEvent.Data.Object as Subscription;
                    // Handle subscription creation
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
