$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

dotnet ef database update `
  --project src/AgencySettlement.Infrastructure `
  --startup-project src/AgencySettlement.API
