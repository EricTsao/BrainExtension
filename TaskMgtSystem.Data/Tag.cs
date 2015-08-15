using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainExtension.Data
{
    public class Tag
    {
        public string Id { get; set; }
        public TagType Type { get; set; }
        public string Name { get; set; }
    }

    public enum TagType { 
        TaskType
    }
}
