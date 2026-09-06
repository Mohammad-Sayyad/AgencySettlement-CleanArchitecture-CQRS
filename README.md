# AgencySettlement - Final

Backend for Gaj agency financial settlement.

## Architecture

Exactly four application layers:

```text
AgencySettlement.Domain
        ↑
AgencySettlement.Application
        ↑
AgencySettlement.Infrastructure
        ↑
AgencySettlement.API
```

`Domain` has no EF Core or HTTP dependency.
`Application` contains CQRS commands/queries and interfaces only.
`Infrastructure` contains EF Core, SQL Server repositories and the external API adapter.
`API` contains controllers and middleware.

There is intentionally **no Service Layer**.

## Repository design

Every domain entity has its own repository interface and implementation. `IRepository<TEntity>` only contains common operations (`GetByIdAsync`, `GetAllAsync`, `AddAsync`). Entity-specific queries stay in the repository for that entity.

Examples:

- `IPriceRepository` → `GetActivePriceAsync`
- `IPercentRepository` → `GetActivePercentPairAsync`
- `ISettlementRepository` → `GetDetailsAsync`, `GetByIdempotencyKeyAsync`, `GetAgencySummaryAsync`
- Every lookup entity has its own repository files.

## Financial flow

```text
POST /api/settlements/calculate
        ↓
MediatR Command
        ↓
CalculateSettlementHandler
        ↓
validate Agency + Lookup data
        ↓
select active Price
        ↓
select one valid Percent pair
        ↓
External API
        ↓
CandidateExamId + Amount (one row per item)
        ↓
SUM(Amount) = GrossAmount
        ↓
AgencyAmount = ROUND(GrossAmount * AgencyPercent / 100, 2)
GajAmount    = GrossAmount - AgencyAmount
        ↓
Settlement + SettlementInput + ExternalExamAmounts + SettlementItems + SettlementHistory
        ↓
Commit
```

The external import supports large candidate lists page-by-page. The current settlement calculation remains single-CandidateExamId and should be extended to a batch settlement use case if one settlement must contain thousands of candidates.

## Percent rule

There are exactly two rows in `Percents` for a valid percentage rule:

```text
PercentType = Agency
PercentType = Gaj
```

For the selected rule:

```text
AgencyPercent + GajPercent = 100
```

The repository selects the pair by the same `AgencyId`, `EffectiveFrom`, `EffectiveTo`, and `Priority` and requires `Gaj.Value = 100 - Agency.Value`.

At settlement time the selected values are snapshotted into `Settlements` and `SettlementHistories`.

## Example

Suppose 200 CandidateExamId rows come from the external API:

```text
GrossAmount = 1,100,000,000
Agency      = 25%
Gaj         = 75%

AgencyAmount = 275,000,000
GajAmount    = 825,000,000
```

The invariant is always:

```text
AgencyPercent + GajPercent = 100
AgencyAmount + GajAmount = GrossAmount
```

## Tables

Reference:

- `StageTypes`
- `GradeTypes`
- `Majors`
- `States`
- `Regions`
- `ExamTypes`
- `QuotaTypes`
- `YearTypes`
- `ScheduledDates`
- `ExamPhases`
- `ExamModes`
- `Agencies`

Business/configuration:

- `Prices`
- `Percents`
- `SettlementInputs`
- `ExternalExamAmounts`
- `Settlements`
- `SettlementItems`
- `SettlementHistories`

`ExternalExamAmounts` stores the external API amount as source/audit data for every `CandidateExamId`; the settlement financial amount comes from the internal `Prices` configuration.

`SettlementInputs` stores the exact input combination used by a calculation.

`SettlementHistories` is an immutable audit snapshot of the calculation, including price/percent validity information, agency snapshot, candidate amounts and final financial result.

## API

### Calculate

`POST /api/settlements/calculate`

Header:

```text
Idempotency-Key: unique-key-123
```

Body:

```json
{
  "agencyId": 37,
  "yearId": 5,
  "persianExecutionDate": "2026-08-20T00:00:00",
  "candidateExamId": 100001,
  "packageId": 2,
  "educationalLevelId": 3,
  "examModeId": 1,
  "examTypeId": 2
}
```

The CandidateExamId must already exist in `ExternalExamRecords`; the calculation endpoint does not call the external API.

### Get settlement

`GET /api/settlements/{id}`

### Agency summary

`GET /api/settlements/agency/{agencyId}/summary?fromUtc=2026-01-01T00:00:00Z&toUtc=2027-01-01T00:00:00Z`

This gives the number of settlements and total Gross/Agency/Gaj amounts for the agency in the requested period.

## EF Core migrations

The design-time factory is in Infrastructure.

Package Manager Console:

```powershell
Add-Migration InitialCreate -Project AgencySettlement.Infrastructure -StartupProject AgencySettlement.API -OutputDir Persistence/Migrations
Update-Database -Project AgencySettlement.Infrastructure -StartupProject AgencySettlement.API
```

CLI:

```bash
dotnet ef migrations add InitialCreate --project src/AgencySettlement.Infrastructure --startup-project src/AgencySettlement.API --output-dir Persistence/Migrations
dotnet ef database update --project src/AgencySettlement.Infrastructure --startup-project src/AgencySettlement.API
```

The SQL schema is also available at `scripts/schema.sql`.

## External API contract

The real API is not available in this project, so its URL is configurable via `ExternalExamApi` settings.
Expected response shape:

```json
[
  { "candidateExamId": 100001, "amount": 5500000.00 },
  { "candidateExamId": 100002, "amount": 5500000.00 }
]
```

The adapter rejects timeouts/unavailable API responses as 504/502 and performs a small bounded retry for transient failures.

## Important production rules

- Do not mutate `SettlementHistories`.
- Treat `Idempotency-Key` as mandatory.
- Do not write duplicate `CandidateExamId` values into one settlement.
- Keep the Agency/Gaj percentage pair exactly complementary.
- Do not overlap active Price rules for the same price key unless Priority intentionally differentiates them.
- Keep DB timestamps in UTC; convert to Persian calendar in the presentation layer.
- Use a least-privilege DB user in production.

## External exam import

External exam data is imported independently from settlement calculation. The import endpoint calls the external API, receives a list, maps it to the local `ExternalExamRecords` table, and ignores duplicate `CandidateExamId` values. There is no separate import-batch or synchronization table.

`POST /api/external-exams/import`

```json
{
  "pageSize": 5000,
  "maxPages": 1000
}
```

Expected external API request:

```text
GET {BaseUrl}{ExamsPath}?page=1&pageSize=5000
```

The adapter accepts either a JSON array or an envelope containing `items`/`data`. Each item is expected to contain the normalized exam fields represented by `ExternalExamItem`.

The imported table is deliberately independent from `Settlements`. `CalculateSettlement` reads the already-imported `CandidateExamId` from `ExternalExamRecords`; it does not call the external API during financial calculation.
