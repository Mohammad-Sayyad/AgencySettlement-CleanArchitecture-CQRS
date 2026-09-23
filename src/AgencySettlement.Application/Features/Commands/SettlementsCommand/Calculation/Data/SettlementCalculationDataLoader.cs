using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.DTOs;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Models;
using AgencySettlement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Data
{
    public sealed class SettlementCalculationDataLoader
    {
        private readonly IExternalExamRecordRepository _externalRepository;
        private readonly IPriceRepository _priceRepository;
        private readonly IPercentRuleRepository _percentRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly ISettlementOrderRepository _settlementOrderRepository;
        private readonly ISettlementRepository _settlementRepository;

        public SettlementCalculationDataLoader(
            IExternalExamRecordRepository externalRepository,
            IPriceRepository priceRepository,
            IPercentRuleRepository percentRepository,
            IAgencyRepository agencyRepository,
            ISettlementOrderRepository settlementOrderRepository,
            ISettlementRepository settlementRepository)
        {
            _externalRepository = externalRepository;
            _priceRepository = priceRepository;
            _percentRepository = percentRepository;
            _agencyRepository = agencyRepository;
            _settlementOrderRepository = settlementOrderRepository;
            _settlementRepository = settlementRepository;
        }

        public async Task<SettlementCalculationContext> LoadAsync(
            CalculateSettlementRequest request,
            CancellationToken cancellationToken)
        {
            ValidateRequest(request);

            var records =
                await _externalRepository.GetByAgencyAndYearAsync(
                    request.AgencyId,
                    request.YearId,
                    request.PersianExecutionDate,
                    request.RegistrationOrder,
                    cancellationToken);

            if (records.Count == 0)
            {
                throw new InvalidOperationException(
                    $"هیچ رکوردی برای نماینده {request.AgencyId}، سال {request.YearId}، تاریخ {request.PersianExecutionDate} و Order {request.RegistrationOrder} پیدا نشد.");
            }

            var orderRecords =
                records
                    .Where(x =>
                        x.RegistrationOrder ==
                        request.RegistrationOrder)
                    .ToList();

            if (orderRecords.Count == 0)
            {
                throw new InvalidOperationException(
                    $"هیچ رکوردی برای RegistrationOrder={request.RegistrationOrder} پیدا نشد.");
            }

            var agency =
                await _agencyRepository.GetByIdAsync(
                    request.AgencyId,
                    cancellationToken);

            if (agency is null)
            {
                throw new InvalidOperationException(
                    "نماینده پیدا نشد.");
            }

            var existingOrder =
                await _settlementOrderRepository
                    .GetByAgencyYearAndOrderAsync(
                        request.AgencyId,
                        request.YearId,
                        request.RegistrationOrder,
                        cancellationToken);

            var order1 =
                request.RegistrationOrder == 1
                    ? existingOrder
                    : await _settlementOrderRepository
                        .GetByAgencyYearAndOrderAsync(
                            request.AgencyId,
                            request.YearId,
                            1,
                            cancellationToken);

            var order2 =
                request.RegistrationOrder == 2
                    ? existingOrder
                    : await _settlementOrderRepository
                        .GetByAgencyYearAndOrderAsync(
                            request.AgencyId,
                            request.YearId,
                            2,
                            cancellationToken);

            var settlement =
                await _settlementRepository.GetByAgencyAndYearAsync(
                    request.AgencyId,
                    request.YearId,
                    cancellationToken);

            var groups =
                await LoadGroupsAsync(
                    orderRecords,
                    cancellationToken);

            var context =
                new SettlementCalculationContext
                {
                    Input =
                        new SettlementCalculationContext.CalculateSettlementInput(
                            request.AgencyId,
                            request.YearId,
                            request.PersianExecutionDate,
                            request.RegistrationOrder),

                    Records = records,

                    OrderRecords = orderRecords,

                    Agency = agency,

                    Groups = groups,

                    ExistingOrder = existingOrder,

                    Order1 = order1,

                    Order2 = order2,

                    Settlement = settlement
                };

            context.QuotaState.Initialize(
                agency.FreeQuotaCount,
                agency.OneHundredThousandQuotaCount);

            return context;
        }

        private async Task<List<SettlementCalculationContext.GroupData>>
            LoadGroupsAsync(
                List<ExternalExamRecord> records,
                CancellationToken cancellationToken)
        {
            var groupedRecords =
                records
                    .GroupBy(x =>
                        new
                        {
                            x.PackageId,
                            x.EducationalLevelId,
                            x.ExamModeId,
                            x.RegistrationPlanId,
                            x.StudyFieldId,
                            x.YearId,
                            x.AgencyId,
                            x.PersianExecutionDate
                        })
                    .ToList();

            var priceCache =
                new Dictionary<PriceKey, decimal?>();

            var percentCache =
                new Dictionary<PercentKey, Percent?>();

            var result =
                new List<SettlementCalculationContext.GroupData>(
                    groupedRecords.Count);

            foreach (var group in groupedRecords)
            {
                var priceKey =
                    new PriceKey(
                        group.Key.PackageId,
                        group.Key.EducationalLevelId,
                        group.Key.ExamModeId,
                        group.Key.RegistrationPlanId,
                        group.Key.YearId);

                if (!priceCache.TryGetValue(
                        priceKey,
                        out var unitPrice))
                {
                    var price =
                        await _priceRepository.GetAsync(
                            group.Key.PackageId,
                            group.Key.EducationalLevelId,
                            group.Key.ExamModeId,
                            group.Key.RegistrationPlanId,
                            group.Key.YearId,
                            cancellationToken);

                    unitPrice =
                        price?.Amount;

                    priceCache[priceKey] =
                        unitPrice;
                }

                Percent? percent = null;

                if (group.Key.RegistrationPlanId is 1 or 8)
                {
                    var percentKey =
                        new PercentKey(
                            group.Key.AgencyId,
                            group.Key.ExamModeId);

                    if (!percentCache.TryGetValue(
                            percentKey,
                            out percent))
                    {
                        percent =
                            await _percentRepository.GetAsync(
                                group.Key.AgencyId,
                                group.Key.ExamModeId,
                                cancellationToken);

                        percentCache[percentKey] =
                            percent;
                    }
                }

                result.Add(
                    new SettlementCalculationContext.GroupData(
                        group.Key.PackageId,
                        group.Key.EducationalLevelId,
                        group.Key.ExamModeId,
                        group.Key.RegistrationPlanId,
                        group.Key.StudyFieldId,
                        group.Key.YearId,
                        group.Key.AgencyId,
                        group.Key.PersianExecutionDate,
                        group.Count(),
                        unitPrice,
                        percent));
            }

            return result;
        }

        private static void ValidateRequest(
            CalculateSettlementRequest request)
        {
            if (request.AgencyId <= 0)
            {
                throw new ArgumentException(
                    "AgencyId نامعتبر است.");
            }

            if (request.YearId <= 0)
            {
                throw new ArgumentException(
                    "YearId نامعتبر است.");
            }

            if (string.IsNullOrWhiteSpace(
                request.PersianExecutionDate))
            {
                throw new ArgumentException(
                    "PersianExecutionDate الزامی است.");
            }

            if (request.RegistrationOrder is not (1 or 2))
            {
                throw new ArgumentException(
                    "RegistrationOrder باید 1 یا 2 باشد.");
            }
        }

        private sealed record PriceKey(
            int PackageId,
            int EducationalLevelId,
            int ExamModeId,
            int RegistrationPlanId,
            int YearId);

        private sealed record PercentKey(
            int AgencyId,
            int ExamModeId);
    }
}
