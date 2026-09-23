using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Models
{
    public sealed record GroupCalculationResult(
    bool IsValid,
    int FreeCandidateCount,
    int PaidCandidateCount,
    decimal UnitPrice,
    decimal BaseAmount,
    decimal AgencyPercent,
    decimal GajPercent,
    decimal StudentPercent,
    decimal AgencyAmount,
    decimal GajAmount,
    decimal StudentAmount,
    decimal DebitAmount,
    decimal CreditAmount,
    decimal TotalCreditGaj)
    {
        public static GroupCalculationResult Invalid()
        {
            return new GroupCalculationResult(
                false,
                0,
                0,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m);
        }

        public static GroupCalculationResult Free(
            int candidateCount,
            decimal unitPrice)
        {
            return new GroupCalculationResult(
                true,
                candidateCount,
                0,
                unitPrice,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m);
        }

        public static GroupCalculationResult Debit(
            int freeCandidateCount,
            int paidCandidateCount,
            decimal unitPrice,
            decimal baseAmount)
        {
            return new GroupCalculationResult(
                true,
                freeCandidateCount,
                paidCandidateCount,
                unitPrice,
                baseAmount,
                0m,
                0m,
                0m,
                baseAmount,
                baseAmount,
                0m,
                baseAmount,
                0m,
                0m);
        }

        public static GroupCalculationResult Credit(
            int candidateCount,
            decimal unitPrice,
            decimal baseAmount)
        {
            return new GroupCalculationResult(
                true,
                0,
                candidateCount,
                unitPrice,
                baseAmount,
                0m,
                0m,
                0m,
                baseAmount,
                baseAmount,
                0m,
                0m,
                baseAmount,
                0m);
        }

        public static GroupCalculationResult Regular(
            decimal unitPrice,
            decimal baseAmount,
            decimal agencyPercent,
            decimal gajPercent,
            decimal studentPercent,
            decimal agencyAmount,
            decimal gajAmount,
            decimal studentAmount)
        {
            return new GroupCalculationResult(
                true,
                0,
                0,
                unitPrice,
                baseAmount,
                agencyPercent,
                gajPercent,
                studentPercent,
                gajAmount,
                agencyAmount,
                studentAmount,
                agencyAmount,
                0m,
                gajAmount);
        }

        public static GroupCalculationResult Site(
            decimal unitPrice,
            decimal baseAmount,
            decimal agencyPercent,
            decimal gajPercent,
            decimal studentPercent,
            decimal agencyAmount,
            decimal gajAmount,
            decimal studentAmount)
        {
            return new GroupCalculationResult(
                true,
                0,
                0,
                unitPrice,
                baseAmount,
                agencyPercent,
                gajPercent,
                studentPercent,
                gajAmount,
                agencyAmount,
                studentAmount,
                0m,
                agencyAmount,
                0m);
        }
    }
}
