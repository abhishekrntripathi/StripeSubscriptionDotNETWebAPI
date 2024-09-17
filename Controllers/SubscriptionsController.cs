using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using StripeUsingDotNETWebAPI.Model;
using StripeUsingDotNETWebAPI.Service;

namespace StripeUsingDotNETWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly StripeService _stripeService;
        private readonly StripeSettings _stripeSettings;

        public SubscriptionsController(StripeService stripeService, IOptions<StripeSettings> stripeSettings)
        {
            _stripeService = stripeService;
            _stripeSettings = stripeSettings.Value;
        }

        // Endpoint to create a subscription
        [HttpPost("create-subscription")]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequest request)
        {
            // Step 1: Create customer
            var customer = await _stripeService.CreateCustomerAsync(request.Email, request.PaymentMethodId);

            // Step 2: Create subscription
            var subscription = await _stripeService.CreateSubscriptionAsync(customer.Id, request.SubscriptionPlanId);

            return Ok(new
            {
                CustomerId = customer.Id,
                SubscriptionId = subscription.Id
            });
        }

        // Endpoint to handle Stripe webhooks
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _stripeSettings.WebhookSecret
                );

                // Pass the event to the StripeService for handling
                await _stripeService.HandleWebhookEventAsync(stripeEvent);

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(e.Message);
            }
        }
    }

}
