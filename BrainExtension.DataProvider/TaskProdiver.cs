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
                    cmd.Parameters.AddWithValue("StartDate", task.StartDate.ToUniversalTime());
                    cmd.Parameters.AddWithValue("EndDate", task.EndDate.ToUniversalTime());
                    cmd.Parameters.AddWithValue("Status", task.Status);
                    cmd.Parameters.AddWithValue("IsDelete", task.IsDelete);
                    cmd.Parameters.AddWithValue("CreateBy", task.CreateBy);
                    cmd.Parameters.AddWithValue("CreateDate", task.CreateDate.ToUniversalTime());
                    cmd.Parameters.AddWithValue("UpdateBy", task.UpdateBy);
                    cmd.Parameters.AddWithValue("UpdateDate", task.UpdateDate.ToUniversalTime());

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

                string queryString = "";
                if (filter != null && filter.StartDateMin.HasValue)
                {
                    queryString = queryString + " AND StartDate >= @StartDateMin";
                    cmd.Parameters.AddWithValue("StartDateMin", filter.StartDateMin.Value);
                }

                if (filter != null && filter.StartDateMax.HasValue)
                {
                    queryString = queryString + " AND StartDate < @StartDateMax";
                    cmd.Parameters.AddWithValue("StartDateMax", filter.StartDateMax.Value.AddDays(1));
                }

                if (filter != null && filter.StatusList.Count > 0)
                {
                    queryString = queryString + " AND [Status] IN (" + string.Join(",", filter.StatusList.Select(d => string.Format("'{0}'", d)).ToArray()) + ")";
                }

                queryString = string.Format(@"
SELECT [Id]
      ,[Name]
      ,[Description]
      ,[StartDate]
      ,[EndDate]
      ,[Status]
      ,[IsDelete]
      ,[CreateBy]
      ,[CreateDate]
      ,[UpdateBy]
      ,[UpdateDate]
  FROM [dbo].[Task]
  WHERE IsDelete = 0
  {0}
  ORDER BY [StartDate]
", queryString);

                cmd.CommandText = queryString;

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd != null && rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            var startDateTime = new DateTime(((DateTime)rd["StartDate"]).Ticks, DateTimeKind.Utc);
                            var endDateTime = new DateTime(((DateTime)rd["EndDate"]).Ticks, DateTimeKind.Utc);
                            var minDate = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));

                            result.Add(new TaskItem()
                            { 
                                Id=rd["Id"].ToString(),
                                Name = rd["Name"].ToString(),
                                Description = rd["Description"].ToString(),
                                StartDate = startDateTime,
                                StartTime = minDate + startDateTime.TimeOfDay,
                                EndDate = endDateTime,
                                EndTime = minDate + endDateTime.TimeOfDay,
                                Status = rd["Status"].ToString(),
                                IsDelete = (bool)rd["IsDelete"],
                                CreateBy = rd["CreateBy"].ToString(),
                                CreateDate = (DateTime)rd["CreateDate"],
                                UpdateBy = rd["UpdateBy"].ToString(),
                                UpdateDate = (DateTime)rd["UpdateDate"]
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
      ,[StartDate]
      ,[EndDate]
      ,[Status]
      ,[IsDelete]
      ,[CreateBy]
      ,[CreateDate]
      ,[UpdateBy]
      ,[UpdateDate]
  FROM [dbo].[Task]
  WHERE  IsDelete = 0 AND
  (
      1=2
      OR ([StartDate] >= @StartDate AND [StartDate] < @EndDate)
      OR ([EndDate] >= @StartDate AND [EndDate] < @EndDate)
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
                                StartDate = (DateTime)rd["StartDate"],
                                StartTime = DateTime.MinValue + ((DateTime)rd["StartDate"]).TimeOfDay,
                                EndDate = (DateTime)rd["EndDate"],
                                EndTime = DateTime.MinValue + ((DateTime)rd["EndDate"]).TimeOfDay,
                                Status = rd["Status"].ToString(),
                                IsDelete = (bool)rd["IsDelete"],
                                CreateBy = rd["CreateBy"].ToString(),
                                CreateDate = (DateTime)rd["CreateDate"],
                                UpdateBy = rd["UpdateBy"].ToString(),
                                UpdateDate = (DateTime)rd["UpdateDate"]
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
