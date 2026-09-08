using AgencySettlement.Application.Settlements.Queries.ReportQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgencySettlement.API.Controllers
{
    [ApiController]
    [Route("api/report")]
    public sealed class SettlementReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SettlementReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("settlement")]
        public async Task<IActionResult> GetSettlementReport(
            [FromQuery] int? agencyId,
            [FromQuery] int yearId,
            [FromQuery] string persianExecutionDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetSettlementReportQuery(
                    agencyId,
                    yearId,
                    persianExecutionDate,
                    pageNumber,
                    pageSize),
                cancellationToken);

            return Ok(result);
        }
    }
}
