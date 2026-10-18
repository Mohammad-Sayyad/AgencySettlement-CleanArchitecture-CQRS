using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.ExternalExamsFeatures.Commands.CreateSettlementPaymentCommand
{
    public sealed record CreateSettlementPaymentResponse(
    bool Success,
    string Message,
    long PaymentId,
    long SettlementId,
    long? SingleLessonSettlementId,
    int AgencyId,
    int YearId,
    string PersianExecutionDate,
    decimal ExamAmount,
    decimal SingleLessonAmount,
    decimal Amount,
    string PaymentDate,
    string TrackingNumber
);
}
