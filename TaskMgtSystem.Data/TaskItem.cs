using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace BrainExtension.Data
{
    public class TaskItem
    {
        public string Id { get; set; }

        public string Name { get; set; }

        private string _description;
        public string Description { get { return _description; } set { _description = value ?? ""; } }

        public DateTime StartDate { get; set; }
        public DateTime StartTime { get; set; }

        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }

        public string Status { get; set; }

        public bool IsDelete { get; set; }

        public string CreateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public string UpdateBy { get; set; }

        public DateTime UpdateDate { get; set; }

        public List<Member> Members { get; set; }
    }

    public class TaskQueryFilter
    {
        public DateTime? StartDateMin { get; set; }
        public DateTime? StartDateMax { get; set; }
        public List<string> StatusList { get; set; }

        public TaskQueryFilter() {
            StatusList = new List<string>();
        }
    }
}
