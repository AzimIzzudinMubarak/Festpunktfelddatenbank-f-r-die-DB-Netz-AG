using System;
using System.Collections.Generic;

namespace FestpunktDB.Business.Entities
{
    public partial class Pp
    {
        public Pp()
        {
            Ph = new HashSet<Ph>();
            Pk = new HashSet<Pk>();
            Pl = new HashSet<Pl>();
            Ps = new HashSet<Ps>();
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
        public virtual ICollection<Ph> Ph { get; set; }
        public virtual ICollection<Pk> Pk { get; set; }
        public virtual ICollection<Pl> Pl { get; set; }
        public virtual ICollection<Ps> Ps { get; set; }
    }
}
