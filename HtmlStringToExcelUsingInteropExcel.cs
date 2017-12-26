using System;
using System.Collections.Generic;
using System.Text;
using ExcelInterop = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Runtime.InteropServices;

namespace Components.Excel
{
    public static class ExcelHandle
    {
        public static byte[] DoConvertXlDataToOpenXml(string htmlString, string saveFileName, string tempBasePath)
        {
            DirectoryInfo dir = new DirectoryInfo(tempBasePath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            byte[] data = Encoding.UTF8.GetBytes(htmlString);
            ExcelInterop.Application excelApp = null;
            ExcelInterop.Workbooks workBooks = null;
            ExcelInterop.Workbook workBook = null;
            FileInfo tempFile = null;
            FileInfo convertedTempFile = null;
            try
            {
                var guid = Guid.NewGuid();
                var tempFileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-") + guid.ToString();
                //Stream the file to temporary location, overwrite if exists
                tempFile = new FileInfo(Path.Combine(tempBasePath, tempFileName + ".html"));

                using (var destStream = new FileStream(tempFile.FullName, FileMode.Create, FileAccess.Write))
                {
                    destStream.Write(data, 0, data.Length);
                }
                //解决Excel.exe进程无法退出问题 Never use two dots with COM objects.like excelApp.Workbooks.Open(...);
                //open original
                excelApp = new ExcelInterop.Application();
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;

                workBooks = excelApp.Workbooks;

                workBook = workBooks.Open(tempFile.FullName);

                convertedTempFile = new FileInfo(Path.Combine(tempBasePath, tempFileName + ".xls"));

                //Save as XLSX
                workBook.SaveAs(
                     convertedTempFile.FullName
                     , Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel8
                     , ConflictResolution: ExcelInterop.XlSaveConflictResolution.xlLocalSessionChanges);

                workBook.Close();

                return File.ReadAllBytes(convertedTempFile.FullName);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (workBooks != null)
                    Marshal.ReleaseComObject(workBooks);

                if (workBook != null)
                    Marshal.ReleaseComObject(workBook);

                if (excelApp != null)
                    Marshal.ReleaseComObject(excelApp);

                if (tempFile != null && tempFile.Exists)
                    tempFile.Delete();

                if (convertedTempFile != null && convertedTempFile.Exists)
                {
                    convertedTempFile.Delete();
                }
            }
        }

    }
}
