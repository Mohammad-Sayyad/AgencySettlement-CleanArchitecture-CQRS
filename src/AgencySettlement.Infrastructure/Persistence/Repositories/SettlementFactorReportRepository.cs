using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.DTOs;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AgencySettlement.Infrastructure.Persistence.Repositories;

public sealed class SettlementFactorReportRepository : ISettlementFactorReportRepository
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
        int examModeId,
        CancellationToken cancellationToken)
    {
        var connectionString =
            _configuration.GetConnectionString("AgencySettlementDb");

        await using var connection = new SqlConnection(connectionString);

        var agencyFilter = agencyId.HasValue
            ? "AND soi.AgencyId = @AgencyId"
            : string.Empty;

        var sql = $"""
        WITH ReportStructure AS
        (
            SELECT
                1 AS PackageId,
                N'پیشرفت' AS PackageName,
                el.EducationalLevelId,
                el.LevelName,
                el.EducationalLevelId AS StudyFieldId,
                el.LevelName AS FieldName,
                1 AS SortOrder
            FROM
            (
                VALUES
                    (4, N'چهارم'),
                    (5, N'پنجم'),
                    (6, N'ششم'),
                    (7, N'هفتم'),
                    (8, N'هشتم'),
                    (9, N'نهم')
            ) el(EducationalLevelId, LevelName)

            UNION ALL

            SELECT
                2,
                N'آمادگی برای کنکور',
                el.EducationalLevelId,
                el.LevelName,
                el.EducationalLevelId,
                el.LevelName,
                2
            FROM
            (
                VALUES
                    (6, N'ششم'),
                    (7, N'هفتم'),
                    (8, N'هشتم'),
                    (9, N'نهم')
            ) el(EducationalLevelId, LevelName)

            UNION ALL

            SELECT
                2,
                N'آمادگی برای کنکور',
                10,
                N'دهم',
                sf.Id,
                sf.Name,
                2
            FROM StudyFields sf
            WHERE sf.EducationalLevelId = 10

            UNION ALL

            SELECT
                2,
                N'آمادگی برای کنکور',
                11,
                N'یازدهم',
                sf.Id,
                sf.Name,
                2
            FROM StudyFields sf
            WHERE sf.EducationalLevelId = 11

            UNION ALL

            SELECT
                2,
                N'آمادگی برای کنکور',
                12,
                N'دوازدهم',
                sf.Id,
                sf.Name,
                2
            FROM StudyFields sf
            WHERE sf.EducationalLevelId = 12
        ),

        RealData AS
        (
            SELECT
                soi.AgencyId,
                soi.PackageId,
                soi.EducationalLevelId,
                soi.StudyFieldId,

                SUM(soi.CandidateCount) AS TotalCandidates,

                SUM(CASE
                    WHEN soi.RegistrationPlanId = 1
                    THEN soi.CandidateCount
                    ELSE 0
                END) AS Free,

                SUM(CASE
                    WHEN soi.RegistrationPlanId = 2
                    THEN soi.CandidateCount
                    ELSE 0
                END) AS Hekmat,

                SUM(CASE
                    WHEN soi.RegistrationPlanId = 8
                    THEN soi.CandidateCount
                    ELSE 0
                END) AS SiteReg,

                SUM(CASE
                    WHEN soi.RegistrationPlanId = 3
                    THEN soi.CandidateCount
                    ELSE 0
                END) AS SchoolBursary,

                SUM(CASE
                    WHEN soi.RegistrationPlanId = 5
                    THEN soi.CandidateCount
                    ELSE 0
                END) AS FreeBursary,

                SUM(soi.DebitAmount) AS Debit,
                SUM(soi.CreditAmount) AS Credit,
                SUM(soi.DebitAmount) - SUM(soi.CreditAmount) AS Balance

            FROM SettlementOrderItems soi

            WHERE soi.ExamModeId = @ExamModeId
              AND soi.YearId = @YearId
              AND soi.PersianExecutionDate = @PersianExecutionDate
              {agencyFilter}

            GROUP BY
                soi.AgencyId,
                soi.PackageId,
                soi.EducationalLevelId,
                soi.StudyFieldId
        ),

        AgenciesInData AS
        (
            SELECT DISTINCT AgencyId
            FROM RealData
        ),

        Joined AS
        (
            SELECT
                a.Id AS AgencyId,
                a.DetailCode,
                a.Name AS AgencyName,

                ap.AgencyPercent,

                rs.EducationalLevelId,
                rs.LevelName,

                rs.StudyFieldId,
                rs.FieldName,

                rs.PackageName,
                rs.PackageId,
                rs.SortOrder,

                ISNULL(rd.TotalCandidates, 0) AS TotalCandidates,
                ISNULL(rd.Free, 0) AS Free,
                ISNULL(rd.Hekmat, 0) AS Hekmat,
                ISNULL(rd.SiteReg, 0) AS SiteReg,
                ISNULL(rd.SchoolBursary, 0) AS SchoolBursary,
                ISNULL(rd.FreeBursary, 0) AS FreeBursary,

                ISNULL(rd.Debit, 0) AS Debit,
                ISNULL(rd.Credit, 0) AS Credit,
                ISNULL(rd.Balance, 0) AS Balance,

                a.FreeQuotaCount,

                SUM(ISNULL(rd.FreeBursary, 0)) OVER
                (
                    PARTITION BY a.Id
                    ORDER BY
                        rs.SortOrder,
                        rs.EducationalLevelId,
                        rs.StudyFieldId
                    ROWS UNBOUNDED PRECEDING
                ) AS RunningFreeBursary

            FROM ReportStructure rs

            CROSS JOIN AgenciesInData aid

            INNER JOIN Agencies a
                ON a.Id = aid.AgencyId

            INNER JOIN Percents ap
                ON ap.AgencyId = a.Id
               AND ap.ExamModeId = @ExamModeId

            LEFT JOIN RealData rd
                ON rd.AgencyId = a.Id
               AND rd.PackageId = rs.PackageId
               AND rd.EducationalLevelId = rs.EducationalLevelId
               AND rd.StudyFieldId = rs.StudyFieldId
        )

        SELECT
            AgencyId,

            DetailCode,
            AgencyName,
            AgencyPercent,

            EducationalLevelId,
            LevelName AS EducationalLevelName,

            StudyFieldId,
            FieldName AS StudyFieldName,

            PackageName,

            TotalCandidates AS CandidateCount,

            Free AS FreeCount,
            Hekmat AS HekmatCount,
            SiteReg AS SiteCount,
            SchoolBursary AS SchoolScholarshipCount,
            FreeBursary AS FreeVolunteerScholarshipCount,

            Debit AS DebitAmount,
            Credit AS CreditAmount,
            Balance,

            CASE
                WHEN FreeBursary = 0
                    THEN 0

                WHEN RunningFreeBursary - FreeBursary >= FreeQuotaCount
                    THEN 0

                WHEN RunningFreeBursary <= FreeQuotaCount
                    THEN FreeBursary

                ELSE FreeQuotaCount -
                     (RunningFreeBursary - FreeBursary)
            END AS FreeQuota

        FROM Joined

        ORDER BY
            AgencyId,
            SortOrder,
            EducationalLevelId,
            StudyFieldId;
        """;

        var parameters = new
        {
            YearId = yearId,
            PersianExecutionDate = persianExecutionDate,
            ExamModeId = examModeId,
            AgencyId = agencyId
        };

        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        var result =
            await connection.QueryAsync<SettlementFactorReportDto>(command);

        return result.AsList();
    }
}