using AgencySettlement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsMonthlyInsertCommand.Models
{
    public sealed class MonthlySettlementCalculationContext
    {
        public required int YearId { get; init; }

        public required int Month { get; init; }

        public required List<ExternalExamRecord> Records { get; init; }

        public required Dictionary<int, Agency> Agencies { get; init; }

        public required Dictionary<PriceKey, decimal?> Prices { get; init; }

        public required Dictionary<PercentKey, Percent?> Percents { get; init; }

        public required Dictionary<OrderKey, SettlementOrder> ExistingOrders { get; init; }

        public required Dictionary<int, Settlement> ExistingSettlements { get; init; }

        public sealed record PriceKey(
            int PackageId,
            int EducationalLevelId,
            int ExamModeId,
            int RegistrationPlanId,
            int YearId);

        public sealed record PercentKey(
            int AgencyId,
            int ExamModeId);

        public sealed record OrderKey(
            int AgencyId,
            int YearId,
            int RegistrationOrder);
    }
}
