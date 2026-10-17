using AgencySettlement.Application.DTOs;
using AgencySettlement.Application.DTOs.SingleLessonDto;
using AgencySettlement.Application.ExternalExams.Commands;
using AgencySettlement.Application.ExternalExams.Commands.ImportExternalExams;
using AgencySettlement.Application.ExternalExamsFeatures.Commands.CreateSettlementPaymentCommand;
using AgencySettlement.Application.ExternalExamsFeatures.Commands.ImportSingleLessons;
using AgencySettlement.Application.ExternalExamsFeatures.Queris.GetExternalSettlementStatusQuery;
using AgencySettlement.Application.ExternalExamsFeatures.Queris.GetSettlementDatesQuery;
using AgencySettlement.Application.ExternalExamsFeatures.Queris.GetSettlementOrderReportQuery;
using AgencySettlement.Application.Settlements.Queries.DebtQuery;
using AgencySettlement.Application.Settlements.Queries.PaymentQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgencySettlement.API.ExteralWebService.Controller;

[ApiController]
[Route("api/external-exams")]
public sealed class ExternalExamsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMediator _mediator;
    private readonly ILogger<ExternalExamsController> _logger;
    public ExternalExamsController(ISender sender, IMediator mediator, ILogger<ExternalExamsController> logger)
    {
        _sender = sender;
        _mediator = mediator;
        _logger = logger;
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

    [HttpGet("send-debt")]
    public async Task<IActionResult> SendDebt(
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
    [HttpGet("get-payment")]
    public async Task<IActionResult> GetPayment(
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

    [HttpGet("agencies-exam-dates")]
    public async Task<IActionResult> GetAgenciesDates(
           [FromQuery] int agencyId,
           [FromQuery] int yearId,
           CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetPersianAgenciesnDatesQuery(
                    agencyId,
                    yearId
                    ),
                cancellationToken);

        return Ok(result);
    }


    [HttpPost("add-payment")]
    public async Task<IActionResult> AddPayment(
       [FromBody] CreateSettlementPaymentCommand command,
       CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }


    [HttpGet("settlement-order-1")]
    public async Task<IActionResult> GetSettlementOrder1(
    [FromQuery] int agencyId,
    [FromQuery] int yearId,
    [FromQuery] string persianExecutionDate,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSettlementOrderReportQuery(
                agencyId,
                yearId,
                persianExecutionDate,
                1),
            cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }


    [HttpGet("settlement-order-2")]
    public async Task<IActionResult> GetSettlementOrder2(
    [FromQuery] int agencyId,
    [FromQuery] int yearId,
    [FromQuery] string persianExecutionDate,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSettlementOrderReportQuery(
                agencyId,
                yearId,
                persianExecutionDate,
                2),
            cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("single-lessons/import")]
    public async Task<IActionResult> ImportSingleLessons(
    [FromBody] ImportSingleLessonRequest request,
    CancellationToken cancellationToken)
    {

        var sw = Stopwatch.StartNew();
        var result = await _sender.Send(
            new ImportSingleLessonsCommand(request),
            cancellationToken);

       

        return Ok(new
        {
            message = result.DuplicateRecords > 0
                ? $"اطلاعات تک‌درس دریافت شد. {result.DuplicateRecords} رکورد تکراری بود."
                : "اطلاعات تک‌درس با موفقیت دریافت شد.",

            totalReceived = result.TotalReceived,
            newRecords = result.NewRecords,
            duplicateRecords = result.DuplicateRecords
        });
    }

}

