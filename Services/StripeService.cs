using Stripe;
using Stripe.Checkout;

namespace MaharlikaGrandEstate.Services
{
    public class StripeService
    {
        private readonly IConfiguration _configuration;

        public StripeService(IConfiguration configuration)
        {
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        public string CreateCheckoutSession(decimal amount, string propertyId)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "php",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Property Reservation",
                                Description = "Reserve a property for 10% of its price"
                            },
                            UnitAmount = (long)(amount * 100) // Convert PHP to cents
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = $"https://localhost:44342/Buyer/PaymentSuccess?propertyId={propertyId}",
                CancelUrl = $"https://localhost:44342/Buyer/Details/{propertyId}"
            };

            var service = new SessionService();
            Session session = service.Create(options);
            return session.Url;
        }
    }
}
