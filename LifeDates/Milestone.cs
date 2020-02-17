using System;

namespace LifeDates
{
    public class Milestone
    {
        public DateTime Date { get; set; }
        public string Person { get; set; }
        public string Description { get; set; }

        public Milestone(DateTime date, string person, string description)
        {
            Date = date;
            Person = person;
            Description = description;
        }
    }
}
