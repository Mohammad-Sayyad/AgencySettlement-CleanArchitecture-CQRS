using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Data;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Persistence;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Rules;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AgencySettlement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddScoped<SettlementCalculationCoordinator>();
        services.AddScoped<SettlementCalculationDataLoader>();
        services.AddScoped<SettlementCalculationPersistence>();

        services.AddScoped<ISettlementCalculationRule, RegularSettlementRule>();
        services.AddScoped<ISettlementCalculationRule, HekmatSettlementRule>();
        services.AddScoped<ISettlementCalculationRule, SchoolScholarshipSettlementRule>();
        services.AddScoped<ISettlementCalculationRule, FreeVolunteerSettlementRule>();
        services.AddScoped<ISettlementCalculationRule, SiteSettlementRule>();
        return services;
    }
}
