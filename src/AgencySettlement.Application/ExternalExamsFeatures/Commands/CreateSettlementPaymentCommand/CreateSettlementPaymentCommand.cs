using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.ExternalExamsFeatures.Commands.CreateSettlementPaymentCommand
{
    public sealed record CreateSettlementPaymentCommand(
    long SettlementId,
    int AgencyId,
    int YearId,
    string PersianExecutionDate,
    decimal Amount,
    string PaymentDate,
    string TrackingNumber
) : IRequest<CreateSettlementPaymentResponse>;
}
