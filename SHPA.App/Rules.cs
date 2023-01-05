using System;

namespace SHPA.App;

public static class Rules
{
	/// <summary>
	/// The expected work time for each employee.
	/// </summary>
	public static TimeSpan WorkTime { get; set; } = new(8, 30, 0);

	/// <summary>
	/// The minimum expected time of arrival for each employee.
	/// </summary>
	public static TimeOnly MinimumArrivalTime { get; set; } = new(8, 30);

	/// <summary>
	/// The maximum expected time of arrival for each employee.
	/// </summary>
	public static TimeOnly MaximumArrivalTime { get; set; } = new(8, 46);

	/// <summary>
	/// The minimum expected time of leaving for each employee.
	/// </summary>
	public static TimeOnly MinimumLeavingTime { get; set; } = new(17, 0);

	/// <summary>
	/// The maximum expected time of leaving for each employee.
	/// </summary>
	public static TimeOnly MaximumLeavingTime { get; set; } = new(17, 16);
}