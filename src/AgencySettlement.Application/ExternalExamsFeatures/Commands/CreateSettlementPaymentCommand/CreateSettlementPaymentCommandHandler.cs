using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.ExternalExamsFeatures.Commands.CreateSettlementPaymentCommand;
using AgencySettlement.Domain.Entities;
using MediatR;

namespace AgencySettlement.Application.Features.SettlementPayments.Commands.CreateSettlementPayment;

public sealed class CreateSettlementPaymentCommandHandler
    : IRequestHandler<CreateSettlementPaymentCommand, CreateSettlementPaymentResponse>
{
    private readonly ISettlementRepository _settlementRepository;
    private readonly ISettlementPaymentRepository _paymentRepository;

    public CreateSettlementPaymentCommandHandler(
        ISettlementRepository settlementRepository,
        ISettlementPaymentRepository paymentRepository)
    {
        _settlementRepository = settlementRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<CreateSettlementPaymentResponse> Handle(
        CreateSettlementPaymentCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ExamAmount < 0 || request.SingleLessonAmount < 0)
            throw new ArgumentException("مبلغ پرداخت نمی‌تواند منفی باشد.");

        var amount = request.ExamAmount + request.SingleLessonAmount;

        if (amount <= 0)
            throw new ArgumentException("مبلغ پرداخت باید بیشتر از صفر باشد.");

        if (request.AgencyId <= 0 || request.YearId <= 0)
            throw new ArgumentException("شناسه نمایندگی یا سال معتبر نیست.");

        if (string.IsNullOrWhiteSpace(request.PersianExecutionDate))
            throw new ArgumentException("تاریخ اجرای آزمون الزامی است.");

        if (string.IsNullOrWhiteSpace(request.PaymentDate))
            throw new ArgumentException("تاریخ پرداخت الزامی است.");

        if (string.IsNullOrWhiteSpace(request.TrackingNumber))
            throw new ArgumentException("شماره پیگیری الزامی است.");

        var settlement = await _paymentRepository.GetSettlementByIdAsync(
     request.SettlementId,
     cancellationToken);

        if (settlement is null)
            throw new KeyNotFoundException("تسویه آزمون پیدا نشد.");

        if (settlement.AgencyId != request.AgencyId ||
            settlement.YearId != request.YearId ||
            settlement.PersianExecutionDate != request.PersianExecutionDate)
        {
            throw new ArgumentException(
                "اطلاعات نمایندگی، سال یا تاریخ با تسویه آزمون مطابقت ندارد.");
        }

        var payment = new SettlementPayment
        {
            SettlementId = settlement.Id,
            AgencyId = request.AgencyId,
            YearId = request.YearId,
            PersianExecutionDate = request.PersianExecutionDate,
            ExamAmount = request.ExamAmount,
            SingleLessonSettlementId = request.SingleLessonSettlementId,
            SingleLessonAmount = request.SingleLessonAmount,
            Amount = amount,
            PaymentDate = request.PaymentDate,
            TrackingNumber = request.TrackingNumber,
            IsAppliedToSettlement = false
        };

        // Repository پرداخت را ثبت می‌کند و هر دو تسویه را در یک تراکنش آپدیت می‌کند.
        var paymentId = await _paymentRepository.AddSettlementPaymentAsync(
            payment,
            cancellationToken);

        var updatedSingleLessonSettlementId = payment.SingleLessonSettlementId;

        return new CreateSettlementPaymentResponse(
            Success: true,
            Message: "پرداخت با موفقیت ثبت شد.",
            PaymentId: paymentId,
            SettlementId: settlement.Id,
            SingleLessonSettlementId: updatedSingleLessonSettlementId,
            AgencyId: request.AgencyId,
            YearId: request.YearId,
            PersianExecutionDate: request.PersianExecutionDate,
            ExamAmount: request.ExamAmount,
            SingleLessonAmount: request.SingleLessonAmount,
            Amount: amount,
            PaymentDate: request.PaymentDate,
            TrackingNumber: request.TrackingNumber
        );
    }
}