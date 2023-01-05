using SHPA.Entities;
using SHPA.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace SHPA.App.Utils
{
	public static class Extensions
	{
		
		public static bool ExportToCsv(this List<Record> records, string outputPath) => Export.CsvTools.ExportDataTableToCsv(records.ToDataTable(), outputPath);
		public static bool ExportToCsv(this DataTable dataTable, string outputPath) => Export.CsvTools.ExportDataTableToCsv(dataTable, outputPath);

		public static DataTable ToDataTable(this List<Record> records)
		{
			var dailyRecords = records.GroupBy(rec => rec.DateTime.Date);
			var employees = new List<Employee>();

			foreach (var rec in records)
			{
				var employee = employees.FirstOrDefault(employee => employee.Id == rec.Employee.Id);
				if (employee == null)
					employees.Add(employee = new Employee(rec.Employee.Id, rec.Employee.Name));
				
				employee.Records.Add(rec);
			}
			var persianCalendar = new PersianCalendar();
			var exportTable = new DataTable("گزارش")
			{
				Columns =
				{
					new DataColumn("ردیف", typeof(uint)),
					new DataColumn("تاریخ", typeof(string)),
					new DataColumn("روز", typeof(string)),
					new DataColumn("نام فرد", typeof(string)),
					new DataColumn("نوع", typeof(string)),
					new DataColumn("کارکرد", typeof(string)),
					new DataColumn("اولین ورود", typeof(string)),
					new DataColumn("آخرین خروج", typeof(string)),
					new DataColumn("رکورد ها", typeof(string))
				}
			};
			var index = 1;
			foreach (var recordsOfDay in dailyRecords)
			{
				foreach (var employee in employees)
				{
					var exportRecord = exportTable.NewRow();
					var employeeDailyRecords = employee.Records.Where(rec => rec.DateTime.Date == recordsOfDay.Key.Date)
						.OrderBy(rec => rec.DateTime).ToList();
					exportRecord[0] = index++;
					exportRecord[1] =
						$"{persianCalendar.GetYear(recordsOfDay.Key)}/{persianCalendar.GetMonth(recordsOfDay.Key):00}/{persianCalendar.GetDayOfMonth(recordsOfDay.Key):00}";
					exportRecord[2] = persianCalendar.GetDayOfWeek(recordsOfDay.Key).GetPersianDayOfWeek();
					exportRecord[3] = employee.Name;

					WorkType workType;
					var workTime = TimeSpan.Zero;

					if (!employeeDailyRecords.Any())
						workType = WorkType.Off;
					else if (employeeDailyRecords.Count % 2 == 1)
						workType = WorkType.Error;
					else
					{
						for (var i = 1; i < employeeDailyRecords.Count; i += 2)
							workTime = workTime.Add(employeeDailyRecords[i].DateTime.TimeOfDay.StripSeconds()
								.Add(-employeeDailyRecords[i - 1].DateTime.TimeOfDay.StripSeconds()));

						// This condition changed due to setting work type as late if the employee comes to work before 8:30! so now it counts it as normal
						// Finishing work after the specified time counts as late

						//if (!(employeeDailyRecords.First().DateTime.TimeOfDay > Rules.MinimumArrivalTime.ToTimeSpan() &&
						//	 employeeDailyRecords.First().DateTime.TimeOfDay < Rules.MaximumArrivalTime.ToTimeSpan()) ||
						//	!(employeeDailyRecords.Last().DateTime.TimeOfDay > Rules.MinimumLeavingTime.ToTimeSpan() &&
						//	  employeeDailyRecords.Last().DateTime.TimeOfDay < Rules.MaximumLeavingTime.ToTimeSpan()))

						if (!(employeeDailyRecords.First().DateTime.TimeOfDay < Rules.MaximumArrivalTime.ToTimeSpan()) ||
							!(employeeDailyRecords.Last().DateTime.TimeOfDay > Rules.MinimumLeavingTime.ToTimeSpan() &&
							  employeeDailyRecords.Last().DateTime.TimeOfDay < Rules.MaximumLeavingTime.ToTimeSpan()))
							workType = WorkType.Late;
						else if (workTime < Rules.WorkTime)
							workType = employeeDailyRecords.Count > 2 ? WorkType.Break : WorkType.Late;

						else
							workType = WorkType.Normal;
					}
					exportRecord[4] = GetPersianWorkType(workType);
					exportRecord[5] = workTime == TimeSpan.Zero ? "-" : TimeOnly.FromTimeSpan(workTime).ToString("H:mm");
					exportRecord[6] = workType == WorkType.Off || employeeDailyRecords.Count % 2 == 1
						? "-"
						: TimeOnly.FromTimeSpan(employeeDailyRecords.First().DateTime.TimeOfDay).ToString("H:mm");

					exportRecord[7] = workType == WorkType.Off || employeeDailyRecords.Count % 2 == 1
						? "-"
						: TimeOnly.FromTimeSpan(employeeDailyRecords.Last().DateTime.TimeOfDay).ToString("H:mm");
					var timeRecords = string.Join("\n",
							employeeDailyRecords.Select(rec =>
								TimeOnly.FromTimeSpan(rec.DateTime.TimeOfDay).ToString("H:mm")));
					exportRecord[8] = timeRecords == string.Empty ? "-" : timeRecords;

					exportTable.Rows.Add(exportRecord);
				}
			}

			return exportTable;
		}

		public static string GetPersianDayOfWeek(this DayOfWeek date)
		{
			switch (date)
			{
				case DayOfWeek.Saturday:
					return "شنبه";
				case DayOfWeek.Sunday:
					return "یکشنبه";
				case DayOfWeek.Monday:
					return "دوشنبه";
				case DayOfWeek.Tuesday:
					return "سه‌شنبه";
				case DayOfWeek.Wednesday:
					return "چهارشنبه";
				case DayOfWeek.Thursday:
					return "پنجشنبه";
				case DayOfWeek.Friday:
					return "جمعه";
				default:
					return "نامشخص";

			}
		}
		public static string GetPersianWorkType(this WorkType type)
		{
			switch (type)
			{
				case WorkType.Off:
					return "مرخصی روزانه";
				case WorkType.Break:
					return "مرخصی ساعتی";
				case WorkType.Late:
					return "تاخیر";
				case WorkType.Normal:
					return "عادی";
				case WorkType.Error:
				default:
					return "خطا";

			}
		}
		public static TimeSpan StripSeconds(this TimeSpan time) => new(time.Hours, time.Minutes, 0);
		

	}
}
