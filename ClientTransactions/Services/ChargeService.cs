using Stripe;
using Models.Domain;
using Models.Requests;
using Models.Responses;
using Models.Domain.Tools;
using Services.Tools;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using Data.Structured;
using Services.User;
using System.Linq;


namespace Services.Transactions
{
    public class ChargeService : BaseService, IChargeService
    {
        IEmailMessenger _emailMessenger;
        IEmailTemplateService _emailTemplateService;
        IUserBaseService _userBaseService;

        public ChargeService(IEmailMessenger emailMessenger, IEmailTemplateService emailTemplateService, IUserBaseService userBaseService)
        {
            _emailMessenger = emailMessenger;
            _emailTemplateService = emailTemplateService;
            _userBaseService = userBaseService;
        }

        //ONE-TIME CHARGE
        public void Charge(ChargeRequest model)
        {
            StripeConfiguration.SetApiKey("");

            var options = new StripeChargeCreateOptions
            {
                Amount = (int)(model.Amount * 100), //Stripe arg in units of cents (usd)
                Currency = model.Currency,
                SourceTokenOrExistingSourceId = model.SourceTokenOrExistingSourceId,
                ReceiptEmail = model.Email
            };
            var service = new StripeChargeService();
            StripeCharge charge = service.Create(options);

            //insert into transaction table
            TransactionsAddRequest transactionModel = new TransactionsAddRequest
            {
                UserBaseId = model.UserBaseId,
                PlanId = model.PlanId,
                DurationTypeId = model.DurationTypeId,
                DiscountPercent = model.DiscountPercent,
                ChargeId = charge.Id,
                AmountPaid = model.Amount,
                Currency = model.Currency,
                CardId = charge.Source.Card.Id,
                ExpMonth = charge.Source.Card.ExpirationMonth,
                ExpYear = charge.Source.Card.ExpirationYear,
                CardLast4 = charge.Source.Card.Last4,
                NetworkStatus = charge.Outcome.NetworkStatus,
                Type = charge.Outcome.Type,
                isPaid = charge.Paid,
                CreatedDate = charge.Created
            };
            InsertTransaction(transactionModel);
        }

        //CHARGE CUSTOMER (recurring)
        public void ChargeCustomer(ChargeRequest model)
        {
            StripeConfiguration.SetApiKey("");

            //Create customer for recurring charges
            var customerOptions = new StripeCustomerCreateOptions
            {
                SourceToken = model.SourceTokenOrExistingSourceId,
                Email = model.Email
            };
            var customerService = new StripeCustomerService();
            StripeCustomer customer = customerService.Create(customerOptions);

            //Charge the customer
            var chargeOptions = new StripeChargeCreateOptions
            {
                Amount = (int)(model.Amount * 100), //Stripe arg in units of cents (usd)
                Currency = model.Currency,
                CustomerId = customer.Id
            };

            var chargeService = new StripeChargeService();
            StripeCharge charge = chargeService.Create(chargeOptions);

            //insert into transaction table
            TransactionsAddRequest transactionModel = new TransactionsAddRequest
            {
                UserBaseId = model.UserBaseId,
                PlanId = model.PlanId,
                DurationTypeId = model.DurationTypeId,
                DiscountPercent = model.DiscountPercent,
                ChargeId = charge.Id,
                AmountPaid = model.Amount,
                Currency = model.Currency,
                CardId = charge.Source.Card.Id,
                ExpMonth = charge.Source.Card.ExpirationMonth,
                ExpYear = charge.Source.Card.ExpirationYear,
                CardLast4 = charge.Source.Card.Last4,
                NetworkStatus = charge.Outcome.NetworkStatus,
                Type = charge.Outcome.Type,
                isPaid = charge.Paid,
                CreatedDate = charge.Created
            };

            InsertTransaction(transactionModel);

            //insert into customer subscription table
            DateTime _nextBillingDate = (customer.Created).AddMonths(model.DurationMonths);

            UserSubscriptionsAddRequest usersubModel = new UserSubscriptionsAddRequest
            {
                CustomerId = customer.Id,
                CustomerName = model.Name,
                CustomerEmail = model.Email,
                PlanName = model.PlanName,
                DurationName = model.DurationName,
                DiscountPercent = model.DiscountPercent,
                Price = model.Amount,
                Currency = model.Currency,
                StartDate = customer.Created,
                NextBillingDate = _nextBillingDate,
                UserBaseId = model.UserBaseId,
                PlanId = model.PlanId,
                DurationTypeId = model.DurationTypeId,
                DurationMonths = model.DurationMonths,
                IsActive = true,
                CurrentCard = charge.Source.Card.Last4,
                CardExpMonth = charge.Source.Card.ExpirationMonth,
                CardExpYear = charge.Source.Card.ExpirationYear
            };

            InsertCustomerSubscription(usersubModel);

        }

