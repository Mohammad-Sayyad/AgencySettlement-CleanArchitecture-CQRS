
using AgencySettlement.Application.DTOs;
using AgencySettlement.Application.ExternalExams.Commands;
using AgencySettlement.Application.ExternalExams.Commands.ImportExternalExams;
using AgencySettlement.Application.ExternalExamsFeatures.Queris.GetExternalSettlementStatusQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgencySettlement.API.Controllers;

[ApiController]
[Route("api/external-exams")]
public sealed class ExternalExamsController : ControllerBase
{
    private readonly ISender _sender;

    public ExternalExamsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import(
      [FromBody] ImportExamRequest request,
      CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ImportExamRecordsCommand(request),
            cancellationToken);

        return Ok(new
        {
            message = result.DuplicateRecords > 0
                ? $"اطلاعات دریافت شد. {result.DuplicateRecords} رکورد تکراری بود."
                : "اطلاعات آزمون‌ها با موفقیت دریافت شد.",

            totalReceived = result.TotalReceived,
            newRecords = result.NewRecords,
            duplicateRecords = result.DuplicateRecords
        });
    }

    [HttpGet("settlement-status")]
    public async Task<IActionResult> GetSettlementStatus(
      [FromQuery] int agencyId,
      [FromQuery] int yearId,
      [FromQuery] string persianExecutionDate,
      CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetExternalSettlementStatusQuery(
                agencyId,
                yearId,
                persianExecutionDate),
            cancellationToken);

        return Ok(result);
    }
}

