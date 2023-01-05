using System.Data;
using System.Text;

namespace SHPA.Export
{
	/// <summary>
	/// The class which contains the methods to work with csv files.
	/// </summary>
	public static class CsvTools
	{
		/// <summary>
		/// Exports a data table to a csv file in the selected directory.
		/// </summary>
		/// <param name="exportTable">
		/// the data table to be exported.
		/// </param>
		/// <param name="outputPath">
		/// the directory and the file name of the csv file.
		/// </param>
		/// <returns>True if the operation was successful and false tif the operation failed</returns>
		public static bool ExportDataTableToCsv(DataTable exportTable, string outputPath)
		{ 
			var writer = new StreamWriter(new FileStream(outputPath, FileMode.Create), Encoding.Unicode);
			try
			{
				for (var i = 0; i < exportTable.Columns.Count; i++)
				{
					writer.Write(exportTable.Columns[i]);
					if (i < exportTable.Columns.Count - 1)
						writer.Write('\t');
				}
				writer.Write(writer.NewLine);
				foreach (DataRow dataRow in exportTable.Rows)
				{
					for (var i = 0; i < exportTable.Columns.Count; i++)
					{

						if (!Convert.IsDBNull(dataRow[i]))
						{
							var value = dataRow[i].ToString() ?? "";
							if (value.Contains('\t') || value.Contains('\n'))
								writer.Write($"\"{value}\"");
							else
								writer.Write(dataRow[i].ToString());

						}
						if (i < exportTable.Columns.Count - 1)
							writer.Write('\t');
					}
					writer.Write(writer.NewLine);
				}
				writer.Close();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
