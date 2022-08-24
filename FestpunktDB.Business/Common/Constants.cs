using System;
using System.Collections.Generic;
using System.Text;

namespace FestpunktDB.Business.Common
{
    public sealed class Constants
    {
        /// <summary>
        /// The value used to represent a null DateTime value
        /// </summary>
        public static int NullInt = int.MinValue;
        /// <summary>
        /// The value used to represent a null decimal value
        /// </summary>
        public static decimal NullDecimal = decimal.MinValue;
        /// <summary>
        /// The value used to represent a null double value
        /// </summary>
        public static double NullDouble = double.MinValue;

        /// <summary>
        /// The value used to represent a null DateTime value
        /// </summary>
        public static DateTime NullDateTime = DateTime.MinValue;


        /// <summary>
        /// The value used to represent a null Guid value
        /// </summary>
        public static Guid NullGuid = Guid.Empty;

        /// <summary>
        /// The value used to represent a null long value
        /// </summary>
        public static long NullLong = long.MinValue;

        /// <summary>
        /// The value used to represent a null float value
        /// </summary>
        public static float NullFloat = float.MinValue;

        /// <summary>
        /// The value used to represent a null string value
        /// </summary>
        public static string NullString = string.Empty;

        /// <summary>
        /// Maximum DateTime value allowed by SQL Server
        /// </summary>
        public static DateTime SqlMaxDate = DateTime.Now;

        /// <summary>
        /// Minimum DateTime value allowed by SQL Server
        /// </summary>
        public static DateTime SqlMinDate = new DateTime(1753, 1, 1, 00, 00, 00);
    }
}
