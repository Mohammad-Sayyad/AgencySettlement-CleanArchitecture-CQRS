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

        public string Month { get; set; } = string.Empty;

        public required List<ExternalExamRecord> Records { get; init; }

        public required Dictionary<int, Agency> Agencies { get; init; }

        public required Dictionary<PriceKey, decimal?> Prices { get; init; }

        public required Dictionary<PercentKey, Percent?> Percents { get; init; }

        public required Dictionary<OrderKey, SettlementOrder> ExistingOrders { get; init; }

        public Dictionary<SettlementKey, Settlement> ExistingSettlements { get; init; } = new();

        public sealed record PriceKey(
            int PackageId,
            int EducationalLevelId,
            int ExamModeId,
            int RegistrationPlanId,
            int YearId);

        public sealed record PercentKey(
            int AgencyId,
            int ExamModeId);

        public readonly record struct OrderKey(
     int AgencyId,
     int YearId,
     int RegistrationOrder,
     string PersianExecutionDate);

        public readonly record struct SettlementKey(
    int AgencyId,
    int YearId,
    string PersianExecutionDate);
    }
}
