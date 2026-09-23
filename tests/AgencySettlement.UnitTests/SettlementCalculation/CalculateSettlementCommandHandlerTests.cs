using AgencySettlement.Application.Abstractions.Persistence.Common;
using AgencySettlement.Application.DTOs;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AgencySettlement.UnitTests.SettlementCalculation
{
    public sealed class CalculateSettlementCommandHandlerTests
    {
        [Fact]
        public async Task Handle_WhenCalculationSucceeds_ShouldCommitTransaction()
        {
            var coordinator = new Mock<ISettlementCalculationCoordinator>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var request = CreateRequest();

            var expected = new SettlementResultDto
            {
                SettlementId = 100,
                AgencyId = 106,
                TotalDebit = 5_000_000m,
                TotalCredit = 5_000_000m,
                Balance = 0m
            };

            coordinator
                .Setup(x => x.CalculateAsync(
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var handler = new CalculateSettlementCommandHandler(
                coordinator.Object,
                unitOfWork.Object);

            var result = await handler.Handle(
                new CalculateSettlementCommand(request),
                CancellationToken.None);

            Assert.Same(expected, result);

            unitOfWork.Verify(
                x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            unitOfWork.Verify(
                x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            unitOfWork.Verify(
                x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Never);

            coordinator.Verify(
                x => x.CalculateAsync(
                    request,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCalculationFails_ShouldRollbackTransaction()
        {
            var coordinator = new Mock<ISettlementCalculationCoordinator>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var request = CreateRequest();

            var expectedException =
                new InvalidOperationException("Calculation failed.");

            coordinator
                .Setup(x => x.CalculateAsync(
                    request,
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            var handler = new CalculateSettlementCommandHandler(
                coordinator.Object,
                unitOfWork.Object);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.Handle(
                    new CalculateSettlementCommand(request),
                    CancellationToken.None));

            Assert.Same(expectedException, exception);

            unitOfWork.Verify(
                x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            unitOfWork.Verify(
                x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            unitOfWork.Verify(
                x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhenCalculationSucceeds_ShouldReturnCoordinatorResult()
        {
            var coordinator = new Mock<ISettlementCalculationCoordinator>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var request = CreateRequest();

            var expected = new SettlementResultDto
            {
                SettlementId = 120,
                AgencyId = 106,
                TotalDebit = 15_900_000m,
                TotalCredit = 6_440_000m,
                Balance = 9_460_000m
            };

            coordinator
                .Setup(x => x.CalculateAsync(
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var handler = new CalculateSettlementCommandHandler(
                coordinator.Object,
                unitOfWork.Object);

            var result = await handler.Handle(
                new CalculateSettlementCommand(request),
                CancellationToken.None);

            Assert.Equal(expected.SettlementId, result.SettlementId);
            Assert.Equal(expected.AgencyId, result.AgencyId);
            Assert.Equal(expected.TotalDebit, result.TotalDebit);
            Assert.Equal(expected.TotalCredit, result.TotalCredit);
            Assert.Equal(expected.Balance, result.Balance);
        }

        private static CalculateSettlementRequest CreateRequest()
        {
            return new CalculateSettlementRequest
            {
                AgencyId = 106,
                YearId = 10,
                PersianExecutionDate = "05/05/23",
                RegistrationOrder = 1
            };
        }
    }
}
