-- CandidateExamId is the unique business key for imported records.
-- Run this only if the unique index does not already exist.

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_ExternalExamRecords_CandidateExamId'
      AND object_id = OBJECT_ID('dbo.ExternalExamRecords')
)
BEGIN
    CREATE UNIQUE INDEX UX_ExternalExamRecords_CandidateExamId
        ON dbo.ExternalExamRecords(CandidateExamId);
END
