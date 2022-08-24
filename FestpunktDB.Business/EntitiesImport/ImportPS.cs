using System;

namespace FestpunktDB.Business.EntitiesImport
{
    public partial class ImportPs
    {
        public string PAD { get; set; }
        public string PStrecke { get; set; }
        public int PSTRRiKz { get; set; }
        public double? Station { get; set; }
        public DateTime? Import { get; set; }
        public DateTime? LoeschDatum { get; set; }
        public string SDatum { get; set; }
        //public ImportPp ImportPADNavigation { get; set; }

    }
}
