USE AgencySettlementDb;
GO

-- Sample only. Replace IDs with the real 210 agency and lookup IDs.
DECLARE @AgencyId BIGINT = 37;
DECLARE @RuleKey UNIQUEIDENTIFIER = NEWID();

-- A valid percentage rule always consists of one Agency row + one Gaj row
-- sharing the same RuleKey, validity window and Priority.
INSERT INTO dbo.Percents
    (AgencyId, RuleKey, PercentType, Value, EffectiveFrom, EffectiveTo, Priority, IsActive)
VALUES
    (@AgencyId, @RuleKey, 1, 25.00, '2026-01-01T00:00:00', NULL, 10, 1),
    (@AgencyId, @RuleKey, 2, 75.00, '2026-01-01T00:00:00', NULL, 10, 1);

-- Example price. Adjust all IDs and amount for the real exam rule.
INSERT INTO dbo.Prices
    (StageTypeId, GradeTypeId, MajorId, ExamTypeId, QuotaTypeId, YearTypeId,
     ExamPhaseId, ExamModeId, Amount, EffectiveFrom, EffectiveTo, Priority, IsActive)
VALUES
    (1, 3, 5, 2, 1, 5, 1, 1, 5500000.00, '2026-01-01T00:00:00', NULL, 10, 1);
GO
