using Microsoft.Extensions.Options;
using Stripe;
using StripeUsingDotNETWebAPI.Model;

namespace StripeUsingDotNETWebAPI.Service
{
    public class StripeService
    {
        private readonly StripeSettings _stripeSettings;

        public StripeService(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        // Create customer
        public async Task<Customer> CreateCustomerAsync(string email, string paymentMethodId)
        {
            var options = new CustomerCreateOptions
            {
                Email = email,
                PaymentMethod = paymentMethodId,
                InvoiceSettings = new CustomerInvoiceSettingsOptions
                {
                    DefaultPaymentMethod = paymentMethodId
                }
            };

            var service = new CustomerService();
            return await service.CreateAsync(options);
        }

        // Create subscription
        public async Task<Subscription> CreateSubscriptionAsync(string customerId, string priceId)
        {
            var options = new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = new List<SubscriptionItemOptions>
            {
                new SubscriptionItemOptions
                {
                    Price = priceId
                }
            },
                Expand = new List<string> { "latest_invoice.payment_intent" }
            };

            var service = new SubscriptionService();
            return await service.CreateAsync(options);
        }

        // Handle webhook events (e.g., subscription update or payment failures)
        public async Task HandleWebhookEventAsync(Event stripeEvent)
        {
            switch (stripeEvent.Type)
            {
                case "invoice.payment_succeeded":
                    // Handle payment success
                    break;

                case "invoice.payment_failed":
                    // Handle payment failure
                    break;

                default:
                    break;
            }

            await Task.CompletedTask;
        }
    }
}
