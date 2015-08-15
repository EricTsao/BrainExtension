using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BrainExtension.Data;

namespace TaskMgtSystem.Models.Calendar
{
    public class WeekItem
    {
        public int Year { get; set; }
        public int WeekFromNow { get; set; }
        public List<DayItem> Days { get; set; }
    }

    public class DayItem
    {
        public int Year { get; set; }
        public int WeekFromNow { get; set; }
        public int DayOfYear { get; set; }
        public int DayOfWeek { get; set; }
        public string DayNameOfWeek { get; set; }
        public DateTime Date { get; set; }
        public List<TaskItem> Tasks { get; set; }
    }
}