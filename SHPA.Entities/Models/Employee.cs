namespace SHPA.Entities.Models
{
    /// <summary>
    /// The class representing an employee.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// The employee's Id
        /// </summary>
        public ulong Id { get; init; }

        /// <summary>
        /// The employee's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The constructor for the employee.
        /// </summary>
        /// <param name="id">
        /// The unique Id for the employee.
        /// </param>
        /// <param name="name">
        /// The name of the employee.
        /// </param>
        public Employee(ulong id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// All the records of the employee.
        /// </summary>
        public virtual List<Record> Records { get; set; } = new();
    }
}
