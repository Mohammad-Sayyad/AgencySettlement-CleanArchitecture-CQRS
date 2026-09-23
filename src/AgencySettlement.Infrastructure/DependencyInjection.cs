using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.Abstractions.Persistence.ComboBoxRepository;
using AgencySettlement.Application.Abstractions.Persistence.Common;
using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.Abstractions.Persistence.UserRepository;
using AgencySettlement.Application.ExternalExams.Commands.ImportExternalExams;
using AgencySettlement.Domain.Entities;
using AgencySettlement.Infrastructure.External;
using AgencySettlement.Infrastructure.Persistence;
using AgencySettlement.Infrastructure.Persistence.Authentication;
using AgencySettlement.Infrastructure.Persistence.Common;
using AgencySettlement.Infrastructure.Persistence.Repositories;
using AgencySettlement.Infrastructure.Persistence.Repositories.UserRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AgencySettlement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AgencySettlementDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("AgencySettlementDb"));
        });

        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<ImportExamRecordsCommandValidator>();

        services.AddScoped<
            IExternalExamRecordRepository,
            ExternalExamRecordRepository>();

        services.AddScoped<IPriceRepository, PriceRepository>();

        services.AddScoped<
            IPercentRuleRepository,
            PercentRuleRepository>();

        services.AddScoped<IAgencyRepository, AgencyRepository>();

        services.AddScoped<
            ISettlementRepository,
            SettlementRepository>();


        services.AddScoped<
    ISettlementOrderRepository,
    SettlementOrderRepository>();

        services.AddScoped<
            ISettlementHistoryRepository,
            SettlementHistoryRepository>();

        services.AddScoped<
            ISettlementDebtRepository,
            SettlementDebtRepository>();

        services.AddScoped<
            ISettlementPaymentRepository,
            SettlementPaymentRepository>();

        services.AddScoped<
            ISettlementReportRepository,
            SettlementReportRepository>();

        services.AddScoped<
            ISettlementLookupRepository,
            SettlementLookupRepository>();

        services.AddScoped<
    IExternalSettlementStatusRepository,
    ExternalSettlementStatusRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Configure<ExternalExamApiOptions>(
            configuration.GetSection("ExternalExamApi"));

        services.AddHttpClient<IExternalExamApi, ExternalExamApi>(
            (serviceProvider, client) =>
            {
                var options =
                    serviceProvider
                        .GetRequiredService<IOptions<ExternalExamApiOptions>>()
                        .Value;

                client.BaseAddress = new Uri(options.BaseUrl);

                client.Timeout = TimeSpan.FromSeconds(
                    Math.Max(1, options.TimeoutSeconds));
            });

        return services;
    }
}