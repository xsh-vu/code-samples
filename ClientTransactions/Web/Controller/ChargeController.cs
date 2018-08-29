using Models.Responses;
using Models.Domain;
using Models.Requests;
using Models.Requests.AppToken;
using Services;
using Services.Transactions;
using Services.User;
using Services.AppToken;
using System;
using System.Web.Http;
using Stripe;


namespace Web.Controllers.Api.Transactions
{
    [RoutePrefix("api/charges")]
    public class ChargeController : ApiController
    {
        IChargeService _chargeService;
        IUserService _userService;
        IUserBaseService _userBaseService;
        IAppLogService _appLogService;
        int _currentUserId;

        public ChargeController(IChargeService ChargeService, IUserService userService, IUserBaseService userBaseService, IAppLogService appLogService)
        {
            _chargeService = ChargeService;
            _userService = userService;
            _userBaseService = userBaseService;
            _currentUserId = _userService.GetCurrentUserId();
            _appLogService = appLogService;
        }

        [Route(), HttpPost]
        public IHttpActionResult PostCharge(ChargeRequest model)
        {
            model.UserBaseId = _currentUserId;
            try
            {
                _chargeService.ChargeCustomer(model);
                return Ok(new SuccessResponse());
            }
            catch (Exception ex)
            {
                _chargeService.InsertTransactionFail(new TransactionFailAddRequest
                {
                  UserBaseId = model.UserBaseId,
                  Amount = model.Amount,
                  Currency = model.Currency,
                  Name = model.Name,
                  Email = model.Email,
                  PlanId = model.PlanId,
                  DurationTypeId = model.DurationTypeId,
                  DiscountPercent = model.DiscountPercent,
                  ErrorMessage = ex.Message
                });

                _appLogService.Insert(new AppLogAddRequest
                {
                    AppLogTypeId = 1,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Title = "Error in " + GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    UserBaseId = _currentUserId
                });

                return BadRequest(ex.Message);
            }
        }

        [Route(), HttpGet]
        public IHttpActionResult GetUserSubscriptions()
        {
            try
            {
                ItemsResponse<UserSubscriptions> response = new ItemsResponse<UserSubscriptions>();
                response.Items = _chargeService.ReadAllSubscriptions();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _appLogService.Insert(new AppLogAddRequest
                {
                    AppLogTypeId = 1,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Title = "Error in " + GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    UserBaseId = _currentUserId
                });

                return BadRequest(ex.Message);
            }
        }

        [Route("update"), HttpPost]
        public IHttpActionResult UpdateCard(UserSubscriptionUpdateStripeRequest model)
        {
            try
            {
                _chargeService.UpdateCustomerCard(model);
                return Ok(new SuccessResponse());
            }
            catch (Exception ex)
            {
                _appLogService.Insert(new AppLogAddRequest
                {
                    AppLogTypeId = 1,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Title = "Error in " + GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    UserBaseId = _currentUserId
                });

                return BadRequest(ex.Message);
            }
        }

        [Route("getallcards"), HttpGet]
        public IHttpActionResult GetAllCustomerCards(UserSubscriptionUpdateStripeRequest model)
        {
            try
            {
                StripeList<StripeCard> response = new StripeList<StripeCard>();
                response = _chargeService.ListAllCards(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _appLogService.Insert(new AppLogAddRequest
                {
                    AppLogTypeId = 1,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Title = "Error in " + GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    UserBaseId = _currentUserId
                });

                return BadRequest(ex.Message);
            }
        }

        //TEMPORARY
        [Route("testing"), HttpGet]
        public void GetUsersToBill()
        {
            _chargeService.GetUsersToCharge();
        }

        [Route("renewsubscription"), HttpPost]
        public IHttpActionResult ChargeExisting(ChargeExistingUser model)
        {
            try
            {
                _chargeService.RenewalCustomerCharge(model);
                return Ok(new SuccessResponse());
            }
            catch (Exception ex)
            {
                _chargeService.InsertTransactionFail(new TransactionFailAddRequest
                {
                    UserBaseId = model.UserBaseId,
                    Amount = model.Price,
                    Currency = model.Currency,
                    Name = model.CustomerName,
                    Email = model.CustomerEmail,
                    PlanId = model.PlanId,
                    DurationTypeId = model.DurationTypeId,
                    DiscountPercent = model.DiscountPercent,
                    ErrorMessage = ex.Message
                });

                _appLogService.Insert(new AppLogAddRequest
                {
                    AppLogTypeId = 1,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Title = "Error in " + GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    UserBaseId = _currentUserId
                });

                return BadRequest(ex.Message);
            }
        }
    }
}