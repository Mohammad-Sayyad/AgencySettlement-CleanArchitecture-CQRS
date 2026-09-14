using AgencySettlement.Application.Features.Queries.GetSettlementDetailsQuery;
using AgencySettlement.Application.Features.Queries.GetSettlementSelectionDetailsQuery;
using AgencySettlement.Application.Settlements.Queries.ReportQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgencySettlement.API.Settlment.Controllers
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
        //  [Authorize]
        //  [HttpGet("settlement")]
        //  public async Task<IActionResult> GetSettlementReport(
        //[FromQuery] int? agencyId,
        //[FromQuery] int yearId,
        //[FromQuery] string[] persianExecutionDate,
        //[FromQuery] int pageNumber = 1,
        //[FromQuery] int pageSize = 50,
        //CancellationToken cancellationToken = default)
        //  {
        //      var result = await _mediator.Send(
        //          new GetSettlementReportQuery(
        //              agencyId,
        //              yearId,
        //              persianExecutionDate,
        //              pageNumber,
        //              pageSize),
        //          cancellationToken);

        //      return Ok(result);
        //  }

        [HttpGet("settlement")]
        public async Task<IActionResult> GetSettlementReport(
    [FromQuery] int? agencyId,
    [FromQuery] int yearId,
    [FromQuery] string fromDateId,
    [FromQuery] string toDateId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 50,
    CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetSettlementReportQuery(
                    agencyId,
                    yearId,
                    fromDateId,
                    toDateId,
                    pageNumber,
                    pageSize),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("settlement/details")]
        public async Task<IActionResult> GetSettlementDetails(
     [FromQuery] int agencyId,
     [FromQuery] int yearId,
     [FromQuery] string persianExecutionDate,
     CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetSettlementDetailsQuery(
                    agencyId,
                    yearId,
                    persianExecutionDate),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("settlement/selection-details")]
        public async Task<IActionResult> GetSettlementSelectionDetails(
    [FromQuery] int agencyId,
    [FromQuery] long[] settlementIds,
    CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetSettlementSelectionDetailsQuery(
                    agencyId,
                    settlementIds),
                cancellationToken);

            return Ok(result);
        }
    }
}
