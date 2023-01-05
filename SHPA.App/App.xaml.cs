using Microsoft.Win32;
using SHPA.App.Utils;
using SHPA.Data;
using SHPA.Entities.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace SHPA.App
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public record InputRecord(ulong Id, string Name, DateTime DateTime);
		public App()
		{
			MessageBox.Show("Please select the inputs text file!");
			//var inputRecords = ImportInputRecords();
			if (ImportInputRecords(out var inputRecords))
			{
				ImportInputRecordsToRepository(inputRecords);
				MessageBox.Show("Please enter the path to export the csv file!");
				ExportRepositoryRecordsToCsv();
			}
			Current.Shutdown();
		}

		public bool ImportInputRecords(out List<InputRecord> records)
		{
			records = new List<InputRecord>();
			var dialog = new OpenFileDialog
			{
				DefaultExt = ".txt",
				Filter = "Text documents (.txt)|*.txt",
			};
			var res = dialog.ShowDialog();
			if (!res.HasValue || !res.Value)
			{
				return false;
			}
			if (!File.Exists(dialog.FileName))
			{
				MessageBox.Show("Error while opening the file!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			var input = File.ReadAllLines(dialog.FileName);
			var inputRecords = new List<InputRecord>();
			for (var i = 0; i < input.Length; i++)
			{
				var record = input[i];
				var properties = record.Split('\t');

				if (properties.Length != 3)
				{
					MessageBox.Show(
						$"Record at the line {i + 1} is not in the correct format!\nThe correct format for each record is \"Id\tName\tDate  Time\".",
						"Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
					return false;
				}

				if (!ulong.TryParse(properties[0].Trim(), out var id))
				{
					MessageBox.Show(
						$"The Id of the record at the line {i + 1} is not in the correct format!\nIt has to be a number.",
						"Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
					return false;
				}
				var name = properties[1].Trim();
				if (!DateTime.TryParseExact(properties[2].Trim(), "yyyy/MM/dd  HH:mm:ss", CultureInfo.InvariantCulture,
						DateTimeStyles.None, out var dateTime))
				{
					MessageBox.Show(
						$"The date and time of the record at the line {i + 1} is not in the correct format!\nThe correct format for the time is \"yyyy/MM/dd  HH:mm:ss\".",
						"Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
					return false;
				}
				var inputRecord = new InputRecord(id, name, dateTime);
				inputRecords.Add(inputRecord);
			}
			records = inputRecords;
			return true;
		}
		public void ImportInputRecordsToRepository(List<InputRecord> inputRecords)
		{

			foreach (var inputRecord in inputRecords)
			{
				var employee = Repository.Employees.FirstOrDefault(employee => employee.Id == inputRecord.Id);
				if (employee == null)
				{
					employee = new Employee(inputRecord.Id, inputRecord.Name);
					Repository.Employees.Add(employee);
				}

				if (employee.Name != inputRecord.Name && MessageBox.Show(
						$"The name of the employee with the id {employee.Id} has recorded as \"{inputRecord.Name}\" instead of \"{employee.Name}\" at {inputRecord.DateTime:yyyy/MM/dd}!\nDo you wish to change the employee's name to \"{inputRecord.Name}\"?",
						"Unrecognized Name", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) ==
					MessageBoxResult.Yes)
					employee.Name = inputRecord.Name;

				var record = new Record(employee, inputRecord.DateTime);
				Repository.Records.Add(record);
				employee.Records.Add(record);
			}
		}

		public void ExportRepositoryRecordsToCsv()
		{

			var saveFileDialog = new SaveFileDialog
			{
				DefaultExt = ".csv",
				Filter = "CSV documents (.csv)|*.csv",
				FileName = "Report.csv",
				CheckPathExists = true
			};
			var success = saveFileDialog.ShowDialog();
			if (!success.HasValue || !success.Value)
				return;
			
			if (!Repository.Records.ExportToCsv(saveFileDialog.FileName))
			{
				MessageBox.Show($"Could not create the file {saveFileDialog.SafeFileName}!\n The file might be in use.",
					"Error while creating the file", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (Repository.Records.ExportToCsv(saveFileDialog.FileName))
			{

				new Process
				{
					StartInfo = new ProcessStartInfo(saveFileDialog.FileName)
					{
						UseShellExecute = true
					}
				}.Start();
			}
		}
	}
}
