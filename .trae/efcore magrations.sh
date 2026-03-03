dotnet ef migrations add InitialAuditSchema --context AuditDbContext --output-dir Migrations
dotnet ef database update --context AuditDbContext