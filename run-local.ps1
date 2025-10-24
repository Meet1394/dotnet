Param(
    [string]$EnvFile = ".env"
)

Write-Host "run-local: loading env file: $EnvFile"

if (Test-Path $EnvFile) {
    Get-Content $EnvFile | ForEach-Object {
        $line = $_.Trim()
        if ([string]::IsNullOrWhiteSpace($line)) { return }
        if ($line.TrimStart().StartsWith('#')) { return }
        $parts = $line -split '=', 2
        if ($parts.Length -eq 2) {
            $name = $parts[0].Trim()
            $value = $parts[1].Trim()
            Write-Host "Setting environment variable: $name"
            $env:$name = $value
        }
    }
} else {
    Write-Host "No $EnvFile found — skipping environment load"
}

Write-Host "Starting application..."

dotnet run --project .\PersonalCloudDrive.csproj -c Debug
