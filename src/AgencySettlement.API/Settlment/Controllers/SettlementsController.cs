using AgencySettlement.Application.DTOs;
using AgencySettlement.Application.Settlements.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgencySettlement.API.Settlment.Controllers;


    [ApiController]
    [Route("api/settlements")]
    public sealed class SettlementsController : ControllerBase
    {
        private readonly ISender _sender;

        public SettlementsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate(
            [FromBody] CalculateSettlementRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new CalculateSettlementCommand(request),
                cancellationToken);

            return Ok(result);
        }
    }

