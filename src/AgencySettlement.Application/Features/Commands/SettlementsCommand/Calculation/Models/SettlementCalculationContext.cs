using AgencySettlement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Models
{
    public sealed class SettlementCalculationContext
    {
        public required CalculateSettlementInput Input { get; init; }

        public required List<ExternalExamRecord> Records { get; init; }

        public required List<ExternalExamRecord> OrderRecords { get; init; }

        public required Agency Agency { get; init; }

        public required List<GroupData> Groups { get; init; }

        public SettlementOrder? ExistingOrder { get; init; }

        public SettlementOrder? Order1 { get; set; }

        public SettlementOrder? Order2 { get; set; }

        public Settlement? Settlement { get; set; }

        public Plan5QuotaState QuotaState { get; } = new();

        public sealed record CalculateSettlementInput(
            int AgencyId,
            int YearId,
            string PersianExecutionDate,
            int RegistrationOrder);

        public sealed class Plan5QuotaState
        {
            public int RemainingFreeQuota { get; set; }

            public int RemainingOneHundredThousandQuota { get; set; }

            public void Initialize(
                int freeQuotaCount,
                int oneHundredThousandQuotaCount)
            {
                RemainingFreeQuota =
                    Math.Max(0, freeQuotaCount);

                RemainingOneHundredThousandQuota =
                    Math.Max(0, oneHundredThousandQuotaCount);
            }
        }

        public sealed record GroupData(
            int PackageId,
            int EducationalLevelId,
            int ExamModeId,
            int RegistrationPlanId,
            int StudyFieldId,
            int YearId,
            int AgencyId,
            string PersianExecutionDate,
            int CandidateCount,
            decimal? UnitPrice,
            Percent? Percent);
    }
}
