using Microsoft.AspNetCore.Mvc;
using StripeUsingDotNETWebAPI.Model;
using StripeUsingDotNETWebAPI.Service;

namespace StripeUsingDotNETWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly StripeService _stripeService;

        public SubscriptionController(StripeService stripeService)
        {
            _stripeService = stripeService;
        }

        [HttpPost("create-customer")]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            try
            {
                var customer = await _stripeService.CreateCustomerAsync(request.Email, request.PaymentMethodId);
                var subscription = await _stripeService.CreateSubscriptionAsync(customer.Id, request.SubscriptionPlanId);
                return Ok(new { CustomerId = customer.Id, SubscriptionId = subscription.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
