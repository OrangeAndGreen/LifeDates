using System;
using System.Collections.Generic;
using System.IO;

namespace LifeDates
{
    public class Person
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }

        public Person(string name, DateTime birthDate)
        {
            Name = name;
            BirthDate = birthDate;
        }

        public List<string> AgeInfo => GetAgeInfo(BirthDate);

        public static List<string> GetAgeInfo(DateTime birthDate)
        {
            List<string> ret = new List<string>();

            TimeSpan span = DateTime.Now - birthDate;
            ret.Add($"Seconds: {(int)span.TotalSeconds}");
            ret.Add($"Minutes: {(int)span.TotalMinutes}");
            ret.Add($"Hours: {span.TotalHours:0.00}");
            ret.Add($"Days: {span.TotalDays:0.00}");
            ret.Add($"Weeks: {span.TotalDays / Constants.DaysPerWeek:0.00}");
            ret.Add($"Months: {span.TotalDays / Constants.DaysPerMonth:0.00}");
            ret.Add($"Years: {span.TotalDays / Constants.DaysPerYear:0.00}");

            return ret;
        }

        public static List<Person> LoadFromFile(string filename)
        {
            List<Person> ret = new List<Person>();
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    string[] lines = sr.ReadToEnd().Split('\n');

                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(',');

                        if (parts.Length < 2)
                            continue;

                        string dateStr = parts[0].Trim();
                        string name = parts[1].Trim();

                        int.TryParse(dateStr.Substring(0, 4), out int year);
                        int.TryParse(dateStr.Substring(4, 2), out int month);
                        int.TryParse(dateStr.Substring(6, 2), out int day);

                        int hour = 0;
                        int minute = 0;
                        int second = 0;
                        if (dateStr.Length >= 15)
                        {
                            //20200217_154359
                            int.TryParse(dateStr.Substring(9, 2), out hour);
                            int.TryParse(dateStr.Substring(11, 2), out minute);
                            int.TryParse(dateStr.Substring(13, 2), out second);
                        }

                        DateTime date = new DateTime(year, month, day, hour, minute, second);

                        ret.Add(new Person(name, date));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception loading from file: " + e.Message);
            }

            return ret;
        }
    }
}
