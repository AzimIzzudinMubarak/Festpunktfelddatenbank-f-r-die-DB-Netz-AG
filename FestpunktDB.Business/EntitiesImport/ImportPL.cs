using System;

namespace FestpunktDB.Business.EntitiesImport
{
    public partial class ImportPl
    {
        public string PAD { get; set; }
        public string LStat { get; set; }
        public string LSys { get; set; }
        public string LFremd { get; set; }
        public double Y { get; set; }
        public double X { get; set; }
        public double MP { get; set; }
        public double MPEXP { get; set; }
        public string LDatum { get; set; }
        public string LBearb { get; set; }
        public string LAuftr { get; set; }
        public string LProg { get; set; }
        public string LText { get; set; }
        public DateTime? Import { get; set; }
        public DateTime? LoeschDatum { get; set; }
        //public ImportPp ImportPADNavigation { get; set; }

    }
}
