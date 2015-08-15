using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using BrainExtension.Data;

namespace BrainExtension.DataProvider
{
    public class TaskProvider
    {
        public static void SaveTasks(List<TaskItem> taskList)
        {
            var conStr = ConfigurationManager.ConnectionStrings["ConnBrainExtension"].ConnectionString;
            string queryString = @"dbo.EditTask";
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                foreach (var task in taskList)
                {
                    cmd.CommandText = queryString;
                    cmd.Parameters.Clear();
                 
                    cmd.Parameters.AddWithValue("Id", task.Id);
                    cmd.Parameters.AddWithValue("Name", task.Name);
                    cmd.Parameters.AddWithValue("Description", task.Description);
                    cmd.Parameters.AddWithValue("StartTime", task.StartTime);
                    cmd.Parameters.AddWithValue("EndTime", task.EndTime);
                    cmd.Parameters.AddWithValue("Status", task.Status);
                    cmd.Parameters.AddWithValue("IsDelete", task.IsDelete);
                    cmd.Parameters.AddWithValue("CreateBy", task.CreateBy);
                    cmd.Parameters.AddWithValue("CreateTime", task.CreateTime);
                    cmd.Parameters.AddWithValue("UpdateBy", task.UpdateBy);
                    cmd.Parameters.AddWithValue("UpdateTime", task.UpdateTime);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<TaskItem> QueryTasks(TaskQueryFilter filter)
        {
            var result = new List<TaskItem>();

            var conStr = ConfigurationManager.ConnectionStrings["ConnBrainExtension"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                string queryString = @"
SELECT [Id]
      ,[Name]
      ,[Description]
      ,[StartTime]
      ,[EndTime]
      ,[Status]
      ,[IsDelete]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
  FROM [dbo].[Task]
  WHERE IsDelete = 0
";
                if (filter.StartDateMin.HasValue)
                {
                    queryString = queryString + " AND StartTime >= @StartDateMin";
                    cmd.Parameters.AddWithValue("StartDateMin", filter.StartDateMin.Value);
                }

                if (filter.StartDateMax.HasValue)
                {
                    queryString = queryString + " AND StartTime < @StartDateMax";
                    cmd.Parameters.AddWithValue("StartDateMax", filter.StartDateMax.Value.AddDays(1));
                }

                cmd.CommandText = queryString;

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd != null && rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            result.Add(new TaskItem() { 
                            Id=rd["Id"].ToString(),
                            Name = rd["Name"].ToString(),
                            Description = rd["Description"].ToString(),
                            StartTime = (DateTime)rd["StartTime"],
                            EndTime = (DateTime)rd["EndTime"],
                            Status = rd["Status"].ToString(),
                            IsDelete = (bool)rd["IsDelete"],
                            CreateBy = rd["CreateBy"].ToString(),
                            CreateTime = (DateTime)rd["CreateTime"],
                            UpdateBy = rd["UpdateBy"].ToString(),
                            UpdateTime = (DateTime)rd["UpdateTime"]
                            });
                        }
                    }
                }
            }

            return result;
        }

        public static List<TaskItem> QueryTasksByDateRange(DateTime StartDate, DateTime EndDate)
        {
            var result = new List<TaskItem>();

            var conStr = ConfigurationManager.ConnectionStrings["ConnBrainExtension"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                string queryString = @"
SELECT [Id]
      ,[Name]
      ,[Description]
      ,[StartTime]
      ,[EndTime]
      ,[Status]
      ,[IsDelete]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
  FROM [dbo].[Task]
  WHERE  IsDelete = 0 AND
  (
      1=2
      OR ([StartTime] >= @StartDate AND [StartTime] < @EndDate)
      OR ([EndTime] >= @StartDate AND [EndTime] < @EndDate)
  )
";
                cmd.Parameters.AddWithValue("StartDate", StartDate);
                cmd.Parameters.AddWithValue("EndDate", EndDate.AddDays(1));
            
                cmd.CommandText = queryString;

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd != null && rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            result.Add(new TaskItem()
                            {
                                Id = rd["Id"].ToString(),
                                Name = rd["Name"].ToString(),
                                Description = rd["Description"].ToString(),
                                StartTime = (DateTime)rd["StartTime"],
                                EndTime = (DateTime)rd["EndTime"],
                                Status = rd["Status"].ToString(),
                                IsDelete = (bool)rd["IsDelete"],
                                CreateBy = rd["CreateBy"].ToString(),
                                CreateTime = (DateTime)rd["CreateTime"],
                                UpdateBy = rd["UpdateBy"].ToString(),
                                UpdateTime = (DateTime)rd["UpdateTime"]
                            });
                        }
                    }
                }
            }

            return result;
        }

        public static void DeleteTaskById(string taskId)
        {
            var conStr = ConfigurationManager.ConnectionStrings["ConnBrainExtension"].ConnectionString;
            string queryString = @"
UPDATE dbo.Task 
SET IsDelete = 1
WHERE Id = @Id
";
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = queryString;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Id", taskId);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
