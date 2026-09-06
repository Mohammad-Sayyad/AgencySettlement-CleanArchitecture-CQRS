IF DB_ID(N'AgencySettlementDb') IS NULL
BEGIN
    CREATE DATABASE AgencySettlementDb;
END;
GO

USE AgencySettlementDb;
GO

CREATE TABLE dbo.States (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_States PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_States_IsActive DEFAULT(1),
    CONSTRAINT UX_States_Name UNIQUE(Name)
);

CREATE TABLE dbo.Regions (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Regions PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    StateId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Regions_IsActive DEFAULT(1),
    CONSTRAINT FK_Regions_States FOREIGN KEY(StateId) REFERENCES dbo.States(Id),
    CONSTRAINT UX_Regions_State_Name UNIQUE(StateId, Name)
);

CREATE TABLE dbo.Agencies (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Agencies PRIMARY KEY,
    Code NVARCHAR(50) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    StateId BIGINT NULL,
    RegionId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Agencies_IsActive DEFAULT(1),
    CONSTRAINT UX_Agencies_Code UNIQUE(Code),
    CONSTRAINT FK_Agencies_States FOREIGN KEY(StateId) REFERENCES dbo.States(Id),
    CONSTRAINT FK_Agencies_Regions FOREIGN KEY(RegionId) REFERENCES dbo.Regions(Id)
);

CREATE TABLE dbo.StageTypes (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_StageTypes PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_StageTypes_IsActive DEFAULT(1),
    CONSTRAINT UX_StageTypes_Name UNIQUE(Name)
);

CREATE TABLE dbo.GradeTypes (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_GradeTypes PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    StageTypeId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_GradeTypes_IsActive DEFAULT(1),
    CONSTRAINT FK_GradeTypes_StageTypes FOREIGN KEY(StageTypeId) REFERENCES dbo.StageTypes(Id),
    CONSTRAINT UX_GradeTypes_Stage_Name UNIQUE(StageTypeId, Name)
);

CREATE TABLE dbo.Majors (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Majors PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Majors_IsActive DEFAULT(1),
    CONSTRAINT UX_Majors_Name UNIQUE(Name)
);

CREATE TABLE dbo.ExamTypes (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExamTypes PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_ExamTypes_IsActive DEFAULT(1),
    CONSTRAINT UX_ExamTypes_Name UNIQUE(Name)
);

CREATE TABLE dbo.QuotaTypes (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_QuotaTypes PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_QuotaTypes_IsActive DEFAULT(1),
    CONSTRAINT UX_QuotaTypes_Name UNIQUE(Name)
);

CREATE TABLE dbo.YearTypes (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_YearTypes PRIMARY KEY,
    Year INT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_YearTypes_IsActive DEFAULT(1),
    CONSTRAINT UX_YearTypes_Year UNIQUE(Year),
    CONSTRAINT CK_YearTypes_Year CHECK(Year BETWEEN 1300 AND 2200)
);

CREATE TABLE dbo.ScheduledDates (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ScheduledDates PRIMARY KEY,
    DateUtc DATETIME2 NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_ScheduledDates_IsActive DEFAULT(1)
);
CREATE INDEX IX_ScheduledDates_DateUtc ON dbo.ScheduledDates(DateUtc);

CREATE TABLE dbo.ExamPhases (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExamPhases PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_ExamPhases_IsActive DEFAULT(1),
    CONSTRAINT UX_ExamPhases_Name UNIQUE(Name)
);

CREATE TABLE dbo.ExamModes (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExamModes PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_ExamModes_IsActive DEFAULT(1),
    CONSTRAINT UX_ExamModes_Name UNIQUE(Name)
);

