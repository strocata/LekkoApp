# LekkoApp - Productivity Management Application

![LekkoApp1](LekkoApp/wwwroot/LekkoApp1.png)
![LekkoApp2](LekkoApp/wwwroot/LekkoApp2.png)
![LekkoApp3](LekkoApp/wwwroot/LekkoApp3.png)
![LekkoApp4](LekkoApp/wwwroot/LekkoApp4.png)
![LekkoApp5](LekkoApp/wwwroot/LekkoApp5.png)
![LekkoApp6](LekkoApp/wwwroot/LekkoApp6.png)

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![License](https://img.shields.io/badge/license-MIT-blue)
![Status](https://img.shields.io/badge/status-active-success)

A comprehensive ToDo list application with integrated Pomodoro Timer, built with ASP.NET Core MVC and Entity Framework Core.

## 🌟 Features

### Task Management
- ✅ Create, edit, and delete tasks with rich metadata
- 📊 Priority levels (Low, Medium, High)
- 🎯 Status tracking (Not Started, In Progress, Completed)
- 📅 Due dates with overdue tracking
- 🔄 Recurring tasks (Daily, Weekly, Monthly)
- 🏷️ Tag support for categorization
- 📦 Organize tasks into projects
- 🌲 Subtask support (parent-child relationships)

### Pomodoro Timer
- ⏱️ Integrated timer for focused work sessions
- 📈 Session tracking linked to tasks
- 📊 Automatic pomodoro completion counting
- 📉 Historical session analytics

### Dashboard & Analytics
- 📈 Real-time task statistics
- 📊 Pomodoro productivity charts
- 🎯 Task distribution by status
- ⚠️ Overdue task alerts
- 📉 Weekly trend analysis

### Security
- 🔐 ASP.NET Core Identity authentication
- 🛡️ Secure password policies
- 🔒 Account lockout protection
- 👤 Per-user data isolation
- 🍪 Secure cookie management

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server) or [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/strocata/LekkoApp.git
   cd LekkoApp
   ```

2. **Configure Database Connection**

   **For Development (Recommended):**
   ```bash
   cd LekkoApp
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=LekkoAppDb;Integrated Security=true;TrustServerCertificate=True;"
   ```

   **Alternative:** Edit `appsettings.json` (not recommended for credentials)
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=LekkoAppDb;Integrated Security=true;TrustServerCertificate=True;"
     }
   }
   ```

3. **Restore NuGet Packages**
   ```bash
   dotnet restore
   ```

4. **Apply Database Migrations**
   ```bash
   dotnet ef database update
   ```

5. **Install Frontend Dependencies**
   ```bash
   npm install
   ```

6. **Run the Application**
   ```bash
   dotnet run
   ```

7. **Access the Application**

   Open your browser and navigate to: `https://localhost:5001`

## 📁 Project Structure

```
LekkoApp/
├── Controllers/          # MVC Controllers
│   ├── TasksController.cs
│   ├── ProjectsController.cs
│   ├── DashboardController.cs
│   └── TimerController.cs
├── Models/               # Domain models
│   ├── Task.cs
│   ├── Project.cs
│   ├── PomodoroSession.cs
│   └── Enums/
├── Views/                # Razor views
│   ├── Tasks/
│   ├── Projects/
│   ├── Dashboard/
│   └── Shared/
├── Repositories/         # Data access layer
│   ├── TaskRepository.cs
│   └── ProjectRepository.cs
├── Data/                 # DbContext and Identity
│   ├── ApplicationDbContext.cs
│   └── ApplicationUser.cs
├── Migrations/           # EF Core migrations
├── wwwroot/              # Static files (CSS, JS, images)
└── Program.cs            # Application entry point
```

## 🗄️ Database Schema

### Core Tables

- **Tasks** - Main task entity with status, priority, due dates
- **Projects** - Project grouping for tasks
- **PomodoroSessions** - Timer session tracking
- **Tags** - Task categorization
- **AspNetUsers** - User accounts (Identity)

### Relationships

- User → Tasks (one-to-many)
- User → Projects (one-to-many)
- Project → Tasks (one-to-many)
- Task → PomodoroSessions (one-to-many)
- Task → Tags (many-to-many)
- Task → Subtasks (self-referencing)

## 🛠️ Development

### Running Tests

```bash
cd LekkoApp.Tests
dotnet test
```

### Creating Migrations

```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Building for Production

```bash
dotnet publish -c Release -o ./publish
```

## 🔧 Configuration

### User Secrets (Development)

Store sensitive configuration securely:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.gmail.com"
```

### Environment Variables (Production)

```bash
export ConnectionStrings__DefaultConnection="Server=prod-server;..."
export ASPNETCORE_ENVIRONMENT=Production
```

### Application Settings

Key configuration options in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "yourdomain.com"
}
```

## 🔐 Security Best Practices

### Implemented Security Features

- ✅ ASP.NET Core Identity with email confirmation
- ✅ Strong password requirements (8+ chars, mixed case, special chars)
- ✅ Account lockout after failed login attempts
- ✅ Anti-forgery tokens on all forms
- ✅ HTTPS enforcement
- ✅ Secure cookie settings (HttpOnly, Secure, SameSite)
- ✅ User secrets for sensitive data
- ✅ SQL injection protection via EF Core parameterization
- ✅ XSS protection via Razor encoding

### Security Checklist for Production

- [ ] Rotate all credentials and API keys
- [ ] Enable HTTPS with valid SSL certificate
- [ ] Configure allowed hosts in `appsettings.json`
- [ ] Enable audit logging
- [ ] Set up database backups
- [ ] Configure rate limiting
- [ ] Enable CORS policies appropriately
- [ ] Use Azure Key Vault or AWS Secrets Manager for secrets

## 🚀 Deployment

### Azure App Service

1. Create App Service and SQL Database
2. Configure connection strings in App Service Configuration
3. Deploy via GitHub Actions or Azure DevOps

```yaml
# .github/workflows/deploy.yml
name: Deploy to Azure
on:
  push:
    branches: [ main ]
jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: 8.0.x
      - name: Build
        run: dotnet build --configuration Release
      - name: Publish
        run: dotnet publish -c Release -o ./publish
      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: lekkoapp
          publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
          package: ./publish
```

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LekkoApp/LekkoApp.csproj", "LekkoApp/"]
RUN dotnet restore "LekkoApp/LekkoApp.csproj"
COPY . .
WORKDIR "/src/LekkoApp"
RUN dotnet build "LekkoApp.csproj" -c Release -o /app/build
RUN dotnet publish "LekkoApp.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LekkoApp.dll"]
```

## 📊 Performance Optimization

### Implemented Optimizations

- ✅ Eager loading for related entities (`.Include()`)
- ✅ Pagination for large result sets
- ✅ Memory caching for dashboard statistics
- ✅ Async/await throughout
- ✅ Database indexes on frequently queried columns
- ✅ Connection pooling
- ✅ Query optimization with `AsNoTracking()`

### Recommended Production Improvements

- [ ] Add Redis distributed caching
- [ ] Implement CDN for static assets
- [ ] Enable response compression
- [ ] Add database query profiling
- [ ] Implement lazy loading where appropriate
- [ ] Use compiled queries for hot paths

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Coding Standards

- Follow [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Write unit tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting PR

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Authors

- **strocata** - *Initial work* - [GitHub](https://github.com/strocata)

## 🙏 Acknowledgments

- [Tabler](https://tabler.io/) - UI components
- [Tailwind CSS](https://tailwindcss.com/) - Styling framework
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/) - Web framework
- [Entity Framework Core](https://docs.microsoft.com/ef/core/) - ORM

## 📞 Support

For support, email taranovskypasha@icloud.com or open an issue in the repository.

## 🗺️ Roadmap

### Version 1.1
- [ ] Email notifications for due dates
- [ ] Task export/import (CSV, JSON)
- [ ] Dark mode support
- [ ] Mobile responsive improvements

### Version 2.0
- [ ] REST API for integrations
- [ ] Real-time updates with SignalR
- [ ] Team collaboration features
- [ ] Task templates
- [ ] Advanced reporting

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
- [Pomodoro Technique](https://en.wikipedia.org/wiki/Pomodoro_Technique)

---

Made with ❤️ by strocata