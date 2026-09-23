using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.Abstractions.Persistence.Common;
using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.DTOs;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Data;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Persistence;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Rules;
using AgencySettlement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AgencySettlement.UnitTests.SettlementCalculation
{
    public sealed class SettlementCalculationCoordinatorTests
    {
        [Fact]
        public async Task CalculateAsync_WhenNoRecordsExist_ShouldThrow()
        {
            var externalRepository =
                new Mock<IExternalExamRecordRepository>();

            externalRepository
                .Setup(x => x.GetByAgencyAndYearAsync(
                    106,
                    10,
                    "05/05/23",
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var loader =
                CreateLoader(
                    externalRepository);

            var persistence =
                CreatePersistence();

            var coordinator =
                CreateCoordinator(
                    loader,
                    persistence);

            var request =
                new CalculateSettlementRequest
                {
                    AgencyId = 106,
                    YearId = 10,
                    PersianExecutionDate = "05/05/23",
                    RegistrationOrder = 1
                };

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => coordinator.CalculateAsync(
                    request,
                    CancellationToken.None));
        }

        [Fact]
        public async Task CalculateAsync_WhenNoValidRuleResultExists_ShouldThrow()
        {
            var records =
                new List<ExternalExamRecord>
                {
                new()
                {
                    CandidateExamId = 1,
                    PackageId = 1,
                    EducationalLevelId = 12,
                    ExamModeId = 0,
                    StudyFieldId = 1,
                    RegistrationPlanId = 999,
                    AgencyId = 106,
                    PersianExecutionDate = "05/05/23",
                    YearId = 10,
                    RegistrationOrder = 1
                }
                };

            var externalRepository =
                new Mock<IExternalExamRecordRepository>();

            externalRepository
                .Setup(x => x.GetByAgencyAndYearAsync(
                    106,
                    10,
                    "05/05/23",
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(records);

            var loader =
                CreateLoader(
                    externalRepository);

            var persistence =
                CreatePersistence();

            var coordinator =
                CreateCoordinator(
                    loader,
                    persistence);

            var request =
                new CalculateSettlementRequest
                {
                    AgencyId = 106,
                    YearId = 10,
                    PersianExecutionDate = "05/05/23",
                    RegistrationOrder = 1
                };

            var exception =
                await Assert.ThrowsAsync<InvalidOperationException>(
                    () => coordinator.CalculateAsync(
                        request,
                        CancellationToken.None));

            Assert.Contains(
                "هیچ ترکیبی دارای قیمت و درصد معتبر",
                exception.Message);
        }

        [Fact]
        public async Task CalculateAsync_ShouldCalculateRegularPlan()
        {
            var records =
                new List<ExternalExamRecord>
                {
                new()
                {
                    CandidateExamId = 1,
                    PackageId = 1,
                    EducationalLevelId = 12,
                    ExamModeId = 0,
                    StudyFieldId = 1,
                    RegistrationPlanId = 1,
                    AgencyId = 106,
                    PersianExecutionDate = "05/05/23",
                    YearId = 10,
                    RegistrationOrder = 1
                },
                new()
                {
                    CandidateExamId = 2,
                    PackageId = 1,
                    EducationalLevelId = 12,
                    ExamModeId = 0,
                    StudyFieldId = 1,
                    RegistrationPlanId = 1,
                    AgencyId = 106,
                    PersianExecutionDate = "05/05/23",
                    YearId = 10,
                    RegistrationOrder = 1
                }
                };

            var externalRepository =
                new Mock<IExternalExamRecordRepository>();

            externalRepository
                .Setup(x => x.GetByAgencyAndYearAsync(
                    106,
                    10,
                    "05/05/23",
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(records);

            var priceRepository =
                new Mock<IPriceRepository>();

            priceRepository
                .Setup(x => x.GetAsync(
                    1,
                    12,
                    0,
                    1,
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new Price
                    {
                        Amount = 5_000_000m,
                        IsActive = true
                    });

            var percentRepository =
                new Mock<IPercentRuleRepository>();

            percentRepository
                .Setup(x => x.GetAsync(
                    106,
                    0,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new Percent
                    {
                        AgencyPercent = 55m,
                        GajPercent = 45m,
                        StudentPercent = 0m,
                        IsActive = true
                    });

            var agencyRepository =
                CreateAgencyRepository();

            var orderRepository =
                CreateOrderRepository();

            var settlementRepository =
                CreateSettlementRepository();

            var loader =
                new SettlementCalculationDataLoader(
                    externalRepository.Object,
                    priceRepository.Object,
                    percentRepository.Object,
                    agencyRepository.Object,
                    orderRepository.Object,
                    settlementRepository.Object);

            var persistence =
                CreatePersistence();

            var coordinator =
                CreateCoordinator(
                    loader,
                    persistence);

            var result =
                await coordinator.CalculateAsync(
                    new CalculateSettlementRequest
                    {
                        AgencyId = 106,
                        YearId = 10,
                        PersianExecutionDate = "05/05/23",
                        RegistrationOrder = 1
                    },
                    CancellationToken.None);

            Assert.NotNull(result);

            Assert.Equal(106, result.AgencyId);
            Assert.Single(result.Items);

            Assert.Equal(2, result.Items[0].CandidateCount);
            Assert.Equal(10_000_000m, result.Items[0].BaseAmount);
        }

        private static SettlementCalculationCoordinator CreateCoordinator(
            SettlementCalculationDataLoader loader,
            SettlementCalculationPersistence persistence)
        {
            var rules =
                new ISettlementCalculationRule[]
                {
                new RegularSettlementRule(),
                new HekmatSettlementRule(),
                new SchoolScholarshipSettlementRule(),
                new FreeVolunteerSettlementRule(),
                new SiteSettlementRule()
                };

            return new SettlementCalculationCoordinator(
                loader,
                persistence,
                rules);
        }

        private static SettlementCalculationDataLoader CreateLoader(
            Mock<IExternalExamRecordRepository> externalRepository)
        {
            var priceRepository =
                new Mock<IPriceRepository>();

            var percentRepository =
                new Mock<IPercentRuleRepository>();

            var agencyRepository =
                CreateAgencyRepository();

            var orderRepository =
                CreateOrderRepository();

            var settlementRepository =
                CreateSettlementRepository();

            return new SettlementCalculationDataLoader(
                externalRepository.Object,
                priceRepository.Object,
                percentRepository.Object,
                agencyRepository.Object,
                orderRepository.Object,
                settlementRepository.Object);
        }

        private static SettlementCalculationPersistence CreatePersistence()
        {
            var settlementRepository =
                CreateSettlementRepository();

            var orderRepository =
                CreateOrderRepository();

            var historyRepository =
                new Mock<ISettlementHistoryRepository>();

            var unitOfWork =
                new Mock<IUnitOfWork>();

            unitOfWork
                .Setup(x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            return new SettlementCalculationPersistence(
                settlementRepository.Object,
                orderRepository.Object,
                historyRepository.Object,
                unitOfWork.Object);
        }

        private static Mock<IAgencyRepository> CreateAgencyRepository()
        {
            var repository =
                new Mock<IAgencyRepository>();

            repository
                .Setup(x => x.GetByIdAsync(
                    106,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new Agency
                    {
                        Id = 106,
                        FreeQuotaCount = 2,
                        OneHundredThousandQuotaCount = 2,
                        ContractFloorAmount = 2_300_000_000m
                    });

            return repository;
        }

        private static Mock<ISettlementOrderRepository>
            CreateOrderRepository()
        {
            var repository =
                new Mock<ISettlementOrderRepository>();

            repository
                .Setup(x => x.GetByAgencyYearAndOrderAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    (SettlementOrder?)null);

            return repository;
        }

        private static Mock<ISettlementRepository>
            CreateSettlementRepository()
        {
            var repository =
                new Mock<ISettlementRepository>();

            repository
                .Setup(x => x.GetByAgencyAndYearAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    (Settlement?)null);

            return repository;
        }
    }
}
