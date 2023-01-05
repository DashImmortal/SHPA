using System;

namespace SHPA.App;

public static class Rules
{
	public static TimeSpan WorkTime { get; set; } = new(8, 30, 0);
	public static TimeOnly MinimumArrivalTime { get; set; } = new(8, 30);
	public static TimeOnly MaximumArrivalTime { get; set; } = new(8, 46);
	public static TimeOnly MinimumLeavingTime { get; set; } = new(17, 0);
	public static TimeOnly MaximumLeavingTime { get; set; } = new(17, 16);
}