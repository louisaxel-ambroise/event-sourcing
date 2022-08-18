namespace EventSourcing.MVP.Domain.Shared;

public class SharedData
{
    public Store LoadStoreByName(string name)
    {
        return new ()
        {
            Id = 1,
            Name = name
        };
    }
}

public class Store
{
    public int Id { get; set; }
    public string Name { get; set; }
}
