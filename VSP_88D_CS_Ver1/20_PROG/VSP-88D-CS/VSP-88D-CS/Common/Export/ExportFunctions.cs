using System.IO;
using System.Text;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using Newtonsoft.Json;
namespace VSP_88D_CS.Common.Export
{
    public class ExportFunctions
    {
        public static void ExportExcel<T>(List<T> data, string sheetName = "SheetDefault")
        {
            if (data == null || data.Count == 0)
            {
                return;
            }
            try
            {
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add(sheetName);
                var properties = typeof(T).GetProperties();
                //header
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = properties[i].Name;
                }
                //row data
                for (int row = 0; row < data.Count; row++)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        worksheet.Cell(row + 2, col + 1).SetValue(properties[col].GetValue(data[row])?.ToString() ?? "null");
                    }
                }
                worksheet.Columns().AdjustToContents();
                var usedRange = worksheet.RangeUsed();
                if (usedRange != null)
                {
                    usedRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // save file
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Location Excel",
                    Filter = "Excel Files|*.xlsx"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    workbook.SaveAs(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {


            }

        }

        public static void SaveDataGridToText<T>(IEnumerable<T> items, string filePath, bool includeHeader)
        {
            var properties = typeof(T).GetProperties();

            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.ASCII))
            {
                if (includeHeader)
                {
                    sw.WriteLine(string.Join("\t", properties.Select(p => p.Name)));
                }

                foreach (var item in items)
                {
                    var values = properties.Select(p => p.GetValue(item)?.ToString() ?? "");
                    sw.WriteLine(string.Join("\t", values));
                }
            }
        }
        public static void SaveDataGridToAlignedText<T>(IEnumerable<T> items, string filePath, bool includeHeader)
        {
            var props = typeof(T).GetProperties();
            var colWidths = new int[props.Length];
    
            for (int i = 0; i < props.Length; i++)
            {
                colWidths[i] = props[i].Name.Length;
                foreach (var item in items)
                {
                    var value = props[i].GetValue(item)?.ToString() ?? "";
                    colWidths[i] = Math.Max(colWidths[i], value.Length);
                }
            }
            // save file
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Text Location",
                Filter = "Text Files|*.txt"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                filePath = saveFileDialog.FileName;
                using (var writer = new StreamWriter(filePath))
                {
                    if (includeHeader)
                    {
                        for (int i = 0; i < props.Length; i++)
                            writer.Write(props[i].Name.PadRight(colWidths[i] + 2));
                        writer.WriteLine();
                    }

                    foreach (var item in items)
                    {
                        for (int i = 0; i < props.Length; i++)
                        {
                            var value = props[i].GetValue(item)?.ToString() ?? "";
                            writer.Write(value.PadRight(colWidths[i] + 2));
                        }
                        writer.WriteLine();
                    }
                }
            }
        }
    }
}
