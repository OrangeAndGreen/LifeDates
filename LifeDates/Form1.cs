using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LifeDates
{
    public partial class Form1 : Form
    {
        private bool mInitializing;

        public Form1()
        {
            InitializeComponent();

            LoadRelatives();

            UpdateStats();
        }

        public void LoadRelatives()
        {
            mInitializing = true;
            DateTimePicker[] pickers = { dateTimePicker1, dateTimePicker2, dateTimePicker3, dateTimePicker4, dateTimePicker5, dateTimePicker6, dateTimePicker7,
                dateTimePicker8, dateTimePicker9, dateTimePicker10, dateTimePicker11, dateTimePicker12, dateTimePicker13,
                dateTimePicker14, dateTimePicker15, dateTimePicker16, dateTimePicker17, dateTimePicker18, dateTimePicker19 };
            TextBox[] names = { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10, textBox11, textBox12,
                textBox13, textBox14, textBox15, textBox16, textBox17, textBox18, textBox19 };

            List<Person> people = Person.LoadFromFile(Constants.Filename);
            int lineNum = 0;
            foreach (Person person in people)
            {
                if (lineNum >= pickers.Length)
                    continue;

                pickers[lineNum].Value = person.BirthDate;
                names[lineNum].Text = person.Name;

                lineNum++;
            }

            mInitializing = false;
            UpdateStats();
        }

        public void UpdateStats()
        {
            bool futureOnly = checkFutureOnly.Checked;

            List<Person> people = GetPeople();

            statsText.Text = "";

            //Age info for each person
            foreach (Person person in people)
            {
                statsText.Text += person.Name + ":" + Constants.Newline;
                foreach (string line in person.AgeInfo)
                {
                    statsText.Text += line + Constants.Newline;
                }

                statsText.Text += Constants.Newline;
            }

            //Age matrix
            statsText.Text += Constants.Newline + "Age Matrix:" + Constants.Newline;
            foreach (string line in MilestoneGenerator.GenerateAgeMatrix(people))
            {
                statsText.Text += line + Constants.Newline;
            }

            //Milestones
            statsText.Text += Constants.Newline + "Milestones:" + Constants.Newline;
            foreach (Milestone milestone in MilestoneGenerator.GenerateMilestones(people, Constants.YearsToSearch, futureOnly))
            {
                statsText.Text += $"{milestone.Date.ToShortDateString()},{milestone.Person} {milestone.Description}{Constants.Newline}";
            }
        }

        private List<Person> GetPeople()
        {
            List<Person> ret = new List<Person>();

            if (textBox1.Text.Trim().Length > 0)
                ret.Add(new Person(textBox1.Text, dateTimePicker1.Value));
            if (textBox2.Text.Trim().Length > 0)
                ret.Add(new Person(textBox2.Text, dateTimePicker2.Value));
            if (textBox3.Text.Trim().Length > 0)
                ret.Add(new Person(textBox3.Text, dateTimePicker3.Value));
            if (textBox4.Text.Trim().Length > 0)
                ret.Add(new Person(textBox4.Text, dateTimePicker4.Value));
            if (textBox5.Text.Trim().Length > 0)
                ret.Add(new Person(textBox5.Text, dateTimePicker5.Value));
            if (textBox6.Text.Trim().Length > 0)
                ret.Add(new Person(textBox6.Text, dateTimePicker6.Value));
            if (textBox7.Text.Trim().Length > 0)
                ret.Add(new Person(textBox7.Text, dateTimePicker7.Value));
            if (textBox8.Text.Trim().Length > 0)
                ret.Add(new Person(textBox8.Text, dateTimePicker8.Value));
            if (textBox9.Text.Trim().Length > 0)
                ret.Add(new Person(textBox9.Text, dateTimePicker9.Value));
            if (textBox10.Text.Trim().Length > 0)
                ret.Add(new Person(textBox10.Text, dateTimePicker10.Value));
            if (textBox11.Text.Trim().Length > 0)
                ret.Add(new Person(textBox11.Text, dateTimePicker11.Value));
            if (textBox12.Text.Trim().Length > 0)
                ret.Add(new Person(textBox12.Text, dateTimePicker12.Value));
            if (textBox13.Text.Trim().Length > 0)
                ret.Add(new Person(textBox13.Text, dateTimePicker13.Value));
            if (textBox14.Text.Trim().Length > 0)
                ret.Add(new Person(textBox14.Text, dateTimePicker14.Value));
            if (textBox15.Text.Trim().Length > 0)
                ret.Add(new Person(textBox15.Text, dateTimePicker15.Value));
            if (textBox16.Text.Trim().Length > 0)
                ret.Add(new Person(textBox16.Text, dateTimePicker16.Value));
            if (textBox17.Text.Trim().Length > 0)
                ret.Add(new Person(textBox17.Text, dateTimePicker17.Value));
            if (textBox18.Text.Trim().Length > 0)
                ret.Add(new Person(textBox18.Text, dateTimePicker18.Value));
            if (textBox19.Text.Trim().Length > 0)
                ret.Add(new Person(textBox19.Text, dateTimePicker19.Value));

            return ret;
        }

        private void date_ValueChanged(object sender, EventArgs e)
        {
            if (!mInitializing)
            {
                UpdateStats();
            }
        }

        private void checkFutureOnly_CheckedChanged(object sender, EventArgs e)
        {
            UpdateStats();
        }
    }
}
