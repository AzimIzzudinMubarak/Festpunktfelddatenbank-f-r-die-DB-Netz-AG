using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.OleDb;
using System.Data;
using FestpunktDB.Business.Entities;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;


namespace FestpunktDB.Business.ExportService
{
    public static class Export
    {
        #region csv Export
        public static void ToCsvFile(List<Pp> Pp, List<Ph> Ph, List<Pk> Pk, List<Pl> Pl, List<Ps> Ps, string filePath)
        {
            int counterPk = 0;
            int counterPh = 0;
            int counterPl = 0;
            int counterPs = 0;

            if (Pp == null)
            {
                File.WriteAllText(filePath, "");
            }
            else
            {
                File.AppendAllText(filePath, "Punktadresse;PArt;Lsys;LageDatum;LBearb;LAuftr;LText;StationGISKm;RechtswertM2;HochwertM2;RechtswertM3;HochwertM3;RechtswertM4;HochwertM4;RechtswertM5;HochwertM5;ETRF_Xwert;ETRF_Ywert;ETRF_Zwert;HR0HöheM;HR0Datum;HR0Bea;HR0Auf;HR0Text;HO0HöheM;HO0Datum;HO0Bea;HO0Auf;HO0Text\n");

                for (int i = 0; i < Pp.Count; i++)
                {
                    //Declaration
                    string LSys = "0";
                    string LDatum = "0";
                    string LBearb = "0";
                    string LAuftr = "0";
                    string LText = "0";
                    string Station = "0";
                    string RWertM2 = "0";
                    string HWertM2 = "0";
                    string RWertM3 = "0";
                    string HWertM3 = "0";
                    string RWertM4 = "0";
                    string HWertM4 = "0";
                    string RWertM5 = "0";
                    string HWertM5 = "0";
                    string X = "0";
                    string Y = "0";
                    string Z = "0";
                    string HR0Höhe = "0";
                    string HR0Datum = "0";
                    string HR0Bea = "0";
                    string HR0Auf = "0";
                    string HR0Text = "0";
                    string HO0Höhe = "0";
                    string HO0Datum = "0";
                    string HO0Bea = "0";
                    string HO0Auf = "0";
                    string HO0Text = "0";



                    if (Pp[i].Pk.Count > 0 && Pk != null)
                    {
                        X = Pk[counterPk].X;
                        Y = Pk[counterPk].Y;
                        Z = Pk[counterPk].Z;
                        counterPk++;
                    }

                    if (Pp[i].Ph.Count > 0 && Ph != null)
                    {
                        if (Pp[i].Ph.Count > counterPh && Ph[counterPh].HSys == "R00")
                        {
                            HR0Höhe = Ph[counterPh].H.ToString();
                            HR0Datum = Ph[counterPh].HDatum;
                            HR0Bea = Ph[counterPh].HBearb;
                            HR0Auf = Ph[counterPh].HAuftr;
                            HR0Text = Ph[counterPh].HText;
                            counterPh++;

                            if (Pp[i].Ph.Count > counterPh && Ph[counterPh].HSys == "O00")
                            {
                                HO0Höhe = Ph[counterPh].H.ToString();
                                HO0Datum = Ph[counterPh].HDatum;
                                HO0Bea = Ph[counterPh].HBearb;
                                HO0Auf = Ph[counterPh].HAuftr;
                                HO0Text = Ph[counterPh].HText;
                                counterPh++;
                            }
                        }
                        else if (Pp[i].Ph.Count > counterPh && Ph[counterPh].HSys == "O00")
                        {
                            HO0Höhe = Ph[counterPh].H.ToString();
                            HO0Datum = Ph[counterPh].HDatum;
                            HO0Bea = Ph[counterPh].HBearb;
                            HO0Auf = Ph[counterPh].HAuftr;
                            HO0Text = Ph[counterPh].HText;
                            counterPh++;

                            if (Pp[i].Ph.Count > counterPh && Ph[counterPh].HSys == "R00")
                            {
                                HR0Höhe = Ph[counterPh].H.ToString();
                                HR0Datum = Ph[counterPh].HDatum;
                                HR0Bea = Ph[counterPh].HBearb;
                                HR0Auf = Ph[counterPh].HAuftr;
                                HR0Text = Ph[counterPh].HText;
                                counterPh++;
                            }
                        }
                    }

                    if (Pp[i].Pl.Count > 0 && Pl != null)
                    {
                        LSys = Pl[counterPl].LSys;
                        LDatum = Pl[counterPl].LDatum;
                        LBearb = Pl[counterPl].LBearb;
                        LAuftr = Pl[counterPl].LAuftr;
                        LText = Pl[counterPl].LText;

                        if (LSys == "G")
                        {
                            RWertM2 = Pl[counterPl].Y.ToString();
                            HWertM2 = Pl[counterPl].X.ToString();
                            counterPl++;

                            if (LSys == "DR0")
                            {
                                RWertM3 = Pl[counterPl].Y.ToString();
                                HWertM3 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "ER0")
                                {
                                    RWertM4 = Pl[counterPl].Y.ToString();
                                    HWertM4 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "FR0")
                                    {
                                        RWertM5 = Pl[counterPl].Y.ToString();
                                        HWertM5 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "FR0")
                                {
                                    RWertM5 = Pl[counterPl].Y.ToString();
                                    HWertM5 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "ER0")
                                    {
                                        RWertM4 = Pl[counterPl].Y.ToString();
                                        HWertM4 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "ER0")
                            {
                                RWertM4 = Pl[counterPl].Y.ToString();
                                HWertM4 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "DR0")
                                {
                                    RWertM3 = Pl[counterPl].Y.ToString();
                                    HWertM3 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "FR0")
                                    {
                                        RWertM5 = Pl[counterPl].Y.ToString();
                                        HWertM5 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "FR0")
                                {
                                    RWertM5 = Pl[counterPl].Y.ToString();
                                    HWertM5 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "DR0")
                                    {
                                        RWertM3 = Pl[counterPl].Y.ToString();
                                        HWertM3 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "FR0")
                            {
                                RWertM5 = Pl[counterPl].Y.ToString();
                                HWertM5 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "DR0")
                                {
                                    RWertM3 = Pl[counterPl].Y.ToString();
                                    HWertM3 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "ER0")
                                    {
                                        RWertM4 = Pl[counterPl].Y.ToString();
                                        HWertM4 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "ER0")
                                {
                                    RWertM4 = Pl[counterPl].Y.ToString();
                                    HWertM4 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "DR0")
                                    {
                                        RWertM3 = Pl[counterPl].Y.ToString();
                                        HWertM3 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                        }
                        else if (LSys == "DR0")
                        {
                            RWertM3 = Pl[counterPl].Y.ToString();
                            HWertM3 = Pl[counterPl].X.ToString();
                            counterPl++;

                            if (LSys == "G")
                            {
                                RWertM2 = Pl[counterPl].Y.ToString();
                                HWertM2 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "ER0")
                                {
                                    RWertM4 = Pl[counterPl].Y.ToString();
                                    HWertM4 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "FR0")
                                    {
                                        RWertM5 = Pl[counterPl].Y.ToString();
                                        HWertM5 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "FR0")
                                {
                                    RWertM5 = Pl[counterPl].Y.ToString();
                                    HWertM5 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "ER0")
                                    {
                                        RWertM4 = Pl[counterPl].Y.ToString();
                                        HWertM4 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "ER0")
                            {
                                RWertM4 = Pl[counterPl].Y.ToString();
                                HWertM4 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "G")
                                {
                                    RWertM2 = Pl[counterPl].Y.ToString();
                                    HWertM2 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "FR0")
                                    {
                                        RWertM5 = Pl[counterPl].Y.ToString();
                                        HWertM5 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "FR0")
                                {
                                    RWertM5 = Pl[counterPl].Y.ToString();
                                    HWertM5 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "G")
                                    {
                                        RWertM2 = Pl[counterPl].Y.ToString();
                                        HWertM2 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "FR0")
                            {
                                RWertM5 = Pl[counterPl].Y.ToString();
                                HWertM5 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "G")
                                {
                                    RWertM2 = Pl[counterPl].Y.ToString();
                                    HWertM2 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "ER0")
                                    {
                                        RWertM4 = Pl[counterPl].Y.ToString();
                                        HWertM4 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "ER0")
                                {
                                    RWertM4 = Pl[counterPl].Y.ToString();
                                    HWertM4 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "G")
                                    {
                                        RWertM2 = Pl[counterPl].Y.ToString();
                                        HWertM2 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                        }
                        else if (LSys == "ER0")
                        {
                            RWertM4 = Pl[counterPl].Y.ToString();
                            HWertM4 = Pl[counterPl].X.ToString();
                            counterPl++;

                            if (LSys == "G")
                            {
                                RWertM2 = Pl[counterPl].Y.ToString();
                                HWertM2 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "DR0")
                                {
                                    RWertM3 = Pl[counterPl].Y.ToString();
                                    HWertM3 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "FR0")
                                    {
                                        RWertM5 = Pl[counterPl].Y.ToString();
                                        HWertM5 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "FR0")
                                {
                                    RWertM5 = Pl[counterPl].Y.ToString();
                                    HWertM5 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "DR0")
                                    {
                                        RWertM3 = Pl[counterPl].Y.ToString();
                                        HWertM3 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "DR0")
                            {
                                RWertM3 = Pl[counterPl].Y.ToString();
                                HWertM3 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "G")
                                {
                                    RWertM2 = Pl[counterPl].Y.ToString();
                                    HWertM2 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "FR0")
                                    {
                                        RWertM5 = Pl[counterPl].Y.ToString();
                                        HWertM5 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "FR0")
                                {
                                    RWertM5 = Pl[counterPl].Y.ToString();
                                    HWertM5 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "G")
                                    {
                                        RWertM2 = Pl[counterPl].Y.ToString();
                                        HWertM2 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "FR0")
                            {
                                RWertM5 = Pl[counterPl].Y.ToString();
                                HWertM5 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "G")
                                {
                                    RWertM2 = Pl[counterPl].Y.ToString();
                                    HWertM2 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "DR0")
                                    {
                                        RWertM3 = Pl[counterPl].Y.ToString();
                                        HWertM3 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "DR0")
                                {
                                    RWertM3 = Pl[counterPl].Y.ToString();
                                    HWertM3 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "G")
                                    {
                                        RWertM2 = Pl[counterPl].Y.ToString();
                                        HWertM2 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                        }
                        else if (LSys == "FR0")
                        {
                            RWertM5 = Pl[counterPl].Y.ToString();
                            HWertM5 = Pl[counterPl].X.ToString();
                            counterPl++;

                            if (LSys == "G")
                            {
                                RWertM2 = Pl[counterPl].Y.ToString();
                                HWertM2 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "DR0")
                                {
                                    RWertM3 = Pl[counterPl].Y.ToString();
                                    HWertM3 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "ER0")
                                    {
                                        RWertM4 = Pl[counterPl].Y.ToString();
                                        HWertM4 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "ER0")
                                {
                                    RWertM4 = Pl[counterPl].Y.ToString();
                                    HWertM4 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "DR0")
                                    {
                                        RWertM3 = Pl[counterPl].Y.ToString();
                                        HWertM3 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "DR0")
                            {
                                RWertM3 = Pl[counterPl].Y.ToString();
                                HWertM3 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "G")
                                {
                                    RWertM2 = Pl[counterPl].Y.ToString();
                                    HWertM2 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "ER0")
                                    {
                                        RWertM4 = Pl[counterPl].Y.ToString();
                                        HWertM4 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "ER0")
                                {
                                    RWertM4 = Pl[counterPl].Y.ToString();
                                    HWertM4 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "G")
                                    {
                                        RWertM2 = Pl[counterPl].Y.ToString();
                                        HWertM2 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "ER0")
                            {
                                RWertM4 = Pl[counterPl].Y.ToString();
                                HWertM4 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "G")
                                {
                                    RWertM2 = Pl[counterPl].Y.ToString();
                                    HWertM2 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "DR0")
                                    {
                                        RWertM3 = Pl[counterPl].Y.ToString();
                                        HWertM3 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "DR0")
                                {
                                    RWertM3 = Pl[counterPl].Y.ToString();
                                    HWertM3 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "G")
                                    {
                                        RWertM2 = Pl[counterPl].Y.ToString();
                                        HWertM2 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                        }
                    }

                    if (Pp[i].Ps.Count > 0 && Ps != null)
                    {
                        Station = Ps[counterPs].Station.ToString();
                        counterPs++;
                    }

                    File.AppendAllText(filePath, $"{Pp[i].PAD};{Pp[i].PArt};{LSys};{LDatum};{LBearb};{LAuftr};{LText};{Station};{RWertM2};{HWertM2};{RWertM3};{HWertM3};{RWertM4};{HWertM4};{RWertM5};{HWertM5};{X};{Y};{Z};{HR0Höhe};{HR0Datum};{HR0Bea};{HR0Auf};{HR0Text};{HO0Höhe};{HO0Datum};{HO0Bea};{HO0Auf};{HO0Text}\n");

                }
            }
        }

        public static void ToExcelFileAuto(List<Pp> pp, List<Ph> ph, List<Pk> pk, List<Pl> pl, List<Ps> ps, object xlsTestFile)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region dbb Export
        public static void ExportDbb(List<Pp> Pp, List<Ph> Ph, List<Pl> Pl, List<Ps> Ps, string filePath)
        {
            int counterPh = 0;
            int counterPl = 0;
            int counterPs = 0;

            if (Pp == null || Ph == null || Ps == null)
            {
                File.WriteAllText(filePath, "");
            }
            else
            {
                for (int i = 0; i < Pp.Count; i++)
                {
                    string Blattschnitt = "";
                    string PunktNr = "";
                    string PArt = "";
                    string VermArt = "";
                    string Station = "";
                    string PDatum = "";
                    string PBearb = "";
                    string PAuftrag = "";
                    string PProg = "";
                    string PText = "";
                    string PStrecke = "";
                    string PSTRRiKz = "";
                    string LSys = "";
                    string PlY = "";
                    string PlX = "";
                    string Wert1 = "0000-0";
                    string LDatum = "";
                    string LBearb = "";
                    string LAuftrag = "";
                    string LProg = "";
                    string LText = "";
                    string HSys = "";
                    string PhH = "";
                    string Wert2 = "0000-0";
                    string HDatum = "";
                    string HBearb = "";
                    string HAuftrag = "";

                    if (Pp[i].Ph.Count > 0 && Pp[i].Pl.Count > 0 && Pp[i].Ps.Count > 0)
                    {
                        Blattschnitt = Pp[i].Blattschnitt;
                        PunktNr = Pp[i].PunktNr.ToString();
                        PArt = Pp[i].PArt;
                        VermArt = Pp[i].VermArt.ToString();
                        Station = Ps[counterPs].Station.ToString();
                        PDatum = Pp[i].PDatum.ToString();
                        PBearb = Pp[i].PBearb;
                        PAuftrag = Pp[i].PAuftr.ToString();
                        PProg = Pp[i].PProg;
                        PText = Pp[i].PText;
                        PStrecke = Ps[counterPs].PStrecke;
                        PSTRRiKz = Ps[counterPs].PSTRRiKz.ToString();
                        LSys = Pl[counterPl].LSys;
                        PlY = Pl[counterPl].Y.ToString();
                        PlX = Pl[counterPl].X.ToString();
                        LDatum = Pl[counterPl].LDatum.ToString();
                        LBearb = Pl[counterPl].LBearb;
                        LAuftrag = Pl[counterPl].LAuftr;
                        LProg = Pl[counterPl].LProg;
                        LText = Pl[counterPl].LText;
                        HSys = Ph[counterPh].HSys;
                        PhH = Ph[counterPh].H.ToString();
                        HDatum = Ph[counterPh].HDatum.ToString();
                        HBearb = Ph[counterPh].HBearb;
                        HAuftrag = Ph[counterPh].HAuftr;

                        File.AppendAllText(filePath, string.Format(
                            "11{0, -7}{1, 4}{2, -4}{3, 3}{4, -13}{5, -6}{6, -8}{7, -8}{8, -5}{9, -20}{10, -4}{11, -1}\n12{12, -7}{13, 4}{14, 4}               {15, -14}{16, -12}{17, 6}{18, -8}{19, -8}{20, -11}{21, -5}{22, -20}\n13{23, -7}{24, 4}{25, 4}                    {26, -7}{27, 6}{28, -8}{29, -5}{30, -8}{31, -5}{32, -17}{33, -4}\n"
                            , Blattschnitt, PunktNr, PArt, VermArt, Station, PDatum, PBearb, PAuftrag, PProg, PText, PStrecke, PSTRRiKz,
                            Blattschnitt, PunktNr, LSys, PlY, PlX, Wert1, LDatum, LBearb, LAuftrag, LProg, LText,
                            Blattschnitt, PunktNr, HSys, PhH, Wert2, HDatum, HBearb, HAuftrag, PProg, PText.Substring(0, 7), LSys)); ;

                        /*File.AppendAllText(filePath, string.Format(
                            "11{0, -7}{1, 4}{2, -4}{3, 3}{4, -13}{5, -6}{6, -8}{7, -8}{8, -5}{9, -20}{10, -4}{11, -1}\n" +
                            "12{12, -7}{13, 4}{14, 4}               {15, -14}{16, -12}{17, 6}{18, -8}{19, -8}{20, -11}{21, -5}{22, -20}\n" +
                            "13{23, -7}{24, 4}{25, 4}                    {26, -7}{27, 6}{28, -8}{29, -5}{30, -8}{31, -5}{32, -17}{33, -4}\n"
                            , Blattschnitt, PunktNr, PArt, VermArt, Station, PDatum, PBearb, PAuftrag, PProg, PText, PStrecke, PSTRRiKz,
                            Blattschnitt, PunktNr, LSys, PlY, PlX, Wert1, LDatum, LBearb, LAuftrag, LProg, LText,
                            Blattschnitt, PunktNr, HSys, PhH, Wert2, HDatum, HBearb, HAuftrag, PProg, PText.Substring(0, 7), LSys)); ;*/
                    }

                    if (Pp[i].Ph.Count > 0)
                    {
                        counterPh++;
                    }

                    if (Pp[i].Pl.Count > 0)
                    {
                        counterPl++;
                    }

                    if (Pp[i].Ps.Count > 0)
                    {
                        counterPs++;
                    }
                }
            }
        }

        #endregion

        #region nap Export
        public static void ExportNap(List<Pp> Pp, List<Ph> Ph, List<Pk> Pk, List<Pl> Pl, List<Ps> Ps, string filePath)
        {
            int counterPk = 0;
            int counterPh = 0;
            int counterPl = 0;
            int counterPs = 0;

            if (Pp == null)
            {
                File.WriteAllText(filePath, "");
            }
            else
            {
                for (int i = 0; i < Pp.Count; i++)
                {
                    string Wert1 = "4";
                    string PAD = Pp[i].PAD;
                    string PkX = "0000000,0000";
                    string PkY = "000000,0000";
                    string PkZ = "0000000,0000";
                    string PkMP = "0,0000";
                    string PlMP = "0,0000";
                    string Wert2 = "0,0000";
                    string PkMPEXP = "+0,0000";
                    string Wert3 = "+0,0000";
                    string Wert4 = "+0,0000";
                    string PArt = Pp[i].PArt;
                    string GPSC = "GPSC";
                    string PlY = "0000000,0000";
                    string PlX = "0000000,0000";
                    string PhH = "+00,0000";
                    string Wert5 = "0,0000";
                    string Wert6 = "0,0000";
                    string Wert7 = "+0,0000";
                    string Wert8 = "+0,0000";
                    string Wert9 = "+0,0000";
                    string PsStrecke = "0000";
                    string PsSTRRiKz = "0";
                    string StationKm = "000,0";
                    string StationM = "00.00";
                    string Wert10 = "+00.00";
                    string PDatum = Pp[i].PDatum;
                    string Wert11 = "000000";
                    string Wert12 = "0";
                    string PText = Pp[i].PText;



                    if (Pp[i].Pk.Count > 0 && Pk != null)
                    {
                        PkX = Pk[counterPk].X;
                        PkY = Pk[counterPk].Y;
                        PkZ = Pk[counterPk].Z;
                        PkMP = Pk[counterPk].MP;
                        PkMPEXP = Pk[counterPk].MP;

                        counterPk++;
                    }
                    if (Pp[i].Ph.Count > 0 && Ph != null)
                    {
                        PhH = Ph[counterPh].H.ToString();

                        counterPh++;
                    }
                    if (Pp[i].Pl.Count > 0 && Pl != null)
                    {
                        PlMP = Pl[counterPl].MP.ToString();
                        PlY = Pl[counterPl].Y.ToString();
                        PlX = Pl[counterPl].X.ToString();

                        counterPl++;
                    }
                    if (Pp[i].Ps.Count > 0 && Ps != null)
                    {
                        PsStrecke = Ps[counterPs].PStrecke;
                        PsSTRRiKz = Ps[counterPs].PSTRRiKz.ToString();

                        counterPs++;
                    }

                    if (Pp[i].Pk.Count == 0)
                    {
                        File.AppendAllText(filePath, string.Format(
                            " {0, 1}           {1, -11}       {2, -12}  {3, -11} {4, -12}    {5, -6}    {6, -6}    {7, -6} {8, -7}    {9, -7}    {10, -7}    {11, -3} {12, -4} {13, -12} {14, -12} {15, -8}     {16, -6}    {17, -6}    {18, -6} {19, -7}    {20, -7}    {21, -7}    {22, -4} {23, -1}   {24, -4}   {25, -5} {26, -5}     {27, -8}   {28, -6} {29, -1} {30, -1} {31, -7}\n"
                            , Wert1, PAD, PkX, PkY, PkZ, PkMP, PlMP, Wert2, PkMPEXP, Wert3, Wert4, PArt, GPSC, PlY, PlX, PhH, PlMP, Wert5, Wert6, Wert7, Wert8, Wert9, PsStrecke, PsSTRRiKz, StationKm, StationM, Wert10, PDatum, Wert11, PsSTRRiKz ,Wert12, PText));

                        /*File.AppendAllText(filePath, string.Format(
                            " {0, 1}           {1, -11}       {2, -12}  {3, -11} {4, -12}    {5, -6}    {6, -6}    {7, -6} {8, -7}    {9, -7}    {10, -7}    {11, -3} {12, -4} {13, -12} {14, -12} {15, -8}     {16, -6}    {17, -6}    {18, -6}" +
                            " {19, -7}    {20, -7}    {21, -7}    {22, -4} {23, -1}   {24, -4}   {25, -5} {26, -5}     {27, -8}   {28, -6} {29, -1} {30, -1} {31, -7}\n"
                            , Wert1, PAD, PkX, PkY, PkZ, PkMP, PlMP, Wert2, PkMPEXP, Wert3, Wert4, PArt, GPSC, PlY, PlX, PhH, PlMP, Wert5, Wert6, Wert7, Wert8, Wert9, PsStrecke, PsSTRRiKz, StationKm, StationM, Wert10, PDatum, Wert11, PsSTRRiKz, Wert12, PText));*/
                    }
                }
            }
        }

        #endregion

        #region xls and xlsx Export
        public static System.Data.DataTable ToDataTable<T>(List<T> items)
        {
            var dataTable = new System.Data.DataTable(typeof(T).Name);

            //Get all the properties
            var properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in properties)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (var item in items)
            {
                var values = new object[properties.Length];
                for (var i = 0; i < properties.Length; i++)
                {
                    //inserting property values to data table rows
                    values[i] = properties[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check data table
            return dataTable;
        }

        public static void ToExcelFile(System.Data.DataTable dataTablePp, System.Data.DataTable dataTablePh, System.Data.DataTable dataTablePk, System.Data.DataTable dataTablePl, System.Data.DataTable dataTablePs, string filePath, bool overwriteFile = true)
        {
            if (File.Exists(filePath) && overwriteFile)
                File.Delete(filePath);

            using (var connection = new OleDbConnection())
            {
                connection.ConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};" +
                                              "Extended Properties='Excel 12.0 Xml;HDR=YES;'";
                connection.Open();
                using (var command = new OleDbCommand())
                {
                    command.Connection = connection;
                    if (dataTablePp != null)
                    {
                        var columnNamesPp = (from DataColumn dataColumn in dataTablePp.Columns where dataColumn.ColumnName != "Pk" && dataColumn.ColumnName != "Ps" && dataColumn.ColumnName != "Ph" && dataColumn.ColumnName != "Pl" select dataColumn.ColumnName).ToList();
                        var tableNamePp = !string.IsNullOrWhiteSpace(dataTablePp.TableName) ? dataTablePp.TableName : Guid.NewGuid().ToString();
                        command.CommandText = $"CREATE TABLE [{tableNamePp}] ({string.Join(",", columnNamesPp.Select(c => $"[{c}] VARCHAR").ToArray())});";
                        command.ExecuteNonQuery();
                        foreach (DataRow row in dataTablePp.Rows)
                        {
                            var rowValues = (from DataColumn column in dataTablePp.Columns where column.ColumnName != "Pk" && column.ColumnName != "Ps" && column.ColumnName != "Ph" && column.ColumnName != "Pl" select (row[column] != null && row[column] != DBNull.Value) ? row[column].ToString() : string.Empty).ToList();
                            command.CommandText = $"INSERT INTO [{tableNamePp}]({string.Join(",", columnNamesPp.Select(c => $"[{c}]"))}) VALUES ({string.Join(",", rowValues.Select(r => $"'{r}'").ToArray())});";
                            command.ExecuteNonQuery();
                        }
                    }

                    if (dataTablePh != null)
                    {
                        var columnNamesPh = (from DataColumn dataColumn in dataTablePh.Columns where dataColumn.ColumnName != "PadNavigation" && dataColumn.ColumnName != "Pk" && dataColumn.ColumnName != "Ps" && dataColumn.ColumnName != "Ph" && dataColumn.ColumnName != "Pl" select dataColumn.ColumnName).ToList();
                        var tableNamePh = !string.IsNullOrWhiteSpace(dataTablePh.TableName) ? dataTablePh.TableName : Guid.NewGuid().ToString();
                        command.CommandText = $"CREATE TABLE [{tableNamePh}] ({string.Join(",", columnNamesPh.Select(c => $"[{c}] VARCHAR").ToArray())});";
                        command.ExecuteNonQuery();
                        foreach (DataRow row in dataTablePh.Rows)
                        {
                            var rowValues = (from DataColumn column in dataTablePh.Columns where column.ColumnName != "PadNavigation" && column.ColumnName != "Pk" && column.ColumnName != "Ps" && column.ColumnName != "Ph" && column.ColumnName != "Pl" select (row[column] != null && row[column] != DBNull.Value) ? row[column].ToString() : string.Empty).ToList();
                            command.CommandText = $"INSERT INTO [{tableNamePh}]({string.Join(",", columnNamesPh.Select(c => $"[{c}]"))}) VALUES ({string.Join(",", rowValues.Select(r => $"'{r}'").ToArray())});";
                            command.ExecuteNonQuery();
                        }
                    }

                    if (dataTablePk != null)
                    {
                        var columnNamesPk = (from DataColumn dataColumn in dataTablePk.Columns where dataColumn.ColumnName != "PadNavigation" && dataColumn.ColumnName != "Pk" && dataColumn.ColumnName != "Ps" && dataColumn.ColumnName != "Ph" && dataColumn.ColumnName != "Pl" select dataColumn.ColumnName).ToList();
                        var tableNamePk = !string.IsNullOrWhiteSpace(dataTablePk.TableName) ? dataTablePk.TableName : Guid.NewGuid().ToString();
                        command.CommandText = $"CREATE TABLE [{tableNamePk}] ({string.Join(",", columnNamesPk.Select(c => $"[{c}] VARCHAR").ToArray())});";
                        command.ExecuteNonQuery();
                        foreach (DataRow row in dataTablePk.Rows)
                        {
                            var rowValues = (from DataColumn column in dataTablePk.Columns where column.ColumnName != "PadNavigation" && column.ColumnName != "Pk" && column.ColumnName != "Ps" && column.ColumnName != "Ph" && column.ColumnName != "Pl" select (row[column] != null && row[column] != DBNull.Value) ? row[column].ToString() : string.Empty).ToList();
                            command.CommandText = $"INSERT INTO [{tableNamePk}]({string.Join(",", columnNamesPk.Select(c => $"[{c}]"))}) VALUES ({string.Join(",", rowValues.Select(r => $"'{r}'").ToArray())});";
                            command.ExecuteNonQuery();
                        }
                    }

                    if (dataTablePl != null)
                    {
                        var columnNamesPl = (from DataColumn dataColumn in dataTablePl.Columns where dataColumn.ColumnName != "PadNavigation" && dataColumn.ColumnName != "Pk" && dataColumn.ColumnName != "Ps" && dataColumn.ColumnName != "Ph" && dataColumn.ColumnName != "Pl" select dataColumn.ColumnName).ToList();
                        var tableNamePl = !string.IsNullOrWhiteSpace(dataTablePl.TableName) ? dataTablePl.TableName : Guid.NewGuid().ToString();
                        command.CommandText = $"CREATE TABLE [{tableNamePl}] ({string.Join(",", columnNamesPl.Select(c => $"[{c}] VARCHAR").ToArray())});";
                        command.ExecuteNonQuery();
                        foreach (DataRow row in dataTablePl.Rows)
                        {
                            var rowValues = (from DataColumn column in dataTablePl.Columns where column.ColumnName != "PadNavigation" && column.ColumnName != "Pk" && column.ColumnName != "Ps" && column.ColumnName != "Ph" && column.ColumnName != "Pl" select (row[column] != null && row[column] != DBNull.Value) ? row[column].ToString() : string.Empty).ToList();
                            command.CommandText = $"INSERT INTO [{tableNamePl}]({string.Join(",", columnNamesPl.Select(c => $"[{c}]"))}) VALUES ({string.Join(",", rowValues.Select(r => $"'{r}'").ToArray())});";
                            command.ExecuteNonQuery();
                        }
                    }

                    if (dataTablePs != null)
                    {
                        var columnNamesPs = (from DataColumn dataColumn in dataTablePs.Columns where dataColumn.ColumnName != "PadNavigation" && dataColumn.ColumnName != "Pk" && dataColumn.ColumnName != "Ps" && dataColumn.ColumnName != "Ph" && dataColumn.ColumnName != "Pl" select dataColumn.ColumnName).ToList();
                        var tableNamePs = !string.IsNullOrWhiteSpace(dataTablePs.TableName) ? dataTablePs.TableName : Guid.NewGuid().ToString();
                        command.CommandText = $"CREATE TABLE [{tableNamePs}] ({string.Join(",", columnNamesPs.Select(c => $"[{c}] VARCHAR").ToArray())});";
                        command.ExecuteNonQuery();
                        foreach (DataRow row in dataTablePs.Rows)
                        {
                            var rowValues = (from DataColumn column in dataTablePs.Columns where column.ColumnName != "PadNavigation" && column.ColumnName != "Pk" && column.ColumnName != "Ps" && column.ColumnName != "Ph" && column.ColumnName != "Pl" select (row[column] != null && row[column] != DBNull.Value) ? row[column].ToString() : string.Empty).ToList();
                            command.CommandText = $"INSERT INTO [{tableNamePs}]({string.Join(",", columnNamesPs.Select(c => $"[{c}]"))}) VALUES ({string.Join(",", rowValues.Select(r => $"'{r}'").ToArray())});";
                            command.ExecuteNonQuery();
                        }
                    }
                }


                connection.Close();
            }
        }

        public static void ToExcelFileAuto(List<Pp> Pp, List<Ph> Ph, List<Pk> Pk, List<Pl> Pl, List<Ps> Ps, string filePath)
        {
            int counterPk = 0;
            int counterPh = 0;
            int counterPl = 0;
            int counterPs = 0;

            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create($@"{filePath}", SpreadsheetDocumentType.Workbook))
            {

                WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet();

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "ExcelAutoDat" };

                sheets.Append(sheet);

                workbookPart.Workbook.Save();


                SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                // Constructing header
                Row row = new Row();

                row.Append(
                    ConstructCell("Punktadresse", CellValues.String),
                    ConstructCell("PArt", CellValues.String),
                    ConstructCell("LSys", CellValues.String),
                    ConstructCell("LageDatum", CellValues.String),
                    ConstructCell("LBearb", CellValues.String),
                    ConstructCell("LAuftr", CellValues.String),
                    ConstructCell("LText", CellValues.String),
                    ConstructCell("StationGISKm", CellValues.String),
                    ConstructCell("RechtswertM2", CellValues.String),
                    ConstructCell("HochwertM2", CellValues.String),
                    ConstructCell("RechtswertM3", CellValues.String),
                    ConstructCell("HochwertM3", CellValues.String),
                    ConstructCell("RechtswertM4", CellValues.String),
                    ConstructCell("HochwertM4", CellValues.String),
                    ConstructCell("RechtswertM5", CellValues.String),
                    ConstructCell("HochwertM5", CellValues.String),
                    ConstructCell("ETRF_Xwert", CellValues.String),
                    ConstructCell("ETRF_Ywert", CellValues.String),
                    ConstructCell("ETRF_Zwert", CellValues.String),
                    ConstructCell("HR0HöheM", CellValues.String),
                    ConstructCell("HR0Datum", CellValues.String),
                    ConstructCell("HR0Bea", CellValues.String),
                    ConstructCell("HR0Auf", CellValues.String),
                    ConstructCell("HR0Text", CellValues.String),
                    ConstructCell("HO0HöheM", CellValues.String),
                    ConstructCell("HO0Datum", CellValues.String),
                    ConstructCell("HO0Bea", CellValues.String),
                    ConstructCell("HO0Auf", CellValues.String),
                    ConstructCell("HO0Text", CellValues.String));

                // Insert the header row to the Sheet Data
                sheetData.AppendChild(row);

                for (int i = 0; Pp.Count > i; i++)
                {
                    //Declaration
                    string LSys = "0";
                    string LDatum = "0";
                    string LBearb = "0";
                    string LAuftr = "0";
                    string LText = "0";
                    string Station = "0";
                    string RWertM2 = "0";
                    string HWertM2 = "0";
                    string RWertM3 = "0";
                    string HWertM3 = "0";
                    string RWertM4 = "0";
                    string HWertM4 = "0";
                    string RWertM5 = "0";
                    string HWertM5 = "0";
                    string X = "0";
                    string Y = "0";
                    string Z = "0";
                    string HR0Höhe = "0";
                    string HR0Datum = "0";
                    string HR0Bea = "0";
                    string HR0Auf = "0";
                    string HR0Text = "0";
                    string HO0Höhe = "0";
                    string HO0Datum = "0";
                    string HO0Bea = "0";
                    string HO0Auf = "0";
                    string HO0Text = "0";

                    row = new Row();

                    if(Pp[i].Pk.Count > 0 && Pk != null)
                    {
                        X = Pk[counterPk].X;
                        Y = Pk[counterPk].Y;
                        Z = Pk[counterPk].Z;
                        counterPk++;
                    }

                    if(Pp[i].Ph.Count > 0 && Ph != null)
                    {
                        if (Pp[i].Ph.Count > counterPh && Ph[counterPh].HSys == "R00")
                        {
                            HR0Höhe = Ph[counterPh].H.ToString();
                            HR0Datum = Ph[counterPh].HDatum;
                            HR0Bea = Ph[counterPh].HBearb;
                            HR0Auf = Ph[counterPh].HAuftr;
                            HR0Text = Ph[counterPh].HText;
                            counterPh++;

                            if (Pp[i].Ph.Count > counterPh && Ph[counterPh].HSys == "O00")
                            {
                                HO0Höhe = Ph[counterPh].H.ToString();
                                HO0Datum = Ph[counterPh].HDatum;
                                HO0Bea = Ph[counterPh].HBearb;
                                HO0Auf = Ph[counterPh].HAuftr;
                                HO0Text = Ph[counterPh].HText;
                                counterPh++;
                            }
                        }
                        else if (Pp[i].Ph.Count > counterPh && Ph[counterPh].HSys == "O00")
                        {
                            HO0Höhe = Ph[counterPh].H.ToString();
                            HO0Datum = Ph[counterPh].HDatum;
                            HO0Bea = Ph[counterPh].HBearb;
                            HO0Auf = Ph[counterPh].HAuftr;
                            HO0Text = Ph[counterPh].HText;
                            counterPh++;

                            if (Pp[i].Ph.Count > counterPh && Ph[counterPh].HSys == "R00")
                            {
                                HR0Höhe = Ph[counterPh].H.ToString();
                                HR0Datum = Ph[counterPh].HDatum;
                                HR0Bea = Ph[counterPh].HBearb;
                                HR0Auf = Ph[counterPh].HAuftr;
                                HR0Text = Ph[counterPh].HText;
                                counterPh++;
                            }
                        }
                    }

                    if(Pp[i].Pl.Count > 0 && Pl != null)
                    {
                        LSys = Pl[counterPl].LSys;
                        LDatum = Pl[counterPl].LDatum;
                        LBearb = Pl[counterPl].LBearb;
                        LAuftr = Pl[counterPl].LAuftr;
                        LText = Pl[counterPl].LText;

                        if(LSys == "G")
                        {
                            RWertM2 = Pl[counterPl].Y.ToString();
                            HWertM2 = Pl[counterPl].X.ToString();
                            counterPl++;

                            if (LSys == "DR0")
                            {
                                RWertM3 = Pl[counterPl].Y.ToString();
                                HWertM3 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "ER0")
                                {
                                    RWertM4 = Pl[counterPl].Y.ToString();
                                    HWertM4 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "FR0")
                                    {
                                        RWertM5 = Pl[counterPl].Y.ToString();
                                        HWertM5 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "FR0")
                                {
                                    RWertM5 = Pl[counterPl].Y.ToString();
                                    HWertM5 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "ER0")
                                    {
                                        RWertM4 = Pl[counterPl].Y.ToString();
                                        HWertM4 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "ER0")
                            {
                                RWertM4 = Pl[counterPl].Y.ToString();
                                HWertM4 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "DR0")
                                {
                                    RWertM3 = Pl[counterPl].Y.ToString();
                                    HWertM3 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "FR0")
                                    {
                                        RWertM5 = Pl[counterPl].Y.ToString();
                                        HWertM5 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "FR0")
                                {
                                    RWertM5 = Pl[counterPl].Y.ToString();
                                    HWertM5 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "DR0")
                                    {
                                        RWertM3 = Pl[counterPl].Y.ToString();
                                        HWertM3 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "FR0")
                            {
                                RWertM5 = Pl[counterPl].Y.ToString();
                                HWertM5 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "DR0")
                                {
                                    RWertM3 = Pl[counterPl].Y.ToString();
                                    HWertM3 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "ER0")
                                    {
                                        RWertM4 = Pl[counterPl].Y.ToString();
                                        HWertM4 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "ER0")
                                {
                                    RWertM4 = Pl[counterPl].Y.ToString();
                                    HWertM4 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "DR0")
                                    {
                                        RWertM3 = Pl[counterPl].Y.ToString();
                                        HWertM3 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                        }
                        else if (LSys == "DR0")
                        {
                            RWertM3 = Pl[counterPl].Y.ToString();
                            HWertM3 = Pl[counterPl].X.ToString();
                            counterPl++;

                            if (LSys == "G")
                            {
                                RWertM2 = Pl[counterPl].Y.ToString();
                                HWertM2 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "ER0")
                                {
                                    RWertM4 = Pl[counterPl].Y.ToString();
                                    HWertM4 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "FR0")
                                    {
                                        RWertM5 = Pl[counterPl].Y.ToString();
                                        HWertM5 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "FR0")
                                {
                                    RWertM5 = Pl[counterPl].Y.ToString();
                                    HWertM5 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "ER0")
                                    {
                                        RWertM4 = Pl[counterPl].Y.ToString();
                                        HWertM4 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "ER0")
                            {
                                RWertM4 = Pl[counterPl].Y.ToString();
                                HWertM4 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "G")
                                {
                                    RWertM2 = Pl[counterPl].Y.ToString();
                                    HWertM2 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "FR0")
                                    {
                                        RWertM5 = Pl[counterPl].Y.ToString();
                                        HWertM5 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "FR0")
                                {
                                    RWertM5 = Pl[counterPl].Y.ToString();
                                    HWertM5 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "G")
                                    {
                                        RWertM2 = Pl[counterPl].Y.ToString();
                                        HWertM2 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "FR0")
                            {
                                RWertM5 = Pl[counterPl].Y.ToString();
                                HWertM5 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "G")
                                {
                                    RWertM2 = Pl[counterPl].Y.ToString();
                                    HWertM2 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "ER0")
                                    {
                                        RWertM4 = Pl[counterPl].Y.ToString();
                                        HWertM4 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "ER0")
                                {
                                    RWertM4 = Pl[counterPl].Y.ToString();
                                    HWertM4 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "G")
                                    {
                                        RWertM2 = Pl[counterPl].Y.ToString();
                                        HWertM2 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                        }
                        else if (LSys == "ER0")
                        {
                            RWertM4 = Pl[counterPl].Y.ToString();
                            HWertM4 = Pl[counterPl].X.ToString();
                            counterPl++;

                            if (LSys == "G")
                            {
                                RWertM2 = Pl[counterPl].Y.ToString();
                                HWertM2 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "DR0")
                                {
                                    RWertM3 = Pl[counterPl].Y.ToString();
                                    HWertM3 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "FR0")
                                    {
                                        RWertM5 = Pl[counterPl].Y.ToString();
                                        HWertM5 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "FR0")
                                {
                                    RWertM5 = Pl[counterPl].Y.ToString();
                                    HWertM5 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "DR0")
                                    {
                                        RWertM3 = Pl[counterPl].Y.ToString();
                                        HWertM3 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "DR0")
                            {
                                RWertM3 = Pl[counterPl].Y.ToString();
                                HWertM3 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "G")
                                {
                                    RWertM2 = Pl[counterPl].Y.ToString();
                                    HWertM2 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "FR0")
                                    {
                                        RWertM5 = Pl[counterPl].Y.ToString();
                                        HWertM5 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "FR0")
                                {
                                    RWertM5 = Pl[counterPl].Y.ToString();
                                    HWertM5 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "G")
                                    {
                                        RWertM2 = Pl[counterPl].Y.ToString();
                                        HWertM2 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "FR0")
                            {
                                RWertM5 = Pl[counterPl].Y.ToString();
                                HWertM5 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "G")
                                {
                                    RWertM2 = Pl[counterPl].Y.ToString();
                                    HWertM2 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "DR0")
                                    {
                                        RWertM3 = Pl[counterPl].Y.ToString();
                                        HWertM3 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "DR0")
                                {
                                    RWertM3 = Pl[counterPl].Y.ToString();
                                    HWertM3 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "G")
                                    {
                                        RWertM2 = Pl[counterPl].Y.ToString();
                                        HWertM2 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                        }
                        else if (LSys == "FR0")
                        {
                            RWertM5 = Pl[counterPl].Y.ToString();
                            HWertM5 = Pl[counterPl].X.ToString();
                            counterPl++;

                            if (LSys == "G")
                            {
                                RWertM2 = Pl[counterPl].Y.ToString();
                                HWertM2 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "DR0")
                                {
                                    RWertM3 = Pl[counterPl].Y.ToString();
                                    HWertM3 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "ER0")
                                    {
                                        RWertM4 = Pl[counterPl].Y.ToString();
                                        HWertM4 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "ER0")
                                {
                                    RWertM4 = Pl[counterPl].Y.ToString();
                                    HWertM4 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "DR0")
                                    {
                                        RWertM3 = Pl[counterPl].Y.ToString();
                                        HWertM3 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "DR0")
                            {
                                RWertM3 = Pl[counterPl].Y.ToString();
                                HWertM3 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "G")
                                {
                                    RWertM2 = Pl[counterPl].Y.ToString();
                                    HWertM2 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "ER0")
                                    {
                                        RWertM4 = Pl[counterPl].Y.ToString();
                                        HWertM4 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "ER0")
                                {
                                    RWertM4 = Pl[counterPl].Y.ToString();
                                    HWertM4 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "G")
                                    {
                                        RWertM2 = Pl[counterPl].Y.ToString();
                                        HWertM2 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                            else if (LSys == "ER0")
                            {
                                RWertM4 = Pl[counterPl].Y.ToString();
                                HWertM4 = Pl[counterPl].X.ToString();
                                counterPl++;

                                if (LSys == "G")
                                {
                                    RWertM2 = Pl[counterPl].Y.ToString();
                                    HWertM2 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "DR0")
                                    {
                                        RWertM3 = Pl[counterPl].Y.ToString();
                                        HWertM3 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                                else if (LSys == "DR0")
                                {
                                    RWertM3 = Pl[counterPl].Y.ToString();
                                    HWertM3 = Pl[counterPl].X.ToString();
                                    counterPl++;

                                    if (LSys == "G")
                                    {
                                        RWertM2 = Pl[counterPl].Y.ToString();
                                        HWertM2 = Pl[counterPl].X.ToString();
                                        counterPl++;
                                    }
                                }
                            }
                        }
                    }

                    if(Pp[i].Ps.Count > 0 && Ps != null)
                    {
                        Station = Ps[counterPs].Station.ToString();
                        counterPs++;
                    }

                    row.Append(
                    ConstructCell(Pp[i].PAD, CellValues.String),
                    ConstructCell(Pp[i].PArt, CellValues.String),
                    ConstructCell(LSys, CellValues.String),
                    ConstructCell(LDatum, CellValues.String),
                    ConstructCell(LBearb, CellValues.String),
                    ConstructCell(LAuftr, CellValues.String),
                    ConstructCell(LText, CellValues.String),
                    ConstructCell(Station, CellValues.String),
                    ConstructCell(RWertM2, CellValues.String),
                    ConstructCell(HWertM2, CellValues.String),
                    ConstructCell(RWertM3, CellValues.String),
                    ConstructCell(HWertM3, CellValues.String),
                    ConstructCell(RWertM4, CellValues.String),
                    ConstructCell(HWertM4, CellValues.String),
                    ConstructCell(RWertM5, CellValues.String),
                    ConstructCell(HWertM5, CellValues.String),
                    ConstructCell(X, CellValues.String),
                    ConstructCell(Y, CellValues.String),
                    ConstructCell(Z, CellValues.String),
                    ConstructCell(HR0Höhe, CellValues.String),
                    ConstructCell(HR0Datum, CellValues.String),
                    ConstructCell(HR0Bea, CellValues.String),
                    ConstructCell(HR0Auf, CellValues.String),
                    ConstructCell(HR0Text, CellValues.String),
                    ConstructCell(HO0Höhe, CellValues.String),
                    ConstructCell(HO0Datum, CellValues.String),
                    ConstructCell(HO0Bea, CellValues.String),
                    ConstructCell(HO0Auf, CellValues.String),
                    ConstructCell(HO0Text, CellValues.String));

                    sheetData.AppendChild(row);
                }

                worksheetPart.Worksheet.Save();
            }
                
        }

        public static void ToExcelFileEinfach(List<Pp> Pp, List<Ph> Ph, List<Pk> Pk, List<Pl> Pl, List<Ps> Ps, string filePath)
        {
            int counterPk = 0;
            int counterPh = 0;
            int counterPl = 0;
            int counterPs = 0;

            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create($@"{filePath}", SpreadsheetDocumentType.Workbook))
            {

                WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet();

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "ExcelEinfach" };

                sheets.Append(sheet);

                workbookPart.Workbook.Save();


                SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                // Constructing header
                Row row = new Row();

                row.Append(
                    ConstructCell("PAD", CellValues.String),
                    ConstructCell("Rechtswert", CellValues.String),
                    ConstructCell("Hochwert", CellValues.String),
                    ConstructCell("Höhe", CellValues.String),
                    ConstructCell("Strecke", CellValues.String),
                    ConstructCell("RIKZ", CellValues.String),
                    ConstructCell("KilometerGND-Edit", CellValues.String),
                    ConstructCell("Lagesystem", CellValues.String),
                    ConstructCell("Höhensystem", CellValues.String),
                    ConstructCell("Punktart", CellValues.String),
                    ConstructCell("ErläuterungLage", CellValues.String),
                    ConstructCell("ErläuterungHohe", CellValues.String),
                    ConstructCell("Vermarkung", CellValues.String),
                    ConstructCell("MP", CellValues.String),
                    ConstructCell("MH", CellValues.String),
                    ConstructCell("Auftrag", CellValues.String),
                    ConstructCell("Firma", CellValues.String),
                    ConstructCell("Datum", CellValues.String));

                // Insert the header row to the Sheet Data
                sheetData.AppendChild(row);

                for (int i = 0; Pp.Count > i; i++)
                {
                    //Declaration
                    string RWert = "0";
                    string HWert = "0";
                    string Höhe = "0";
                    string Strecke = "0";
                    string RIKZ = "0";
                    string Km = "0";
                    string LSys = "0";
                    string HSys = "0";
                    string ELage = "0";
                    string EHöhe = "0";
                    string PlMP = "0";
                    string PhMH = "0";

                    row = new Row();

                    if (Pp[i].Pk.Count > 0 && Pk != null)
                    {
                        counterPk++;
                    }

                    if (Pp[i].Ph.Count > 0 && Ph != null)
                    {
                        Höhe = Ph[counterPh].H.ToString();
                        HSys = Ph[counterPh].HSys.ToString();
                        PhMH = Ph[counterPh].MH.ToString();

                        counterPh++;
                    }

                    if (Pp[i].Pl.Count > 0 && Pl != null)
                    {
                        RWert = Pl[counterPl].Y.ToString();
                        HWert = Pl[counterPl].X.ToString();
                        LSys = Pl[counterPl].LSys;
                        PlMP = Pl[counterPl].MP.ToString();

                        counterPl++;
                    }

                    if (Pp[i].Ps.Count > 0 && Ps != null)
                    {
                        Strecke = Ps[counterPs].PStrecke;
                        RIKZ = Ps[counterPs].PSTRRiKz.ToString();
                        Km = Ps[counterPs].Station.ToString();

                        counterPs++;
                    }

                    row.Append(
                    ConstructCell(Pp[i].PAD, CellValues.String),
                    ConstructCell(RWert, CellValues.String),
                    ConstructCell(HWert, CellValues.String),
                    ConstructCell(Höhe, CellValues.String),
                    ConstructCell(Strecke, CellValues.String),
                    ConstructCell(RIKZ, CellValues.String),
                    ConstructCell(Km, CellValues.String),
                    ConstructCell(LSys, CellValues.String),
                    ConstructCell(HSys, CellValues.String),
                    ConstructCell(Pp[i].PArt, CellValues.String),
                    ConstructCell(ELage, CellValues.String),
                    ConstructCell(EHöhe, CellValues.String),
                    ConstructCell(Pp[i].VermArt.ToString(), CellValues.String),
                    ConstructCell(PlMP, CellValues.String),
                    ConstructCell(PhMH, CellValues.String),
                    ConstructCell(Pp[i].PAuftr, CellValues.String),
                    ConstructCell(Pp[i].PBearb, CellValues.String),
                    ConstructCell(Pp[i].PDatum, CellValues.String));

                    sheetData.AppendChild(row);
                }

                worksheetPart.Worksheet.Save();
            }

        }

        private static Cell ConstructCell(string value, CellValues dataType)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType)
            };
        }
        #endregion

        #region Skizze Export

        public static void SkizzeExportJPG(List<Pp> Pp, string ZielPath, string SkizzePath)
        {
            for (int i = 0; i < Pp.Count; i++)
            {
                string PAD1 = $"{Pp[i].PAD.Substring(0, 1)}000";
                string PAD2 = $"{Pp[i].PAD.Substring(0, 2)}00";
                string PAD3 = $"{Pp[i].PAD.Substring(0, 4)}";
                string Skizze = $"{Pp[i].PAD}.jpg";
                string FinalPath = $@"{Path.Combine(SkizzePath, "JPG", PAD1, PAD2, PAD3, Skizze)}";

                if (File.Exists(FinalPath))
                {
                    string DirectoryTargetPath = $@"{Path.Combine(ZielPath, "JPG", PAD1, PAD2, PAD3)}";
                    Directory.CreateDirectory(DirectoryTargetPath);
                    string TargetPath = $@"{Path.Combine(DirectoryTargetPath, Skizze)}";

                    File.Copy(FinalPath, TargetPath, true);
                }
            }
        }

        public static void SkizzeExportPDF(List<Pp> Pp, string ZielPath, string SkizzePath)
        {

            for (int i = 0; i < Pp.Count; i++)
            {
                string PAD1 = $"{Pp[i].PAD.Substring(0, 1)}000";
                string PAD2 = $"{Pp[i].PAD.Substring(0, 2)}00";
                string PAD3 = $"{Pp[i].PAD.Substring(0, 4)}";
                string Skizze = $"{Pp[i].PAD}.pdf";
                string FinalPath = $@"{Path.Combine(SkizzePath, "PDF", PAD1, PAD2, PAD3, Skizze)}";

                if (File.Exists(FinalPath))
                {
                    string DirectoryTargetPath = $@"{Path.Combine(ZielPath, "PDF", PAD1, PAD2, PAD3)}";
                    Directory.CreateDirectory(DirectoryTargetPath);
                    string TargetPath = $@"{Path.Combine(DirectoryTargetPath, Skizze)}";

                    File.Copy(FinalPath, TargetPath, true);
                }
            }
        }

        public static void SkizzeExportPPT(List<Pp> Pp, string ZielPath, string SkizzePath)
        {
            for (int i = 0; i < Pp.Count; i++)
            {
                string PAD1 = $"{Pp[i].PAD.Substring(0, 1)}000";
                string PAD2 = $"{Pp[i].PAD.Substring(0, 2)}00";
                string PAD3 = $"{Pp[i].PAD.Substring(0, 4)}";
                string Skizze = $"{Pp[i].PAD}.ppt";
                string FinalPath = $@"{Path.Combine(SkizzePath, "Powerpoint", PAD1, PAD2, PAD3, Skizze)}";

                if (File.Exists(FinalPath))
                {
                    string DirectoryTargetPath = $@"{Path.Combine(ZielPath, "Powerpoint", PAD1, PAD2, PAD3)}";
                    Directory.CreateDirectory(DirectoryTargetPath);
                    string TargetPath = $@"{Path.Combine(DirectoryTargetPath, Skizze)}";

                    File.Copy(FinalPath, TargetPath, true);
                }
            }
        }

        #endregion
    }
}
