using AgencySettlement.Application.Settlements.Queries.DebtQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgencySettlement.API.Controllers
{
    [ApiController]
    [Route("api/external/debt")]
    public sealed class ExternalDebtController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExternalDebtController(
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
                    new GetDebtQuery(
                        agencyId,
                        yearId,
                        persianExecutionDate),
                    cancellationToken);

            return Ok(result);
        }
    }
}
