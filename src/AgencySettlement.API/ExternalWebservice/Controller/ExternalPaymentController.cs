using AgencySettlement.Application.Settlements.Queries.PaymentQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgencySettlement.API.ExteralWebService.Controller
{
    [ApiController]
    [Route("api/external/payment")]
    public sealed class ExternalPaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExternalPaymentController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] int agencyId,
            [FromQuery] int yearId,
            [FromQuery] string persianExecutionDate,
            CancellationToken cancellationToken)
        {
            var result =
                await _mediator.Send(
                    new GetPaymentQuery(
                        agencyId,
                        yearId,
                        persianExecutionDate),
                    cancellationToken);

            return Ok(result);
        }
    }
}
