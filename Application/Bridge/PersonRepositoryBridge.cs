using Domain.Entities;

public class PersonRepositoryBridge : IPersonRepository
{
    private readonly PersonTableRepository _personTableRepository;

    public PersonRepositoryBridge(PersonTableRepository personTableRepository) 
    {
        _personTableRepository = personTableRepository;
    }

    public Person GetPersonById(int id)
    {
        var whereClause = $"Id = {id}";
        var rows = _personTableRepository.GetAllPersonsWhere(whereClause);
        var first = rows.Cast<dynamic>().FirstOrDefault();
        if (first == null)
            throw new KeyNotFoundException($"Person with id {id} not found");

        string firstName = first.FirstName;
        string lastName = first.LastName;
        int age = (int)first.Age;
        return new Person(firstName, lastName, age);
        
    }

    public void AddPerson(Person person)
    {
        throw new NotImplementedException();
    }

    public void DeletePerson(int id)
    {
        throw new NotImplementedException();
    }

    public void UpdatePerson(Person person)
    {
        throw new NotImplementedException();
    }
}