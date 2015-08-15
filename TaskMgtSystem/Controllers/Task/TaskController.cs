using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Globalization;
using BrainExtension.Logical;
using BrainExtension.Data;
using TaskMgtSystem.Models.Calendar;

namespace TaskMgtSystem.Controllers.Task
{
    public class TaskController : Controller
    {
        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult List()
        {
            return View("List");
        }

        public ActionResult Calendar()
        {
            return View("Calendar");
        }

        public ActionResult Delete([FromBody]string Id)
        {
            TaskManager.DeleteTaskById(Id);

            return Content("Done");
        }

        public ActionResult Edit([FromBody]TaskItem task)
        {
            task.IsDelete = false;
            task.CreateBy = User.Identity.Name;
            task.CreateTime = DateTime.Now;
            task.UpdateBy = User.Identity.Name;
            task.UpdateTime = DateTime.Now;

            var taskList = new List<TaskItem>();


            taskList.Add(task);

            return Content(string.Join("\n", TaskManager.SaveTasks(taskList).Select(d => d.Id).ToArray()));
        }

        public ActionResult QueryTasks()
        {
            var taskList = TaskManager.QueryTasks(new TaskQueryFilter() { });

            return Json(taskList);
        }

        public ActionResult GetCalendar(int startWeekFromNow)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            DateTime today = DateTime.Now.Date;
            Calendar cal = dfi.Calendar;

            var dayOfWeek = cal.GetDayOfWeek(today);
            var thisWeekOfYear = cal.GetWeekOfYear(today, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            var daysOfWeek = (int)dayOfWeek;
            var firstDayOfThisWeek = today.AddDays(-1 * daysOfWeek);
            var firstDayOfCurrentWeek = firstDayOfThisWeek.AddDays(7 * startWeekFromNow);
            var firstDayOfLastWeek = firstDayOfCurrentWeek.AddDays(-7);

            var weekCount = 10;
            var dayCount = 7;

            var allTasks = TaskManager.QueryTasksByDateRange(firstDayOfLastWeek, firstDayOfLastWeek.AddDays((5 * 7) - 1)); ;

            var weeks = new List<WeekItem>();

            for (int i = 0; i < weekCount; i++)
            {
                var firstDayOfWeek = firstDayOfLastWeek.AddDays(i * dayCount);
                var weekFromNow = (firstDayOfWeek - firstDayOfThisWeek).Days / 7;
                var week = new WeekItem()
                {
                    Year = firstDayOfWeek.Year,
                    WeekFromNow = weekFromNow
                };

                var days = new List<DayItem>();
                for (int j = 0; j < dayCount; j++)
                {
                    var day = firstDayOfWeek.AddDays(j);

                    var dayItem = new DayItem()
                    {
                        Year = day.Year,
                        WeekFromNow = weekFromNow,
                        DayOfYear = cal.GetDayOfYear(day),
                        DayOfWeek = (int)day.DayOfWeek,
                        DayNameOfWeek = day.DayOfWeek.ToString(),
                        Date = day
                    };

                    dayItem.Tasks = allTasks.Where(d => d.StartTime.Date <= day && d.EndTime.Date >= day).ToList();

                    days.Add(dayItem);
                }

                week.Days = days;
                weeks.Add(week);
            }

            return Json(weeks, JsonRequestBehavior.AllowGet);
        }
    }
}
