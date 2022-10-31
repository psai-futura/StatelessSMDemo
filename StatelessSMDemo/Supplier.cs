namespace StatelessSMDemo;

public class Supplier
{
    public int Id { get; set; }
    
    public string? Name { get; set; }
    
    public string Email { get; }

    public Supplier(int id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }
}