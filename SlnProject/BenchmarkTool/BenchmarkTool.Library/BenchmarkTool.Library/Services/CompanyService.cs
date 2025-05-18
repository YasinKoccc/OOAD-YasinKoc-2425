using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BenchmarkTool.Library.Models;

namespace BenchmarkTool.Library.Services
{
    public static class CompanyService
    {
        public static List<Company> GetAllCompanies()
        {
            var companies = new List<Company>();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT id, name, contact, address, zip, city, country, phone, email, btw, login, password, regdate, acceptdate, lastmodified, status, language, logo, nacecode_code, sector
                 FROM Companies";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var company = new Company
                    {
                        Id = (int)reader["id"],
                        Name = reader["name"] as string,
                        Contact = reader["contact"] as string,
                        Address = reader["address"] as string,
                        Zip = reader["zip"] as string,
                        City = reader["city"] as string,
                        Country = reader["country"] as string,
                        Phone = reader["phone"] as string,
                        Email = reader["email"] as string,
                        Btw = reader["btw"] as string,
                        Login = reader["login"] as string,
                        Password = reader["password"] as string,
                        RegDate = (DateTime)reader["regdate"],
                        AcceptDate = reader["acceptdate"] as DateTime?,
                        LastModified = reader["lastmodified"] as DateTime?,
                        Status = reader["status"] as string,
                        Language = reader["language"] as string,
                        Logo = reader["logo"] as byte[],
                        Nacecode_Code = reader["nacecode_code"] as string,
                        Sector = reader["sector"] as string,

                    };
                    companies.Add(company);
                }
            }
            return companies;
        }

        public static void AddCompany(Company company)
        {
            int newId = GetMaxCompanyId() + 1;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Companies 
            (id, name, contact, email, login, password, regdate, status, nacecode_code) 
            VALUES (@id, @name, @contact, @email, @login, @password, @regdate, @status, @nacecode_code)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", newId);
                    cmd.Parameters.AddWithValue("@name", company.Name);
                    cmd.Parameters.AddWithValue("@contact", company.Contact);
                    cmd.Parameters.AddWithValue("@email", company.Email);
                    cmd.Parameters.AddWithValue("@login", company.Login);
                    cmd.Parameters.AddWithValue("@password", company.Password);
                    cmd.Parameters.AddWithValue("@regdate", company.RegDate);
                    cmd.Parameters.AddWithValue("@status", company.Status);
                    cmd.Parameters.AddWithValue("@nacecode_code", company.Nacecode_Code ?? "99999");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int GetMaxCompanyId()
        {
            int maxId = 0;
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT ISNULL(MAX(id), 0) FROM Companies";
                SqlCommand cmd = new SqlCommand(query, conn);
                maxId = (int)cmd.ExecuteScalar();
            }
            return maxId;
        }

        public static Company GetCompanyById(int companyId)
        {
            Company company = null;
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Companies WHERE id = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", companyId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            company = new Company
                            {
                                Id = (int)reader["id"],
                                Name = reader["name"] as string,
                                Contact = reader["contact"] as string,
                                Address = reader["address"] as string,
                                Zip = reader["zip"] as string,
                                City = reader["city"] as string,
                                Country = reader["country"] as string,
                                Phone = reader["phone"] as string,
                                Email = reader["email"] as string,
                                Btw = reader["btw"] as string,
                                Login = reader["login"] as string,
                                Password = reader["password"] as string,
                                RegDate = (DateTime)reader["regdate"],
                                AcceptDate = reader["acceptdate"] as DateTime?,
                                LastModified = reader["lastmodified"] as DateTime?,
                                Status = reader["status"] as string,
                                Language = reader["language"] as string,
                                Logo = reader["logo"] as byte[],
                                Nacecode_Code = reader["nacecode_code"] as string,
                                Sector = reader["sector"] as string,

                            };
                        }
                    }
                }
            }
            return company;
        }

        public static void UpdateCompany(Company company)
        {
            // Zorg dat verplichte velden niet null zijn
            if (company.Address == null) company.Address = "";
            if (company.Zip == null) company.Zip = "";
            if (company.City == null) company.City = "";
            if (company.Country == null) company.Country = "";
            if (company.Phone == null) company.Phone = "";
            if (company.Btw == null) company.Btw = "";
            if (company.Language == null) company.Language = "NL";
            if (company.Status == null) company.Status = "Actief";
            if (company.Nacecode_Code == null) company.Nacecode_Code = "99999";
            company.RegDate = company.RegDate < new DateTime(1753, 1, 1) ? DateTime.Now : company.RegDate;
            company.LastModified = DateTime.Now;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"UPDATE Companies SET 
                            name = @name, contact = @contact, address = @address, zip = @zip, city = @city, country = @country,
                            phone = @phone, email = @email, btw = @btw, login = @login, password = @password, regdate = @regdate, 
                            acceptdate = @acceptdate, lastmodified = @lastmodified, status = @status, language = @language, 
                            logo = @logo, nacecode_code = @nacecode_code
                         WHERE id = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", company.Id);
                    cmd.Parameters.AddWithValue("@name", company.Name);
                    cmd.Parameters.AddWithValue("@contact", company.Contact);
                    cmd.Parameters.AddWithValue("@address", company.Address);
                    cmd.Parameters.AddWithValue("@zip", company.Zip);
                    cmd.Parameters.AddWithValue("@city", company.City);
                    cmd.Parameters.AddWithValue("@country", company.Country);
                    cmd.Parameters.AddWithValue("@phone", company.Phone);
                    cmd.Parameters.AddWithValue("@email", company.Email);
                    cmd.Parameters.AddWithValue("@btw", company.Btw);
                    cmd.Parameters.AddWithValue("@login", company.Login);
                    cmd.Parameters.AddWithValue("@password", company.Password);
                    cmd.Parameters.AddWithValue("@regdate", company.RegDate);
                    cmd.Parameters.AddWithValue("@acceptdate",
                        company.AcceptDate.HasValue && company.AcceptDate.Value >= new DateTime(1753, 1, 1)
                        ? (object)company.AcceptDate.Value
                        : DBNull.Value);
                    cmd.Parameters.AddWithValue("@lastmodified", company.LastModified);
                    cmd.Parameters.AddWithValue("@status", company.Status);
                    cmd.Parameters.AddWithValue("@language", company.Language);
                    cmd.Parameters.AddWithValue("@logo", (object)company.Logo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@nacecode_code", company.Nacecode_Code);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static void UpdateCompanyStatus(int companyId, string newStatus)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Companies SET status = @status WHERE id = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@status", newStatus);
                    cmd.Parameters.AddWithValue("@id", companyId);
                    cmd.ExecuteNonQuery();
                }
            }
        }




        public static Company Login(string email, string password)
        {
            string hashed = HashPassword(password);

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Companies WHERE email = @Email AND password = @Password";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", hashed);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Company
                            {
                                Id = (int)reader["id"],
                                Name = reader["name"] as string,
                                Email = reader["email"] as string,
                                Password = reader["password"] as string, // ✅ voeg dit toe
                                Logo = reader["logo"] as byte[]
                            };
                        }
                    }
                }
            }

            return null;
        }


        private static string HashPassword(string password)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                var sb = new System.Text.StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public static void DeleteCompany(int companyId)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM Companies WHERE id = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", companyId);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static List<string> GetNacecodes()
        {
            var codes = new List<string>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT code FROM Nacecodes ORDER BY code";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        codes.Add(reader["code"].ToString());
                    }
                }
            }
            return codes;
        }

        public static void UpdateStatus(int companyId, string newStatus)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Companies SET status = @status WHERE id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@status", newStatus);
                    cmd.Parameters.AddWithValue("@id", companyId);
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}
