# AgencySettlement - Final

Backend for Gaj agency financial settlements, external exam imports, monthly settlement calculations, payment tracking, and financial reporting.

The application uses Clean Architecture, CQRS, MediatR, Entity Framework Core, SQL Server, and repository-based data access.

## 1. Architecture

The solution is organized into four application layers:

```text
AgencySettlement.API
        |
        v
AgencySettlement.Infrastructure
        |
        v
AgencySettlement.Application
        |
        v
AgencySettlement.Domain
```

The dependency direction follows Clean Architecture: outer layers depend on inner layers, and the Domain does not depend on Infrastructure or API.

### Layer responsibilities

**AgencySettlement.Domain**
- Core business entities and domain rules.
- Financial and settlement-related models.
- No EF Core or HTTP dependencies.

**AgencySettlement.Application**
- CQRS commands, queries, and handlers.
- Request/response DTOs.
- Repository and unit-of-work abstractions.
- Application-level validation and orchestration.

**AgencySettlement.Infrastructure**
- EF Core and SQL Server persistence.
- Repository implementations.
- External exam API integration.
- Database queries, transactions, and persistence operations.

**AgencySettlement.API**
- HTTP controllers and endpoint definitions.
- Request binding and response handling.
- Middleware and API-level configuration.

There is intentionally **no separate Service Layer**. Application use cases are implemented through commands, queries, and their handlers.

## 2. Design patterns and implementation principles

The project uses the following patterns:

- Clean Architecture for separation of concerns.
- CQRS for separating write operations from read operations.
- MediatR for dispatching commands and queries.
- Repository Pattern for data access.
- Dependency Injection for managing dependencies.
- Unit of Work where coordinated persistence is required.
- Rule-based calculation logic for registration plans and settlement scenarios.

Repositories are responsible for database access. Business calculations and use-case orchestration belong to the Application layer.

The implementation should avoid unnecessary abstractions and preserve the existing financial rules when extending functionality.

## 3. Main business capabilities

The application supports the following business workflows:

1. Import external exam registration records.
2. Calculate agency settlements for a specified year and execution date.
3. Apply registration-plan pricing, percentage splits, and contractual quotas.
4. Persist settlement records and their associated items and history.
5. Register and track settlement payments.
6. Calculate payment-adjusted financial balances.
7. Retrieve settlement details and agency summaries.
8. Generate financial reports, including settlement and factor reports.
9. Retrieve external settlement status and item-level information.

## 4. External exam import

### Endpoint

`POST /api/external-exams/import`

This endpoint imports exam registration records from an external API into the local database.

Example request:

```json
{
  "pageSize": 5000,
  "maxPages": 1000
}
```

The external API URL is configured through the `ExternalExamApi` settings.

Expected request pattern:

```text
GET {BaseUrl}{ExamsPath}?page=1&pageSize=5000
```

The adapter supports an external response represented as either a JSON array or an envelope containing `items` or `data`, provided the payload matches the supported contract.

### Import behavior

- External records are mapped to the local `ExternalExamRecords` table.
- `CandidateExamId` is the principal identifier used to recognize an exam registration.
- An existing `CandidateExamId` must not be inserted as a second independent record.
- Duplicate handling must follow the current import implementation; it must not be assumed to be insert-only or update-only without checking the handler.
- Importing exam records is a separate operation from calculating settlements.
- Settlement calculations use locally imported records rather than requiring an external API request for every candidate.

The import request includes pagination settings so large external datasets can be retrieved in bounded pages.

### Imported data

The import contract includes the registration and exam attributes needed by settlement calculations, including:

- `CandidateExamId`
- `PackageId`
- `EducationalLevelId`
- `ExamModeId`
- `StudyFieldId`
- `RegistrationPlanId`
- `AgencyId`
- `PersianExecutionDate`
- `YearId`

`RegistrationOrder` should not be treated as a required client-supplied field in the current import contract.

The exact entity properties and optional fields are determined by the current `ExternalExamItem` and persistence model.

## 5. Monthly settlement calculation

### Endpoint

`POST /api/settlements/monthly-calculate`

The monthly calculation use case processes imported exam registration records for a specified year and execution-date selection.

Example request:

```json
{
  "yearId": 10,
  "month": "05/08/08"
}
```

