using System;
using System.Collections.Generic;

namespace LifeDates
{
    public static class MilestoneGenerator
    {
        public static List<Milestone> GenerateMilestones(List<Person> people, int yearsToSearch, bool futureOnly)
        {
            List<Milestone> ret = new List<Milestone>();

            for(int x=0; x<people.Count; x++)
            {
                Person person = people[x];
                ret.AddRange(GenerateIndividualMilestones(person, yearsToSearch, futureOnly));

                for(int y=x+1; y<people.Count; y++)
                {
                    Person person2 = people[y];
                    ret.AddRange(GenerateComparisonMilestones(person, person2, yearsToSearch, futureOnly));
                }
            }

            ret.Sort((m1, m2) => m1.Date.CompareTo(m2.Date));

            return ret;
        }

        private static DateTime ResetTime(DateTime input)
        {
            input = input.AddHours(-1 * input.Hour);
            input = input.AddMinutes(-1 * input.Minute);
            input = input.AddSeconds(-1 * input.Second);
            input = input.AddMilliseconds(-1 * input.Millisecond);

            return input;
        }

        public static List<Milestone> GenerateIndividualMilestones(Person person, int yearsToSearch, bool futureOnly)
        {
            List<AgeMilestoneDefinitions> definitions = new List<AgeMilestoneDefinitions>
            {
                new AgeMilestoneDefinitions
                {
                    Unit = "second",
                    SpecificMilestones = new List<int>{10000, 100000},
                    RepeatingMilestone = 100000000
                },
                new AgeMilestoneDefinitions
                {
                    Unit = "minute",
                    SpecificMilestones = new List<int>{1000, 10000, 100000},
                    RepeatingMilestone = 500000
                },
                new AgeMilestoneDefinitions
                {
                    Unit = "hour",
                    SpecificMilestones = new List<int>{500, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000},
                    RepeatingMilestone = 10000
                },
                new AgeMilestoneDefinitions
                {
                    Unit = "day",
                    SpecificMilestones = new List<int>{100, 200, 300, 400, 500},
                    RepeatingMilestone = 1000
                },
                new AgeMilestoneDefinitions
                {
                    Unit = "week",
                    SpecificMilestones = new List<int>{10, 20, 30, 40, 50},
                    RepeatingMilestone = 100
                },
                new AgeMilestoneDefinitions
                {
                    Unit = "month",
                    SpecificMilestones = new List<int>{},
                    RepeatingMilestone = 50
                }
            };

            List<int> specialIntervals = new List<int>();
            int total = 0;
            //int total2 = 0;
            for (int i = 0; i < 9; i++)
            {
                total = total * 10 + i + 1; //1, 12, 123, 1234, 12345... 123456789
                if (total > 100)
                {
                    specialIntervals.Add(total);
                }

                //This would include 987, 9876, 98765...
                //total2 = total2 * 10 + 9 - i;
                //if (total2 > 100)
                //{
                //    specialIntervals.Add(total2);
                //}

                //TODO: I want code to include 321, 4321, 54321...
            }

            List<Milestone> ret = new List<Milestone>();

            DateTime birth = ResetTime(person.BirthDate);

            //"Pi day"
            CheckAndAddMilestone(ret, new Milestone(birth.AddDays(Math.PI * Constants.DaysPerYear), person.Name, "Pi Years"), yearsToSearch, futureOnly);

            double maxAge = (DateTime.Now - person.BirthDate).TotalDays / Constants.DaysPerYear + yearsToSearch;

            foreach (AgeMilestoneDefinitions definition in definitions)
            {
                foreach (int interval in definition.SpecificMilestones)
                {
                    if (definition.YearsForInterval(interval) < maxAge)
                    {
                        CheckAndAddMilestone(ret, new Milestone(definition.GenerateNewDate(birth, interval), person.Name, $"{interval} {definition.Unit}s"), yearsToSearch, futureOnly);
                    }
                }

                foreach (int interval in specialIntervals)
                {
                    if (definition.YearsForInterval(interval) < maxAge)
                    {
                        CheckAndAddMilestone(ret, new Milestone(definition.GenerateNewDate(birth, interval), person.Name, $"{interval} {definition.Unit}s"), yearsToSearch, futureOnly);
                    }
                }

                for (int i = 0; ; i++)
                {
                    int interval = definition.RepeatingMilestone * i;
                    DateTime date = definition.GenerateNewDate(birth, interval);
                    CheckAndAddMilestone(ret, new Milestone(date, person.Name, $"{interval} {definition.Unit}s"), yearsToSearch, futureOnly);

                    if ((date - DateTime.Now).TotalDays / Constants.DaysPerYear > yearsToSearch)
                    {
                        break;
                    }
                }
            }

            return ret;
        }

        public static List<Milestone> GenerateComparisonMilestones(Person person1, Person person2, int yearsToSearch, bool futureOnly)
        {
            int[] mAgeFractions = {2, 3, 4, 5, 10, 20, 25, 50, 100, 200, 300, 400, 500, 1000};
            List<Milestone> ret = new List<Milestone>();

            //Calculate "comparison dates"
            long birth1Ticks = ResetTime(person1.BirthDate).Ticks;
            long birth2Ticks = ResetTime(person2.BirthDate).Ticks;

            long difference = birth1Ticks - birth2Ticks;
            if (difference > 0)
            {
                foreach (int fraction in mAgeFractions)
                {
                    CheckAndAddMilestone(ret, new Milestone(person1.BirthDate.AddTicks(difference / (fraction - 1)), person1.Name, $"1/{fraction} {person2.Name}'s age"), yearsToSearch, futureOnly);
                }
                    
            }
            else if (difference < 0)
            {
                difference *= -1;
                foreach (int fraction in mAgeFractions)
                {
                    DateTime curDate = person1.BirthDate.AddTicks((long) (-1 * (double) fraction / (1 - fraction) * difference));
                    CheckAndAddMilestone(ret, new Milestone(curDate, person1.Name, $"{fraction}x {person2.Name}'s age"), yearsToSearch, futureOnly);
                }
            }

            return ret;
        }

        public static List<string> GenerateAgeMatrix(List<Person> people)
        {
            List<string> ret = new List<string>();

            string header = "";
            foreach (Person person in people)
            {
                header += "," + person.Name;
            }

            ret.Add(header);

            foreach (Person person1 in people)
            {
                string line = person1.Name;
                foreach (Person person2 in people)
                {
                    double age1 = (DateTime.Now - person1.BirthDate).TotalDays;
                    double age2 = (DateTime.Now - person2.BirthDate).TotalDays;
                    line += $",{age1/age2:0.00}";
                }

                ret.Add(line);
            }

            return ret;
        }

        private static void CheckAndAddMilestone(List<Milestone> list, Milestone milestone, int yearsToSearch, bool futureOnly)
        {
            DateTime now = DateTime.Now;

            if (milestone.Date < now)
            {
                if (futureOnly || now - milestone.Date > TimeSpan.FromDays(yearsToSearch * Constants.DaysPerYear))
                    return;
            }
            else
            {
                if (milestone.Date - now > TimeSpan.FromDays(yearsToSearch * Constants.DaysPerYear))
                    return;
            }

            list.Add(milestone);
        }
    }
}
