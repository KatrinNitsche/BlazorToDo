using System;
using System.Globalization;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Helper
{
    public static class Extensions
    {
        public static string ToMonthName(this DateTime dateTime) => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);

        public static string ToShortMonthName(this DateTime dateTime) => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dateTime.Month);
    }
}
