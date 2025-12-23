using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

public static class DbConfig
{
    private const string InMemoryConn = "Data Source=InMemoryDb;Mode=Memory;Cache=Shared";

    public static void AddSQLConnection(this WebApplicationBuilder builder)
    {
        var useInMemory = builder.Configuration.GetValue<bool>("UseInMemorySqlite");

        if (useInMemory)
        {
            var root = new SqliteConnection(InMemoryConn);
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

            builder.Services.AddSingleton(root); // keep DB alive
            builder.Services.AddTransient<Func<IDbConnection>>(_ => () => new SqliteConnection(InMemoryConn));
        }
        else
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddTransient<Func<IDbConnection>>(_ => () => new SqlConnection(connectionString));
        }

        builder.Services.AddTransient<PersonTableRepository>();
        builder.Services.AddTransient<IPersonRepository, PersonRepositoryBridge>();
        builder.Services.AddTransient<PersonService>();
    }
}