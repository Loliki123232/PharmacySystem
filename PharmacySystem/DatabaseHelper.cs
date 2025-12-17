using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace PharmacySystem
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["PharmacyConnection"].ConnectionString;
            }
            catch
            {
                connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PharmacyDB;Integrated Security=True";
            }
        }

        // Проверка подключения
        public bool TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
                return false;
            }
        }

        // CRUD операции для сотрудников
        public List<Employee> GetEmployees(string search = "", string position = "")
        {
            var employees = new List<Employee>();

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Employees WHERE 1=1";

                if (!string.IsNullOrEmpty(search))
                {
                    query += " AND FullName LIKE @Search";
                }

                if (!string.IsNullOrEmpty(position) && position != "Все должности")
                {
                    query += " AND Position = @Position";
                }

                using (var command = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(search))
                        command.Parameters.AddWithValue("@Search", "%" + search + "%");

                    if (!string.IsNullOrEmpty(position) && position != "Все должности")
                        command.Parameters.AddWithValue("@Position", position);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new Employee
                            {
                                EmployeeID = (int)reader["EmployeeID"],
                                FullName = reader["FullName"].ToString(),
                                Position = reader["Position"].ToString(),
                                Salary = reader["Salary"] != DBNull.Value ? (decimal)reader["Salary"] : 0,
                                HireDate = reader["HireDate"] != DBNull.Value ? (DateTime)reader["HireDate"] : DateTime.MinValue
                            });
                        }
                    }
                }
            }

            return employees;
        }

        public bool AddEmployee(Employee employee)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Employees 
                                (FullName, Position, Salary, HireDate) 
                                VALUES (@FullName, @Position, @Salary, @HireDate)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", employee.FullName);
                    command.Parameters.AddWithValue("@Position", employee.Position);
                    command.Parameters.AddWithValue("@Salary", employee.Salary);
                    command.Parameters.AddWithValue("@HireDate", employee.HireDate);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateEmployee(Employee employee)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Employees SET 
                                FullName = @FullName,
                                Position = @Position,
                                Salary = @Salary,
                                HireDate = @HireDate
                                WHERE EmployeeID = @EmployeeID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                    command.Parameters.AddWithValue("@FullName", employee.FullName);
                    command.Parameters.AddWithValue("@Position", employee.Position);
                    command.Parameters.AddWithValue("@Salary", employee.Salary);
                    command.Parameters.AddWithValue("@HireDate", employee.HireDate);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteEmployee(int employeeId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // CRUD операции для лекарств
        public List<Medicine> GetMedicines(string search = "", bool prescriptionOnly = false)
        {
            var medicines = new List<Medicine>();

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Medicines WHERE 1=1";

                if (!string.IsNullOrEmpty(search))
                {
                    query += " AND (Name LIKE @Search OR Manufacturer LIKE @Search)";
                }

                if (prescriptionOnly)
                {
                    query += " AND PrescriptionRequired = 1";
                }

                using (var command = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(search))
                        command.Parameters.AddWithValue("@Search", "%" + search + "%");

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            medicines.Add(new Medicine
                            {
                                MedicineID = (int)reader["MedicineID"],
                                Name = reader["Name"].ToString(),
                                Manufacturer = reader["Manufacturer"].ToString(),
                                Form = reader["Form"].ToString(),
                                Price = reader["Price"] != DBNull.Value ? (decimal)reader["Price"] : 0,
                                Quantity = (int)reader["Quantity"],
                                ExpiryDate = reader["ExpiryDate"] != DBNull.Value ? (DateTime)reader["ExpiryDate"] : DateTime.MinValue,
                                PrescriptionRequired = (bool)reader["PrescriptionRequired"]
                            });
                        }
                    }
                }
            }

            return medicines;
        }

        public bool AddMedicine(Medicine medicine)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Medicines 
                                (Name, Manufacturer, Form, Price, Quantity, ExpiryDate, PrescriptionRequired) 
                                VALUES (@Name, @Manufacturer, @Form, @Price, @Quantity, @ExpiryDate, @PrescriptionRequired)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", medicine.Name);
                    command.Parameters.AddWithValue("@Manufacturer", medicine.Manufacturer);
                    command.Parameters.AddWithValue("@Form", medicine.Form);
                    command.Parameters.AddWithValue("@Price", medicine.Price);
                    command.Parameters.AddWithValue("@Quantity", medicine.Quantity);
                    command.Parameters.AddWithValue("@ExpiryDate", medicine.ExpiryDate);
                    command.Parameters.AddWithValue("@PrescriptionRequired", medicine.PrescriptionRequired);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateMedicine(Medicine medicine)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Medicines SET 
                                Name = @Name,
                                Manufacturer = @Manufacturer,
                                Form = @Form,
                                Price = @Price,
                                Quantity = @Quantity,
                                ExpiryDate = @ExpiryDate,
                                PrescriptionRequired = @PrescriptionRequired
                                WHERE MedicineID = @MedicineID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MedicineID", medicine.MedicineID);
                    command.Parameters.AddWithValue("@Name", medicine.Name);
                    command.Parameters.AddWithValue("@Manufacturer", medicine.Manufacturer);
                    command.Parameters.AddWithValue("@Form", medicine.Form);
                    command.Parameters.AddWithValue("@Price", medicine.Price);
                    command.Parameters.AddWithValue("@Quantity", medicine.Quantity);
                    command.Parameters.AddWithValue("@ExpiryDate", medicine.ExpiryDate);
                    command.Parameters.AddWithValue("@PrescriptionRequired", medicine.PrescriptionRequired);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteMedicine(int medicineId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Medicines WHERE MedicineID = @MedicineID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MedicineID", medicineId);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // Получить все поставщики
        public List<string> GetSuppliers()
        {
            var suppliers = new List<string>();

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT DISTINCT CompanyName FROM Suppliers";

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            suppliers.Add(reader["CompanyName"].ToString());
                        }
                    }
                }
            }

            return suppliers;
        }
    }
}