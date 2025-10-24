# Personal Cloud Drive — Complete Setup & Supabase Guide

This document expands the README with explicit, step-by-step instructions for creating and configuring a Supabase project, wiring the keys into this ASP.NET Core application, and testing every feature (upload, download, rename, delete). Follow these steps exactly on Windows PowerShell for a working local development environment.

Table of contents
- Prerequisites
- Create a Supabase project (step-by-step)
- Configure Supabase Storage bucket
- Database schema & RLS (SQL) — create tables and policies
- Get keys and decide which to use locally
- Configure the app locally (appsettings, secrets, .env)
- Run the app locally (PowerShell & helper)
- Feature walkthrough (register, login, upload, download, rename, delete)
- Troubleshooting & common fixes
- Security & deployment notes
- Useful SQL snippets and commands


## Prerequisites
- Windows or macOS/Linux with PowerShell (this guide uses PowerShell commands)
- .NET SDK 9.0 installed (run `dotnet --info`)
- Git
- A Supabase account (https://supabase.com/) with ability to create a new project
- Recommended: Postgres and Storage knowledge for policies and RLS


## 1) Create a Supabase project (step-by-step)
1. Sign in to https://app.supabase.com and create a new project.
   - Choose a project name (e.g., personal-cloud-drive-dev)
   - Choose a database password (store it securely)
   - Choose a region
   - Wait for provisioning to finish (a few minutes)

2. From the Supabase dashboard, open the project.
3. Click "Settings" → "API" and note these values:
   - Project URL (example: `https://xyzabc.supabase.co`) — we'll call this SUPABASE_URL
   - anon/public key (the "anon" key) — used for client-side public access
   - service_role key — powerful server-side key (do NOT expose in client code)

Note: For local development you can use the anon key for public buckets or use service_role for server-side operations. If you rely on Row Level Security (RLS) and private buckets, ensure you follow the policy guidance below.


## 2) Configure Supabase Storage bucket
1. In Supabase dashboard, go to "Storage" → "Buckets".
2. Click "New bucket".
   - Name: `cloud-drive-files` (or the name you prefer; matches the default used in the app)
   - Public: choose `Private` for real deployments. For local experimentation you can choose `Public` but it bypasses RLS.
3. Click "Create bucket".

Directory layout we recommend
- Use user-scoped paths: `{user_id}/{guid}_{filename}`
  - Example: `u_123/0c5e7f2b-..._photo.jpg`
  - This makes it easy for RLS or file ownership checks.


## 3) Database schema & RLS (SQL)
If you want the app to track uploaded file metadata, create a `files` table. Example SQL (run in Supabase SQL editor):

-- Create `users` table (if not using Supabase Auth directly for metadata)
```sql
create table if not exists users (
  id uuid primary key default gen_random_uuid(),
  email text unique,
  created_at timestamptz default now()
);
```

-- Create `files` table
```sql
create table if not exists files (
  id uuid primary key default gen_random_uuid(),
  user_id uuid not null,
  file_name text not null,
  file_path text not null,
  size bigint,
  mime text,
  created_at timestamptz default now()
);
```

Enable Row Level Security (RLS) on files to protect rows by `user_id`:

```sql
alter table files enable row level security;
```

Create example RLS policy allowing owners to insert/select/delete rows where `user_id` = auth.uid():

```sql
-- Allow users to insert rows where their auth uid matches user_id
create policy "Insert own files" on files
  for insert
  with check (auth.uid() = user_id);

-- Allow users to select rows they own
create policy "Select own files" on files
  for select
  using (auth.uid() = user_id);

-- Allow users to delete rows they own
create policy "Delete own files" on files
  for delete
  using (auth.uid() = user_id);

-- Allow users to update their file metadata if they remain owners
create policy "Update own files" on files
  for update
  using (auth.uid() = user_id)
  with check (auth.uid() = user_id);
```

Notes about auth.uid(): this returns the user's UUID when using Supabase Auth. If you store user IDs differently, adapt the policy accordingly.


## 4) Keys and which to use locally
In Settings -> API:
- SUPABASE_URL = Project URL
- SUPABASE_ANON_KEY = anon/public key (safe for public operations, limited)
- SUPABASE_SERVICE_ROLE_KEY = service_role key (powerful; keep secret)

For this server-side app (PersonalCloudDrive) the `SupabaseService` currently uses a single key from configuration. For a secure production app:
- Use service_role key only on server-side for privileged operations, but do not use it for user-scoped requests that rely on RLS.
- Or initialize a user-authenticated Supabase client with the user's JWT so that RLS works as intended.

For local testing you can set `Supabase:Key` to the anon key if the bucket is public, or the service_role key if you need server-side access to private storage. Be careful: service_role bypasses RLS, so it’s not suitable for enforcing per-user access.


## 5) Configure this application locally
You can provide configuration via `appsettings.Development.json`, environment variables, or the .NET Secret Manager.

A) Using `appsettings.Development.json` (not recommended for secrets):
Create a file `appsettings.Development.json` in `PersonalCloudDrive/` (do not commit this file to Git):

```json
{
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "Key": "YOUR_KEY_HERE",
    "BucketName": "cloud-drive-files"
  }
}
```

B) Using the Secret Manager (recommended for local dev):

```powershell
cd E:\PersonalCloudDrive
# Initialize user secrets for the project if not done yet
dotnet user-secrets init --project .\PersonalCloudDrive.csproj
# Set keys
dotnet user-secrets set "Supabase:Url" "https://your-project.supabase.co" --project .\PersonalCloudDrive.csproj
dotnet user-secrets set "Supabase:Key" "YOUR_KEY_HERE" --project .\PersonalCloudDrive.csproj
dotnet user-secrets set "Supabase:BucketName" "cloud-drive-files" --project .\PersonalCloudDrive.csproj
```

