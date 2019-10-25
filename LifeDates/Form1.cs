using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace LifeDates
{
    public partial class Form1 : Form
    {
        private int[] mSecondCounts = { 10000, 100000, 1000000, 10000000 };
        private int[] mMinuteCounts = { 100, 1000, 10000, 100000, 1000000 };
        private int[] mHourCounts = { 100, 123, 500, 1000, 1234, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000, 12345, 100000, 1000000 };
        private int[] mDayCounts = { 50, 100, 123, 200, 300, 400, 500, 1000, 1234, 2000, 3000, 4000, 5000, 10000, 12345, 15000, 20000 };
        private int[] mWeekCounts = { 10, 20, 30, 40, 50, 100, 123, 200, 300, 400, 500, 1000, 1234, 2000, 3000 };
        private int[] mAgeFractions = { 2, 3, 4, 5, 10, 20, 25, 50, 100, 200, 300, 400, 500, 1000 };

        public Form1()
        {
            InitializeComponent();

            LoadRelatives();

            UpdateStats();
        }

        private DateTime ResetTime(DateTime input)
        {
            input = input.AddHours(-1 * input.Hour);
            input = input.AddMinutes(-1 * input.Minute);
            input = input.AddSeconds(-1 * input.Second);
            input = input.AddMilliseconds(-1 * input.Millisecond);

            return input;
        }

        public void LoadRelatives()
        {
            try
            {
                using (StreamReader sr = new StreamReader("Relatives.txt"))
                {
                    string[] lines = sr.ReadToEnd().Split('\n');

                    DateTimePicker[] pickers = { dateTimePicker1, dateTimePicker2, dateTimePicker3, dateTimePicker4, dateTimePicker5, dateTimePicker6, dateTimePicker7,
                                                       dateTimePicker8, dateTimePicker9, dateTimePicker10, dateTimePicker11, dateTimePicker12, dateTimePicker13,
                                                       dateTimePicker14, dateTimePicker15, dateTimePicker16, dateTimePicker17, dateTimePicker18, dateTimePicker19 };
                    TextBox[] names = { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10, textBox11, textBox12,
                                              textBox13, textBox14, textBox15, textBox16, textBox17, textBox18, textBox19 };

                    int lineNum = 0;
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(',');

                        if (parts.Length < 2)
                            continue;

                        string dateStr = parts[0].Trim();
                        string name = parts[1].Trim();

                        int year = 0;
                        int month = 0;
                        int day = 0;
                        int.TryParse(dateStr.Substring(0, 4), out year);
                        int.TryParse(dateStr.Substring(4, 2), out month);
                        int.TryParse(dateStr.Substring(6, 2), out day);

                        DateTime date = new DateTime(year, month, day);

                        if (name != null && name.Equals("Birth"))
                        {
                            birthDate.Value = date;
                        }
                        else
                        {
                            pickers[lineNum].Value = date;
                            names[lineNum].Text = name;

                            lineNum++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        public void UpdateStats()
        {
            SortedDictionary<DateTime, string> dates = new SortedDictionary<DateTime, string>();

            bool futureOnly = checkFutureOnly.Checked;

            birthDate.Value = ResetTime(birthDate.Value);

            DateTime birth = birthDate.Value;

            //"Pi day"
            AddDate(dates, birth.AddDays(Math.PI * 365.25), "Pi Day", futureOnly);

            //Calculate "birth dates"
            foreach (int second in mSecondCounts)
                AddDate(dates, birth.AddSeconds(second), String.Format("Second {0}", second), futureOnly);
            foreach (int minute in mMinuteCounts)
                AddDate(dates, birth.AddMinutes(minute), String.Format("Minute {0}", minute), futureOnly);
            foreach (int hour in mHourCounts)
                AddDate(dates, birth.AddHours(hour), String.Format("Hour {0}", hour), futureOnly);
            foreach (int day in mDayCounts)
                AddDate(dates, birth.AddDays(day), String.Format("Day {0}", day), futureOnly);
            foreach (int week in mWeekCounts)
                AddDate(dates, birth.AddDays(week * 7), String.Format("Week {0}", week), futureOnly);

            SortedDictionary<double, string> currentFractions = new SortedDictionary<double, string>();
            //Calculate "comparison dates"
            foreach (KeyValuePair<string, DateTime> pair in GetComparisons())
            {
                string label = pair.Key;
                DateTime date = ResetTime(pair.Value);

                double nowTicks = DateTime.Now.Ticks;
                double nowFraction = (nowTicks - birth.Ticks) / (nowTicks - date.Ticks);
                currentFractions.Add(nowFraction, label);

                long difference = birth.Ticks - date.Ticks;
                if (difference > 0)
                {
                    foreach (int fraction in mAgeFractions)
                        AddDate(dates, birth.AddTicks(difference / (fraction - 1)), string.Format("1/{0} {1}'s age", fraction, label), futureOnly);
                }
                else if (difference < 0)
                {
                    difference *= -1;
                    foreach (int fraction in mAgeFractions)
                    {
                        DateTime curDate = birth.AddTicks((long)(-1 * (double)fraction / (1 - fraction) * difference));

                        AddDate(dates, curDate, string.Format("{0}x {1}'s age", fraction, label), futureOnly);
                    }
                }
            }

            statsText.Text = "Currently:\r\n";
            TimeSpan span = DateTime.Now - birth;
            statsText.Text += string.Format("{0} total seconds\r\n", span.TotalSeconds);
            statsText.Text += string.Format("{0} total minutes\r\n", span.TotalMinutes);
            statsText.Text += string.Format("{0} total hours\r\n", span.TotalHours);
            statsText.Text += string.Format("{0} total days\r\n", span.TotalDays);
            statsText.Text += string.Format("{0:0.00} total weeks\r\n", span.TotalDays / 7.0);
            statsText.Text += string.Format("{0:0.00} total months\r\n", span.TotalDays / 30.0);
            statsText.Text += string.Format("{0:0.00} total years\r\n", span.TotalDays / 365.0);

            foreach (KeyValuePair<double, string> pair in currentFractions)
            {
                if (pair.Key > 1)
                    statsText.Text += string.Format("{0:#.##}x the age of {1}\r\n", pair.Key, pair.Value);
                else
                    statsText.Text += string.Format("1/{0} the age of {1}\r\n", (int)(1 / pair.Key), pair.Value);
            }

            statsText.Text += "\r\nMilestones:\r\n";

            //Display the results
            foreach (KeyValuePair<DateTime, string> pair in dates)
            {
                statsText.Text += String.Format("{0}: {1}\r\n", pair.Key.ToShortDateString(), pair.Value);
            }

        }

        private Dictionary<string, DateTime> GetComparisons()
        {
            Dictionary<string, DateTime> ret = new Dictionary<string, DateTime>();

            if (textBox1.Text.Trim().Length > 0)
                ret.Add(textBox1.Text, dateTimePicker1.Value);
            if (textBox2.Text.Trim().Length > 0)
                ret.Add(textBox2.Text, dateTimePicker2.Value);
            if (textBox3.Text.Trim().Length > 0)
                ret.Add(textBox3.Text, dateTimePicker3.Value);
            if (textBox4.Text.Trim().Length > 0)
                ret.Add(textBox4.Text, dateTimePicker4.Value);
            if (textBox5.Text.Trim().Length > 0)
                ret.Add(textBox5.Text, dateTimePicker5.Value);
            if (textBox6.Text.Trim().Length > 0)
                ret.Add(textBox6.Text, dateTimePicker6.Value);
            if (textBox7.Text.Trim().Length > 0)
                ret.Add(textBox7.Text, dateTimePicker7.Value);
            if (textBox8.Text.Trim().Length > 0)
                ret.Add(textBox8.Text, dateTimePicker8.Value);
            if (textBox9.Text.Trim().Length > 0)
                ret.Add(textBox9.Text, dateTimePicker9.Value);
            if (textBox10.Text.Trim().Length > 0)
                ret.Add(textBox10.Text, dateTimePicker10.Value);
            if (textBox11.Text.Trim().Length > 0)
                ret.Add(textBox11.Text, dateTimePicker11.Value);
            if (textBox12.Text.Trim().Length > 0)
                ret.Add(textBox12.Text, dateTimePicker12.Value);
            if (textBox13.Text.Trim().Length > 0)
                ret.Add(textBox13.Text, dateTimePicker13.Value);
            if (textBox14.Text.Trim().Length > 0)
                ret.Add(textBox14.Text, dateTimePicker14.Value);
            if (textBox15.Text.Trim().Length > 0)
                ret.Add(textBox15.Text, dateTimePicker15.Value);
            if (textBox16.Text.Trim().Length > 0)
                ret.Add(textBox16.Text, dateTimePicker16.Value);
            if (textBox17.Text.Trim().Length > 0)
                ret.Add(textBox17.Text, dateTimePicker17.Value);
            if (textBox18.Text.Trim().Length > 0)
                ret.Add(textBox18.Text, dateTimePicker18.Value);
            if (textBox19.Text.Trim().Length > 0)
                ret.Add(textBox19.Text, dateTimePicker19.Value);

            return ret;
        }

        private void AddDate(SortedDictionary<DateTime, string> collection, DateTime date, string label, bool futureOnly)
        {
            if (futureOnly && date.Ticks < DateTime.Now.Ticks)
                return;

            while (collection.ContainsKey(date))
                date = date.AddTicks(1);

            collection.Add(date, label);
        }

        private void date_ValueChanged(object sender, EventArgs e)
        {
            UpdateStats();
        }

        private void checkFutureOnly_CheckedChanged(object sender, EventArgs e)
        {
            UpdateStats();
        }
    }
}
