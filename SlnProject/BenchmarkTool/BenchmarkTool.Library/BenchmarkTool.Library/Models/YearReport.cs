using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkTool.Library.Models
{
    public class YearReport
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Fte { get; set; }            // Full Time Equivalent medewerkers
        public int CompanyId { get; set; }
    }
}