The request fields follow the existing application contract. In particular, `month` represents the supplied Persian-date selection in the format expected by the handler; it should not automatically be interpreted as a Gregorian month number.

### Calculation flow

```text
Monthly Calculation Request
            |
            v
      MediatR Command
            |
            v
    Application Handler
            |
            v
 Validate required lookup data
            |
            v
 Load imported exam registrations
            |
            v
 Group records by calculation key
            |
            v
 Apply pricing and plan rules
            |
            v
 Apply percentage and quota rules
            |
            v
 Calculate agency and Gaj amounts
            |
            v
 Persist settlement and related records
            |
            v
          Commit
```

The grouping key includes the relevant business dimensions:

- Package
- Educational level
- Exam mode
- Registration plan
- Study field
- Year
- Agency
- Persian execution date

The calculation must preserve the existing grouping rules and should not combine registrations belonging to different calculation groups.

## 6. Registration plans and pricing

Registration plans have distinct financial behavior. The calculation must use the applicable plan, grade, exam mode, and configured pricing rules.

The current plan identifiers are:

| Plan ID | Registration plan |
|---:|---|
| 1 | آزاد |
| 2 | حکمت |
| 3 | بورسیه مدارس |
| 5 | بورسیه داوطلب آزاد |
| 8 | ثبت نام از سایت |

Package identifiers are separate from registration-plan identifiers.

For example:

- `PackageId = 1`: پیشرفت
- `PackageId = 2`: آمادگی برای کنکور
- `PackageId = 9`: فرهنگیان

### Pricing rules

The calculation distinguishes between in-person and online examinations.

The current configured business rules include:

- Plans 1 and 8 share the same base pricing rules where applicable.
- Plan 2 uses Hekmat-specific pricing.
- Plan 3 uses a fixed 100,000-toman registration amount for in-person examinations and does not consume the contractual quotas used by Plan 5.
- Plan 5 applies its special quota and registration-fee rules.
- Online registrations for Plans 3 and 5 follow their special free-registration treatment.
- Grade-specific pricing must be applied according to the existing pricing configuration.

Prices are monetary values and should be stored and calculated using appropriate decimal precision.

The rules must not be simplified into one universal price formula because the registration plan, grade, and exam mode affect the outcome.

### Example pricing configuration

The following are existing business-rule examples, not a replacement for the database configuration:

| Scenario | Amount |
|---|---:|
| Hekmat, grade 12, in-person | 1,400,000 toman |
| Hekmat, grade 12, online | 800,000 toman |
| Hekmat, other applicable grades, in-person | 1,250,000 toman |
| Hekmat, other applicable grades, online | 700,000 toman |
| Plan 3, in-person | 100,000 toman |
| Plan 1 or 8, grade 12, online | 3,000,000 toman |
| Plan 1 or 8, grades 10 and 11, online | 2,350,000 toman |
| Plan 1 or 8, grade 9, online | 2,650,000 toman |

Other grade-specific prices must be taken from the current business rules and configuration. The examples above do not constitute a complete price list.

## 7. Percentage allocation and financial calculations

The financial calculation distinguishes between the agency share, Gaj share, and student-related amounts where applicable.

For the applicable percentage split:

```text
AgencyPercent + GajPercent = 100
```

The current standard percentage examples are:

| Exam mode | Agency | Gaj | Student |
|---|---:|---:|---:|
| In-person | 55% | 45% | 0% |
| Online | 40% | 40% | 20% |

These percentages describe the standard allocation rules. Special registration plans and free-registration scenarios must continue to follow their existing calculation logic.

The student share must not be incorrectly included in the agency's debit amount.

### Financial terminology

- **Agency debit:** Amount charged to the agency under the applicable settlement rules.
- **Agency credit:** Amount credited through applicable payments and other recognized credit entries.
- **Gaj amount:** Amount allocated to Gaj under the configured financial rules.
- **Gaj debit and credit:** Separate financial totals used for calculating the Gaj balance.
- **Balance:** Difference between the applicable credit and debit amounts, using the sign convention defined by the corresponding report.

The application contains distinct agency-level and Gaj-level financial calculations. These values must not be treated as interchangeable.

For example, the Gaj balance calculation may use:

```text
BalanceGaj = TotalCreditGaj - TotalDebitGaj
```

