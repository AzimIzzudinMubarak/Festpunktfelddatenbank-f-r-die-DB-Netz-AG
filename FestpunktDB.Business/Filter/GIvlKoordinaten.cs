using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace FestpunktDB.Business
{
    public partial class GIvlKoordinaten
    {
        public int? Id { get; set; }
        public string Segment { get; set; }
        public string DgnText { get; set; }
        public double? Zeichenflaeche { get; set; }
        public double? X1 { get; set; }
        public double? Y1 { get; set; }
        public double? X2 { get; set; }
        public double? Y2 { get; set; }
        public double? X3 { get; set; }
        public double? Y3 { get; set; }
        public double? X4 { get; set; }
        public double? Y4 { get; set; }
        public double? X5 { get; set; }
        public double? Y5 { get; set; }
        public string LsysText { get; set; }
        public string BstCh80 { get; set; }
        public double? RSchwerpktGk3 { get; set; }
        public double? HSchwerpktGk3 { get; set; }
        public double? RSchwerpktGk { get; set; }
        public double? HSchwerpktGk { get; set; }
    }
}
