using System;
using System.Linq.Expressions;

namespace WIS.Filtering
{
    public class FilterTypes
    {
        public const string Today = "today";
        public const string Yesterday = "yesterday";
        public const string Tomorrow = "tomorrow";
        public const string Day = "day";
        public const string DayBeforeYesterday = "daybeforeyesterday";
        public const string DayAfterTomorrow= "dayaftertomorrow";
        public const string StartOfWeek = "startweek";
        public const string EndOfWeek = "endweek";
        public const string StartOfMonth = "startmonth";
        public const string MidMonth = "midmonth";
        public const string EndOfMonth = "endmonth";
        public const string StartOfYear = "startyear";
        public const string MidYear = "midyear";
        public const string EndOfYear = "endyear";
    }
}