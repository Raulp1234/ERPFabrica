

dotnet ef migrations add InitDB --project ERPFabrica.Infrastructure --startup-project ERPFabrica.WebApi


dotnet ef database update --project ERPFabrica.Infrastructure --startup-project ERPFabrica.WebApi