CREATE TABLE dbo.Prices (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Prices PRIMARY KEY,
    StageTypeId BIGINT NOT NULL,
    GradeTypeId BIGINT NOT NULL,
    MajorId BIGINT NOT NULL,
    ExamTypeId BIGINT NOT NULL,
    QuotaTypeId BIGINT NOT NULL,
    YearTypeId BIGINT NOT NULL,
    ExamPhaseId BIGINT NOT NULL,
    ExamModeId BIGINT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    EffectiveFrom DATETIME2 NOT NULL,
    EffectiveTo DATETIME2 NULL,
    Priority INT NOT NULL CONSTRAINT DF_Prices_Priority DEFAULT(0),
    IsActive BIT NOT NULL CONSTRAINT DF_Prices_IsActive DEFAULT(1),
    CONSTRAINT CK_Prices_Amount CHECK(Amount >= 0),
    CONSTRAINT CK_Prices_EffectiveRange CHECK(EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),
    CONSTRAINT CK_Prices_Priority CHECK(Priority >= 0),
    CONSTRAINT FK_Prices_StageTypes FOREIGN KEY(StageTypeId) REFERENCES dbo.StageTypes(Id),
    CONSTRAINT FK_Prices_GradeTypes FOREIGN KEY(GradeTypeId) REFERENCES dbo.GradeTypes(Id),
    CONSTRAINT FK_Prices_Majors FOREIGN KEY(MajorId) REFERENCES dbo.Majors(Id),
    CONSTRAINT FK_Prices_ExamTypes FOREIGN KEY(ExamTypeId) REFERENCES dbo.ExamTypes(Id),
    CONSTRAINT FK_Prices_QuotaTypes FOREIGN KEY(QuotaTypeId) REFERENCES dbo.QuotaTypes(Id),
    CONSTRAINT FK_Prices_YearTypes FOREIGN KEY(YearTypeId) REFERENCES dbo.YearTypes(Id),
    CONSTRAINT FK_Prices_ExamPhases FOREIGN KEY(ExamPhaseId) REFERENCES dbo.ExamPhases(Id),
    CONSTRAINT FK_Prices_ExamModes FOREIGN KEY(ExamModeId) REFERENCES dbo.ExamModes(Id)
);
CREATE INDEX IX_Prices_ActiveLookup ON dbo.Prices(StageTypeId, GradeTypeId, MajorId, ExamTypeId, QuotaTypeId, YearTypeId, ExamPhaseId, ExamModeId, IsActive, EffectiveFrom, EffectiveTo, Priority);

CREATE TABLE dbo.Percents (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Percents PRIMARY KEY,
    AgencyId BIGINT NOT NULL,
    RuleKey UNIQUEIDENTIFIER NOT NULL,
    PercentType TINYINT NOT NULL,
    Value DECIMAL(5,2) NOT NULL,
    EffectiveFrom DATETIME2 NOT NULL,
    EffectiveTo DATETIME2 NULL,
    Priority INT NOT NULL CONSTRAINT DF_Percents_Priority DEFAULT(0),
    IsActive BIT NOT NULL CONSTRAINT DF_Percents_IsActive DEFAULT(1),
    CONSTRAINT CK_Percents_Value CHECK(Value BETWEEN 0 AND 100),
    CONSTRAINT CK_Percents_EffectiveRange CHECK(EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),
    CONSTRAINT CK_Percents_Priority CHECK(Priority >= 0),
    CONSTRAINT CK_Percents_Type CHECK(PercentType IN (1,2)),
    CONSTRAINT FK_Percents_Agencies FOREIGN KEY(AgencyId) REFERENCES dbo.Agencies(Id)
);
CREATE UNIQUE INDEX UX_Percents_RuleRow ON dbo.Percents(AgencyId, RuleKey, PercentType);
CREATE INDEX IX_Percents_ActiveLookup ON dbo.Percents(AgencyId, PercentType, IsActive, EffectiveFrom, EffectiveTo, Priority);

