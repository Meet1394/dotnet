# Personal Cloud Drive

A secure cloud storage solution built with ASP.NET Core and Supabase, providing a user-friendly interface for file management and storage.

## Features

- 📁 File and folder management
- 🔒 Secure authentication
- 📊 Storage usage tracking
- 🔍 File search functionality
- 📱 Responsive design
- ⚡ Fast upload and download

## Technology Stack

- ASP.NET Core 9.0
- Supabase for storage and authentication
- Bootstrap for UI
- JavaScript for interactive features

## Setup

1. Clone the repository
```bash
git clone https://github.com/Meet1394/dotnet.git
cd PersonalCloudDrive
```

2. Configure Supabase
- Create a new Supabase project
- Set up storage bucket named "cloud-drive-files"
- Configure RLS policies for files and storage
- Update `appsettings.json` with your Supabase credentials

3. Run the application
```bash
dotnet restore
dotnet run
```

4. Access the application at `https://localhost:5276`

## Configuration

Update `appsettings.json` with your Supabase configuration:

```json
{
  "Supabase": {
    "Url": "YOUR_SUPABASE_URL",
    "Key": "YOUR_SUPABASE_KEY",
    "BucketName": "cloud-drive-files"
  }
}
```

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.