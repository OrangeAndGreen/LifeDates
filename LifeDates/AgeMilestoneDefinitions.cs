using System;
using System.Collections.Generic;

namespace LifeDates
{
    public class AgeMilestoneDefinitions
    {
        public string Unit { get; set; }
        public List<int> SpecificMilestones { get; set; }
        public int RepeatingMilestone { get; set; }

        public DateTime GenerateNewDate(DateTime start, int units)
        {
            switch (Unit.ToLower())
            {
                case "second":
                    return start.AddSeconds(units);
                case "minute":
                    return start.AddMinutes(units);
                case "hour":
                    return start.AddHours(units);
                case "day":
                    return start.AddDays(units);
                case "week":
                    return start.AddDays(units * 7);
                case "month":
                    return start.AddMonths(units);
                case "year":
                    return start.AddYears(units);
                default:
                    return new DateTime(start.Ticks);
            }
        }

        public double YearsForInterval(int units)
        {
            switch (Unit.ToLower())
            {
                case "second":
                    return units / 3600.0 / 24 / Constants.DaysPerYear;
                case "minute":
                    return units / 60.0 / 24 / Constants.DaysPerYear;
                case "hour":
                    return units / 24.0 / Constants.DaysPerYear;
                case "day":
                    return units / Constants.DaysPerYear;
                case "week":
                    return units / Constants.DaysPerYear * 7;
                case "month":
                    return units / 12.0;
                case "year":
                    return units;
                default:
                    return 0;
            }
        }
    }
}
