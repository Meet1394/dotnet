## Personal Cloud Drive (ASP.NET Core + Supabase)

This repository contains a small ASP.NET Core Razor application called "PersonalCloudDrive" that uses Supabase for storage and PostgREST models for data. This README explains how to clone, configure, build, and run the project on a Windows machine (PowerShell). It also documents the exact configuration keys the application expects.

## Quick summary / contract
- Inputs: `appsettings.json` (or environment variables) with Supabase configuration keys: `Supabase:Url`, `Supabase:Key`, optional `Supabase:BucketName`.
- Outputs: an ASP.NET web app served locally (default Kestrel URL shown on startup).
- Error modes: missing .NET SDK, missing Supabase config, port in use.

## Prerequisites
- .NET SDK 9.0 (the project targets `net9.0`). Install from https://dotnet.microsoft.com/en-us/download
- Git (to clone the repo)
- An editor: Visual Studio 2022/2023+ with ASP.NET workload, or VS Code with C# extension

Optional
- Supabase project (for storage and Postgres). If you only want to run UI without storage integration, you can stub or mock storage calls but some features will fail.

## What this project expects (configuration)
The project reads Supabase configuration from the standard ASP.NET configuration providers. The exact keys used by `Services/SupabaseService.cs` are:

- `Supabase:Url` (required) — the Supabase project URL (e.g. `https://xyzcompany.supabase.co`)
- `Supabase:Key` (required) — the Supabase anon/public or service role key used by the Supabase .NET client
- `Supabase:BucketName` (optional) — defaults to `cloud-drive-files` if omitted

You can provide these values in `appsettings.json`, `appsettings.Development.json`, or via environment variables. Environment variables should use the colon-based name mapping used by ASP.NET Core (e.g., set `Supabase__Url` for PowerShell).

Example `appsettings.Development.json` snippet (store sensitive keys securely in production):

```json
{
	"Supabase": {
		"Url": "https://your-project.supabase.co",
		"Key": "YOUR_SUPABASE_KEY",
		"BucketName": "cloud-drive-files"
	}
}
```

Or set environment variables in PowerShell:

```powershell
$env:Supabase__Url = 'https://your-project.supabase.co'
$env:Supabase__Key = 'YOUR_SUPABASE_KEY'
$env:Supabase__BucketName = 'cloud-drive-files'  # optional
```

Notes about Supabase keys
- For simple local testing a public/anon key may be sufficient for storage public buckets. If your bucket is private and you use RLS policies, you must authenticate users and supply the correct JWT to the Supabase client (this project currently initializes the Supabase client using the project key in `SupabaseService` — review for your security needs).

## Clone, restore, build and run (PowerShell)
Open PowerShell (as a normal user) and run the following commands from the folder where you want to clone the repo.

```powershell
# Clone the repository
git clone https://github.com/Meet1394/dotnet.git PersonalCloudDrive
cd PersonalCloudDrive

# (Optional) Show .NET SDK version
dotnet --info

# Restore and build solution
dotnet restore "PersonalCloudDrive.sln"
dotnet build "PersonalCloudDrive.sln" -c Debug

# Run the app (Kestrel) in Development
dotnet run --project .\PersonalCloudDrive.csproj -c Debug
```

When the app starts, Kestrel will print the URLs it is listening on (for example `http://localhost:5000`). Open that URL in your browser.

If you prefer to open in Visual Studio
1. Open `PersonalCloudDrive.sln` in Visual Studio
2. Ensure the selected framework is `.NET 9.0` and the launch profile is `IIS Express` or the project name (Kestrel)
3. Set environment (Development) and run

## Editing configuration safely
- Do not commit real API keys to Git. Use `appsettings.Development.json` for local dev (which you can keep out of the repo via .gitignore), or use a secrets manager.

To use the Secret Manager (recommended for dev):

```powershell
# In repository root
dotnet user-secrets init --project .\PersonalCloudDrive.csproj
dotnet user-secrets set "Supabase:Url" "https://your-project.supabase.co" --project .\PersonalCloudDrive.csproj
dotnet user-secrets set "Supabase:Key" "YOUR_SUPABASE_KEY" --project .\PersonalCloudDrive.csproj
```

## Common tasks
- Run unit or integration tests: (none included) — add tests under a test project and run `dotnet test`.
- Publish for production: `dotnet publish -c Release -o ./publish`

## Troubleshooting
- Build fails with "SDK not found": ensure .NET 9 SDK is installed and added to PATH.
- Application throws `ArgumentNullException("Supabase URL not configured")` or `Supabase Key not configured": set the `Supabase:Url` and `Supabase:Key` values in `appsettings` or environment variables.
- Supabase upload/download errors: verify the bucket name, the bucket's privacy settings, and the key/credentials used. Use the Supabase dashboard to inspect storage objects and permissions.
- Port in use: change the port with `--urls` when running: `dotnet run --project .\PersonalCloudDrive.csproj --urls "http://localhost:5010"`

## Files of interest
- `Program.cs` — app startup and DI registrations (registers `SupabaseService`)
- `Services/SupabaseService.cs` — Supabase client initialization and storage helpers (see configuration keys above)
- `Controllers/FileController.cs` — endpoints that upload/download files using `SupabaseService`

## Contributing
- Fork, create a branch, implement a small change, run `dotnet build`, and open a PR. Include a short description and testing steps.

## License
This project does not currently include a license file. Add a `LICENSE` file if you want to publish a license.

## Contact
Project: https://github.com/Meet1394/dotnet

---

If you'd like, I can also:
- Add a sample `appsettings.Development.json.example` with the keys shown above (safe stub values),
- Add a small PowerShell script `run-local.ps1` that sets env vars from a `.env` file and runs the app,
- Or run `dotnet build` now and share the exact build output.
