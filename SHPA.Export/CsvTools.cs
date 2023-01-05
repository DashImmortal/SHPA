using System.Data;
using System.Text;

namespace SHPA.Export
{
	public static class CsvTools
	{
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
