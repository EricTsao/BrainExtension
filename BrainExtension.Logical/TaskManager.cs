using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using BrainExtension.Data;
using BrainExtension.DataProvider;

namespace BrainExtension.Logical
{
    public class TaskManager
    {
        public static List<TaskItem> SaveTasks(List<TaskItem> taskList)
        {
            foreach (var task in taskList)
            {
                if (string.IsNullOrEmpty(task.Id))
                {
                    task.Id = Guid.NewGuid().ToString().ToUpper();
                }
            }

            TaskProvider.SaveTasks(taskList);

            return taskList;
        }

        public static void DeleteTaskById(string taskId)
        {
           TaskProvider.DeleteTaskById(taskId);
        }

        public static List<TaskItem> QueryTasks(TaskQueryFilter filter) {
            return TaskProvider.QueryTasks(filter);
        }

        public static List<TaskItem> QueryTasksByDateRange(DateTime StartDate, DateTime EndDate)
        {
            return TaskProvider.QueryTasksByDateRange(StartDate, EndDate);
        }
    }
}
