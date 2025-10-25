# 📘 COMPLETE DETAILED SETUP INSTRUCTIONS
## Personal Cloud Drive - Step-by-Step Installation Guide

**⏱️ Total Time Required:** 45-60 minutes (first time)  
**💻 Skill Level:** Beginner to Intermediate  
**📌 Last Updated:** October 2024

---

## 📑 TABLE OF CONTENTS

1. [Introduction](#1-introduction)
2. [System Requirements](#2-system-requirements)
3. [Installing Prerequisites](#3-installing-prerequisites)
4. [Downloading the Project](#4-downloading-the-project)
5. [Setting Up Supabase](#5-setting-up-supabase)
6. [Configuring the Application](#6-configuring-the-application)
7. [Building and Running](#7-building-and-running)
8. [Testing Your Installation](#8-testing-your-installation)
9. [Troubleshooting Common Issues](#9-troubleshooting-common-issues)
10. [Next Steps](#10-next-steps)

---

## 1. INTRODUCTION

### 1.1 What You're Building

Personal Cloud Drive is a **self-hosted cloud storage application** similar to Google Drive or Dropbox. By the end of this guide, you will have:

✅ A fully functional web application  
✅ User authentication system  
✅ File upload and download capabilities  
✅ Folder organization  
✅ File sharing with links  
✅ Beautiful modern interface  

### 1.2 Architecture Overview

```
┌─────────────┐      ┌──────────────┐      ┌─────────────┐
│   Browser   │ ───► │  ASP.NET MVC │ ───► │  Supabase   │
│  (Frontend) │      │   (Backend)  │      │ (Database + │
│             │ ◄─── │              │ ◄─── │  Storage)   │
└─────────────┘      └──────────────┘      └─────────────┘
```

### 1.3 What You Need

- A computer (Windows, macOS, or Linux)
- Internet connection (at least 5 Mbps)
- 1-2 hours of free time
- Basic understanding of command line
- A free Supabase account

---

## 2. SYSTEM REQUIREMENTS

### 2.1 Operating System

**Windows:**
- Windows 10 (version 1607 or higher)
- Windows 11 (any version)

**macOS:**
- macOS 10.15 (Catalina) or higher
- macOS 11+ (Big Sur, Monterey, Ventura, Sonoma)

**Linux:**
- Ubuntu 18.04, 20.04, 22.04, or 24.04
- Debian 10 or higher
- Fedora 33 or higher
- Other distributions with .NET support

### 2.2 Hardware Requirements

**Minimum:**
- CPU: Dual-core processor (2 GHz)
- RAM: 4 GB
- Disk Space: 2 GB free space
- Internet: 5 Mbps connection

**Recommended:**
- CPU: Quad-core processor (2.5 GHz or higher)
- RAM: 8 GB or more
- Disk Space: 5 GB free space
- Internet: 10 Mbps or faster

### 2.3 Required Software

We'll install these in the next section:
- .NET 6.0 SDK or higher
- Visual Studio Code or Visual Studio 2022
- Git
- Web browser (Chrome, Firefox, or Edge)

---

## 3. INSTALLING PREREQUISITES

### 3.1 Installing .NET SDK

#### **For Windows Users:**

**Step 1:** Open your web browser

**Step 2:** Go to: https://dotnet.microsoft.com/download

**Step 3:** Click the large **"Download .NET SDK"** button
- Make sure it says **.NET 6.0** or higher (7.0, 8.0 also work)

**Step 4:** Wait for download to complete (approximately 150 MB)

**Step 5:** Run the downloaded installer
- Double-click the `.exe` file
- If Windows asks "Do you want to allow this app?", click **Yes**

**Step 6:** Follow the installation wizard
1. Click **"Install"**
2. Read and accept the license agreement
3. Wait for installation (2-3 minutes)
4. Click **"Close"** when finished

**Step 7:** Verify installation
1. Press `Windows Key + R` to open Run dialog
2. Type: `cmd` and press Enter
3. In the command prompt, type:
   ```cmd
   dotnet --version
   ```
4. You should see something like: `6.0.xxx` or `7.0.xxx`

**✅ If you see a version number, .NET is installed correctly!**

---

#### **For macOS Users:**

**Step 1:** Open Terminal
- Press `Cmd + Space`
- Type: `terminal`
- Press Enter

**Step 2:** Install Homebrew (if not already installed)
```bash
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
```

**Step 3:** Install .NET SDK
```bash
brew install --cask dotnet-sdk
```

**Step 4:** Verify installation
```bash
dotnet --version
```

**Expected output:** `6.0.xxx` or higher

---

#### **For Linux (Ubuntu/Debian) Users:**

**Step 1:** Open Terminal (`Ctrl + Alt + T`)

**Step 2:** Add Microsoft package repository
```bash
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb

sudo dpkg -i packages-microsoft-prod.deb

rm packages-microsoft-prod.deb
```

**Step 3:** Update package list
```bash
sudo apt-get update
```

**Step 4:** Install .NET SDK
```bash
sudo apt-get install -y dotnet-sdk-6.0
```

**Step 5:** Verify installation
```bash
dotnet --version
```

---

### 3.2 Installing Visual Studio Code

#### **Why VS Code?**
- Free and lightweight
- Excellent .NET support
- Cross-platform
- Great for beginners

#### **Installation Steps:**

**Step 1:** Go to: https://code.visualstudio.com

**Step 2:** Click **"Download for [Your OS]"**
- Windows: Downloads `.exe` file
- macOS: Downloads `.zip` or `.dmg` file
- Linux: Choose `.deb` or `.rpm` package

**Step 3:** Install the application
- **Windows:** Run the `.exe` and follow wizard
- **macOS:** Open `.dmg` and drag to Applications folder
- **Linux:** 
  ```bash
  sudo dpkg -i code_*.deb
  # OR
  sudo rpm -i code-*.rpm
  ```

**Step 4:** Launch VS Code

**Step 5:** Install C# Extension
1. Click **Extensions** icon on left sidebar (or press `Ctrl+Shift+X`)
2. Search for: **"C# for Visual Studio Code"**
3. Click **"Install"** on the extension by Microsoft
4. Wait for installation to complete

**Step 6:** Restart VS Code

**✅ VS Code is now ready!**

---

### 3.3 Installing Git

#### **For Windows:**

**Step 1:** Go to: https://git-scm.com/download/win

**Step 2:** Download will start automatically (Git-xxx-64-bit.exe)

**Step 3:** Run the installer

**Step 4:** Installation wizard settings:
- **Select Components:** Use defaults (all checked)
- **Text Editor:** Choose default (Vim or Nano)
- **PATH Environment:** Choose "Git from the command line and also from 3rd-party software"
- **Line Ending:** Choose "Checkout Windows-style, commit Unix-style"
- All other options: Use defaults

**Step 5:** Click **"Install"** and wait 2-3 minutes

**Step 6:** Click **"Finish"**

**Step 7:** Verify installation
```cmd
git --version
```

Expected: `git version 2.xx.x`

---

#### **For macOS:**

```bash
# Using Homebrew
brew install git

# Verify
git --version
```

---

#### **For Linux:**

```bash
# Ubuntu/Debian
sudo apt-get install git

# Fedora
sudo dnf install git

# Verify
git --version
```

---

### 3.4 Choose a Web Browser

**Recommended Browsers:**
- Google Chrome (Latest version)
- Mozilla Firefox (Latest version)
- Microsoft Edge (Latest version)

**Why it matters:** Modern browsers have better developer tools and CSS support.

---

## 4. DOWNLOADING THE PROJECT

### 4.1 Create a Projects Folder

**For Windows:**
```cmd
# Open Command Prompt (Win+R, type cmd)

# Create folder
mkdir C:\Projects

# Navigate to it
cd C:\Projects
```

**For macOS/Linux:**
```bash
# Create folder
mkdir ~/Projects

# Navigate to it
cd ~/Projects
```

---

### 4.2 Clone the Repository

**Step 1:** Make sure you're in the Projects folder

**Step 2:** Clone the repository
```bash
git clone https://github.com/YOUR_USERNAME/PersonalCloudDrive.git
```

**⚠️ IMPORTANT:** Replace `YOUR_USERNAME` with the actual GitHub username!

**Step 3:** Wait for download (30 seconds - 2 minutes)

**Expected output:**
```
Cloning into 'PersonalCloudDrive'...
remote: Enumerating objects: 156, done.
remote: Counting objects: 100% (156/156), done.
remote: Compressing objects: 100% (98/98), done.
Receiving objects: 100% (156/156), 45.23 KiB, done.
```

**Step 4:** Navigate into project folder
```bash
cd PersonalCloudDrive
```

**Step 5:** Verify files downloaded
```bash
# Windows
dir

# macOS/Linux
ls -la
```

**You should see:**
- `Controllers/` folder
- `Models/` folder
- `Services/` folder
- `Views/` folder
- `appsettings.json` file
- `Program.cs` file
- And more...

**✅ Project successfully downloaded!**

---

## 5. SETTING UP SUPABASE

### 5.1 Understanding Supabase

**What is Supabase?**
- Open-source Firebase alternative
- Provides PostgreSQL database
- Includes file storage
- Has built-in authentication
- **FREE tier available** (perfect for this project)

**What we'll use it for:**
- Store user accounts
- Store file metadata (names, sizes, dates)
- Store actual file content
- Manage folders structure

---

### 5.2 Creating Supabase Account

**Step 1:** Open web browser and go to: https://supabase.com

**Step 2:** Click **"Start your project"** button (top right)

**Step 3:** Choose sign-up method:

**Option A: Sign up with GitHub (RECOMMENDED)**
1. Click GitHub icon
2. Click "Authorize Supabase"
3. You'll be logged in automatically

**Option B: Sign up with Email**
1. Enter your email address
2. Create a password (at least 8 characters)
3. Click "Sign Up"
4. Check your email for verification link
5. Click the link to verify

**Step 4:** You'll see the Supabase Dashboard

---

### 5.3 Creating Your Project

**Step 1:** Click **"New Project"** button (big green button)

**Step 2:** Create Organization (if first time)
1. Click **"New organization"**
2. Name it: `My Projects` or your name
3. Choose plan: **Free** (it's perfect!)
4. Click **"Create organization"**

**Step 3:** Fill in Project Details

```
┌─────────────────────────────────────────────┐
│ Project Name: personal-cloud-drive          │
│                                             │
│ Database Password: [Create strong password]│
│   ⚠️ IMPORTANT: SAVE THIS PASSWORD!        │
│   Write it down or use password manager    │
│                                             │
│ Region: [Choose closest to you]            │
│   - US East (North Virginia)               │
│   - US West (Oregon)                       │
│   - Europe (Frankfurt)                     │
│   - Asia Pacific (Singapore)               │
│   - etc.                                   │
│                                             │
│ Pricing Plan: Free ✓                       │
└─────────────────────────────────────────────┘
```

**Step 4:** Click **"Create new project"**

**Step 5:** Wait for setup (2-3 minutes)
- You'll see "Setting up project..." message
- Progress bar will fill up
- Don't close the browser!

**Step 6:** Project is ready when you see:
- Green checkmark ✅
- Dashboard loads with statistics
- Left sidebar shows menu items

**✅ Supabase project created!**

---

### 5.4 Getting API Credentials

**Step 1:** In Supabase Dashboard, look at left sidebar

**Step 2:** Click **Settings** icon (⚙️) at the bottom

**Step 3:** Click **"API"** in the settings menu

**Step 4:** You'll see two important values:

**📋 Copy these values:**

**A. Project URL**
```
https://abcdefghijklmnop.supabase.co
```
- Click the copy icon next to it
- Paste it into a text file

**B. anon public Key**
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImFiY2RlZmdoaWprbG1ub3AiLCJyb2xlIjoiYW5vbiIsImlhdCI6MTYzMzAyOTYwMCwiZXhwIjoxOTQ4NjA1NjAwfQ.xxxxxxxxxxxxxxxxxxxxxxxxxxx
```
- Click the copy icon
- Paste it into the same text file

**⚠️ CRITICAL STEP:** Save these credentials!

Create a file on your desktop named: `supabase-credentials.txt`

```
Project URL: https://your-project.supabase.co
Anon Key: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Database Password: YourStrongPassword123
```

**🔒 Keep this file secure!** These are your private credentials.

---

### 5.5 Creating Database Tables

**Step 1:** In Supabase Dashboard, click **SQL Editor** icon (</>) on left sidebar

**Step 2:** Click **"New query"** button

**Step 3:** Copy this ENTIRE SQL script:

```sql
-- ================================================
-- PERSONAL CLOUD DRIVE - DATABASE SETUP
-- Copy this entire script and run it in Supabase
-- ================================================

-- Drop existing tables if they exist (for clean install)
DROP TABLE IF EXISTS shared_files CASCADE;
DROP TABLE IF EXISTS files CASCADE;
DROP TABLE IF EXISTS folders CASCADE;
DROP TABLE IF EXISTS users CASCADE;

-- ================================================
-- 1. USERS TABLE
-- ================================================
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(255) NOT NULL,
    storage_used_mb DECIMAL(10, 2) DEFAULT 0,
    storage_limit_mb DECIMAL(10, 2) DEFAULT 1024,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    is_admin BOOLEAN DEFAULT FALSE
);

-- Index for faster email lookups
CREATE INDEX idx_users_email ON users(email);

-- ================================================
-- 2. FOLDERS TABLE
-- ================================================
CREATE TABLE folders (
    id SERIAL PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    folder_name VARCHAR(255) NOT NULL,
    parent_folder_id INTEGER REFERENCES folders(id) ON DELETE CASCADE,
    created_on TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    is_deleted BOOLEAN DEFAULT FALSE
);

-- Indexes for better performance
CREATE INDEX idx_folders_user_id ON folders(user_id);
CREATE INDEX idx_folders_parent_id ON folders(parent_folder_id);
CREATE INDEX idx_folders_is_deleted ON folders(is_deleted);

-- ================================================
-- 3. FILES TABLE
-- ================================================
CREATE TABLE files (
    id SERIAL PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    file_name VARCHAR(500) NOT NULL,
    file_path VARCHAR(1000) NOT NULL,
    file_type VARCHAR(50),
    file_size BIGINT NOT NULL,
    uploaded_on TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    folder_id INTEGER REFERENCES folders(id) ON DELETE SET NULL,
    version INTEGER DEFAULT 1,
    is_deleted BOOLEAN DEFAULT FALSE,
    storage_url TEXT NOT NULL
);

-- Indexes for better query performance
CREATE INDEX idx_files_user_id ON files(user_id);
CREATE INDEX idx_files_folder_id ON files(folder_id);
CREATE INDEX idx_files_is_deleted ON files(is_deleted);
CREATE INDEX idx_files_uploaded_on ON files(uploaded_on DESC);

-- ================================================
-- 4. SHARED FILES TABLE
-- ================================================
CREATE TABLE shared_files (
    id SERIAL PRIMARY KEY,
    file_id INTEGER NOT NULL REFERENCES files(id) ON DELETE CASCADE,
    share_token VARCHAR(255) UNIQUE NOT NULL,
    password VARCHAR(255),
    expiry_date TIMESTAMP WITH TIME ZONE,
    created_on TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    download_count INTEGER DEFAULT 0
);

-- Index for faster token lookups
CREATE INDEX idx_shared_files_token ON shared_files(share_token);
CREATE INDEX idx_shared_files_file_id ON shared_files(file_id);

-- ================================================
-- 5. DISABLE RLS (For Development)
-- ================================================
-- RLS = Row Level Security
-- We disable it for simplicity in development
ALTER TABLE users DISABLE ROW LEVEL SECURITY;
ALTER TABLE folders DISABLE ROW LEVEL SECURITY;
ALTER TABLE files DISABLE ROW LEVEL SECURITY;
ALTER TABLE shared_files DISABLE ROW LEVEL SECURITY;

-- ================================================
-- 6. CONFIRMATION MESSAGE
-- ================================================
SELECT 'Database setup completed successfully! ✅' as message;

COMMIT;
```

**Step 4:** Paste the script into the SQL Editor

**Step 5:** Click **"Run"** button (or press `Ctrl+Enter`)

**Step 6:** Wait for execution (2-5 seconds)

**Step 7:** Verify success:
- Look at the bottom panel
- You should see: `"Database setup completed successfully! ✅"`
- If you see any RED error messages, copy them and see Troubleshooting section

---

### 5.6 Verifying Tables Were Created

**Step 1:** Click **Table Editor** icon (📊) on left sidebar

**Step 2:** You should see 4 tables:

```
✅ users (0 rows)
✅ folders (0 rows)
✅ files (0 rows)
✅ shared_files (0 rows)
```

**Step 3:** Click on `users` table

**Step 4:** You should see columns:
- id
- email
- password_hash
- full_name
- storage_used_mb
- storage_limit_mb
- created_at
- is_admin

**✅ Database tables created successfully!**

---

### 5.7 Creating Storage Bucket

**What's a storage bucket?**
A bucket is like a big folder where all your uploaded files will be stored.

**Step 1:** Click **Storage** icon (📦) on left sidebar

**Step 2:** Click **"New bucket"** button

**Step 3:** Fill in bucket details:

```
┌─────────────────────────────────────────┐
│ Name: cloud-drive-files                 │
│                                         │
│ Public bucket: ☑ CHECKED (Important!)  │
│                                         │
│ File size limit: 50 MB (default)       │
│                                         │
│ Allowed MIME types: Leave empty        │
└─────────────────────────────────────────┘
```

**⚠️ CRITICAL:** Make sure "Public bucket" is **CHECKED**!

**Step 4:** Click **"Create bucket"**

**Step 5:** You should see `cloud-drive-files` in the bucket list

---

### 5.8 Setting Storage Policies

**What are policies?**
Policies control who can upload, download, and delete files. We'll set permissive policies for development.

**Step 1:** Click on `cloud-drive-files` bucket (from the list)

**Step 2:** Click **"Policies"** tab at the top

**Step 3:** You'll see "No policies created"

**Step 4:** Click **"New Policy"** button

**Step 5:** Click **"For full customization"**

**Step 6:** Fill in policy details:

```
┌──────────────────────────────────────────┐
│ Policy name: Allow all operations       │
│                                          │
│ Allowed operations:                      │
│   ☑ SELECT    (viewing files)           │
│   ☑ INSERT    (uploading files)         │
│   ☑ UPDATE    (modifying files)         │
│   ☑ DELETE    (deleting files)          │
│                                          │
│ Target roles:                            │
│   • public                               │
│   • anon                                 │
│   • authenticated                        │
│                                          │
│ Policy definition (SQL):                 │
│   true                                   │
│   (Just type: true)                      │
└──────────────────────────────────────────┘
```

**Step 7:** Click **"Save"**

**Step 8:** Verify policy created:
- You should see green checkmark ✅
- Policy name appears in list
- Status shows "Active"

**✅ Storage bucket configured!**

---

## 6. CONFIGURING THE APPLICATION

### 6.1 Opening Project in VS Code

**Step 1:** Open VS Code

**Step 2:** Click **File** → **Open Folder**

**Step 3:** Navigate to your Projects folder:
- Windows: `C:\Projects\PersonalCloudDrive`
- macOS/Linux: `~/Projects/PersonalCloudDrive`

**Step 4:** Click **"Select Folder"** or **"Open"**

**Step 5:** VS Code will load the project (may take 10-20 seconds)

**Step 6:** You'll see project files in left sidebar (Explorer panel)

---

### 6.2 Locating appsettings.json

**Step 1:** In VS Code Explorer (left sidebar), find: `appsettings.json`

**Step 2:** Click on it to open

**Step 3:** You'll see configuration template

---

### 6.3 Updating Configuration

**Step 1:** Find these lines in `appsettings.json`:

```json
"Supabase": {
  "Url": "YOUR_SUPABASE_URL",
  "Key": "YOUR_ANON_KEY",
  "BucketName": "cloud-drive-files"
}
```

**Step 2:** Open your `supabase-credentials.txt` file

**Step 3:** Copy your Project URL

**Step 4:** Replace `YOUR_SUPABASE_URL` with your actual URL:

**Before:**
```json
"Url": "YOUR_SUPABASE_URL",
```

**After:**
```json
"Url": "https://abcdefghijklmnop.supabase.co",
```

**⚠️ Important:** Keep the quotes `"` and comma `,`

**Step 5:** Copy your anon public key

**Step 6:** Replace `YOUR_ANON_KEY` with your actual key:

**Before:**
```json
"Key": "YOUR_ANON_KEY",
```

**After:**
```json
"Key": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImFiY2RlZmdoaWprbG1ub3AiLCJyb2xlIjoiYW5vbiIsImlhdCI6MTYzMzAyOTYwMCwiZXhwIjoxOTQ4NjA1NjAwfQ.xxxxx",
```

**Step 7:** Complete `appsettings.json` should look like:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Supabase": {
    "Url": "https://your-actual-project.supabase.co",
    "Key": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.your-actual-key-here",
    "BucketName": "cloud-drive-files"
  },
  "AppSettings": {
    "MaxStoragePerUserMB": 1024,
    "AllowedFileTypes": ".pdf,.jpg,.jpeg,.png,.gif,.doc,.docx,.xls,.xlsx,.zip,.rar,.txt,.mp4,.mp3"
  }
}
```

**Step 8:** Save the file
- Press `Ctrl+S` (Windows/Linux)
- Press `Cmd+S` (macOS)
- OR: File → Save

**✅ Configuration updated!**

---

## 7. BUILDING AND RUNNING

### 7.1 Opening Terminal in VS Code

**Step 1:** In VS Code, press `` Ctrl+` `` (backtick/tilde key)
- OR: Menu → View → Terminal
- OR: Terminal → New Terminal

**Step 2:** Terminal panel opens at bottom of VS Code

**Step 3:** Verify you're in project directory:
```bash
# You should see: C:\Projects\PersonalCloudDrive> (Windows)
# OR: ~/Projects/PersonalCloudDrive$ (macOS/Linux)
```

---

### 7.2 Restoring NuGet Packages

**What are NuGet packages?**
Like app stores for code libraries. We need to download required libraries.

**Step 1:** Run this command:
```bash
dotnet restore
```

**Step 2:** Wait for download (30 seconds - 2 minutes)

**Expected output:**
```
  Determining projects to restore...
  Restored C:\Projects\PersonalCloudDrive\PersonalCloudDrive.csproj (in 2.34 sec).
```

**Step 3:** Verify packages installed:
```bash
dotnet list package
```

**You should see:**
- Supabase (0.11.1 or higher)
- BCrypt.Net-Next
- Newtonsoft.Json

**✅ Packages restored!**

---

### 7.3 Building the Project

**Step 1:** Run build command:
```bash
dotnet build
```

**Step 2:** Wait for compilation (10-30 seconds)

**Expected SUCCESS output:**
```
Microsoft (R) Build Engine version 6.0.xxx
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  All projects are up-to-date for restore.
  PersonalCloudDrive -> C:\Projects\PersonalCloudDrive\bin\Debug\net6.0\PersonalCloudDrive.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:03.45
```

**✅ Build successful!**

**❌ If you see errors:**
- Read error messages carefully
- See Troubleshooting section below
- Common issues:
  - Wrong Supabase credentials
  - Missing files
  - Syntax errors in code

---

### 7.4 Running the Application

**Step 1:** Run this command:
```bash
dotnet run
```

**Step 2:** Wait for server to start (5-10 seconds)

**Expected output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Projects\PersonalCloudDrive
```

**🎉 YOUR APPLICATION IS NOW RUNNING!**

**⚠️ Don't close this terminal window!** The server is running here.

---

### 7.5 Opening in Browser

**Step 1:** Open your web browser (Chrome, Firefox, or Edge)

**Step 2:** Type in address bar:
```
https://localhost:5001
```

**Step 3:** Press Enter

**Step 4:** Security Warning (Expected!)
- Browser shows: "Your connection is not private" or similar
- This is NORMAL for local development
- **Click:** "Advanced" or "Show details"
- **Click:** "Proceed to localhost (unsafe)" or "Accept the risk"

**Step 5:** Page should load!

**You should see:**
- Beautiful dark gradient background
- "Your Personal Cloud Storage" heading
- "Get Started Free" button
- "Sign In" button
- Feature cards showing app capabilities

**✅ Application is live!**

---

## 8. TESTING YOUR INSTALLATION

### 8.1 Creating Your First Account

**Step 1:** On the landing page, click **"Get Started"** or **"Register"**

**Step 2:** Fill in the registration form:

```
Full Name:      Test User
Email:          test@example.com
Password:       Test@123456
Confirm Password: Test@123456
```

**⚠️ Password Requirements:**
- At least 8 characters
- Mix of letters and numbers recommended

**Step 3:** Click **"Create Account"** button

**Step 4:** Wait for processing (2-3 seconds)

**Expected Result:**
- Green success message: "Registration successful! Please login."
- Redirect to login page

**✅ Account created!**

**Verify in Supabase:**
1. Go to Supabase Dashboard
2. Click Table Editor → users
3. You should see 1 row with your email

---

### 8.2 Logging In

**Step 1:** On login page, enter:

```
Email:    test@example.com
Password: Test@123456
```

**Step 2:** (Optional) Check "Remember me" if desired

**Step 3:** Click **"Sign In"** button

**Step 4:** Wait for authentication (1-2 seconds)

**Expected Result:**
- Redirect to Dashboard
- See welcome message: "Welcome back, Test User! 👋"

**Dashboard should display:**
- Total Files: 0
- Total Folders: 0
- Storage Used: 0.00 MB of 1024 MB
- Storage progress bar (empty)
- "No files uploaded yet" message

**✅ Login successful!**

---

### 8.3 Uploading a Test File

**Step 1:** Click **"My Files"** or **"File Manager"** button

**Step 2:** Click **"Upload Files"** button (purple button)

**Step 3:** Modal dialog opens

**Step 4:** Click inside the upload area OR drag and drop a file

**Step 5:** Select a file from your computer
- Try a small file first (< 5 MB)
- Examples: an image (`.jpg`, `.png`),
