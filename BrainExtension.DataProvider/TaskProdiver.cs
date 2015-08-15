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
                    cmd.Parameters.AddWithValue("StartDateTime", task.StartDateTime.ToUniversalTime());
                    cmd.Parameters.AddWithValue("EndDateTime", task.EndDateTime.ToUniversalTime());
                    cmd.Parameters.AddWithValue("Status", task.Status);
                    cmd.Parameters.AddWithValue("IsDelete", task.IsDelete);
                    cmd.Parameters.AddWithValue("CreateBy", task.CreateBy);
                    cmd.Parameters.AddWithValue("CreateDateTime", task.CreateDateTime.ToUniversalTime());
                    cmd.Parameters.AddWithValue("UpdateBy", task.UpdateBy);
                    cmd.Parameters.AddWithValue("UpdateDateTime", task.UpdateDateTime.ToUniversalTime());

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
      ,[StartDateTime]
      ,[EndDateTime]
      ,[Status]
      ,[IsDelete]
      ,[CreateBy]
      ,[CreateDateTime]
      ,[UpdateBy]
      ,[UpdateDateTime]
  FROM [dbo].[Task]
  WHERE IsDelete = 0
";
                if (filter.StartDateMin.HasValue)
                {
                    queryString = queryString + " AND StartDateTime >= @StartDateMin";
                    cmd.Parameters.AddWithValue("StartDateMin", filter.StartDateMin.Value);
                }

                if (filter.StartDateMax.HasValue)
                {
                    queryString = queryString + " AND StartDateTime < @StartDateMax";
                    cmd.Parameters.AddWithValue("StartDateMax", filter.StartDateMax.Value.AddDays(1));
                }

                cmd.CommandText = queryString;

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd != null && rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            var startDateTime = new DateTime(((DateTime)rd["StartDateTime"]).Ticks, DateTimeKind.Utc);
                            var endDateTime = new DateTime(((DateTime)rd["EndDateTime"]).Ticks, DateTimeKind.Utc);
                            var minDate = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));

                            result.Add(new TaskItem()
                            { 
                                Id=rd["Id"].ToString(),
                                Name = rd["Name"].ToString(),
                                Description = rd["Description"].ToString(),
                                StartDateTime = startDateTime,
                                StartDate = startDateTime.Date,
                                StartTime = minDate + startDateTime.TimeOfDay,
                                EndDateTime = endDateTime,
                                EndDate = endDateTime.Date,
                                EndTime = minDate + startDateTime.TimeOfDay,
                                Status = rd["Status"].ToString(),
                                IsDelete = (bool)rd["IsDelete"],
                                CreateBy = rd["CreateBy"].ToString(),
                                CreateDateTime = (DateTime)rd["CreateDateTime"],
                                UpdateBy = rd["UpdateBy"].ToString(),
                                UpdateDateTime = (DateTime)rd["UpdateDateTime"]
                            });
                        }
                    }
                }
            }

            return result;
        }

        public static List<TaskItem> QueryTasksByDateRange(DateTime StartDateTime, DateTime EndDateTime)
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
      ,[StartDateTime]
      ,[EndDateTime]
      ,[Status]
      ,[IsDelete]
      ,[CreateBy]
      ,[CreateDateTime]
      ,[UpdateBy]
      ,[UpdateDateTime]
  FROM [dbo].[Task]
  WHERE  IsDelete = 0 AND
  (
      1=2
      OR ([StartDateTime] >= @StartDateTime AND [StartDateTime] < @EndDateTime)
      OR ([EndDateTime] >= @StartDateTime AND [EndDateTime] < @EndDateTime)
  )
";
                cmd.Parameters.AddWithValue("StartDateTime", StartDateTime);
                cmd.Parameters.AddWithValue("EndDateTime", EndDateTime.AddDays(1));
            
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
                                StartDateTime = (DateTime)rd["StartDateTime"],
                                StartDate = ((DateTime)rd["StartDateTime"]).Date,
                                StartTime = DateTime.MinValue + ((DateTime)rd["StartDateTime"]).TimeOfDay,
                                EndDateTime = (DateTime)rd["EndDateTime"],
                                EndDate = ((DateTime)rd["EndDateTime"]).Date,
                                EndTime = DateTime.MinValue + ((DateTime)rd["EndDateTime"]).TimeOfDay,
                                Status = rd["Status"].ToString(),
                                IsDelete = (bool)rd["IsDelete"],
                                CreateBy = rd["CreateBy"].ToString(),
                                CreateDateTime = (DateTime)rd["CreateDateTime"],
                                UpdateBy = rd["UpdateBy"].ToString(),
                                UpdateDateTime = (DateTime)rd["UpdateDateTime"]
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
