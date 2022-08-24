using NUnit.Framework;
using FestpunktDB.Business.ImportService;
using System.IO;
using System.Collections.Generic;
using FestpunktDB.Business;

namespace FestpunktDB.Tests
{
    public class ImportTests
    {
        // Vorbereitung der Variablen(Datentypen) fur Tests
        public static System.Data.DataTable dataTableforImportTestDbb = new System.Data.DataTable();
        public static System.Data.DataTable dataTableforImportTestNap = new System.Data.DataTable();
        public static System.Data.DataTable dataTableforImportTestCsv = new System.Data.DataTable();
        public static System.Data.DataTable dataTableforImportTestExcel = new System.Data.DataTable();
        public static System.Data.DataTable dataTableforImportTestExcelCsv = new System.Data.DataTable();
        public static System.Data.DataTable dataTableforImportTestCsvExcel = new System.Data.DataTable();
        List<string> testFileName = new List<string>();
        List<string> sketchName = new List<string>();
        public System.Data.DataTable dataTableForSkizzen = new System.Data.DataTable();
        // Variablen fur PPTX Test (Skizze)
        List<string> testFileNamePPT = new List<string>();
        List<string> sketchNamePPT = new List<string>();
        public System.Data.DataTable dataTableForSkizzenPPT = new System.Data.DataTable();
        readonly string projectDirectory = Path.GetFullPath(@"..\..\..\..\");

        [Test]
        public void TestDbbFormatCount()
        {

            //string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string dbbFileName = "FPF_DBB.dbb";
            // als Parameter ubergeben DataTable dataTableForImport, string filename
            System.Data.DataTable test_input = Import.ImportDbbFiles(dataTableforImportTestDbb, Path.Combine(projectDirectory, dbbFileName));
            Assert.IsTrue(test_input.Rows.Count == 139, "Anzahl an Zeilen stimmt nicht überein in der .dbb Datei!");
        }

        [Test]
        public void TestNapFormatCount()
        {

            //string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string napFileName = "FPF_NAP_PS0.nap";
            System.Data.DataTable nap_test = Import.ImportNapFiles(dataTableforImportTestNap, Path.Combine(projectDirectory, napFileName));

            Assert.IsTrue(nap_test.Rows.Count == 1, "Anzahl an Zeilen stimmt nicht überein in der .nap Datei!");
        }

        [Test]
        public void TestCsvFormatCount()
        {
            string csvFileName = "FPF_CSV_AutoDat.csv";
            System.Data.DataTable csv_test = Import.ImportCsvFiles(dataTableforImportTestCsv, Path.Combine(projectDirectory, csvFileName));

            Assert.IsTrue(csv_test.Rows.Count == 10, "Anzahl an Zeilen stimmt nicht überein in der .csv Datei!");
        }

        [Test]
        public void TestXlsxFormatCount()
        {

            //string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

            string excelFileName = "FPF_XLSX_AutoDat.xlsx";
            System.Data.DataTable excel_test = Import.ImportExcelFiles(dataTableforImportTestExcel, Path.Combine(projectDirectory, excelFileName));

            Assert.IsTrue(excel_test.Rows.Count == 10, "Anzahl an Zeilen stimmt nicht überein in der .xlsx Datei!");
        }

        [Test]
        public void TestXlsxWithCsv()
        {

            //string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string excelFileName = "FPF_XLSX_AutoDat.xlsx";
            string csvFileName = "FPF_CSV_AutoDat.csv";

            System.Data.DataTable excel_test = Import.ImportExcelFiles(dataTableforImportTestExcelCsv, Path.Combine(projectDirectory, excelFileName));
            System.Data.DataTable csv_test = Import.ImportCsvFiles(dataTableforImportTestCsvExcel, Path.Combine(projectDirectory, csvFileName));

            for (int i = 0; i < 10; i++) // Anzahl an Zeilen in Excel und in csv
            {
                CollectionAssert.AreEqual((System.Collections.IEnumerable)excel_test.Rows[i][0], (System.Collections.IEnumerable)csv_test.Rows[i][0]);
            }
        }

        [Test]
        public void TestImportSkizze()
        {
           
            string jpgTestFile = "1122BQ00401.jpg";
            string jpgTestFile2 = "1122DB00040.jpg";

            testFileName.Add(Path.Combine(projectDirectory, jpgTestFile));
            testFileName.Add(Path.Combine(projectDirectory, jpgTestFile2));

            System.Data.DataTable testSkizzen = Import.ImportSketchesInDataGrid(testFileName, sketchName, dataTableForSkizzen, "Test");

            Assert.IsTrue(testSkizzen.Rows.Count == 2);
        }

        [Test]
        public void TestImportSkizzePPT()
        {


            string pptTestFile = "1122CQ00401.pptx";

            testFileNamePPT.Add(Path.Combine(projectDirectory, pptTestFile));


            System.Data.DataTable testSkizzePPT = Import.ImportSketchesInDataGrid(testFileNamePPT, sketchNamePPT, dataTableForSkizzenPPT, "Test");

            Assert.IsTrue(testSkizzePPT.Rows.Count == 1);
        }

        [Test]
        public void TestImportTemp()
        {
            System.Data.DataTable test = new System.Data.DataTable();
            string testTemp = "test";
            EntityFrameworkContext testentity = new EntityFrameworkContext();

            try
            {
                Import.ImportToTemps(test, testTemp, testentity);
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }
    }
}