The agency balance uses the agency report's own debit/credit convention. A report's sign convention must not be changed without reviewing its existing contract.

All financial operations should use decimal arithmetic and the established database precision.

## 8. Contractual quotas

Quota handling is part of the registration-plan calculation and must preserve the existing business rules.

The relevant quota fields include:

- `FreeQuotaCount`
- `OneHundredThousandQuotaCount`
- `ContractFloorAmount`

### Plan 5 quota consumption

For Plan 5, in-person registrations consume available quota in this order:

1. Free quota.
2. 100,000-toman quota.
3. Normal paid registration.

Plan 3 does not consume these contractual quotas.

The contract floor amount is carried into the corresponding settlement-related records, including the settlement, settlement item, and history where defined by the current model.

Quota processing must be deterministic. A candidate must not consume the same quota more than once during the same calculation.

The removed `FreeCandidateQuotaCount` field must not be reintroduced as a replacement for the current quota model.

## 9. Settlement persistence and history

Settlement calculation persists the financial outcome and its associated details using the current persistence model.

Relevant entities include:

- `Settlements`
- `SettlementItems`
- `SettlementHistories`
- `SettlementPayments`
- `ExternalExamRecords`
- `ExternalExamRecordHistory`

The exact relationships and any additional supporting entities are defined by the current EF Core model and migrations.

### Settlement items

Settlement items preserve the item-level calculation details needed for reporting and reconciliation.

Item-level information should remain consistent with the parent settlement, including the applicable agency, year, execution date, registration plan, exam mode, amounts, and quota-related information.

### Settlement history

History records provide an audit trail of settlement calculations and changes supported by the application.

Historical data must not be modified casually. Any correction or recalculation must follow the application's existing audit and persistence rules.

The older design's `SettlementInputs` and `ExternalExamAmounts` tables must not be assumed to exist in the current implementation unless they are present in the actual EF Core model and database schema.

## 10. Settlement payments

The application supports creating and tracking payments associated with settlements.

Payment processing must distinguish between registering a payment and applying that payment to settlement financial totals.

The current payment workflow uses the `IsAppliedToSettlement` state to prevent the same payment from being applied repeatedly.

Important rules:

- A payment must not be applied to a settlement more than once.
- Read-only report queries must not reapply payments.
- `GetReport` must remain a read operation and must not change payment-application state.
- Payment-related database relationships must be respected when deleting or correcting settlements.
- Dependent payment records must be handled before deleting a referenced settlement where the foreign-key relationship requires it.
- Tracking numbers and payment dates must follow the current payment model.

Payment registration, payment application, and report generation are separate responsibilities. A successful payment insert does not automatically imply that every report should recalculate or mutate the settlement.

## 11. Settlement reports

The application provides financial reporting for settlements and agency accounts.

### Settlement report

The settlement report supports filtering by year and Persian execution date, with an optional agency filter where defined by the endpoint contract.

When an agency is supplied, the report returns that agency's data. When no agency is supplied, the report can return the applicable agencies for the requested year and execution date.

The report uses the existing settlement and payment data to calculate debit, credit, balance, and settlement status.

### Factor report

Endpoint:

`GET /api/report/settlement/factor`

Supported filters include:

- `yearId`
- `persianExecutionDate`
- Optional `agencyId`
- Optional `examModeId`

The factor report provides a financial breakdown by the relevant package, educational level, registration plan, and exam mode.

Its output includes the applicable registration-plan counts, in-person and online totals, debit and credit amounts, balance, and percentage information.

The registration-plan breakdown includes:

- آزاد
- حکمت
- ثبت نام از سایت
- بورسیه مدارس
- بورسیه داوطلب آزاد

The report must preserve the established grouping and pricing rules.

### Zero-value rows

The report must retain the required package and educational-level combinations even when no matching registration exists.

For an expected combination with no records, the numeric totals and counts should be zero rather than null. Text fields should retain their appropriate labels when the combination is known.

This allows the report grid to display a stable set of rows for comparison, including rows without financial activity.

### Free quota reporting

Free quota consumption must be reflected in the applicable registration-plan totals. It must not be counted as an additional paid registration where the existing calculation treats it as a free candidate.

Report queries should not mutate settlements or payment records.

## 12. External settlement status

The application includes an external settlement status query based on agency, year, and Persian execution date.

