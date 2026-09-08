using MediatR;
using Microsoft.AspNetCore.Mvc;
using static AgencySettlement.Application.Settlements.Queries.GetSettlementFiltersLookupQuery.GetSettlementLookupDtoQuery;

namespace AgencySettlement.API.Controllers
{
    [ApiController]
    [Route("api/Combo")]
    public sealed class SettlementLookupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SettlementLookupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("agencies")]
        public async Task<IActionResult> GetAgencies(
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetAgenciesQuery(),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("years")]
        public async Task<IActionResult> GetYears(
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetYearsQuery(),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("exam-dates")]
        public async Task<IActionResult> GetExamDates(
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetExamDatesQuery(),
                cancellationToken);

            return Ok(result);
        }
    }
}
