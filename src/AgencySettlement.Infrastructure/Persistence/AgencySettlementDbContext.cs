using AgencySettlement.Domain;
using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgencySettlement.Infrastructure.Persistence;

public sealed class AgencySettlementDbContext(DbContextOptions<AgencySettlementDbContext> options) : DbContext(options)
{
    public DbSet<StageType> StageTypes { get; set; }
    public DbSet<EducationalLevel> EducationalLevels { get; set; }
    public DbSet<StudyField> StudyFields { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<RegistrationPlan> RegistrationPlans { get; set; }
    public DbSet<YearType> YearTypes { get; set; }
    public DbSet<ExamPhase> ExamPhases { get; set; }
    public DbSet<ExamMode> ExamModes { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<Agency> Agencies { get; set; }
    public DbSet<Price> Prices { get; set; }
    public DbSet<Percent> Percents { get; set; }
    public DbSet<Settlement> Settlements { get; set; }
    public DbSet<ExternalExamRecord> ExternalExamRecords { get; set; }
    public DbSet<SettlementHistory> SettlementHistories { get; set; }
    public DbSet<ExamBooklet> ExamBooklets { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<ExamDate> ExamDates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgencySettlementDbContext).Assembly);
}
