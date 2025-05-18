using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkTool.Library.Models;

namespace BenchmarkTool.Library.Services
{
    public static class YearReportService
    {
        public static List<YearReport> GetReportsByCompanyId(int companyId)
        {
            var reports = new List<YearReport>();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT id, year, fte, company_id 
                 FROM Yearreports 
                 WHERE company_id = @CompanyId 
                 ORDER BY year DESC";


                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reports.Add(new YearReport
                            {
                                Id = (int)reader["id"],
                                Year = (int)reader["year"],
                                Fte = reader["fte"] != DBNull.Value ? (int)reader["fte"] : 0,
                                CompanyId = (int)reader["company_id"]
                            });

                        }
                    }
                }
            }
            return reports;
        }

        public static void AddYearReport(YearReport report)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Yearreports (year, fte, company_id) VALUES (@Year, @Fte, @CompanyId)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Year", report.Year);
                    cmd.Parameters.AddWithValue("@Fte", report.Fte);
                    cmd.Parameters.AddWithValue("@CompanyId", report.CompanyId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateYearReport(YearReport report)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Yearreports SET year = @Year, fte = @Fte WHERE id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Year", report.Year);
                    cmd.Parameters.AddWithValue("@Fte", report.Fte);
                    cmd.Parameters.AddWithValue("@Id", report.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<YearReport> GetAllYearReports()
        {
            var list = new List<YearReport>();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, year, fte, company_id FROM Yearreports";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new YearReport
                        {
                            Id = (int)reader["id"],
                            Year = (int)reader["year"],
                            Fte = (int)reader["fte"],
                            CompanyId = (int)reader["company_id"]
                        });
                    }
                }
            }

            return list;
        }

        public static List<BenchmarkRow> GetFilteredYearReports(string sector, int? year)
        {
            var companies = CompanyService.GetAllCompanies();
            var reports = GetAllYearReports();

            var query = from report in reports
                        join company in companies on report.CompanyId equals company.Id
                        where (sector == null || company.Sector == sector)
                           && (!year.HasValue || report.Year == year.Value)
                        select new BenchmarkRow
                        {
                            Company = company,
                            Report = report
                        };

            return query.ToList();
        }



        public static void DeleteYearReport(int id)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // Eerst gerelateerde Answers verwijderen
                string deleteAnswers = "DELETE FROM Answers WHERE yearreport_id = @Id";
                using (SqlCommand cmd = new SqlCommand(deleteAnswers, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }

                // Daarna gerelateerde Costs verwijderen
                string deleteCosts = "DELETE FROM Costs WHERE yearreport_id = @Id";
                using (SqlCommand cmd = new SqlCommand(deleteCosts, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }

                // Daarna pas het YearReport verwijderen
                string deleteYearReport = "DELETE FROM Yearreports WHERE id = @Id";
                using (SqlCommand cmd = new SqlCommand(deleteYearReport, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }




    }
}
