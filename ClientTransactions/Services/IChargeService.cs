using Models.Domain;
using Models.Requests;
using Stripe;
using System.Collections.Generic;

namespace Services.Transactions
{
    public interface IChargeService
    {
        void Charge(ChargeRequest model);
        void ChargeCustomer(ChargeRequest model);
        List<UserSubscriptions> ReadAllSubscriptions();
        void InsertCustomerSubscription(UserSubscriptionsAddRequest model);
        int InsertTransaction(TransactionsAddRequest model);
        int InsertTransactionFail(TransactionFailAddRequest model);
        List<ChargeExistingUser> GetUsersToCharge();
        TransactionsAddRequest ChargeExistingCustomer(ChargeExistingUser model);
        void UpdateNextBillingDate(ChargeExistingUser model);
        void UpdateMultipleCustomerSubscription(List<ChargeExistingUser> models);
        void UpdateCustomerCard(UserSubscriptionUpdateStripeRequest model);
        void UpdateCustomerCardInfo(UserSubscriptionsUpdateRequest model);
        StripeList<StripeCard> ListAllCards(UserSubscriptionUpdateStripeRequest model);
        void RenewalCustomerCharge(ChargeExistingUser model);
    }
}