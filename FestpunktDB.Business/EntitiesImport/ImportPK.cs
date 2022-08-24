using System;

namespace FestpunktDB.Business.EntitiesImport
{
    public partial class ImportPk
    {
        public string PAD { get; set; }
        public string KStat { get; set; }
        public string KSys { get; set; }
        public string HFremd { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public string KBearb { get; set; }
        public string LProg { get; set; }
        public string LAuftr { get; set; }
        public double MP { get; set; }
        public double MPEXP { get; set; }
        public string KText { get; set; }
        public string KDatum { get; set; }
        public DateTime? Import { get; set; }
        public DateTime? LoeschDatum { get; set; }
       // public ImportPp ImportPADNavigation { get; set; }


    }
}
