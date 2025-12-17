using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PharmacySystem
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper dbHelper;

        public MainWindow()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();

            // Проверка подключения
            if (!dbHelper.TestConnection())
            {
                MessageBox.Show("Не удалось подключиться к базе данных. Проверьте настройки подключения.");
            }

            LoadEmployees();
            LoadMedicines();
            LoadStatistics();
        }

        private void LoadEmployees(string search = "", string position = "")
        {
            try
            {
                var employees = dbHelper.GetEmployees(search, position);
                EmployeesGrid.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMedicines(string search = "", bool prescriptionOnly = false)
        {
            try
            {
                var medicines = dbHelper.GetMedicines(search, prescriptionOnly);
                MedicinesGrid.ItemsSource = medicines;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки лекарств: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStatistics()
        {
            try
            {
                var employees = dbHelper.GetEmployees();
                var medicines = dbHelper.GetMedicines();

                txtEmployeeCount.Text = employees.Count.ToString();
                txtMedicineCount.Text = medicines.Count.ToString();

                // Лекарства с истекающим сроком (менее 30 дней)
                var expiringMedicines = medicines
                    .Where(m => (m.ExpiryDate - DateTime.Now).TotalDays <= 30)
                    .Count();

                txtExpiringMedicines.Text = expiringMedicines.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статистики: {ex.Message}");
            }
        }

        // Обработчики событий для сотрудников
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = SearchBox.Text;
            string position = (PositionFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            LoadEmployees(search, position);
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            LoadEmployees(SearchBox.Text, (PositionFilter.SelectedItem as ComboBoxItem)?.Content.ToString());
        }

        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            LoadEmployees();
            LoadMedicines();
            LoadStatistics();
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EmployeeDialog();
            if (dialog.ShowDialog() == true)
            {
                if (dbHelper.AddEmployee(dialog.Employee))
                {
                    MessageBox.Show("Сотрудник добавлен",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadEmployees();
                    LoadStatistics();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении сотрудника",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem is Employee selectedEmployee)
            {
                var dialog = new EmployeeDialog(selectedEmployee);
                if (dialog.ShowDialog() == true)
                {
                    if (dbHelper.UpdateEmployee(dialog.Employee))
                    {
                        MessageBox.Show("Данные сотрудника обновлены",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadEmployees();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении данных сотрудника",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для редактирования",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem is Employee selectedEmployee)
            {
                var result = MessageBox.Show($"Удалить сотрудника '{selectedEmployee.FullName}'?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (dbHelper.DeleteEmployee(selectedEmployee.EmployeeID))
                    {
                        MessageBox.Show("Сотрудник удален",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadEmployees();
                        LoadStatistics();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении сотрудника",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для удаления",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Обработчики событий для лекарств
        private void MedicineSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadMedicines(MedicineSearch.Text, PrescriptionOnly.IsChecked ?? false);
        }

        private void AddMedicine_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MedicineDialog();
            if (dialog.ShowDialog() == true)
            {
                if (dbHelper.AddMedicine(dialog.Medicine))
                {
                    MessageBox.Show("Лекарство добавлено",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadMedicines();
                    LoadStatistics();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении лекарства",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditMedicine_Click(object sender, RoutedEventArgs e)
        {
            if (MedicinesGrid.SelectedItem is Medicine selectedMedicine)
            {
                var dialog = new MedicineDialog(selectedMedicine);
                if (dialog.ShowDialog() == true)
                {
                    if (dbHelper.UpdateMedicine(dialog.Medicine))
                    {
                        MessageBox.Show("Лекарство обновлено",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadMedicines();
                        LoadStatistics();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении лекарства",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите лекарство для редактирования",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteMedicine_Click(object sender, RoutedEventArgs e)
        {
            if (MedicinesGrid.SelectedItem is Medicine selectedMedicine)
            {
                var result = MessageBox.Show($"Удалить лекарство '{selectedMedicine.Name}'?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (dbHelper.DeleteMedicine(selectedMedicine.MedicineID))
                    {
                        MessageBox.Show("Лекарство удалено",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadMedicines();
                        LoadStatistics();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении лекарства",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите лекарство для удаления",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Обработчик двойного клика по таблице сотрудников
        private void EmployeesGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditEmployee_Click(sender, e);
        }

        // Обработчик двойного клика по таблице лекарств
        private void MedicinesGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditMedicine_Click(sender, e);
        }
    }
}