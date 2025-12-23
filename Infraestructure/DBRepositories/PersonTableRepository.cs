using System.Data;
using Dapper;

public class PersonTableRepository
{
    private readonly Func<IDbConnection> _connectionFactory;

    public PersonTableRepository(Func<IDbConnection> connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public virtual IEnumerable<dynamic> GetAllPersonsWhere(string whereClause)
    {
        var sql = $"SELECT Id, FirstName, LastName, Age FROM persons WHERE {whereClause}";
        using var connection = _connectionFactory();
        connection.Open();
        return connection.Query(sql);
    }
}