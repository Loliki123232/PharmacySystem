using System;
using System.Windows;
using System.Windows.Controls;

namespace PharmacySystem
{
    /// <summary>
    /// Диалоговое окно для добавления/редактирования сотрудников
    /// </summary>
    public partial class EmployeeDialog : Window
    {
        /// <summary>
        /// Объект сотрудника для работы с данными
        /// </summary>
        public Employee Employee { get; private set; }

        /// <summary>
        /// Конструктор для добавления нового сотрудника
        /// </summary>
        public EmployeeDialog()
        {
            InitializeComponent();

            // Создаем нового сотрудника с датой приема по умолчанию
            Employee = new Employee
            {
                HireDate = DateTime.Now
            };

            // Устанавливаем DataContext для привязки данных
            this.DataContext = Employee;

            // Устанавливаем текущую дату в DatePicker
            dpHireDate.SelectedDate = DateTime.Now;

            // Выбираем первую должность по умолчанию
            cmbPosition.SelectedIndex = 0;

            // Устанавливаем фокус на поле ФИО
            txtFullName.Focus();
        }

        /// <summary>
        /// Конструктор для редактирования существующего сотрудника
        /// </summary>
        /// <param name="employee">Сотрудник для редактирования</param>
        public EmployeeDialog(Employee employee) : this()
        {
            // Сохраняем переданного сотрудника
            Employee = employee;

            // Заполняем поля формы
            txtFullName.Text = employee.FullName;
            txtSalary.Text = employee.Salary.ToString("0.##");

            // Устанавливаем дату приема
            dpHireDate.SelectedDate = employee.HireDate;

            // Устанавливаем выбранную должность
            foreach (ComboBoxItem item in cmbPosition.Items)
            {
                if (item.Content.ToString() == employee.Position)
                {
                    cmbPosition.SelectedItem = item;
                    break;
                }
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сохранить"
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка валидации
            if (!ValidateInput())
                return;

            try
            {
                // Обновляем объект Employee данными из формы
                UpdateEmployeeFromForm();

                // Устанавливаем результат диалога
                this.DialogResult = true;

                // Закрываем окно
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Отмена"
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Устанавливаем результат диалога
            this.DialogResult = false;

            // Закрываем окно
            this.Close();
        }

        /// <summary>
        /// Валидация введенных данных
        /// </summary>
        /// <returns>True если данные валидны, иначе False</returns>
        private bool ValidateInput()
        {
            // Проверка ФИО
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИО сотрудника",
                    "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFullName.Focus();
                return false;
            }

            // Проверка зарплаты
            if (!decimal.TryParse(txtSalary.Text, out decimal salary) || salary < 0)
            {
                MessageBox.Show("Введите корректную зарплату (положительное число)",
                    "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtSalary.Focus();
                txtSalary.SelectAll();
                return false;
            }

            // Проверка даты приема
            if (dpHireDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату приема на работу",
                    "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpHireDate.Focus();
                return false;
            }

            // Проверка должности
            if (cmbPosition.SelectedItem == null)
            {
                MessageBox.Show("Выберите должность сотрудника",
                    "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbPosition.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Обновление объекта Employee данными из формы
        /// </summary>
        private void UpdateEmployeeFromForm()
        {
            Employee.FullName = txtFullName.Text.Trim();
            Employee.Position = (cmbPosition.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (decimal.TryParse(txtSalary.Text, out decimal salary))
                Employee.Salary = salary;

            if (dpHireDate.SelectedDate.HasValue)
                Employee.HireDate = dpHireDate.SelectedDate.Value;
        }

        /// <summary>
        /// Обработчик загрузки окна
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Можно добавить дополнительную инициализацию
        }

        /// <summary>
        /// Обработчик нажатия клавиш
        /// </summary>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Enter - сохранить
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SaveButton_Click(sender, e);
            }
            // Escape - отмена
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                CancelButton_Click(sender, e);
            }
        }
    }
}