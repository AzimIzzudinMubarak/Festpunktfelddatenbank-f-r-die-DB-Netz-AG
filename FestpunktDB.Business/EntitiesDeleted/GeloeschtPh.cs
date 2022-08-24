using System;

namespace FestpunktDB.Business.EntitiesDeleted
{
    public partial class GeloeschtPh
    {
        public string PAD { get; set; }
        public string HStat { get; set; }
        public string HSys { get; set; }
        public string HFremd { get; set; }
        public double? H { get; set; }
        public short? MH { get; set; }
        public short? MHEXP { get; set; }
        public string HDatum { get; set; }
        public string HBearb { get; set; }
        public string HAuftr { get; set; }
        public string HProg { get; set; }
        public string HText { get; set; }
        public DateTime? Import { get; set; }
        public DateTime? LoeschDatum { get; set; }

        //public GeloeschtPh(Ph ph)
        //{
        //    PAD = ph.PAD;
        //    HProg = ph.HProg;
        //    MHEXP = ph.MHEXP;
        //    H = ph.H;
        //    HAuftr = ph.HAuftr;
        //    HBearb = ph.HBearb;
        //    HDatum = ph.HDatum;
        //    HFremd = ph.HFremd;
        //    HStat = ph.HStat;
        //    HSys = ph.HSys;
        //    HText = ph.HText;
        //    MH = ph.MH;
        //    LoeschDatum = DateTime.Now;
        //    Import = ph.Import;
        //}
    }
}
