using Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationContext context, IConfiguration configuration)
    {
        // Si ya existe algún Superadmin no creamos uno nuevo 
        if (context.Superadmins.Any())
            return;

        var superAdminSection = configuration.GetSection("SuperAdmin");
        var email = superAdminSection["Email"];
        var password = superAdminSection["Password"];

        // Si no hay credenciales no se crea nada
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        // Usamos el constructor seguro de la entidad (Encapsulamiento POO)
        var superadmin = new Superadmin(email.Trim().ToLowerInvariant(), password);

        context.Superadmins.Add(superadmin);
        context.SaveChanges();
    }
}