CREATE TABLE dbo.Settlements (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Settlements PRIMARY KEY,
    AgencyId BIGINT NOT NULL,
    AgencyCodeSnapshot NVARCHAR(50) NOT NULL,
    AgencyNameSnapshot NVARCHAR(200) NOT NULL,
    PriceId BIGINT NOT NULL,
    PriceAmountSnapshot DECIMAL(18,2) NOT NULL,
    PriceEffectiveFrom DATETIME2 NOT NULL,
    PriceEffectiveTo DATETIME2 NULL,
    PricePriority INT NOT NULL,
    AgencyPercentId BIGINT NOT NULL,
    PercentRuleKey UNIQUEIDENTIFIER NOT NULL,
    AgencyPercent DECIMAL(5,2) NOT NULL,
    AgencyPercentEffectiveFrom DATETIME2 NOT NULL,
    AgencyPercentEffectiveTo DATETIME2 NULL,
    AgencyPercentPriority INT NOT NULL,
    GajPercentId BIGINT NOT NULL,
    GajPercent DECIMAL(5,2) NOT NULL,
    GajPercentEffectiveFrom DATETIME2 NOT NULL,
    GajPercentEffectiveTo DATETIME2 NULL,
    GajPercentPriority INT NOT NULL,
    GrossAmount DECIMAL(18,2) NOT NULL,
    AgencyAmount DECIMAL(18,2) NOT NULL,
    GajAmount DECIMAL(18,2) NOT NULL,
    IdempotencyKey NVARCHAR(100) NOT NULL,
    RequestHash CHAR(64) NOT NULL,
    CalculatedAtUtc DATETIME2 NOT NULL,
    CONSTRAINT CK_Settlements_PercentRange CHECK(AgencyPercent BETWEEN 0 AND 100 AND GajPercent BETWEEN 0 AND 100),
    CONSTRAINT CK_Settlements_PercentSplit CHECK(AgencyPercent + GajPercent = 100),
    CONSTRAINT CK_Settlements_AmountSplit CHECK(AgencyAmount + GajAmount = GrossAmount),
    CONSTRAINT CK_Settlements_Amounts CHECK(PriceAmountSnapshot >= 0 AND GrossAmount >= 0 AND AgencyAmount >= 0 AND GajAmount >= 0),
    CONSTRAINT FK_Settlements_Agencies FOREIGN KEY(AgencyId) REFERENCES dbo.Agencies(Id),
    CONSTRAINT FK_Settlements_Prices FOREIGN KEY(PriceId) REFERENCES dbo.Prices(Id),
    CONSTRAINT FK_Settlements_AgencyPercent FOREIGN KEY(AgencyPercentId) REFERENCES dbo.Percents(Id),
    CONSTRAINT FK_Settlements_GajPercent FOREIGN KEY(GajPercentId) REFERENCES dbo.Percents(Id),
    CONSTRAINT UX_Settlements_IdempotencyKey UNIQUE(IdempotencyKey)
);
CREATE INDEX IX_Settlements_AgencyCalculatedAt ON dbo.Settlements(AgencyId, CalculatedAtUtc);
CREATE INDEX IX_Settlements_PriceCalculatedAt ON dbo.Settlements(PriceId, CalculatedAtUtc);

