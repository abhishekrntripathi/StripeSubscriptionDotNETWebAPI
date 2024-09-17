﻿namespace StripeUsingDotNETWebAPI.Model
{
    public class CreateCustomerRequest
    {
        public string Email { get; set; }
        public string PaymentMethodId { get; set; }
        public string SubscriptionPlanId { get; set; }
    }
}
