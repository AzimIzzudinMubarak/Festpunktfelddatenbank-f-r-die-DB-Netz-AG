using System;
using System.Collections.Generic;

namespace FestpunktDB.Business.EntitiesImport
{
    public partial class ImportPh
    {
        public string PAD { get; set; }
        public string HStat { get; set; }
        public string HSys { get; set; }
        public string HFremd { get; set; }
        public double H { get; set; }
        public short? MH { get; set; }
        public short? MHEXP { get; set; }
        public string HDatum { get; set; }
        public string HBearb { get; set; }
        public string HAuftr { get; set; }
        public string HProg { get; set; }
        public string HText { get; set; }
        public DateTime? Import { get; set; }
        public DateTime? LoeschDatum { get; set; }

        //public ImportPp ImportPADNavigation { get; set; }

        
    }
}