The existing query retrieves the applicable settlement, prioritizing the latest matching record according to the repository's ordering.

The status response has been extended to include:

- Settlement items (`Items`).
- Total online registration count.
- Total in-person registration count.

The status response should not expose obsolete aggregate quota fields that have been removed from the current response contract, including:

- `ContractFloorAmount`
- `TotalInPersonFreeQuota`
- `TotalInPersonOneHundredThousandQuota`

The status query should return the settlement information and item-level details required by the current DTO. It should not silently change the underlying settlement calculation or reapply payments.

## 13. Lookup data

The application relies on lookup data for registration, exam classification, agencies, and related business dimensions.

The current domain includes or references lookup concepts such as:

- Packages
- Educational levels
- Exam modes
- Study fields
- Registration plans
- Years
- Agencies
- Exam dates and execution dates
- Stage types
- Other reference data required by the current model

The actual table names must be taken from the current EF Core entities and migrations. The older reference-table list should not be treated as a complete or authoritative database schema.

## 14. EF Core and database

The application uses Entity Framework Core with SQL Server.

The Infrastructure layer owns database configuration, repository implementations, and migrations.

Before creating or applying a migration, verify:

- The Infrastructure project is selected as the migrations assembly.
- The API project is the startup project where required.
- The connection string targets the intended database.
- Existing migrations are reviewed to avoid accidental schema changes.
- Production configuration and secrets are not committed to source control.

Example commands, assuming the current solution retains the corresponding project paths and migration configuration:

### Package Manager Console

```powershell
Add-Migration MigrationName -Project AgencySettlement.Infrastructure -StartupProject AgencySettlement.API -OutputDir Persistence/Migrations

Update-Database -Project AgencySettlement.Infrastructure -StartupProject AgencySettlement.API
```

### .NET CLI

```bash
dotnet ef migrations add MigrationName \
  --project src/AgencySettlement.Infrastructure \
  --startup-project src/AgencySettlement.API \
  --output-dir Persistence/Migrations

dotnet ef database update \
  --project src/AgencySettlement.Infrastructure \
  --startup-project src/AgencySettlement.API
```

Replace `MigrationName` with a descriptive migration name. Confirm the real project paths before executing these commands.

## 15. External API configuration

External exam API settings are managed through configuration rather than hard-coded URLs.

The configuration includes the base URL and the path used to retrieve exam records.

The external API adapter is responsible for HTTP communication and mapping supported responses to the application's import model.

External API failures must be handled at the integration boundary. The precise retry policy, timeout behavior, and HTTP status mapping must match the implementation currently deployed.

Settlement calculation should not depend on the external API being available when the required exam registration records have already been imported locally.

## 16. Production and data-integrity rules

The following rules are important for reliable settlement processing:

1. Preserve the existing financial calculations when adding or changing endpoints.
2. Do not apply the same payment more than once.
3. Keep candidate registration records identifiable by `CandidateExamId`.
4. Prevent duplicate candidate entries within a settlement where required by the current business rules.
5. Keep percentage allocations consistent with the applicable registration plan and exam mode.
6. Apply contractual quotas only to the plans that use them.
7. Do not count student shares as agency debit.
8. Keep historical records consistent with the audit requirements.
9. Ensure report queries do not mutate financial data.
10. Respect foreign-key relationships when deleting or correcting records.
11. Use decimal types and appropriate database precision for monetary values.
12. Use the existing Persian-date format consistently across imports, calculations, and reports.
13. Validate required lookup data before calculating settlements.
14. Keep database credentials and environment-specific configuration out of source control.
15. Use least-privilege database permissions in production.
16. Test changes against representative financial scenarios before deployment.

## 17. Scope and design principles

The application is designed to manage Gaj agency settlement workflows, not merely to calculate a percentage of a single gross amount.

The primary business responsibilities are:

- Importing external registration data.
- Applying the appropriate registration-plan rules.
- Calculating settlements by the required business dimensions.
- Managing agency and Gaj financial totals.
- Applying contractual quota rules.
- Tracking payments and settlement balances.
- Providing consistent financial reports and audit information.

Any future changes should preserve the separation between importing source data, calculating settlements, registering payments, and reading reports.

The current implementation and its database migrations remain the authoritative sources for exact entity properties, endpoint contracts, repository methods, and persistence behavior.
