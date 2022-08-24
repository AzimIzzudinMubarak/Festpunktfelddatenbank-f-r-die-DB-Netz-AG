using System;
using System.Collections.Generic;

namespace FestpunktDB.Business.EntitiesImport
{
    public partial class ImportPp
    {
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

       /* public IList<ImportPh> ImportPh { get; set; }
        public IList<ImportPk> ImportPk { get; set; }
        public IList<ImportPl> ImportPl { get; set; }
        public IList<ImportPs> ImportPs { get; set; }*/

    }
}
