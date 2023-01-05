using SHPA.Entities.Models;

namespace SHPA.Data
{
	/// <summary>
	/// The application repository.
	/// </summary>
    public static class Repository
	{
		/// <summary>
		/// The employees in the company.
		/// </summary>
		public static List<Employee> Employees { get; set; } = new ();

		/// <summary>
		/// The recorded data of employees timestamps.
		/// </summary>
		public static List<Record> Records { get; set; } = new ();
	}
}