CREATE TABLE dbo.SettlementInputs (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SettlementInputs PRIMARY KEY,
    SettlementId BIGINT NOT NULL,
    AgencyId BIGINT NOT NULL,
    StageTypeId BIGINT NOT NULL,
    GradeTypeId BIGINT NOT NULL,
    MajorId BIGINT NOT NULL,
    ExamTypeId BIGINT NOT NULL,
    QuotaTypeId BIGINT NOT NULL,
    YearTypeId BIGINT NOT NULL,
    ScheduledDateId BIGINT NOT NULL,
    ExamPhaseId BIGINT NOT NULL,
    ExamModeId BIGINT NOT NULL,
    CandidateExamIdsJson NVARCHAR(MAX) NOT NULL,
    ScheduledDateUtcSnapshot DATETIME2 NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    CONSTRAINT UX_SettlementInputs_Settlement UNIQUE(SettlementId),
    CONSTRAINT FK_SettlementInputs_Settlements FOREIGN KEY(SettlementId) REFERENCES dbo.Settlements(Id) ON DELETE CASCADE,
    CONSTRAINT FK_SettlementInputs_Agencies FOREIGN KEY(AgencyId) REFERENCES dbo.Agencies(Id),
    CONSTRAINT FK_SettlementInputs_StageTypes FOREIGN KEY(StageTypeId) REFERENCES dbo.StageTypes(Id),
    CONSTRAINT FK_SettlementInputs_GradeTypes FOREIGN KEY(GradeTypeId) REFERENCES dbo.GradeTypes(Id),
    CONSTRAINT FK_SettlementInputs_Majors FOREIGN KEY(MajorId) REFERENCES dbo.Majors(Id),
    CONSTRAINT FK_SettlementInputs_ExamTypes FOREIGN KEY(ExamTypeId) REFERENCES dbo.ExamTypes(Id),
    CONSTRAINT FK_SettlementInputs_QuotaTypes FOREIGN KEY(QuotaTypeId) REFERENCES dbo.QuotaTypes(Id),
    CONSTRAINT FK_SettlementInputs_YearTypes FOREIGN KEY(YearTypeId) REFERENCES dbo.YearTypes(Id),
    CONSTRAINT FK_SettlementInputs_ScheduledDates FOREIGN KEY(ScheduledDateId) REFERENCES dbo.ScheduledDates(Id),
    CONSTRAINT FK_SettlementInputs_ExamPhases FOREIGN KEY(ExamPhaseId) REFERENCES dbo.ExamPhases(Id),
    CONSTRAINT FK_SettlementInputs_ExamModes FOREIGN KEY(ExamModeId) REFERENCES dbo.ExamModes(Id)
);
CREATE INDEX IX_SettlementInputs_AgencyId ON dbo.SettlementInputs(AgencyId);

CREATE TABLE dbo.ExternalExamAmounts (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExternalExamAmounts PRIMARY KEY,
    SettlementId BIGINT NOT NULL,
    CandidateExamId BIGINT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    RetrievedAtUtc DATETIME2 NOT NULL,
    SourceSystem NVARCHAR(100) NOT NULL,
    CONSTRAINT CK_ExternalExamAmounts_Amount CHECK(Amount >= 0),
    CONSTRAINT FK_ExternalExamAmounts_Settlements FOREIGN KEY(SettlementId) REFERENCES dbo.Settlements(Id) ON DELETE CASCADE,
    CONSTRAINT UX_ExternalExamAmounts_SettlementCandidate UNIQUE(SettlementId, CandidateExamId)
);
CREATE INDEX IX_ExternalExamAmounts_CandidateExamId ON dbo.ExternalExamAmounts(CandidateExamId);
CREATE INDEX IX_ExternalExamAmounts_RetrievedAtUtc ON dbo.ExternalExamAmounts(RetrievedAtUtc);

CREATE TABLE dbo.SettlementItems (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SettlementItems PRIMARY KEY,
    SettlementId BIGINT NOT NULL,
    CandidateExamId BIGINT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    ExternalExamAmountId BIGINT NOT NULL,
    CONSTRAINT CK_SettlementItems_Amount CHECK(Amount >= 0),
    CONSTRAINT FK_SettlementItems_Settlements FOREIGN KEY(SettlementId) REFERENCES dbo.Settlements(Id) ON DELETE CASCADE,
    CONSTRAINT FK_SettlementItems_ExternalExamAmount FOREIGN KEY(ExternalExamAmountId) REFERENCES dbo.ExternalExamAmounts(Id),
    CONSTRAINT UX_SettlementItems_SettlementCandidate UNIQUE(SettlementId, CandidateExamId)
);
CREATE INDEX IX_SettlementItems_CandidateExamId ON dbo.SettlementItems(CandidateExamId);

