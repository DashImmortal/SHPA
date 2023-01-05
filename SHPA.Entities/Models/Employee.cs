namespace SHPA.Entities.Models
{
    public class Employee
    {
        public ulong Id { get; init; }
        public string Name { get; set; }

        public Employee(ulong id, string name)
        {
            Id = id;
            Name = name;
        }

        public virtual List<Record> Records { get; set; } = new();
    }
}
