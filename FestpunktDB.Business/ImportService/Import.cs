using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using FestpunktDB.Business.Entities;
using FestpunktDB.Business.EntitiesImport;

namespace FestpunktDB.Business.ImportService
{
    public static class Import
    {

        #region Declarations

        public static List<ImportPp> ImportPps = new List<ImportPp>();
        public static List<ImportPh> ImportPhs = new List<ImportPh>();
        public static List<ImportPk> ImportPks = new List<ImportPk>();
        public static List<ImportPl> ImportPls = new List<ImportPl>();
        public static List<ImportPs> ImportPss = new List<ImportPs>();
        public static List<ImportPp> PpsToSave = new List<ImportPp>();
        public static List<ImportPh> PhsToSave = new List<ImportPh>();
        public static List<ImportPk> PksToSave = new List<ImportPk>();
        public static List<ImportPl> PlsToSave = new List<ImportPl>();
        public static List<ImportPs> PssToSave = new List<ImportPs>();
        public static List<string> ColumnNames = new List<string>();
        public static List<string> PadsToUpdate = new List<string>();
        private static List<string> _jpgSketchesInDb = new List<string>();
        private static List<string> _pdfSketchesInDb = new List<string>();
        private static List<string> _pptSketchesInDb = new List<string>();
        private static List<string> _jpgSketches = new List<string>();
        private static List<string> _pdfSketches = new List<string>();
        private static List<string> _pptSketches = new List<string>();
        private static string tempString;
        private static DataRow Row;

        #endregion

        #region Records: Import Different Formats
        public static DataTable ImportExcelFiles(DataTable dataTableForImport, string text)
        {
            string connectingString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + text + "; Extended Properties=\"Excel 12.0 Xml; HDR= Yes;\";";

            using (OleDbConnection oleDbConnection = new OleDbConnection(connectingString))
            {
                string sql = "SELECT * FROM [FPF$]";
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(sql, oleDbConnection);

                dataAdapter.Fill(dataTableForImport);
            }
            return dataTableForImport;
        }
        public static DataTable ImportCsvFiles(DataTable dataTableForImport, string filename)
        {
            string[] csvFile = File.ReadAllLines(filename);

            //if no data
            if (csvFile.Length == 0)
            {
                return dataTableForImport;
            }

            string[] headings = csvFile[0].Split(',');

            //for each heading
            for (int i = 0; i < headings.Length; i++)
            {
                dataTableForImport.Columns.Add(headings[i]);
            }
            for (int i = 1; i < csvFile.Length; i++)
            {
                Row = dataTableForImport.NewRow();

                for (int j = 0; j < headings.Length; j++)
                {
                    Row[j] = csvFile[i].Split(',')[j];
                }

                dataTableForImport.Rows.Add(Row);
            }
            return dataTableForImport;
        }
        public static DataTable ImportNapFiles(DataTable dataTableForImport, string filename)
        {
            // The Definition of each column should be defined by the client since the headings are not given by the client
            string[] napFile = File.ReadAllLines(filename);
            
            //if no data
            if (napFile.Length == 0)
            {
                return dataTableForImport;
            }            
            // if nap2 (contains one line)
            else if (napFile.Length == 1)
            {
                tempString = "D1,PAD,D2,PK.X,PK.Y,PK.Z,PK.MP,D3,D4,PK.MPEXP,D5,D6,PP.PArt,D7,PL.Y,PL.X,PH.H,PL.MP,D8,D9,PH.MP,D10,D11,PS.PStrecke,PS.PSTRRiKz,PS.Station(KM),PS.Station(M), D12,PP/PL/PK/PS/PH.Datum,PP/PL/PH/PK.Text";
                string[] headings = tempString.Split(',');

                foreach (var heading in headings)
                {
                    dataTableForImport.Columns.Add(heading);
                }              
                for (int i = 0; i < napFile.Length; i++)
                {
                    string[] substrings = Regex.Split(napFile[0], @"\s+");

                    Row = dataTableForImport.NewRow();
                    for (int k = 0; k < headings.Length; k++)
                    {
                        Row[k] = substrings[k + 1];
                    }
                    
                    dataTableForImport.Rows.Add(Row);
                }
                return dataTableForImport;

            }
            // if nap 1 (contains multiple lines)
            else
            {
                tempString = "D1,PAD,PK.X,PK.Y,PK.Z,PK.MP,D2,D3,PK.MPEXP,D4,D5,PP.PArt,D6,PL.Y,PL.X,PH.H,PL.MP,D7,D8,PH.MP,D9,D10,PS.PStrecke,PS.PSTRRiKz,PS.Station(KM),PS.Station(M), D11,PP/PL/PK/PS/PH.Datum,PP/PL/PH/PK.Text";
                string[] headings = tempString.Split(',');

                foreach (var heading in headings)
                {
                    dataTableForImport.Columns.Add(heading);
                }

                for (int j = 0; j < napFile.Length; j++)
                {
                    string[] substrings = Regex.Split(napFile[j], @"\s+");

                    // adding the data to the Row
                    Row = dataTableForImport.NewRow();

                    for (int k = 0; k < headings.Length; k++)
                    {
                        Row[k] = substrings[k + 1];
                    }
                    dataTableForImport.Rows.Add(Row);
                }
                
                return dataTableForImport;
            }
        }
        public static DataTable ImportDbbFiles(DataTable dataTableForImport, string filename)
        {
            string[] dbbFile = File.ReadAllLines(filename);
            //if no data
            if (dbbFile.Length == 0)
            {
                return dataTableForImport;
            }

            string rowsCombined = "PAD,PP.PArt,PS.Station,PP/PS.Datum,SBearb/PBearb,PAuftr,PProgram,PText,,PS.Strecke,PS.PSTRRiKz,Lsys,PL.Y,PL.X,LDatum,LBearb,LAuftr,LProgramm,LText,HSys,H,HDatum,HBearb,HAuftr,HProgramm,HText";
            string[] headings = rowsCombined.Split(',');

            // adding the headings, ohne the "d" headings

            for (int i = 0; i < headings.Length; i++)
            {
                dataTableForImport.Columns.Add(headings[i]);

            }
            // Each row consists of 3 Zeilen 
            //for each heading

            for (int rowCounter = 0; rowCounter < dbbFile.Length; rowCounter += 3)
            {
                DataRow row = dataTableForImport.NewRow();

                string[] substringsvon11 = Regex.Split(dbbFile[rowCounter], @"\s+");
                string[] substringsvon12 = Regex.Split(dbbFile[rowCounter + 1], @"\s+");
                string[] substringsvon13 = Regex.Split(dbbFile[rowCounter + 2], @"\s+");

                if (substringsvon11[1].Length == 6)
                {
                    row["PAD"] = substringsvon11[0].Substring(2, 6) + "00" + substringsvon11[1].Substring(0, 3);
                    row["PP.PArt"] = substringsvon11[1].Substring(3, 3);
                }
                else
                {
                    row["PAD"] = substringsvon11[0].Substring(2, 6) + "0" + substringsvon11[1].Substring(0, 4);
                    row["PP.PArt"] = substringsvon11[1].Substring(4, 4);
                }

                row["PS.Station"] = substringsvon11[2].Substring(0, 15);
                row["PP/PS.Datum"] = substringsvon11[2].Substring(15, 8);
                row["SBearb/PBearb"] = substringsvon11[2].Substring(23, (substringsvon11[2].Length - 23));
                row["PAuftr"] = substringsvon11[3];

                if (substringsvon11.Length == 6)
                {
                    row["PProgram"] = substringsvon11[4].Substring(0, 5);
                    row["PText"] = substringsvon11[4].Substring(5, substringsvon11[4].Length - 5);
                    row["PS.Strecke"] = substringsvon11[5].Substring(0, 10);
                    row["PS.PSTRRiKz"] = substringsvon11[5].Substring(10, 1);
                }
                else
                {
                    row["PProgram"] = substringsvon11[4];
                    row["PText"] = substringsvon11[5];
                    row["PS.Strecke"] = substringsvon11[6].Substring(0, 10);
                    row["PS.PSTRRiKz"] = substringsvon11[6].Substring(10, 1);
                }

                if (substringsvon12.Length == 8 && substringsvon12[4].Length < 16 || substringsvon12.Length == 9 && substringsvon12[1].Length == 7)
                {
                    if (substringsvon12[1].Length == 8)
                    {
                        row["Lsys"] = substringsvon12[1].Substring(5, 3);
                    }
                    else
                    {
                        row["Lsys"] = substringsvon12[1].Substring(4, 3);
                    }
                    row["PL.Y"] = substringsvon12[2];
                    row["PL.X"] = substringsvon12[3].Substring(0, 13);
                    row["LDatum"] = substringsvon12[4].Substring(1, 8);
                    row["LBearb"] = substringsvon12[4].Substring(9, substringsvon12[4].Length - 9);
                    row["LAuftr"] = substringsvon12[5];

                    if (substringsvon12.Length == 8 && substringsvon12[4].Length < 16)
                    {
                        row["LProgramm"] = substringsvon12[6].Substring(0, 5);
                        row["LText"] = substringsvon12[6].Substring(5, substringsvon12[6].Length - 5);
                    }
                    else
                    {
                        row["LProgramm"] = substringsvon12[6];
                        row["LText"] = substringsvon12[7];
                    }

                }
                else if (substringsvon12.Length == 8 && substringsvon12[4].Length > 16)
                {
                    row["Lsys"] = substringsvon12[2];
                    row["PL.Y"] = substringsvon12[3];
                    row["PL.X"] = substringsvon12[4].Substring(0, 13);
                    row["LDatum"] = substringsvon12[4].Substring(18, 8);
                    row["LBearb"] = substringsvon12[4].Substring(26, substringsvon12[4].Length - 26);
                    row["LAuftr"] = substringsvon12[5];
                    row["LProgramm"] = substringsvon12[6].Substring(0, 5);
                    row["LText"] = substringsvon12[6].Substring(5, substringsvon12[6].Length - 5);
                }
                else if (substringsvon12.Length == 7 && substringsvon12[3].Length > 16)
                {
                    row["Lsys"] = substringsvon12[1].Substring(4, 3);
                    row["PL.Y"] = substringsvon12[2];
                    row["PL.X"] = substringsvon12[3].Substring(0, 13);
                    row["LDatum"] = substringsvon12[3].Substring(18, 8);
                    row["LBearb"] = substringsvon12[3].Substring(26, substringsvon12[3].Length - 26);
                    row["LAuftr"] = substringsvon12[4];
                    row["LProgramm"] = substringsvon12[5].Substring(0, 5);
                    row["LText"] = substringsvon12[5].Substring(5, substringsvon12[5].Length - 5);
                }
                else
                {

                    row["Lsys"] = substringsvon12[2];
                    row["PL.Y"] = substringsvon12[3];
                    row["PL.X"] = substringsvon12[4].Substring(0, 13);
                    row["LDatum"] = substringsvon12[5].Substring(1, 8);
                    row["LBearb"] = substringsvon12[5].Substring(9, substringsvon12[5].Length - 9);
                    row["LAuftr"] = substringsvon12[6];
                    row["LProgramm"] = substringsvon12[7].Substring(0, 5);
                    row["LText"] = substringsvon12[7].Substring(5, substringsvon12[7].Length - 5);
                }
                if (substringsvon13.Length == 7 || substringsvon13.Length == 8 && substringsvon13[1].Length == 7)
                {
                    if (substringsvon13.Length == 7 && substringsvon13[1].Length == 8)
                    {
                        row["HSys"] = substringsvon13[1].Substring(5, 3);
                    }
                    else
                    {
                        row["HSys"] = substringsvon13[1].Substring(4, 3);
                    }
                    row["H"] = substringsvon13[2];
                    row["HDatum"] = substringsvon13[3].Substring(1, 8);
                    row["HBearb"] = substringsvon13[3].Substring(9, substringsvon13[3].Length - 9);
                    row["HAuftr"] = substringsvon13[4];
                    row["HProgramm"] = substringsvon13[5].Substring(0, 5);
                    row["HText"] = substringsvon13[5].Substring(5, substringsvon13[5].Length - 5);

                    if (substringsvon13.Length == 7)
                    {
                        row["HProgramm"] = substringsvon13[5].Substring(0, 5);
                        row["HText"] = substringsvon13[5].Substring(5, substringsvon13[5].Length - 5);
                    }
                    else
                    {
                        row["HProgramm"] = substringsvon13[5];
                        row["HText"] = substringsvon13[6];
                    }
                }
                else
                {
                    row["HSys"] = substringsvon13[2];
                    row["H"] = substringsvon13[3];
                    row["HDatum"] = substringsvon13[4].Substring(1, 8);
                    row["HBearb"] = substringsvon13[4].Substring(9, substringsvon13[4].Length - 9);
                    row["HAuftr"] = substringsvon13[5];
                    row["HProgramm"] = substringsvon13[6].Substring(0, 5);
                    row["HText"] = substringsvon13[6].Substring(5, substringsvon13[6].Length - 5);
                }

                dataTableForImport.Rows.Add(row);
            }

            return dataTableForImport;
        }
        #endregion

