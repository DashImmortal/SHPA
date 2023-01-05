using SHPA.Entities.Models;

namespace SHPA.Data
{
    public static class Repository
	{
		public static List<Employee> Employees { get; set; } = new ();
		public static List<Record> Records { get; set; } = new ();
	}
}