        public List<UserSubscriptions> ReadAllSubscriptions()
        {
            List<UserSubscriptions> list = new List<UserSubscriptions>();
            DataProvider.ExecuteCmd("dbo.UserSubscriptions_SelectAll",
                inputParamMapper: null,
                singleRecordMapper: (IDataReader reader, short resultSet) =>
                {
                    list.Add(DataMapper<UserSubscriptions>.Instance.MapToObject(reader));
                });
            return list;
        }

        public void InsertCustomerSubscription(UserSubscriptionsAddRequest model)
        {
            DataProvider.ExecuteNonQuery("dbo.UserSubscriptions_Insert",
                inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@CustomerId", model.CustomerId, SqlDbType.NVarChar, 50));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@CustomerName", model.CustomerName, SqlDbType.NVarChar));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@CustomerEmail", model.CustomerEmail, SqlDbType.NVarChar));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@PlanName", model.PlanName, SqlDbType.NVarChar));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@DurationName", model.DurationName, SqlDbType.NVarChar));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@DiscountPercent", model.DiscountPercent, SqlDbType.Decimal));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@Price", model.Price, SqlDbType.Decimal));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@Currency", model.Currency, SqlDbType.NVarChar, 10));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@StartDate", model.StartDate, SqlDbType.DateTime));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@NextBillingDate", model.NextBillingDate, SqlDbType.DateTime));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@UserBaseId", model.UserBaseId, SqlDbType.Int));
                    //isActive automatically true when user is charged 
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@IsActive", model.IsActive, SqlDbType.Bit));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@CurrentCard", model.CurrentCard, SqlDbType.NVarChar, 4));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@CardExpMonth", model.CardExpMonth, SqlDbType.Int));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@CardExpYear", model.CardExpYear, SqlDbType.Int));
                });
        }

        public int InsertTransaction(TransactionsAddRequest model)
        {
            int returnValue = 0;

            DataProvider.ExecuteNonQuery("dbo.PlanTransactions_Insert",
                inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@UserBaseId", model.UserBaseId, SqlDbType.Int));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@PlanId", model.PlanId, SqlDbType.Int));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@DurationTypeId", model.DurationTypeId, SqlDbType.Int));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@ChargeId", model.ChargeId, SqlDbType.NVarChar, 50));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@AmountPaid", model.AmountPaid, SqlDbType.Int));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@Currency", model.Currency, SqlDbType.NVarChar, 10));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@NetworkStatus", model.NetworkStatus, SqlDbType.NVarChar, 50));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@Type", model.Type, SqlDbType.NVarChar, 50));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@CreatedDate", model.CreatedDate, SqlDbType.DateTime));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@isPaid", model.isPaid, SqlDbType.Bit));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@CardId", model.CardId, SqlDbType.NVarChar, 50));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@ExpMonth", model.ExpMonth, SqlDbType.Int));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@ExpYear", model.ExpYear, SqlDbType.Int));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@CardLast4", model.CardLast4, SqlDbType.VarChar, 4));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@DiscountPercent", model.DiscountPercent, SqlDbType.Decimal));

                    SqlParameter idOut = new SqlParameter("@Id", 0);
                    idOut.Direction = ParameterDirection.Output;

                    inputs.Add(idOut);
                },
                returnParameters: (SqlParameterCollection inputs) =>
                {
                    int.TryParse(inputs["@Id"].Value.ToString(), out returnValue);
                });

            return returnValue;
        }

        //CHARGE EXISTING CUSTOMERS 
        public List<ChargeExistingUser> GetUsersToCharge()
        {
            List<ChargeExistingUser> existingUserList = null;
            DataProvider.ExecuteCmd("dbo.UserSubscriptions_SelectCustomersToBill",
                inputParamMapper: null,
                singleRecordMapper: (IDataReader reader, short resultSet) =>
                {
                    if (existingUserList == null)
                    {
                        existingUserList = new List<ChargeExistingUser>();
                    }

                    existingUserList.Add(DataMapper<ChargeExistingUser>.Instance.MapToObject(reader));
                });

            if (existingUserList != null)
            {
                ChargeExistingUser model = null;

                foreach (ChargeExistingUser existingUser in existingUserList.ToList())
                {
                    try
                    {
                        model = existingUser;
                        TransactionsAddRequest transaction = ChargeExistingCustomer(existingUser);
                        InsertTransaction(transaction);
                        DateTime _newBillDate = existingUser.NextBillingDate.AddMonths(existingUser.DurationMonths);
                        existingUser.NextBillingDate = _newBillDate;
                    }
                    catch (Exception ex)
                    {
                        TransactionFailAddRequest failedTransaction = new TransactionFailAddRequest
                        {
                            UserBaseId = model.UserBaseId,
                            CustomerId = model.CustomerId,
                            Amount = model.Price,
                            Currency = model.Currency,
                            Name = model.CustomerName,
                            Email = model.CustomerEmail,
                            PlanId = model.PlanId,
                            DurationTypeId = model.DurationTypeId,
                            DiscountPercent = model.DiscountPercent,
                            ErrorMessage = ex.Message
                        };

                        //will lock UserBaseId when transaction failure is inserted
                        this.InsertTransactionFail(failedTransaction);
                        //will deactivate user once transaction fails
                        //this.UpdateUserIsActive(failedTransaction);

                        //create Token/GUID on AppToken table with the UserBaseId
                        ItemResponse<string> Token = new ItemResponse<string> { Item = this.CreateTokenForFailedTransactions(failedTransaction) };

                        Email eml = new Email();

                        MessageAddress msgAdd = new MessageAddress
                        {
                            Email = "jtruongen@yahoo.com",
                            Name = model.CustomerName
                        };

                        List<MessageAddress> list = new List<MessageAddress>
                        {
                            msgAdd
                        };

                        eml.To = list;
                        eml.FromAddress = "Eleveightc56@gmail.com";
                        eml.FromName = "Eleveight";
                        eml.Subject = "Subscription Charge Failure";
                        eml.HtmlBody = _emailTemplateService.CreateFailedPayments(new EmailPaymentFailTemplateInput
                        {
                            CustomerName = msgAdd.Name,
                            Token = Token.Item,
                            ExtraInfo = "Your subscription has been deactivated due to failed recurring charges. " +
                            "Please call Eleveight customer service to reactivate your subscription"
                        });

                        _emailMessenger.SendMail(eml);

                        //remove from list if transaction failed
                        existingUserList.Remove(existingUser); 
                    }

                }
                if (existingUserList.Count != 0)
                {
                    UpdateMultipleCustomerSubscription(existingUserList);
                }
            }

            return existingUserList;
        }

        //Deactivate user
        public void UpdateUserIsActive(TransactionFailAddRequest model)
        {
            DataProvider.ExecuteNonQuery("dbo.UserSubscriptions_UpdateUserIsActive",
            inputParamMapper: (SqlParameterCollection inputs) =>
            {
                inputs.Add(SqlDbParameter.Instance.BuildParameter("@CustomerId", model.CustomerId, SqlDbType.NVarChar, 50));
                inputs.Add(SqlDbParameter.Instance.BuildParameter("@IsActive", false, SqlDbType.Bit));
            });
        }

        public void UpdateMultipleCustomerSubscription(List<ChargeExistingUser> models)
        {
            DataProvider.ExecuteNonQuery("dbo.UserSubscriptions_UpdateMultipleBillingDates",
                    inputParamMapper: (SqlParameterCollection inputs) =>
                    {
                        SqlParameter umbdUDT = new SqlParameter("@UserSubBillDate", SqlDbType.Structured);
                        umbdUDT.Value = new GenericTable<ChargeExistingUser>(models);
                        inputs.Add(umbdUDT);
                    });
        }

        public void UpdateNextBillingDate(ChargeExistingUser model)
        {
            DataProvider.ExecuteNonQuery("dbo.UserSubscriptions_UpdateBillingDate",
            inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@CustomerId", model.CustomerId, SqlDbType.NVarChar, 50));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@NextBillingDate", model.NextBillingDate, SqlDbType.DateTime));
                });
        }

        public TransactionsAddRequest ChargeExistingCustomer(ChargeExistingUser model)
        {
            StripeConfiguration.SetApiKey("");

            double _convertAmountToPennies = model.Price * 100;
            int _convertToIntAmount = Convert.ToInt32(_convertAmountToPennies);
            //Charge the customer
            var chargeOptions = new StripeChargeCreateOptions
            {
                Amount = _convertToIntAmount,
                Currency = model.Currency,
                CustomerId = model.CustomerId
            };

            var chargeService = new StripeChargeService();
            StripeCharge charge = chargeService.Create(chargeOptions);

            //insert into transaction table
            TransactionsAddRequest transaction = new TransactionsAddRequest
            {
                UserBaseId = model.UserBaseId,
                PlanId = model.PlanId,
                DurationTypeId = model.DurationTypeId,
                ChargeId = charge.Id,
                AmountPaid = _convertToIntAmount / 100,
                Currency = model.Currency,
                CardId = charge.Source.Card.Id,
                ExpMonth = charge.Source.Card.ExpirationMonth,
                ExpYear = charge.Source.Card.ExpirationYear,
                CardLast4 = charge.Source.Card.Last4,
                NetworkStatus = charge.Outcome.NetworkStatus,
                Type = charge.Outcome.Type,
                isPaid = charge.Paid,
                CreatedDate = charge.Created,
                DiscountPercent = model.DiscountPercent
            };

            return transaction;
        }

        public void UpdateCustomerCard(UserSubscriptionUpdateStripeRequest model)
        {
            StripeConfiguration.SetApiKey("");

            var options = new StripeCustomerUpdateOptions
            {
                SourceToken = model.SourceTokenOrExistingSourceId
            };

            var customerService = new StripeCustomerService();
            StripeCustomer customer = customerService.Update(model.CustomerId, options);

            UserSubscriptionsUpdateRequest userSubCardUpdate = new UserSubscriptionsUpdateRequest
            {
                CustomerId = model.CustomerId,
                CurrentCard = customer.Sources.Data[0].Card.Last4,
                CardExpMonth = customer.Sources.Data[0].Card.ExpirationMonth,
                CardExpYear = customer.Sources.Data[0].Card.ExpirationYear
            };

            UpdateCustomerCardInfo(userSubCardUpdate);
        }

        public void UpdateCustomerCardInfo(UserSubscriptionsUpdateRequest model)
        {
            DataProvider.ExecuteNonQuery("dbo.UserSubscriptions_UpdateCardInfo",
                inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@CustomerId", model.CustomerId, SqlDbType.NVarChar, 50));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@CurrentCard", model.CurrentCard, SqlDbType.NVarChar, 4));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@CardExpMonth", model.CardExpMonth, SqlDbType.Int));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@CardExpYear", model.CardExpYear, SqlDbType.Int));
                });
        }

        public int InsertTransactionFail(TransactionFailAddRequest model)
        {
            int returnValue = 0;

            DataProvider.ExecuteNonQuery("dbo.PlanTransactionsFailed_Insert",
                inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@UserBaseId", model.UserBaseId, SqlDbType.Int));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@CustomerId", model.CustomerId, SqlDbType.NVarChar, 50));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@Name", model.Name, SqlDbType.NVarChar, 50));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@Email", model.Email, SqlDbType.NVarChar, 50));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@PlanId", model.PlanId, SqlDbType.Int));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@DurationTypeId", model.DurationTypeId, SqlDbType.Int));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@AmountAttempted", model.Amount, SqlDbType.Int));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@Currency", model.Currency, SqlDbType.NVarChar, 10));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@DiscountPercent", model.DiscountPercent, SqlDbType.Decimal));
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@ErrorMessage", model.ErrorMessage, SqlDbType.NVarChar, 100));

                    SqlParameter idOut = new SqlParameter("@Id", 0);
                    idOut.Direction = ParameterDirection.Output;

                    inputs.Add(idOut);
                },
                returnParameters: (SqlParameterCollection inputs) =>
                {
                    int.TryParse(inputs["@Id"].Value.ToString(), out returnValue);
                });

            return returnValue;
        }

        //list all customer cards
        public StripeList<StripeCard> ListAllCards(UserSubscriptionUpdateStripeRequest model)
        {
            StripeConfiguration.SetApiKey("");
            var cardService = new StripeCardService();
            StripeList<StripeCard> response = cardService.List(
              model.CustomerId,
              new StripeCardListOptions()
              {
                  Limit = 3
              }
            );
            return response;
        }

        //search for users with transaction failures but only ones who are existing customers
        public List<UserSubscriptionUpdateMultiLock> SelectUsersToLock()
        {
            List<UserSubscriptionUpdateMultiLock> list = null;
            DataProvider.ExecuteCmd("dbo.PlanTransactionsFailed_SelectUsersToLock",
                inputParamMapper: null,
                singleRecordMapper: (IDataReader reader, short resultSet) =>
                {
                    if (list == null)
                    {
                        list = new List<UserSubscriptionUpdateMultiLock>();
                    }

                    list.Add(DataMapper<UserSubscriptionUpdateMultiLock>.Instance.MapToObject(reader));
                });

            UpdateMultipleUnpaidUsers(list);

            return list;
        }

        //lock multiple list of userbaseids that match transactions failures
        public void UpdateMultipleUnpaidUsers(List<UserSubscriptionUpdateMultiLock> models)
        {
            DataProvider.ExecuteNonQuery("dbo.UserBase_LockMultipleUnpaid",
                inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    SqlParameter ublUDT = new SqlParameter("@UserBaseLock", SqlDbType.Structured);
                    ublUDT.Value = new GenericTable<UserSubscriptionUpdateMultiLock>(models);
                    inputs.Add(ublUDT);
                });
        }

        public string CreateTokenForFailedTransactions(TransactionFailAddRequest model)
        {
            string GUID = null;

            DataProvider.ExecuteNonQuery("dbo.AppToken_InsertFailedTransaction",
                inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    inputs.Add(SqlDbParameter.Instance.BuildParameter("@UserBaseId", model.UserBaseId, SqlDbType.Int));

                    SqlParameter guidOut = new SqlParameter("@Token", SqlDbType.NVarChar, 128);
                    guidOut.Direction = ParameterDirection.Output;

                    inputs.Add(guidOut);
                },
                returnParameters: (SqlParameterCollection inputs) =>
                {
                    GUID = inputs["@Token"].Value.ToString();
                });

            return GUID;
        }

        public void RenewalCustomerCharge(ChargeExistingUser model)
        {
            StripeConfiguration.SetApiKey("");

            double _convertAmountToPennies = model.Price * 100;
            int _convertToIntAmount = Convert.ToInt32(_convertAmountToPennies);
            //Charge the customer
            var chargeOptions = new StripeChargeCreateOptions
            {
                Amount = _convertToIntAmount,
                Currency = model.Currency,
                CustomerId = model.CustomerId
                //model.CustomerId
            };

            var chargeService = new StripeChargeService();
            StripeCharge charge = chargeService.Create(chargeOptions);

            //insert into transaction table
            TransactionsAddRequest transaction = new TransactionsAddRequest
            {
                UserBaseId = model.UserBaseId,
                PlanId = model.PlanId,
                DurationTypeId = model.DurationTypeId,
                ChargeId = charge.Id,
                AmountPaid = _convertToIntAmount / 100,
                Currency = model.Currency,
                CardId = charge.Source.Card.Id,
                ExpMonth = charge.Source.Card.ExpirationMonth,
                ExpYear = charge.Source.Card.ExpirationYear,
                CardLast4 = charge.Source.Card.Last4,
                NetworkStatus = charge.Outcome.NetworkStatus,
                Type = charge.Outcome.Type,
                isPaid = charge.Paid,
                CreatedDate = charge.Created,
                DiscountPercent = model.DiscountPercent
            };

            InsertTransaction(transaction);

            ChargeExistingUser customer = model;

            DateTime _newBillDate = charge.Created.AddMonths(customer.DurationMonths);
            customer.NextBillingDate = _newBillDate;

            UpdateNextBillingDate(customer);

            _userBaseService.UpdateAccoutUnlocked(model.UserBaseId);
        }
        
    }
}