        #region Records: Import Data To TempTables
        public static void ImportToTemps(System.Data.DataTable dataTableForTemp, string fileType, EntityFrameworkContext dbGlobal)
        {
            ColumnNames.Clear();
            foreach (DataColumn column in dataTableForTemp.Columns)
            {
                ColumnNames.Add(column.ColumnName);
            }
            if (fileType == "Excel"|| fileType == "CSV")
            {
                foreach (DataRow r in dataTableForTemp.Rows)
                {
                    ImportPps.Add(new ImportPp()
                    {

                        PAD = ColumnNames.Contains("Punktadresse") ? r["Punktadresse"].ToString() : r["PAD"].ToString(),
                        PArt = ColumnNames.Contains("PArt") ? r["PArt"].ToString() : r["PP.PArt"].ToString(),
                        Blattschnitt = ColumnNames.Contains("Blattschnitt") ? r["Blattschnitt"].ToString() : " ",
                        PunktNr = ColumnNames.Contains("PunktNr") ? (int?)r["PunktNr"] : 0,
                        VermArt = ColumnNames.Contains("VermArt") ? (short?)r["VermArt"] : 0,
                        Stabil = ColumnNames.Contains("Stabil") ? (short?)r["Stabil"] : 0,
                        PDatum = ColumnNames.Any(str => str.Contains("PDatum")) ? r["PDatum"].ToString() : " ",
                        PBearb = ColumnNames.Contains("PBearb") ? r["PBearb"].ToString() : " ",
                        PAuftr = ColumnNames.Contains("PAuftr") ? r["PAuftr"].ToString() : " ",
                        PProg = ColumnNames.Contains("PProg") ? r["PProg"].ToString() : " ",
                        PText = ColumnNames.Contains("PText") ? r["PText"].ToString() : " ",
                        Import = ColumnNames.Contains("Import") ? (DateTime?)r["Import"] : null,
                        LoeschDatum = ColumnNames.Contains("LoeschDatum") ? (DateTime?)r["LoeschDatum"] : null
                    });

                    ImportPhs.Add(new ImportPh()
                    {
                        PAD = ColumnNames.Contains("Punktadresse") ? r["Punktadresse"].ToString() : r["PAD"].ToString(),
                        HStat = ColumnNames.Contains("HStat") ? r["HStat"].ToString() : " ",
                        HSys = ColumnNames.Any(str => str.Contains("HR0")) ? "R00" : " ",
                        HFremd = ColumnNames.Contains("HFremd") ? r["HFremd"].ToString() : " ",
                        H = ColumnNames.Contains("HR0HöheM") ? Convert.ToDouble(r["HR0HöheM"]) : 0,
                        MH = ColumnNames.Contains("MH") ? (short?)r["MH"] : 0,
                        MHEXP = ColumnNames.Contains("MHEXP") ? (short?)r["MHEXP"] : 0,
                        HDatum = ColumnNames.Contains("HR0Datum") ? r["HR0Datum"].ToString() : " ",
                        HBearb = ColumnNames.Contains("HR0Bea") ? r["HR0Bea"].ToString() : " ",
                        HAuftr = ColumnNames.Contains("HR0Auf") ? r["HR0Auf"].ToString() : " ",
                        HProg = ColumnNames.Contains("HProg") ? r["HProg"].ToString() : " ",
                        HText = ColumnNames.Contains("HR0Text") ? r["HR0Text"].ToString() : " ",
                        Import = ColumnNames.Contains("Import") ? (DateTime?)r["Import"] : null,
                        LoeschDatum = ColumnNames.Contains("LoeschDatum") ? (DateTime?)r["LoeschDatum"] : null
                    });

                    ImportPhs.Add(new ImportPh()
                    {
                        PAD = ColumnNames.Contains("Punktadresse") ? r["Punktadresse"].ToString() : r["PAD"].ToString(),
                        HStat = ColumnNames.Contains("HStat") ? r["HStat"].ToString() : " ",
                        HSys = ColumnNames.Any(str => str.Contains("HO0")) ? "O00" : " ",
                        HFremd = ColumnNames.Contains("HFremd") ? r["HFremd"].ToString() : " ",
                        H = ColumnNames.Contains("HO0HöheM") ? Convert.ToDouble(r["HO0HöheM"]) : 0,
                        MH = ColumnNames.Contains("MH") ? (short?)r["MH"] : 0,
                        MHEXP = ColumnNames.Contains("MHEXP") ? (short?)r["MHEXP"] : 0,
                        HDatum = ColumnNames.Contains("HO0Datum") ? r["HO0Datum"].ToString() : " ",
                        HBearb = ColumnNames.Contains("HO0Bea") ? r["HO0Bea"].ToString() : " ",
                        HAuftr = ColumnNames.Contains("HO0Auf") ? r["HO0Auf"].ToString() : " ",
                        HProg = ColumnNames.Contains("HProg") ? r["HProg"].ToString() : " ",
                        HText = ColumnNames.Contains("HO0Text") ? r["HO0Text"].ToString() : " ",
                        Import = ColumnNames.Contains("Import") ? (DateTime?)r["Import"] : null,
                        LoeschDatum = ColumnNames.Contains("LoeschDatum") ? (DateTime?)r["LoeschDatum"] : null
                    });

                    ImportPks.Add(new ImportPk()
                    {
                        PAD = ColumnNames.Contains("Punktadresse") ? r["Punktadresse"].ToString() : r["PAD"].ToString(),
                        KStat = ColumnNames.Contains("KStat") ? r["KStat"].ToString() : " ",
                        KSys = ColumnNames.Contains("KSys") ? r["KSys"].ToString() : " ",
                        HFremd = ColumnNames.Contains("HFremd") ? r["HFremd"].ToString() : " ",
                        X = ColumnNames.Contains("ETRF_Xwert") ? Convert.ToDouble(r["ETRF_Xwert"]) : 0,
                        Y = ColumnNames.Contains("ETRF_Ywert") ? Convert.ToDouble(r["ETRF_Ywert"]) : 0,
                        Z = ColumnNames.Contains("ETRF_Zwert") ? Convert.ToDouble(r["ETRF_Zwert"]) : 0,
                        MP = ColumnNames.Contains("MP") ? Convert.ToDouble(r["MP"]) : 0,
                        MPEXP = ColumnNames.Contains("MPEXP") ? Convert.ToDouble(r["MPEXP"]) : 0,
                        KDatum = ColumnNames.Contains("KDatum") ? r["KDatum"].ToString() : null,
                        KBearb = ColumnNames.Contains("KBearb") ? r["KBearb"].ToString() : " ",
                        LAuftr = ColumnNames.Contains("LAuftr") ? r["LAuftr"].ToString() : " ",
                        LProg = ColumnNames.Contains("LProg") ? r["LProg"].ToString() : " ",
                        KText = ColumnNames.Contains("KText") ? r["KText"].ToString() : " ",
                        Import = ColumnNames.Contains("Import") ? (DateTime?)r["Import"] : null,
                        LoeschDatum = ColumnNames.Contains("LoeschDatum") ? (DateTime?)r["LoeschDatum"] : null
                    }) ;
                    string lsys = ColumnNames.Contains("LSys") ? r["LSys"].ToString() : " ";

                    ImportPls.Add(new ImportPl()
                    {
                        PAD = ColumnNames.Contains("Punktadresse") ? r["Punktadresse"].ToString() : r["PAD"].ToString(),
                        LStat = ColumnNames.Contains("LStat") ? r["LStat"].ToString() : " ",
                        LSys = ColumnNames.Contains("LSys") ? lsys : " ",
                        LFremd = ColumnNames.Contains("LFremd") ? r["LFremd"].ToString() : " ",
                        Y = (lsys == "FR0") ? Convert.ToDouble(r["RechtswertM5"]) : Convert.ToDouble(r["RechtswertM4"]),
                        X = (lsys == "FR0") ? Convert.ToDouble(r["HochwertM5"]) : Convert.ToDouble(r["HochwertM4"]),
                        MP = ColumnNames.Contains("MP") ? Convert.ToDouble(r["MP"]) : 0,
                        MPEXP = ColumnNames.Contains("MPEXP") ? Convert.ToDouble(r["MPEXP"]) : 0,
                        LDatum = ColumnNames.Contains("LageDatum") ? r["LageDatum"].ToString() : " ",
                        LBearb = ColumnNames.Contains("LBearb") ? r["LBearb"].ToString() : " ",
                        LAuftr = ColumnNames.Contains("LAuftr") ? r["LAuftr"].ToString() : " ",
                        LProg = ColumnNames.Contains("LProg") ? r["LProg"].ToString() : " ",
                        LText = ColumnNames.Contains("LText") ? r["LText"].ToString() : " ",
                        Import = ColumnNames.Contains("Import") ? (DateTime?)r["Import"] : null,
                        LoeschDatum = ColumnNames.Contains("LoeschDatum") ? (DateTime?)r["LoeschDatum"] : null
                    });

                    ImportPss.Add(new ImportPs()
                    {
                        PAD = ColumnNames.Contains("Punktadresse") ? r["Punktadresse"].ToString() : r["PAD"].ToString(),
                        PStrecke = ColumnNames.Contains("PStrecke") ? r["PStrecke"].ToString() : " ",
                        PSTRRiKz = ColumnNames.Contains("PSTRRiKz") ? (int)r["PSTRRiKz"] : 0,
                        Station = ColumnNames.Contains("StationGISKm") ? Convert.ToDouble(r["StationGISKm"]) : 0,
                        SDatum = ColumnNames.Contains("SDatum") ? r["SDatum"].ToString() : null,
                        Import = ColumnNames.Contains("Import") ? (DateTime?)r["Import"] : null,
                        LoeschDatum = ColumnNames.Contains("LoeschDatum") ? (DateTime?)r["LoeschDatum"] : null
                    });
                }         
            }
            else if (fileType == "NAP")
            {
                foreach (DataRow r in dataTableForTemp.Rows)
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-En");
                    ImportPps.Add(new ImportPp()
                    {
                        PAD = r["PAD"].ToString(),
                        PArt = r["PP.PArt"].ToString(),
                        Blattschnitt = ColumnNames.Contains("Blattschnitt") ? r["Blattschnitt"].ToString() : " ",
                        PunktNr = ColumnNames.Contains("PunktNr") ? (int?)r["PunktNr"] : 0,
                        VermArt = ColumnNames.Contains("VermArt") ? (short?)r["VermArt"] : 0,
                        Stabil = ColumnNames.Contains("Stabil") ? (short?)r["Stabil"] : 0,
                        PDatum = ColumnNames.Any(str => str.Contains("Datum")) ? DateTime(r["PP/PL/PK/PS/PH.Datum"].ToString()) : " ",
                        PBearb = ColumnNames.Contains("PBearb") ? r["PBearb"].ToString() : " ",
                        PAuftr = ColumnNames.Contains("PAuftr") ? r["PAuftr"].ToString() : " ",
                        PProg = ColumnNames.Contains("PProg") ? r["PProg"].ToString() : " ",
                        PText = ColumnNames.Contains("PP/PL/PK/PS/PH.Text") ? r["PP/PL/PK/PS/PH.Text"].ToString() : " ",
                        Import = ColumnNames.Contains("Import") ? (DateTime?)r["Import"] : null,
                        LoeschDatum = ColumnNames.Contains("LoeschDatum") ? (DateTime?)r["LoeschDatum"] : null
                    });
                    
                    ImportPhs.Add(new ImportPh()
                    {
                        PAD = r["PAD"].ToString(),
                        HStat = ColumnNames.Contains("HStat") ? r["HStat"].ToString() : " ",
                        HSys = ColumnNames.Any(str => str.Contains("HR0")) ? "R00" : " ",
                        HFremd = ColumnNames.Contains("HFremd") ? r["HFremd"].ToString() : " ",
                        H = ColumnNames.Contains("PH.H") ? Convert.ToDouble(r["PH.H"].ToString()) : 0,
                        MH = ColumnNames.Contains("PH.MH") ? (short?)r["PH.MH"] : 0,
                        MHEXP = ColumnNames.Contains("PH.MHEXP") ? (short?)r["PH.MHEXP"] : 0,
                        HDatum = ColumnNames.Any(str => str.Contains("Datum")) ? DateTime(r["PP/PL/PK/PS/PH.Datum"].ToString()) : " ",
                        HBearb = ColumnNames.Contains("HR0Bea") ? r["HR0Bea"].ToString() : " ",
                        HAuftr = ColumnNames.Contains("HR0Auf") ? r["HR0Auf"].ToString() : " ",
                        HProg = ColumnNames.Contains("HProg") ? r["HProg"].ToString() : " ",
                        HText = ColumnNames.Contains("PP/PL/PK/PS/PH.Text") ? r["PP/PL/PK/PS/PH.Text"].ToString() : " ",
                        Import = ColumnNames.Contains("Import") ? (DateTime?)r["Import"] : null,
                        LoeschDatum = ColumnNames.Contains("LoeschDatum") ? (DateTime?)r["LoeschDatum"] : null
                    });
                   
                    ImportPks.Add(new ImportPk()
                    {
                        PAD = r["PAD"].ToString(),
                        KStat = ColumnNames.Contains("KStat") ? r["KStat"].ToString() : " ",
                        KSys = ColumnNames.Contains("KSys") ? r["KSys"].ToString() : " ",
                        HFremd = ColumnNames.Contains("HFremd") ? r["HFremd"].ToString() : " ",
                        X = ColumnNames.Contains("PK.X") ? Convert.ToDouble(r["PK.X"]) : 0,
                        Y = ColumnNames.Contains("PK.Y") ? Convert.ToDouble(r["PK.Y"]) : 0,
                        Z = ColumnNames.Contains("PK.Z") ? Convert.ToDouble(r["PK.Z"]) : 0,
                        MP = ColumnNames.Contains("PK.MP") ? Convert.ToDouble(r["PK.MP"]) : 0,
                        MPEXP = ColumnNames.Contains("PK.MPEXP") ? Convert.ToDouble(r["PK.MPEXP"]) : 0,
                        KDatum = ColumnNames.Any(str => str.Contains("Datum")) ? DateTime(r["PP/PL/PK/PS/PH.Datum"].ToString()) : " ",
                        KText = ColumnNames.Contains("PP/PL/PK/PS/PH.Text") ? r["PP/PL/PK/PS/PH.Text"].ToString() : " ",
                        Import = ColumnNames.Contains("Import") ? (DateTime?)r["Import"] : null,
                        LoeschDatum = ColumnNames.Contains("LoeschDatum") ? (DateTime?)r["LoeschDatum"] : null

                    });

                    ImportPls.Add(new ImportPl()
                     {
                        PAD = r["PAD"].ToString(),
                        LStat = ColumnNames.Contains("LStat") ? r["LStat"].ToString() : " ",
                        LSys = ColumnNames.Contains("LSys") ? r["LSys"].ToString()  : " ",
                        LFremd = ColumnNames.Contains("LFremd") ? r["LFremd"].ToString() : " ",
                        Y = ColumnNames.Contains("PL.Y") ? Convert.ToDouble(r["PL.Y"]) : 0,
                        X = ColumnNames.Contains("PL.X") ? Convert.ToDouble(r["PL.X"]) : 0,
                        MP = ColumnNames.Contains("PL.MP") ? Convert.ToDouble(r["PL.MP"]) : 0,
                        MPEXP = ColumnNames.Contains("PL.MPEXP") ? Convert.ToDouble(r["PL.MPEXP"]) : 0,
                        LDatum = ColumnNames.Any(str => str.Contains("Datum")) ? DateTime(r["PP/PL/PK/PS/PH.Datum"].ToString()) : null,
                        LText = ColumnNames.Contains("PP/PL/PK/PS/PH.Text") ? r["PP/PL/PK/PS/PH.Text"].ToString() : " ",
                        Import = ColumnNames.Contains("Import") ? (DateTime?)r["Import"] : null,
                        LoeschDatum = ColumnNames.Contains("LoeschDatum") ? (DateTime?)r["LoeschDatum"] : null
                    });

                     ImportPss.Add(new ImportPs()
                     {
                        PAD = r["PAD"].ToString(),
                        PStrecke = ColumnNames.Contains("PS.PStrecke") ? r["PS.PStrecke"].ToString() : " ",
                        PSTRRiKz = ColumnNames.Contains("PS.PSTRRiKz") ? Convert.ToInt32(r["PS.PSTRRiKz"]) : 0,
                        Station = CalculateKmInDatabankFormat(Convert.ToDouble(r["PS.Station(KM)"]), Convert.ToDouble(r["PS.Station(M)"])),
                        SDatum = ColumnNames.Any(str => str.Contains("Datum")) ? DateTime(r["PP/PL/PK/PS/PH.Datum"].ToString()) : null,
                        Import = ColumnNames.Contains("Import") ? (DateTime?)r["Import"] : null,
                        LoeschDatum = ColumnNames.Contains("LoeschDatum") ? (DateTime?)r["LoeschDatum"] : null
                     });
                }
            }
            else if (fileType == "DBB")
            {
                foreach (DataRow r in dataTableForTemp.Rows)
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-En");
                    ImportPps.Add(new ImportPp()
                    {
                        PAD = r["PAD"].ToString(),
                        PArt = r["PP.PArt"].ToString(),
                        Blattschnitt = ColumnNames.Contains("Blattschnitt") ? r["Blattschnitt"].ToString() : " ",
                        PunktNr = ColumnNames.Contains("PunktNr") ? (int?)r["PunktNr"] : 0,
                        VermArt = ColumnNames.Contains("VermArt") ? (short?)r["VermArt"] : 0,
                        Stabil = ColumnNames.Contains("Stabil") ? (short?)r["Stabil"] : 0,
                        PDatum = ColumnNames.Any(str => str.Contains("Datum")) ? r["PP/PS.Datum"].ToString() : " ",
                        PBearb = ColumnNames.Contains("SBearb/PBearb") ? r["SBearb/PBearb"].ToString() : " ",
                        PAuftr = ColumnNames.Contains("PAuftr") ? r["PAuftr"].ToString() : " ",
                        PProg = ColumnNames.Contains("PProgram") ? r["PProgram"].ToString() : " ",
                        PText = ColumnNames.Contains("PText") ? r["PText"].ToString() : " ",
                        Import = ColumnNames.Contains("Import") ? (DateTime?)r["Import"] : null,
                        LoeschDatum = ColumnNames.Contains("LoeschDatum") ? (DateTime?)r["LoeschDatum"] : null
                    });

                    ImportPhs.Add(new ImportPh()
                    {
                        PAD = r["PAD"].ToString(),
                        HStat = ColumnNames.Contains("HStat") ? r["HStat"].ToString() : " ",
                        HSys = ColumnNames.Contains("HSys") ? r["HSys"].ToString() : " ",
                        HFremd = ColumnNames.Contains("HFremd") ? r["HFremd"].ToString() : " ",
                        H = ColumnNames.Contains("H") ? Convert.ToDouble(r["H"].ToString()) : 0,
                        MH = ColumnNames.Contains("PH.MH") ? (short?)r["PH.MH"] : 0,
                        MHEXP = ColumnNames.Contains("PH.MHEXP") ? (short?)r["PH.MHEXP"] : 0,
                        HDatum = ColumnNames.Contains("HDatum") ? r["HDatum"].ToString() : " ",
                        HBearb = ColumnNames.Contains("HBearb") ? r["HBearb"].ToString() : " ",
                        HAuftr = ColumnNames.Contains("HAuftr") ? r["HAuftr"].ToString() : " ",
                        HProg = ColumnNames.Contains("HProgram") ? r["HProgram"].ToString() : " ",
                        HText = ColumnNames.Contains("HText") ? r["HText"].ToString() : " ",
                        Import = ColumnNames.Contains("Import") ? (DateTime?)r["Import"] : null,
                        LoeschDatum = ColumnNames.Contains("LoeschDatum") ? (DateTime?)r["LoeschDatum"] : null
                    });

                    ImportPls.Add(new ImportPl()
                    {
                        PAD = r["PAD"].ToString(),
                        LStat = ColumnNames.Contains("LStat") ? r["LStat"].ToString() : " ",
                        LSys = ColumnNames.Contains("Lsys") ? r["Lsys"].ToString() : " ",
                        LFremd = ColumnNames.Contains("LFremd") ? r["LFremd"].ToString() : " ",
                        Y = ColumnNames.Contains("PL.Y") ? Convert.ToDouble(r["PL.Y"]) : 0,
                        X = ColumnNames.Contains("PL.X") ? Convert.ToDouble(r["PL.X"]) : 0,
                        MP = ColumnNames.Contains("PL.MP") ? Convert.ToDouble(r["PL.MP"]) : 0,
                        MPEXP = ColumnNames.Contains("PL.MPEXP") ? Convert.ToDouble(r["PL.MPEXP"]) : 0,
                        LDatum = ColumnNames.Contains("LDatum") ? r["LDatum"].ToString() : null,
                        LBearb = ColumnNames.Contains("LBearb") ? r["LBearb"].ToString() : " ",
                        LAuftr = ColumnNames.Contains("LAuftr") ? r["LAuftr"].ToString() : " ",
                        LProg = ColumnNames.Contains("LProgramm") ? r["LProgramm"].ToString() : " ",
                        LText = ColumnNames.Contains("LText") ? r["LText"].ToString() : " ",
                        Import = ColumnNames.Contains("Import") ? (DateTime?)r["Import"] : null,
                        LoeschDatum = ColumnNames.Contains("LoeschDatum") ? (DateTime?)r["LoeschDatum"] : null
                    });

                    ImportPss.Add(new ImportPs()
                    {
                        PAD = r["PAD"].ToString(),
                        PStrecke = ColumnNames.Contains("PS.Strecke") ? r["PS.Strecke"].ToString() : " ",
                        PSTRRiKz = ColumnNames.Contains("PS.PSTRRiKz") ? Convert.ToInt32(r["PS.PSTRRiKz"]) : 0,
                        Station = ColumnNames.Contains("PS.Station") ? Convert.ToDouble(r["PS.Station"].ToString()) : 0,
                        SDatum = ColumnNames.Any(str => str.Contains("Datum")) ? r["PP/PS.Datum"].ToString() : null,
                        Import = ColumnNames.Contains("Import") ? (DateTime?)r["Import"] : null,
                        LoeschDatum = ColumnNames.Contains("LoeschDatum") ? (DateTime?)r["LoeschDatum"] : null
                    });
                }
            }
            dbGlobal.ImportPp.AddRange(ImportPps);
            dbGlobal.ImportPh.AddRange(ImportPhs);
            dbGlobal.ImportPk.AddRange(ImportPks);
            dbGlobal.ImportPl.AddRange(ImportPls);
            dbGlobal.ImportPs.AddRange(ImportPss);

