using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Domain.Entities;
using MediatR;

namespace AgencySettlement.Application.ExternalExamsFeatures.Commands.CreateSettlementPaymentCommand
{
    public sealed class CreateSettlementPaymentCommandHandler
        : IRequestHandler<
            CreateSettlementPaymentCommand,
            CreateSettlementPaymentResponse>
    {
        private readonly ISettlementPaymentRepository _repository;

        public CreateSettlementPaymentCommandHandler(
            ISettlementPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateSettlementPaymentResponse> Handle(
            CreateSettlementPaymentCommand request,
            CancellationToken cancellationToken)
        {
            var settlement =
                await _repository.GetSettlementByAgencyAndDateAsync(
                    request.AgencyId,
                    request.YearId,
                    request.PersianExecutionDate,
                    cancellationToken);

            if (settlement is null)
                throw new KeyNotFoundException(
                    "تسویه مورد نظر پیدا نشد.");

            var payment = new SettlementPayment
            {
                SettlementId = settlement.Id,
                AgencyId = settlement.AgencyId,
                YearId = settlement.YearId,
                PersianExecutionDate = settlement.PersianExecutionDate
            };

            await _repository.AddSettlementPaymentAsync(
                payment,
                cancellationToken);

            var message = payment.Amount > 0 ? "پرداخت با موفقیت دریافت و ثبت شد."
    : "اطلاعات پرداخت دریافت شد، اما مبلغ پرداختی دریافت نشده است.";

            return new CreateSettlementPaymentResponse(
     true,
     message,
     payment.Id,
     payment.SettlementId,
     payment.AgencyId,
     payment.YearId,
     payment.PersianExecutionDate,
     payment.Amount,
     payment.PaymentDate,
     payment.TrackingNumber,
     payment.PaymentReference
 );
        }
    }
}