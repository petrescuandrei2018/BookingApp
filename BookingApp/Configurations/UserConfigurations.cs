using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookingApp.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Creăm un ValueConverter pentru conversie simplificată
        var rolConverter = new ValueConverter<RolUtilizator, string>(
            v => v.ToString().ToLower(), // La salvare: Enum -> string cu litere mici
            v => ConvertToEnum(v) // La citire: String -> Enum (validare externă)
        );

        // Configurăm proprietatea "Rol"
        builder.Property(u => u.Rol)
            .HasConversion(rolConverter) // Aplicăm converter-ul creat
            .UseCollation("SQL_Latin1_General_CP1_CI_AS") // Asigurăm insensibilitatea la litere mari/mici
            .HasDefaultValue(RolUtilizator.User) // Setăm valoarea implicită "User"
            .IsRequired(); // Asigurăm că valoarea este obligatorie
    }

    // Metodă statică pentru conversia string -> Enum
    private static RolUtilizator ConvertToEnum(string value)
    {
        if (Enum.TryParse<RolUtilizator>(value, true, out var result))
        {
            return result;
        }
        throw new InvalidOperationException($"Valoarea '{value}' nu este validă pentru RolUtilizator.");
    }
}
