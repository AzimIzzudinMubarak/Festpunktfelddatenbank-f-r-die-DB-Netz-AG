using System;

namespace FestpunktDB.Business.EntitiesDeleted
{
    public partial class GeloeschtPk
    {
        public string PAD { get; set; }
        public string KStat { get; set; }
        public string KSys { get; set; }
        public string HFremd { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }
        public string KBearb { get; set; }
        public string LProg { get; set; }
        public string LAuftr { get; set; }
        public string MP { get; set; }
        public string MPEXP { get; set; }
        public string KText { get; set; }
        public DateTime? KDatum { get; set; }
        public DateTime? Import { get; set; }
        public DateTime? LoeschDatum { get; set; }
    }
}
