using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.DTOs;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.Persistence.Repositories
{
    public sealed class SettlementFactorReportRepository
     : ISettlementFactorReportRepository
    {
        private readonly IConfiguration _configuration;

        public SettlementFactorReportRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IReadOnlyList<SettlementFactorReportDto>> GetAsync(
            int yearId,
            string persianExecutionDate,
            int? agencyId,
            int? examModeId,
            CancellationToken cancellationToken)
        {
            var connectionString = _configuration.GetConnectionString(
                "AgencySettlementDb");

            await using var connection = new SqlConnection(connectionString);


            const string sql = """
            SELECT
                a.DetailCode,
                a.Name AS AgencyName,

                soi.EducationalLevelId,
                el.Name AS EducationalLevelName,

                soi.StudyFieldId,
                sf.Name AS StudyFieldName,

                pack.Name AS PackageName,

                SUM(soi.CandidateCount) AS CandidateCount,

                SUM(CASE
                    WHEN soi.RegistrationPlanId = 1
                    THEN soi.CandidateCount
                    ELSE 0
                END) AS FreeCount,

                SUM(CASE
                    WHEN soi.RegistrationPlanId = 2
                    THEN soi.CandidateCount
                    ELSE 0
                END) AS HekmatCount,

                SUM(CASE
                    WHEN soi.RegistrationPlanId = 8
                    THEN soi.CandidateCount
                    ELSE 0
                END) AS SiteCount,

                SUM(CASE
                    WHEN soi.RegistrationPlanId = 3
                    THEN soi.CandidateCount
                    ELSE 0
                END) AS SchoolScholarshipCount,

                SUM(CASE
                    WHEN soi.RegistrationPlanId = 5
                    THEN soi.CandidateCount
                    ELSE 0
                END) AS FreeVolunteerScholarshipCount,

                SUM(soi.DebitAmount) AS DebitAmount,

                SUM(soi.CreditAmount) AS CreditAmount,

                SUM(soi.DebitAmount) -
                SUM(soi.CreditAmount) AS Balance

            FROM SettlementOrderItems soi

            INNER JOIN Agencies a
                ON a.Id = soi.AgencyId

            INNER JOIN Packages pack
                ON pack.Id = soi.PackageId

            LEFT JOIN EducationalLevels el
                ON el.Id = soi.EducationalLevelId

            INNER JOIN StudyFields sf
                ON sf.Id = soi.StudyFieldId

            WHERE soi.ExamModeId = 0
              AND soi.YearId = @YearId
              AND soi.PersianExecutionDate = @PersianExecutionDate
              AND (@AgencyId IS NULL OR soi.AgencyId = @AgencyId)
              and soi.ExamModeId = @ExamModeId

            GROUP BY
                a.DetailCode,
                a.Name,
                soi.EducationalLevelId,
                el.Name,
                soi.StudyFieldId,
                sf.Name,
                pack.Name

            ORDER BY
                a.DetailCode,
                soi.EducationalLevelId,
                soi.StudyFieldId,
                pack.Name;
            """;

            var command = new CommandDefinition(
                sql,
                new
                {
                    YearId = yearId,
                    PersianExecutionDate = persianExecutionDate,
                    AgencyId = agencyId,
                    ExamModeId = examModeId
                },
                cancellationToken: cancellationToken);

            var result = await connection.QueryAsync<SettlementFactorReportDto>(
                command);

            return result.AsList();
        }
    }
}
