## Personal Cloud Drive — Professional Overview

Personal Cloud Drive is a compact ASP.NET Core Razor application that provides a personal file storage UI backed by Supabase Storage and Postgres. It demonstrates secure file upload/download flows, user-scoped storage paths, and a small set of controllers and services for file management.

This document explains how to set up a local development environment, configure Supabase keys, run the app on Windows (PowerShell), and contribute improvements.

### Goals and audience
- Intended for individual developers or small teams who want a minimal, realistic example of integrating Supabase with an ASP.NET Core Razor app.
- Audience: .NET developers familiar with ASP.NET Core, Supabase users evaluating .NET integrations.

## Features
- User-scoped file upload and download (paths include userId for RLS compatibility)
- Integration with Supabase Storage via the Supabase .NET client
- Simple Razor UI for browsing, uploading, renaming and deleting files

## Project layout (files you will likely inspect)
- `Program.cs` — app startup, DI and middleware
- `Services/SupabaseService.cs` — Supabase client initialization and helpers for upload/download/delete
- `Services/FileService.cs` — application file operations and business rules
- `Controllers/FileController.cs`, `Controllers/AuthController.cs`, `Controllers/HomeController.cs` — Razor controllers
- `Views/` — Razor views (UI)

## Requirements
- .NET SDK 9.0 (project targets `net9.0`). Download: https://dotnet.microsoft.com/en-us/download
- Git
- A Supabase project (recommended) with a Storage bucket configured. You may run the app without Supabase, but file operations will fail.

## Security notes (quick)
- Do NOT commit real Supabase keys to source control.
- For production, use server-side secrets (Key Vault, environment variables in CI/CD). For local development, use the Secret Manager or environment variables.

## Configuration
The app reads configuration via standard ASP.NET Core providers. The keys used by the project (see `Services/SupabaseService.cs`) are:

- `Supabase:Url` (required) — your Supabase project URL, e.g. `https://xxxx.supabase.co`
- `Supabase:Key` (required) — project anon key or service role key depending on your security model
- `Supabase:BucketName` (optional) — defaults to `cloud-drive-files`

You can supply these values in `appsettings.json`, `appsettings.Development.json`, via the Secret Manager, or as environment variables (PowerShell uses `Supabase__Url` format).

Example `appsettings.Development.json` (do not commit secrets):

```json
{
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "Key": "YOUR_SUPABASE_KEY",
    "BucketName": "cloud-drive-files"
  }
}
```

Recommended: use the Secret Manager to avoid putting keys into files during development:

```powershell
dotnet user-secrets init --project .\\PersonalCloudDrive.csproj
dotnet user-secrets set "Supabase:Url" "https://your-project.supabase.co" --project .\\PersonalCloudDrive.csproj
dotnet user-secrets set "Supabase:Key" "YOUR_SUPABASE_KEY" --project .\\PersonalCloudDrive.csproj
```

Or set environment variables in PowerShell:

```powershell
$env:Supabase__Url = 'https://your-project.supabase.co'
$env:Supabase__Key = 'YOUR_SUPABASE_KEY'
$env:Supabase__BucketName = 'cloud-drive-files'  # optional
```

## Development — clone, build and run (PowerShell)
Run these commands from the folder where you want to keep the repo.

```powershell
# clone repo
git clone https://github.com/Meet1394/dotnet.git PersonalCloudDrive
cd PersonalCloudDrive

# restore & build
dotnet restore "PersonalCloudDrive.sln"
dotnet build "PersonalCloudDrive.sln" -c Debug

# run app (development)
dotnet run --project .\\PersonalCloudDrive.csproj -c Debug
```

When running, the app will print the listening URLs (Kestrel). Open the HTTP URL in your browser.

Alternative: open `PersonalCloudDrive.sln` in Visual Studio and run using the selected launch profile.

## Quick helper: run-local.ps1
There is a `run-local.ps1` helper (committed alongside this README) that optionally loads a `.env` file and runs the app with environment variables set for the session. Use it when you prefer not to use the Secret Manager.

## Testing
- There are no unit tests included by default. Add test projects and run `dotnet test`.

## Troubleshooting
- "SDK not found": install .NET 9 SDK and ensure `dotnet --info` shows it.
- `ArgumentNullException("Supabase URL not configured")`: set `Supabase:Url` in configuration or environment variables.
- Storage errors: check the bucket name, privacy settings and keys on the Supabase dashboard. If the bucket is private, ensure the client is authenticated with an appropriate JWT.
- Build warnings about nullable properties: add `required` modifiers or default values in view models.

## Deployment
- The app is a standard ASP.NET Core app and can be published with `dotnet publish -c Release -o ./publish`.
- For hosting, choose an environment that can supply the Supabase keys securely (App Service, Azure Static Web Apps with serverless API, container host with secrets, etc.).

## Contributing
- Create a branch: `git checkout -b feat/your-feature`
- Make changes, run `dotnet build`, and ensure functionality works locally
- Commit with a descriptive message and open a PR against `main`

If you want, I can open a draft PR on your behalf with these changes.

## Files of interest (quick links)
- `Program.cs` — DI & startup
- `Services/SupabaseService.cs` — Supabase client + storage helpers
- `Services/FileService.cs` — file business logic
- `Controllers/FileController.cs` — upload/download endpoints

## License
This repository currently does not include a `LICENSE` file. Add one if you plan to publish under a specific license.

## Contact & support
Project: https://github.com/Meet1394/dotnet

---

If you'd like, I will also:
- Add `appsettings.Development.json.example` with safe placeholder values (I will add it now),
- Add `run-local.ps1` to load a `.env` file and run the app locally (I will add it now),
- Or create a PR for these edits on `docs/readme-supabase`.
