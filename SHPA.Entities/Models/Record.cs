namespace SHPA.Entities.Models
{
    /// <summary>
    /// The definition of the record.
    /// </summary>
    /// <param name="Employee">
    /// The employee which creates the record.
    /// </param>
    /// <param name="DateTime">
    /// The timestamp of the record.
    /// </param>
    public record Record(Employee Employee, DateTime DateTime);
}
