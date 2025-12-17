using System;

namespace PharmacySystem
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }

        public Employee()
        {
            FullName = string.Empty;
            Position = string.Empty;
            HireDate = DateTime.Now;
        }
    }

    public class Medicine
    {
        public int MedicineID { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Form { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool PrescriptionRequired { get; set; }

        public Medicine()
        {
            Name = string.Empty;
            Manufacturer = string.Empty;
            Form = string.Empty;
            ExpiryDate = DateTime.Now.AddYears(1);
        }
    }
}