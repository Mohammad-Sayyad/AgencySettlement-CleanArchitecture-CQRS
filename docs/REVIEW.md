# Review

## External import

External exam import is intentionally simple:

1. `ExternalExamApi` calls the other project's API and receives a list.
2. `ImportExternalExamsHandler` maps that list to `ExternalExamRecord`.
3. `IExternalExamRecordRepository` inserts the records into `dbo.ExternalExamRecords`.
4. `CandidateExamId` is unique, so running the import again does not create duplicate rows.

There is no import batch table, sync table, event-sourcing mechanism, or extra integration layer.

`ExternalExamRecords` is the local table containing the imported data. Settlement calculation reads this local data and does not need to call the external API again.
