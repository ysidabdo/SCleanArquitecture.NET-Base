using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
namespace Application.Config;
public static class DbConfig
{

    public static void AddConfiguration(this WebApplicationBuilder builder)
    {   

        builder.Services.AddTransient<PersonTableRepository>();
        builder.Services.AddTransient<IPersonRepository, PersonRepositoryBridge>();
        builder.Services.AddTransient<PersonService>();

        var connectionString = builder.Configuration.GetConnectionString("InMemoryConn");
        builder.Services.AddTransient<Func<IDbConnection>>(_ => () => new SqliteConnection(connectionString));

        InicializarBD(connectionString, builder); //Este metodo crea las tablas e inserta datos iniciales, no es responsabilidad del DbConfig, pero ayuda a su exploración

    }

    private static void InicializarBD(string connectionString,WebApplicationBuilder builder)
    {
        var root = new SqliteConnection(connectionString);
        root.Open();
        using (var cmd = root.CreateCommand())
        {
            cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS persons (
                        Id INTEGER PRIMARY KEY,
                        FirstName TEXT NOT NULL,
                        LastName TEXT NOT NULL,
                        Age INTEGER NOT NULL
                    );
                ";
            cmd.ExecuteNonQuery();
        }

        using (var cmd = root.CreateCommand())
        {
            cmd.CommandText = @"
                    INSERT INTO persons (FirstName, LastName, Age)
                    VALUES
                        ('Ada', 'Lovelace', 36),
                        ('Alan', 'Turing', 41),
                        ('Grace', 'Hopper', 85);
                ";
            try { cmd.ExecuteNonQuery(); } catch { /* ignore duplicates on rerun */ }
        }
        builder.Services.AddSingleton<IDbConnection>(root);
    }

    
}