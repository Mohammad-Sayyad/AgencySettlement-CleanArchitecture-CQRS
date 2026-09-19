using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.ExternalExamsFeatures.Commands.CreateSettlementPaymentCommand
{
    public record CreateSettlementPaymentCommand(
      int AgencyId,
      int YearId,
      string PersianExecutionDate
  ) : IRequest<CreateSettlementPaymentResponse>;
}
