using System;
using System.Windows;
using System.Windows.Controls;

namespace PharmacySystem
{
    public partial class MedicineDialog : Window
    {
        public Medicine Medicine { get; private set; }

        public MedicineDialog()
        {
            InitializeComponent();

            Medicine = new Medicine();
            this.DataContext = Medicine;

            // Устанавливаем значения по умолчанию
            dpExpiryDate.SelectedDate = DateTime.Now.AddYears(1);
            cmbForm.SelectedIndex = 0;

            txtName.Focus();
        }

        public MedicineDialog(Medicine medicine) : this()
        {
            Medicine = medicine;

            // Заполняем поля формы
            txtName.Text = medicine.Name;
            txtManufacturer.Text = medicine.Manufacturer;
            txtPrice.Text = medicine.Price.ToString("0.##");
            txtQuantity.Text = medicine.Quantity.ToString();
            dpExpiryDate.SelectedDate = medicine.ExpiryDate;
            chkPrescription.IsChecked = medicine.PrescriptionRequired;

            // Устанавливаем выбранную форму
            foreach (ComboBoxItem item in cmbForm.Items)
            {
                if (item.Content.ToString() == medicine.Form)
                {
                    cmbForm.SelectedItem = item;
                    break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                UpdateMedicineFromForm();
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private bool ValidateInput()
        {
            // Проверка названия
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название лекарства",
                    "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return false;
            }

            // Проверка производителя
            if (string.IsNullOrWhiteSpace(txtManufacturer.Text))
            {
                MessageBox.Show("Введите производителя",
                    "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtManufacturer.Focus();
                return false;
            }

            // Проверка цены
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену (положительное число)",
                    "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPrice.Focus();
                txtPrice.SelectAll();
                return false;
            }

            // Проверка количества
            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Введите корректное количество (неотрицательное число)",
                    "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtQuantity.Focus();
                txtQuantity.SelectAll();
                return false;
            }

            // Проверка срока годности
            if (dpExpiryDate.SelectedDate == null || dpExpiryDate.SelectedDate <= DateTime.Now)
            {
                MessageBox.Show("Выберите корректный срок годности",
                    "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpExpiryDate.Focus();
                return false;
            }

            return true;
        }

        private void UpdateMedicineFromForm()
        {
            Medicine.Name = txtName.Text.Trim();
            Medicine.Manufacturer = txtManufacturer.Text.Trim();
            Medicine.Form = (cmbForm.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (decimal.TryParse(txtPrice.Text, out decimal price))
                Medicine.Price = price;

            if (int.TryParse(txtQuantity.Text, out int quantity))
                Medicine.Quantity = quantity;

            if (dpExpiryDate.SelectedDate.HasValue)
                Medicine.ExpiryDate = dpExpiryDate.SelectedDate.Value;

            Medicine.PrescriptionRequired = chkPrescription.IsChecked ?? false;
        }
    }
}