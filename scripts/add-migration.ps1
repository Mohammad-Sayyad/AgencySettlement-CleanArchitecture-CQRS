param(
    [string]$MigrationName = "InitialCreate"
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

dotnet ef migrations add $MigrationName `
  --project src/AgencySettlement.Infrastructure `
  --startup-project src/AgencySettlement.API `
  --output-dir Persistence/Migrations
