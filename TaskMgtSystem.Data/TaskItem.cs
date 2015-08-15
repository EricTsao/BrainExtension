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

        private DateTime _startTime;
        public DateTime StartTime { get { return _startTime; } set { _startTime = value; } }

        private DateTime _endTime;
        public DateTime EndTime { get { return _endTime; } set { _endTime = value; } }

        public string Status { get; set; }

        public bool IsDelete { get; set; }

        public string CreateBy { get; set; }

        private DateTime _createTime;
        public DateTime CreateTime { get { return _createTime; } set { _createTime = value; } }

        public string UpdateBy { get; set; }

        private DateTime _updateTime;
        public DateTime UpdateTime { get { return _updateTime; } set { _updateTime = value; } }

        public List<Member> Members { get; set; }
    }

    public class TaskQueryFilter
    {
        public DateTime? StartDateMin { get; set; }
        public DateTime? StartDateMax { get; set; }
    }
}