C) Or set environment variables in PowerShell (quick):

```powershell
$env:Supabase__Url = 'https://your-project.supabase.co'
$env:Supabase__Key = 'YOUR_KEY_HERE'
$env:Supabase__BucketName = 'cloud-drive-files'
```

Note: In PowerShell the double underscore `__` maps to `:` in IConfiguration.


## 6) Run the app locally (PowerShell)
You can run the app directly or use the helper script `run-local.ps1` (committed to the repo).

A) Direct run
```powershell
cd E:\PersonalCloudDrive
# ensure env or user-secrets configured
dotnet run --project .\PersonalCloudDrive.csproj -c Debug
```

B) Using `run-local.ps1` (loads .env into environment)
1. Create a `.env` file in repo root with:
```
Supabase__Url=https://your-project.supabase.co
Supabase__Key=YOUR_KEY_HERE
Supabase__BucketName=cloud-drive-files
```
2. Run the script:

```powershell
.\run-local.ps1
```

The app will start and show listening URLs. Open the HTTP URL in your browser.


## 7) Feature walkthrough (how to test each feature)
This section lists exact steps to verify the app features.

Precondition: App is running locally and Supabase keys configured.

1) Register / Log in
- Open the app URL (homepage).
- Register a new user using the Register view (if implemented) or use Supabase Auth from dashboard to create a test user.
- Confirm the email if your Supabase configuration sends confirmation (or set email confirmations off for dev).

2) Upload a file
- Go to the File → Manager page
- Click upload and select a file
- Expected behavior: File uploaded to Supabase Storage and a `files` row created in the DB with `user_id` and `file_path` pointing to `userId/{guid}_{filename}`.
- Verify in Supabase Dashboard → Storage → Files that the file exists under the bucket and path.
- Verify in Supabase Dashboard → Table Editor → `files` that a new row appears.

3) Download a file
- In the UI, click the download action on a file
- Expected: the app downloads bytes from the Supabase Storage via `SupabaseService.DownloadFileAsync` and serves them to the client
- If download fails: check bucket privacy and whether the Supabase client had correct credentials (service role vs user JWT) and whether the file path is correct.

4) Rename a file
- Use the rename UI (if provided). Expected: the DB row `file_name` is updated. Depending on your implementation, the file path may remain the same or you must copy in storage. (This project stores filePath in DB and uses user-scoped naming; renaming may just change metadata.)

5) Delete a file
- Use the delete action. Expected: the app calls `SupabaseService.DeleteFileAsync`, which removes the remote storage object and deletes the DB row.

6) Verify RLS and security
- Attempt to access another user's file path (e.g., change the URL to point to a different userId). If RLS and storage are configured, the request should be blocked unless the JWT maps to the target owner.


## 8) Troubleshooting & common fixes
Problem: `ArgumentNullException("Supabase URL not configured")`
- Fix: Provide `Supabase:Url` via user-secrets, env var, or `appsettings`.

Problem: Storage `Download` returns file not found
- Check the stored file path format; ensure `filePath` in DB is a relative path inside the bucket (no leading slash).
- If you stored a full public URL, the service extracts the path after bucket name; ensure that logic matches how your DB stores paths.
- Ensure you used the correct key for private buckets.

Problem: Access denied even though file exists
- If the bucket is private and you're using anon key, you'll get permission errors. Use authenticated client (user JWT) or server-side service key as appropriate. For per-user access, use user JWT so RLS works.

Problem: Warnings about non-nullable properties during build
- Add `required` modifier or default initialization to view model properties (e.g., `public string UserName { get; set; } = string.Empty;`)


## 9) Security & deployment notes
- Never commit `service_role` key to source control.
- For production, host the web app where secrets are provided via environment variables or a secret store.
- If you need to provide signed URLs for private storage, consider using `CreateSignedUrl` on the storage client rather than returning public URLs.


## 10) Useful SQL snippets (summary)
- Create `files` table and policies (repeat):
```sql
create table if not exists files (
  id uuid primary key default gen_random_uuid(),
  user_id uuid not null,
  file_name text not null,
  file_path text not null,
  size bigint,
  mime text,
  created_at timestamptz default now()
);

alter table files enable row level security;

create policy "Insert own files" on files for insert with check (auth.uid() = user_id);
create policy "Select own files" on files for select using (auth.uid() = user_id);
create policy "Delete own files" on files for delete using (auth.uid() = user_id);
create policy "Update own files" on files for update using (auth.uid() = user_id) with check (auth.uid() = user_id);
```


## 11) Checklist — quick if you want to verify everything is ready
- [ ] Supabase project created
- [ ] Supabase Storage bucket created (`cloud-drive-files`)
- [ ] Keys copied to Secret Manager or .env
- [ ] `files` table created and RLS enabled with policies
- [ ] App runs locally: `dotnet run --project .\\PersonalCloudDrive.csproj -c Debug`
- [ ] Upload/download tested
- [ ] Delete tested


## 12) Next improvements (recommended)
- Implement user-authenticated Supabase client session flow so RLS is enforced using user JWTs rather than a single server key.
- Add unit/integration tests for `FileService` and `SupabaseService` (mock Supabase client using interfaces).
- Add CI job to run `dotnet build` and tests.


---

If you want, I will now:
- Overwrite the repository `README.md` with this full version and commit/push it to the `docs/readme-supabase` branch (I will do that if you confirm),
- Or append sections into the existing README if you prefer shorter changes.

Tell me whether to overwrite now, and I'll commit and push the change.
