using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookingApp.Models;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Configurăm proprietatea "Rol" pentru a fi salvată ca text în litere mici
        builder.Property(u => u.Rol)
            .HasConversion(
                v => v.ToString().ToLower(), // Convertim enum-ul în text cu litere mici la salvare
                v => (RolUtilizator)Enum.Parse(typeof(RolUtilizator), v, true)) // Convertim înapoi în enum la citire
            .HasDefaultValue(RolUtilizator.User) // Setăm valoarea implicită "User"
            .IsRequired(); // Asigurăm că valoarea este obligatorie

        // Asigurăm insensibilitatea la majuscule în baza de date
        builder.Property(u => u.Rol)
            .UseCollation("SQL_Latin1_General_CP1_CI_AS"); // Setăm collation pentru insensibilitate la case
    }
}
