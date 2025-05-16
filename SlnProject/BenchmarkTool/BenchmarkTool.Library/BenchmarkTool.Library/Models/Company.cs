using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkTool.Library.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Btw { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime RegDate { get; set; }
        public DateTime? AcceptDate { get; set; }
        public DateTime? LastModified { get; set; }
        public string Status { get; set; }
        public string Language { get; set; }
        public byte[] Logo { get; set; }
        public string Nacecode_Code { get; set; }

        // Toevoegen:
        public string Sector { get; set; }
    }
}