CREATE TABLE dbo.SettlementHistories (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SettlementHistories PRIMARY KEY,
    SettlementId BIGINT NOT NULL,
    AgencyId BIGINT NOT NULL,
    AgencyCodeSnapshot NVARCHAR(50) NOT NULL,
    AgencyNameSnapshot NVARCHAR(200) NOT NULL,
    IdempotencyKey NVARCHAR(100) NOT NULL,
    RequestHash CHAR(64) NOT NULL,
    CalculatedAtUtc DATETIME2 NOT NULL,
    PriceId BIGINT NOT NULL,
    PriceAmount DECIMAL(18,2) NOT NULL,
    PriceEffectiveFrom DATETIME2 NOT NULL,
    PriceEffectiveTo DATETIME2 NULL,
    PricePriority INT NOT NULL,
    AgencyPercentId BIGINT NOT NULL,
    PercentRuleKey UNIQUEIDENTIFIER NOT NULL,
    AgencyPercent DECIMAL(5,2) NOT NULL,
    AgencyPercentEffectiveFrom DATETIME2 NOT NULL,
    AgencyPercentEffectiveTo DATETIME2 NULL,
    AgencyPercentPriority INT NOT NULL,
    GajPercentId BIGINT NOT NULL,
    GajPercent DECIMAL(5,2) NOT NULL,
    GajPercentEffectiveFrom DATETIME2 NOT NULL,
    GajPercentEffectiveTo DATETIME2 NULL,
    GajPercentPriority INT NOT NULL,
    GrossAmount DECIMAL(18,2) NOT NULL,
    AgencyAmount DECIMAL(18,2) NOT NULL,
    GajAmount DECIMAL(18,2) NOT NULL,
    InputSnapshotJson NVARCHAR(MAX) NOT NULL,
    CandidateExamsSnapshotJson NVARCHAR(MAX) NOT NULL,
    CONSTRAINT UX_SettlementHistories_Settlement UNIQUE(SettlementId),
    CONSTRAINT FK_SettlementHistories_Settlements FOREIGN KEY(SettlementId) REFERENCES dbo.Settlements(Id),
    CONSTRAINT CK_SettlementHistories_PercentSplit CHECK(AgencyPercent + GajPercent = 100),
    CONSTRAINT CK_SettlementHistories_AmountSplit CHECK(AgencyAmount + GajAmount = GrossAmount)
);
CREATE INDEX IX_SettlementHistories_AgencyCalculatedAt ON dbo.SettlementHistories(AgencyId, CalculatedAtUtc);
CREATE INDEX IX_SettlementHistories_PriceCalculatedAt ON dbo.SettlementHistories(PriceId, CalculatedAtUtc);
GO

CREATE TRIGGER dbo.TR_SettlementHistories_BlockMutation
ON dbo.SettlementHistories
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    THROW 51001, 'SettlementHistories is immutable. UPDATE and DELETE are not allowed.', 1;
END;
GO

/* Imported external exam source data - independent from Settlements */
CREATE TABLE dbo.ExternalExamRecords (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExternalExamRecords PRIMARY KEY,
    CandidateExamId BIGINT NOT NULL,
    AgencyId INT NOT NULL,
    YearId INT NOT NULL,
    PersianExecutionDate DATETIME2 NOT NULL,
    PackageId INT NOT NULL,
    GradeTypeId INT NOT NULL,
    ExamModeId INT NOT NULL,
    ExamTypeId INT NOT NULL,
    StageTypeId INT NULL,
    MajorId INT NULL,
    QuotaTypeId INT NULL,
    ExamPhaseId INT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    RetrievedAtUtc DATETIME2 NOT NULL,
    SourceSystem NVARCHAR(100) NOT NULL,
    CONSTRAINT CK_ExternalExamRecords_CandidateExamId CHECK(CandidateExamId > 0),
    CONSTRAINT CK_ExternalExamRecords_Amount CHECK(Amount >= 0),
    CONSTRAINT UX_ExternalExamRecords_CandidateExamId UNIQUE(CandidateExamId)
);
CREATE INDEX IX_ExternalExamRecords_Agency_Year_Date ON dbo.ExternalExamRecords(AgencyId, YearId, PersianExecutionDate);
CREATE INDEX IX_ExternalExamRecords_ExamType_Year_Date ON dbo.ExternalExamRecords(ExamTypeId, YearId, PersianExecutionDate);
