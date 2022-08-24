using NUnit.Framework;
using FestpunktDB.Business.ExportService;
using System.IO;
using System.Collections.Generic;
using FestpunktDB.Business;
using FestpunktDB.Business.Entities;
using System;

namespace FestpunktDB.Tests
{
    [TestFixture]
    public class ExportTests
    {
        List<Pp> Pp = new List<Pp>();
        List<Ph> Ph = new List<Ph>();
        List<Pl> Pl = new List<Pl>();
        List<Pk> Pk = new List<Pk>();
        List<Ps> Ps = new List<Ps>();

        string csvTestFile = Path.GetFullPath(@"..\..\..\..\ExportTestCsv.csv");
        string dbbTestFile = Path.GetFullPath(@"..\..\..\..\ExportTestDbb.dbb");
        //string dbbTestFileNotEmpty = @"C:\Users\besar\Desktop\testNotEmpty.dbb";
        string napTestFile = Path.GetFullPath(@"..\..\..\..\ExportTestNap.nap");
        string xlsTestFileAuto = Path.GetFullPath(@"..\..\..\..\ExportTestXlsAuto.xlsx");
        string xlsTestFileEinfach = Path.GetFullPath(@"..\..\..\..\ExportTestXlsEinfach.xlsx");
        string xlsTestFile = Path.GetFullPath(@"..\..\..\..\ExportTestXls.xlsx");

        [Test]
        public void CSVExportTest()
        {
            Export.ToCsvFile(Pp, Ph, Pk, Pl, Ps, csvTestFile); // Leere Datei erstellt 
            Assert.IsTrue(File.Exists(csvTestFile)); // Abfrage ob die Datei worklich im gegebenen Pfad existiert
        }

        [Test]
        public void DBBExportTest()
        {
            Export.ExportDbb(Pp, Ph, Pl, Ps, dbbTestFile);
            Assert.IsTrue(File.Exists(dbbTestFile));
        }

        [Test]
        public void NAPExportTest()
        {
            Export.ExportNap(Pp, Ph, Pk, Pl, Ps, napTestFile);
            Assert.IsTrue(File.Exists(napTestFile));
        }

        [Test]
        public void xlsAutoExportTest()
        {
            Export.ToExcelFileAuto(Pp, Ph, Pk, Pl, Ps, xlsTestFileAuto);
            Assert.IsTrue(File.Exists(xlsTestFileAuto));
        }

        [Test]
        public void xlsEinfachExportTest()
        {
            Export.ToExcelFileEinfach(Pp, Ph, Pk, Pl, Ps, xlsTestFileEinfach);
            Assert.IsTrue(File.Exists(xlsTestFileEinfach));
        }

        [Test]
        public void xlsExportTest()
        {
            System.Data.DataTable dataTablePp = Export.ToDataTable(Pp);
            System.Data.DataTable dataTablePh = Export.ToDataTable(Ph);
            System.Data.DataTable dataTablePk = Export.ToDataTable(Pk);
            System.Data.DataTable dataTablePl = Export.ToDataTable(Pl);
            System.Data.DataTable dataTablePs = Export.ToDataTable(Ps);
            Export.ToExcelFile(dataTablePp, dataTablePh, dataTablePk, dataTablePl, dataTablePs, xlsTestFile);

            Assert.IsTrue(File.Exists(xlsTestFile));
        }
    }
}