            dbGlobal.SaveChanges();
        }
        #endregion

        #region Records: Calculate km in Database format
        public static double CalculateKmInDatabankFormat(double distanceInKm, double distanceInM)
        {
            double dKm = distanceInKm * 100000 + distanceInM + 100000000;

            return dKm;
        }
        #endregion

        #region Records: Convert Date Format
        private static string DateTime(string date)
        {
            char[] charArray = date.ToCharArray();

            string dateFormat = string.Concat(charArray[6], charArray[7], ".", charArray[4],charArray[5] , "." , charArray[0] , charArray[1] , charArray[2] , charArray[3]);
            
            return dateFormat;
        }
        #endregion
        
        #region Records: Replace PArt 
        public static void ReplacePArt(string OldPArt, string NewPArt, EntityFrameworkContext dbGlobal)
        {
            PadsToUpdate = dbGlobal.ImportPp.Where(x => x.PArt == OldPArt)
                .Select(s => s.PAD).ToList();

            var pArtsToUpdate = dbGlobal.ImportPp.Where
                (
                    p => PadsToUpdate.Contains(p.PAD)
                );
            foreach (ImportPp importpp in pArtsToUpdate)
            {
                importpp.PArt = NewPArt;
            }

            dbGlobal.SaveChanges();
        }
        #endregion

        #region Records: Delete Imports From Temps
        public static void DeleteTheSamePADFromOtherTables(string tableName, System.Collections.IList selectedItems, EntityFrameworkContext dbGlobal)
        {
            if (tableName == "ImportPp")
            {
                foreach (ImportPp importPp in selectedItems)
                {
                    dbGlobal.ImportPp.RemoveRange(dbGlobal.ImportPp.Where(x => x.PAD == importPp.PAD));
                    dbGlobal.ImportPh.RemoveRange(dbGlobal.ImportPh.Where(x => x.PAD == importPp.PAD));
                    dbGlobal.ImportPk.RemoveRange(dbGlobal.ImportPk.Where(x => x.PAD == importPp.PAD));
                    dbGlobal.ImportPl.RemoveRange(dbGlobal.ImportPl.Where(x => x.PAD == importPp.PAD));
                    dbGlobal.ImportPs.RemoveRange(dbGlobal.ImportPs.Where(x => x.PAD == importPp.PAD));

                }
            }
            else if (tableName == "ImportPh")
            {
                foreach (ImportPh importPh in selectedItems)
                {
                    dbGlobal.ImportPp.RemoveRange(dbGlobal.ImportPp.Where(x => x.PAD == importPh.PAD));
                    dbGlobal.ImportPh.RemoveRange(dbGlobal.ImportPh.Where(x => x.PAD == importPh.PAD));
                    dbGlobal.ImportPk.RemoveRange(dbGlobal.ImportPk.Where(x => x.PAD == importPh.PAD));
                    dbGlobal.ImportPl.RemoveRange(dbGlobal.ImportPl.Where(x => x.PAD == importPh.PAD));
                    dbGlobal.ImportPs.RemoveRange(dbGlobal.ImportPs.Where(x => x.PAD == importPh.PAD));
                }
            }
            else if (tableName == "ImportPk")
            {
                foreach (ImportPk importPk in selectedItems)
                {
                    dbGlobal.ImportPp.RemoveRange(dbGlobal.ImportPp.Where(x => x.PAD == importPk.PAD));
                    dbGlobal.ImportPh.RemoveRange(dbGlobal.ImportPh.Where(x => x.PAD == importPk.PAD));
                    dbGlobal.ImportPk.RemoveRange(dbGlobal.ImportPk.Where(x => x.PAD == importPk.PAD));
                    dbGlobal.ImportPl.RemoveRange(dbGlobal.ImportPl.Where(x => x.PAD == importPk.PAD));
                    dbGlobal.ImportPs.RemoveRange(dbGlobal.ImportPs.Where(x => x.PAD == importPk.PAD));
                }
            }
            else if (tableName == "ImportPl")
            {
                foreach (ImportPl importPl in selectedItems)
                {
                    dbGlobal.ImportPp.RemoveRange(dbGlobal.ImportPp.Where(x => x.PAD == importPl.PAD));
                    dbGlobal.ImportPh.RemoveRange(dbGlobal.ImportPh.Where(x => x.PAD == importPl.PAD));
                    dbGlobal.ImportPk.RemoveRange(dbGlobal.ImportPk.Where(x => x.PAD == importPl.PAD));
                    dbGlobal.ImportPl.RemoveRange(dbGlobal.ImportPl.Where(x => x.PAD == importPl.PAD));
                    dbGlobal.ImportPs.RemoveRange(dbGlobal.ImportPs.Where(x => x.PAD == importPl.PAD));
                }
            }
            else if (tableName == "ImportPs")
            {
                foreach (ImportPs importPs in selectedItems)
                {
                    dbGlobal.ImportPp.RemoveRange(dbGlobal.ImportPp.Where(x => x.PAD == importPs.PAD));
                    dbGlobal.ImportPh.RemoveRange(dbGlobal.ImportPh.Where(x => x.PAD == importPs.PAD));
                    dbGlobal.ImportPk.RemoveRange(dbGlobal.ImportPk.Where(x => x.PAD == importPs.PAD));
                    dbGlobal.ImportPl.RemoveRange(dbGlobal.ImportPl.Where(x => x.PAD == importPs.PAD));
                    dbGlobal.ImportPs.RemoveRange(dbGlobal.ImportPs.Where(x => x.PAD == importPs.PAD));
                }
            }
           
            dbGlobal.SaveChanges();
        }
        public static void Delete(EntityFrameworkContext dbGlobal, string pad)
        {
            dbGlobal.ImportPp.RemoveRange(dbGlobal.ImportPp.Where(x => x.PAD == pad));
            dbGlobal.ImportPh.RemoveRange(dbGlobal.ImportPh.Where(x => x.PAD == pad));
            dbGlobal.ImportPk.RemoveRange(dbGlobal.ImportPk.Where(x => x.PAD == pad));
            dbGlobal.ImportPl.RemoveRange(dbGlobal.ImportPl.Where(x => x.PAD == pad));
            dbGlobal.ImportPs.RemoveRange(dbGlobal.ImportPs.Where(x => x.PAD == pad));
        }

        public static void ClearTempTables(EntityFrameworkContext dbGlobal, DataTable dataTableForTemp, string text)
        {
            foreach (var item in dbGlobal.ImportPp)
            {
                dbGlobal.ImportPp.Remove(item);
            }
            foreach (var item in dbGlobal.ImportPh)
            {
                dbGlobal.ImportPh.Remove(item);
            }
            foreach (var item in dbGlobal.ImportPk)
            {
                dbGlobal.ImportPk.Remove(item);
            }
            foreach (var item in dbGlobal.ImportPl)
            {
                dbGlobal.ImportPl.Remove(item);
            }
            foreach (var item in dbGlobal.ImportPs)
            {
                dbGlobal.ImportPs.Remove(item);
            }

            ImportPps.Clear();
            ImportPhs.Clear();
            ImportPks.Clear();
            ImportPls.Clear();
            ImportPss.Clear();

            dataTableForTemp.Clear();
            text = null;
            foreach (var column in dataTableForTemp.Columns.Cast<DataColumn>().ToArray())
            {
                if (dataTableForTemp.AsEnumerable().All(dr => dr.IsNull(column)))
                    dataTableForTemp.Columns.Remove(column);
            }
            dbGlobal.SaveChanges();
        }
        #endregion

        #region Records: Save Data In Database
        public static void DeleteUnwantedDataFromTables( List<string> listToRemoveFromTemps, List<string> listToRemoveFromDatabase, EntityFrameworkContext dbGlobal, string tableName)
        {
            switch (tableName)
            {
                case "Pp":
                    foreach (string pad in listToRemoveFromTemps)
                    {
                        dbGlobal.ImportPp.RemoveRange(dbGlobal.ImportPp.Where(x => x.PAD == pad));
                    }
                    foreach (string pad in listToRemoveFromDatabase)
                    {
                        Pp pp = dbGlobal.Pp.Where(x => x.PAD == pad).FirstOrDefault();
                        if (pp != null)
                        {
                            dbGlobal.GeloeschtPp.Add(new EntitiesDeleted.GeloeschtPp
                            {
                                PAD = pp.PAD,
                                PArt = pp.PArt,
                                Blattschnitt = pp.Blattschnitt,
                                PunktNr = pp.PunktNr,
                                VermArt = pp.VermArt,
                                Stabil = pp.Stabil,
                                PDatum = pp.PDatum,
                                PBearb = pp.PBearb,
                                PAuftr = pp.PAuftr,
                                PProg = pp.PProg,
                                PText = pp.PText,
                                Import = pp.Import,
                                LoeschDatum = pp.LoeschDatum
                            });
                        }
                        dbGlobal.Pp.RemoveRange(dbGlobal.Pp.Where(x => x.PAD == pad));
                       
                    }
                    break;
                case "Ph":
                    List<string> distinctedList1 = listToRemoveFromTemps.Distinct().ToList();
                    List<string> distinctedList2 = listToRemoveFromDatabase.Distinct().ToList();
                    foreach (string pad in distinctedList1)
                    {
                        dbGlobal.ImportPh.RemoveRange(dbGlobal.ImportPh.Where(x => x.PAD == pad));
                    }
                    foreach (string pad in distinctedList2)
                    {
                        List<Ph> ph = dbGlobal.Ph.Where(x => x.PAD == pad).ToList();
                        if (ph.Count != 0)
                        {
                            foreach (var newph in ph)
                            {
                                dbGlobal.GeloeschtPh.Add(new EntitiesDeleted.GeloeschtPh
                                {
                                    PAD = newph.PAD,
                                    HStat = newph.HStat,
                                    HSys = newph.HSys,
                                    HFremd = newph.HFremd,
                                    H = newph.H,
                                    MH = newph.MH,
                                    MHEXP = newph.MHEXP,
                                    HDatum = newph.HDatum,
                                    HBearb = newph.HBearb,
                                    HProg = newph.HProg,
                                    HAuftr = newph.HAuftr,
                                    HText = newph.HText,
                                    Import = newph.Import,
                                    LoeschDatum = newph.LoeschDatum
                                });
                                dbGlobal.Ph.RemoveRange(dbGlobal.Ph.Where(x => x.PAD == newph.PAD || x.HSys == newph.HSys));
                            }
                        }
                    }
                    break;
                case "Pk":
                    foreach (string pad in listToRemoveFromTemps)
                    {
                        dbGlobal.ImportPk.RemoveRange(dbGlobal.ImportPk.Where(x => x.PAD == pad));
                    }
                    foreach (string pad in listToRemoveFromDatabase)
                    {
                        Pk pk = dbGlobal.Pk.Where(x => x.PAD == pad).FirstOrDefault();
                        if (pk != null)
                        {
                            dbGlobal.GeloeschtPk.Add(new EntitiesDeleted.GeloeschtPk
                            {
                                PAD = pk.PAD,
                                KStat = pk.KStat,
                                KSys = pk.KSys,
                                HFremd = pk.HFremd,
                                X = pk.X.ToString(),
                                Y = pk.Y.ToString(),
                                Z = pk.Z.ToString(),
                                KBearb = pk.KBearb,
                                LProg = pk.LProg,
                                LAuftr = pk.LAuftr,
                                MP = pk.MP.ToString(),
                                MPEXP = pk.MPEXP.ToString(),
                                KText = pk.KText,
                                //KDatum = pk.KDatum,
                                Import = pk.Import,
                                LoeschDatum = pk.LoeschDatum
                            });
                        }
                        dbGlobal.Pk.RemoveRange(dbGlobal.Pk.Where(x => x.PAD == pad));
                    }
                    break;
                case "Pl":
                    foreach (string pad in listToRemoveFromTemps)
                    {
                        dbGlobal.ImportPl.RemoveRange(dbGlobal.ImportPl.Where(x => x.PAD == pad));
                    }
                    foreach (string pad in listToRemoveFromDatabase)
                    {
                        Pl pl = dbGlobal.Pl.Where(x => x.PAD == pad).FirstOrDefault();
                        if (pl != null)
                        {
                            dbGlobal.GeloeschtPl.Add(new EntitiesDeleted.GeloeschtPl
                            {

                                PAD = pl.PAD,
                                LStat = pl.LStat,
                                LSys = pl.LSys,
                                LFremd = pl.LFremd,
                                X = pl.X,
                                Y = pl.Y,
                                LBearb = pl.LBearb,
                                LProg = pl.LProg,
                                LAuftr = pl.LAuftr,
                                MP = (short?)pl.MP,
                                MPEXP = (short?)pl.MPEXP,
                                LText = pl.LText,
                                LDatum = pl.LDatum,
                                Import = pl.Import,
                                LoeschDatum = pl.LoeschDatum
                            });
                        }
                        dbGlobal.Pl.RemoveRange(dbGlobal.Pl.Where(x => x.PAD == pad));
                    }
                    break;
                case "Ps":
                    foreach (string pad in listToRemoveFromTemps)
                    {
                        dbGlobal.ImportPs.RemoveRange(dbGlobal.ImportPs.Where(x => x.PAD == pad));
                    }
                    foreach (string pad in listToRemoveFromDatabase)
                    {
                        Ps ps = dbGlobal.Ps.Where(x => x.PAD == pad).FirstOrDefault();
                        if (ps != null)
                        {
                            dbGlobal.GeloeschtPs.Add(new EntitiesDeleted.GeloeschtPs
                            {
                                PAD = ps.PAD,
                                PStrecke = ps.PStrecke,
                                PSTRRiKz = (short)ps.PSTRRiKz,
                                Station = ps.Station,
                                Import = ps.Import,
                                LoeschDatum = ps.LoeschDatum
                            });
                        }
                        dbGlobal.Ps.RemoveRange(dbGlobal.Ps.Where(x => x.PAD == pad));
                    }
                    break;
            }
           
            dbGlobal.SaveChanges();
        }
        public static void SaveDataInDataBase(EntityFrameworkContext dbGlobal, DataTable dataTableForTemp, string text)
        {
            PpsToSave = dbGlobal.ImportPp.ToList();
            foreach (ImportPp pp in PpsToSave)
            {
                dbGlobal.Pp.Add(new Entities.Pp
                {
                    PAD = pp.PAD,
                    PArt = pp.PArt,
                    Blattschnitt = pp.Blattschnitt,
                    PunktNr = pp.PunktNr,
                    VermArt = pp.VermArt,
                    Stabil = pp.Stabil,
                    PDatum = pp.PDatum,
                    PBearb = pp.PBearb,
                    PAuftr = pp.PAuftr,
                    PProg = pp.PProg,
                    PText = pp.PText,
                    Import = pp.Import,
                    LoeschDatum = pp.LoeschDatum

                });
            }

            PhsToSave = dbGlobal.ImportPh.ToList();
            foreach (ImportPh ph in PhsToSave)
            {
                dbGlobal.Ph.Add(new Entities.Ph
                {
                    PAD = ph.PAD,
                    HStat = ph.HStat,
                    HSys = ph.HSys,
                    HFremd = ph.HFremd,
                    H = ph.H,
                    MH = ph.MH,
                    MHEXP = ph.MHEXP,
                    HDatum = ph.HDatum,
                    HBearb = ph.HBearb,
                    HProg = ph.HProg,
                    HAuftr = ph.HAuftr,
                    HText = ph.HText,
                    Import = ph.Import,
                    LoeschDatum = ph.LoeschDatum
                });
            }

            PksToSave = dbGlobal.ImportPk.ToList();
            foreach (ImportPk pk in PksToSave)
            {
                dbGlobal.Pk.Add(new Entities.Pk
                {
                    PAD = pk.PAD,
                    KStat = pk.KStat,
                    KSys = pk.KSys,
                    HFremd = pk.HFremd,
                    X = pk.X.ToString(),
                    Y = pk.Y.ToString(),
                    Z = pk.Z.ToString(),
                    KBearb = pk.KBearb,
                    LProg = pk.LProg,
                    LAuftr = pk.LAuftr,
                    MP = pk.MP.ToString(),
                    MPEXP = pk.MPEXP.ToString(),
                    KText = pk.KText,
                    //KDatum = pk.KDatum,
                    Import = pk.Import,
                    LoeschDatum = pk.LoeschDatum

                });
            }

            PlsToSave = dbGlobal.ImportPl.ToList();
            foreach (ImportPl pl in PlsToSave)
            {
                dbGlobal.Pl.Add(new Entities.Pl
                {
                    PAD = pl.PAD,
                    LStat = pl.LStat,
                    LSys = pl.LSys,
                    LFremd = pl.LFremd,
                    X = pl.X,
                    Y = pl.Y,
                    LBearb = pl.LBearb,
                    LProg = pl.LProg,
                    LAuftr = pl.LAuftr,
                    MP = (short?)pl.MP,
                    MPEXP = (short?)pl.MPEXP,
                    LText = pl.LText,
                    LDatum = pl.LDatum,
                    Import = pl.Import,
                    LoeschDatum = pl.LoeschDatum

                });
            }

            PssToSave = dbGlobal.ImportPs.ToList();
            foreach (ImportPs ps in PssToSave)
            {
                dbGlobal.Ps.Add(new Entities.Ps
                {
                    PAD = ps.PAD,
                    PStrecke = ps.PStrecke,
                    PSTRRiKz = (short)ps.PSTRRiKz,
                    Station = ps.Station,
                    Import = ps.Import,
                    LoeschDatum = ps.LoeschDatum
                });
            }
            dbGlobal.SaveChanges();
            ClearTempTables(dbGlobal, dataTableForTemp, text);
        }

        #endregion

        #region Sketches: Import to the Temp Folder
        public static DataTable ImportSketchesInDataGrid(List<string> fileNames, List<string> sketchesNames, DataTable dataTableForSketches, string TestStringExtra = null)
        {
            dataTableForSketches.Columns.Add("PAD", typeof(string));
            dataTableForSketches.Columns.Add("PPT", typeof(bool));
            dataTableForSketches.Columns.Add("PDF", typeof(bool));
            dataTableForSketches.Columns.Add("JPG", typeof(bool));
            string mainPath;

            // Überprüfen ob der optionale Parameter TestStringExtra leer ist (wenn ja, dann import wie gewöhnlich, wenn nein, dann importieren für Test)
            if (string.IsNullOrEmpty(TestStringExtra))
            {
                mainPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\", "temp", "Importierte Skizzen");
            }
            else
            {
                mainPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\", "FestpunktDB.GUI\\temp", "Importierte Skizzen");
            }

            foreach (string filename in fileNames)
            {
                sketchesNames.Add((filename.Substring(filename.LastIndexOf(@"\"))).Substring(1, 11));

                //File.Copy(filename, mainPath + filename.Substring(filename.LastIndexOf(@"\")), true);

                DataRow row = dataTableForSketches.NewRow();
                row["PAD"] = filename.Substring(filename.LastIndexOf(@"\")).Substring(1, 11);
                row["PPT"] = filename.Contains("ppt") || filename.Contains("pptx") ? true : false;
                row["PDF"] = filename.Contains("pdf") ? true : false;
                row["JPG"] = filename.Contains("jpg") ? true : false;

                dataTableForSketches.Rows.Add(row);
            }
            return dataTableForSketches;
            
        }
        #endregion

        #region Sketches: Save Sketches In DataBase
        public static void SaveSketchesInDb(DataTable dataTableForSketches)
        {  
            var sPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\", "temp", "Importierte Skizzen");
            var tPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\", "temp", "Skizzen");
            List<string> allFilesInTheDirectory = Directory.GetFiles(sPath, "*.*", SearchOption.AllDirectories).ToList();
            foreach (string file in allFilesInTheDirectory)
            {
                string sketchNameFirst4Numbers = file[file.LastIndexOf(@"\")..].Substring(1, 4);
                string sourceFile = sPath + file[file.LastIndexOf(@"\")..];

                if (file.Contains("ppt") || file.Contains("pptx"))
                {
                    tPath += "\\PowerPoint";
                    switch (sketchNameFirst4Numbers)
                    {
                        case "1122":
                            tPath += "\\1122";
                            break;
                        case "6020":
                            tPath += "\\6020";
                            break;
                        case "6921":
                            tPath += "\\6921";
                            break;
                        case "6926":
                            tPath += "\\6926";
                            break;
                    }
                }
                else if (file.Contains("pdf"))
                {
                    tPath += "\\PDF";
                    switch (sketchNameFirst4Numbers)
                    {
                        case "1122":
                            tPath += "\\1122";
                            break;
                        case "6020":
                            tPath += "\\6020";
                            break;
                        case "6921":
                            tPath += "\\6921";
                            break;
                        case "6926":
                            tPath += "\\6926";
                            break;
                    }
                }
                else if (file.Contains("jpg"))
                {
                    tPath += "\\JPG";
                    switch (sketchNameFirst4Numbers)
                    {
                        case "1122":
                            tPath += "\\1122";
                            break;
                        case "6020":
                            tPath += "\\6020";
                            break;
                        case "6921":
                            tPath += "\\6921";
                            break;
                        case "6924":
                            tPath += "\\6924";
                            break;
                        case "6926":
                            tPath += "\\6926";
                            break;
                    }
                }
                string destinationFile = tPath + file[file.LastIndexOf(@"\", StringComparison.Ordinal)..];

                if (!Directory.Exists(tPath))
                {
                    Directory.CreateDirectory(tPath);
                }

                File.Move(sourceFile, destinationFile, true);

                sPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\", "temp", "Importierte Skizzen");
                tPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\", "temp", "Skizzen");

            }
            dataTableForSketches.Clear();
            foreach (var column in dataTableForSketches.Columns.Cast<DataColumn>().ToArray())
            {
                if (dataTableForSketches.AsEnumerable().All(dr => dr.IsNull(column)))
                    dataTableForSketches.Columns.Remove(column);
            }
        }
        #endregion

        #region Sketches: Check Sketches which are already in Database
        public static void CheckTheSketchesWhichAreAlreadyInDataBase(List<string> importedSketches, DataTable dataTableForMainDBSketches)
        {
            dataTableForMainDBSketches.Columns.Clear();
            dataTableForMainDBSketches.Rows.Clear();
            dataTableForMainDBSketches.Columns.Add("PAD", typeof(string));
            dataTableForMainDBSketches.Columns.Add("PPT", typeof(bool));
            dataTableForMainDBSketches.Columns.Add("PDF", typeof(bool));
            dataTableForMainDBSketches.Columns.Add("JPG", typeof(bool));

            var sPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\", "temp", "Skizzen\\");
            foreach (string sketchName in importedSketches)
            {
                string sketchNameWithoutPath = Path.GetFileNameWithoutExtension(sketchName);
                string first4NumbersOfTheSketch = sketchNameWithoutPath.Substring(0, 4);

                _jpgSketchesInDb = Directory.GetFiles(sPath + "JPG\\" + first4NumbersOfTheSketch + "\\", sketchNameWithoutPath + ".jpg", SearchOption.AllDirectories).ToList();
                _pdfSketchesInDb = Directory.GetFiles(sPath + "PDF\\" + first4NumbersOfTheSketch + "\\", sketchNameWithoutPath + ".pdf", SearchOption.AllDirectories).ToList();
                _pptSketchesInDb = Directory.GetFiles(sPath + "PowerPoint\\" + first4NumbersOfTheSketch + "\\", sketchNameWithoutPath + ".ppt", SearchOption.AllDirectories).ToList();

                DataRow row = dataTableForMainDBSketches.NewRow();
                row["PAD"] = sketchNameWithoutPath;
                row["JPG"] = _jpgSketchesInDb.Count() == 1 ? true : false;
                row["PDF"] = _pdfSketchesInDb.Count() == 1 ? true : false;
                row["PPT"] = _pptSketchesInDb.Count() == 1 ? true : false;

                dataTableForMainDBSketches.Rows.Add(row);
            }
        } 
        public static DataTable NewlyImportedSketchesInDG(List<string> fileNames, List<string> sketchesNames, DataTable dataTableForSketches)
        {
            var mainPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\", "temp", "Importierte Skizzen");
            foreach (var item in fileNames)
            {
                _jpgSketches = Directory.GetFiles(mainPath + "\\", Path.GetFileNameWithoutExtension(item) + ".jpg", SearchOption.AllDirectories).ToList();
                _pdfSketches = Directory.GetFiles(mainPath + "\\", Path.GetFileNameWithoutExtension(item) + ".pdf", SearchOption.AllDirectories).ToList();
                _pptSketches = Directory.GetFiles(mainPath + "\\", Path.GetFileNameWithoutExtension(item) + ".pptx", SearchOption.AllDirectories).ToList() ;
                      
                DataRow row = dataTableForSketches.NewRow();
                row["PAD"] = Path.GetFileNameWithoutExtension(item);
                row["JPG"] = _jpgSketches.Count() == 1 ? true : false;
                row["PDF"] = _pdfSketches.Count() == 1 ? true : false;
                row["PPT"] = _pptSketches.Count() == 1 ? true : false;

                dataTableForSketches.Rows.Add(row);
            }
            return dataTableForSketches;
        }
        #endregion
    }
}
