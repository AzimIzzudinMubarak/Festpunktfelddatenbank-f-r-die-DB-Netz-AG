using System;
using System.Collections.Generic;

namespace FestpunktDB.Business.EntitiesExport
{
    public partial class ExportPp
    {
        public ExportPp()
        {
            ExportPh = new HashSet<ExportPh>();
            ExportPk = new HashSet<ExportPk>();
            ExportPl = new HashSet<ExportPl>();
            ExportPs = new HashSet<ExportPs>();
        }

        public string PAD { get; set; }
        public string PArt { get; set; }
        public string Blattschnitt { get; set; }
        public int? PunktNr { get; set; }
        public short? VermArt { get; set; }
        public short? Stabil { get; set; }
        public string PDatum { get; set; }
        public string PBearb { get; set; }
        public string PAuftr { get; set; }
        public string PProg { get; set; }
        public string PText { get; set; }
        public DateTime? Import { get; set; }
        public DateTime? LoeschDatum { get; set; }
        public virtual ICollection<ExportPh> ExportPh { get; set; }
        public virtual ICollection<ExportPk> ExportPk { get; set; }
        public virtual ICollection<ExportPl> ExportPl { get; set; }
        public virtual ICollection<ExportPs> ExportPs { get; set; }
    }
}
