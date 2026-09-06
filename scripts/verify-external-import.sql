/* Imported external rows */
SELECT TOP (100)
    Id,
    CandidateExamId,
    AgencyId,
    YearId,
    PersianExecutionDate,
    PackageId,
    GradeTypeId,
    ExamModeId,
    ExamTypeId,
    StageTypeId,
    MajorId,
    QuotaTypeId,
    ExamPhaseId,
    Amount,
    RetrievedAtUtc,
    SourceSystem
FROM dbo.ExternalExamRecords
ORDER BY Id DESC;

/* Duplicate check - should return zero rows */
SELECT CandidateExamId, COUNT(*) AS DuplicateCount
FROM dbo.ExternalExamRecords
GROUP BY CandidateExamId
HAVING COUNT(*) > 1;
