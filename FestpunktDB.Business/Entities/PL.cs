using System;

namespace FestpunktDB.Business.Entities
{
    public partial class Pl
    {
        public string PAD { get; set; }
        public string LStat { get; set; }
        public string LSys { get; set; }
        public string LFremd { get; set; }
        public double? Y { get; set; }
        public double? X { get; set; }
        public short? MP { get; set; }
        public short? MPEXP { get; set; }
        public string LDatum { get; set; }
        public string LBearb { get; set; }
        public string LAuftr { get; set; }
        public string LProg { get; set; }
        public string LText { get; set; }
        public DateTime? Import { get; set; }
        public DateTime? LoeschDatum { get; set; }
        public virtual Pp PadNavigation { get; set; }
    }
}
