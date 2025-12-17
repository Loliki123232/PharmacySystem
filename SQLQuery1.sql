-- Создание базы данных
CREATE DATABASE PharmacyDB;
GO

USE PharmacyDB;
GO

-- Таблица сотрудников
CREATE TABLE Employees (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Position NVARCHAR(50) NOT NULL,
    Salary DECIMAL(10,2),
    HireDate DATE,
    LicenseNumber NVARCHAR(50) NULL,
    Responsibilities NVARCHAR(500) NULL
);

-- Таблица лекарств
CREATE TABLE Medicines (
    MedicineID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Manufacturer NVARCHAR(100),
    Form NVARCHAR(50),
    Price DECIMAL(10,2) NOT NULL,
    Quantity INT NOT NULL,
    ExpiryDate DATE,
    PrescriptionRequired BIT DEFAULT 0,
    SupplierID INT NULL,
    LastRestockDate DATE
);

-- Таблица поставщиков
CREATE TABLE Suppliers (
    SupplierID INT PRIMARY KEY IDENTITY(1,1),
    CompanyName NVARCHAR(100) NOT NULL,
    ContactPerson NVARCHAR(100),
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    Address NVARCHAR(200)
);

-- Таблица рецептов
CREATE TABLE Prescriptions (
    PrescriptionID INT PRIMARY KEY IDENTITY(1,1),
    DoctorName NVARCHAR(100) NOT NULL,
    PatientName NVARCHAR(100) NOT NULL,
    Diagnosis NVARCHAR(200),
    IssueDate DATE NOT NULL,
    ExpiryDate DATE,
    EmployeeID INT,
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

-- Таблица продаж
CREATE TABLE Sales (
    SaleID INT PRIMARY KEY IDENTITY(1,1),
    SaleDate DATETIME DEFAULT GETDATE(),
    EmployeeID INT,
    TotalAmount DECIMAL(10,2),
    PaymentMethod NVARCHAR(20),
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

-- Таблица деталей продаж
CREATE TABLE SaleDetails (
    SaleDetailID INT PRIMARY KEY IDENTITY(1,1),
    SaleID INT,
    MedicineID INT,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2),
    FOREIGN KEY (SaleID) REFERENCES Sales(SaleID),
    FOREIGN KEY (MedicineID) REFERENCES Medicines(MedicineID)
);

-- Таблица инвентаризации
CREATE TABLE InventoryLog (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    MedicineID INT,
    ChangeDate DATETIME DEFAULT GETDATE(),
    OldQuantity INT,
    NewQuantity INT,
    EmployeeID INT,
    ChangeReason NVARCHAR(200),
    FOREIGN KEY (MedicineID) REFERENCES Medicines(MedicineID),
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

-- Внешние ключи
ALTER TABLE Medicines
ADD CONSTRAINT FK_Medicines_Suppliers
FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID);

-- Добавление тестовых данных
INSERT INTO Employees (FullName, Position, Salary, HireDate) VALUES
('Иванов П.С.', 'Заведующий', 75000, '2020-01-15'),
('Петрова А.И.', 'Фармацевт', 45000, '2021-03-10'),
('Сидоров В.М.', 'Товаровед', 40000, '2021-05-20'),
('Козлова Е.П.', 'Кассир', 35000, '2022-02-01'),
('Никитин Д.А.', 'Консультант', 38000, '2022-03-15'),
('Фролова Г.С.', 'Обслуживающий персонал', 28000, '2022-06-10');

INSERT INTO Suppliers (CompanyName, ContactPerson, Phone) VALUES
('Фармакор', 'Смирнов А.В.', '+7(495)123-45-67'),
('Медпрепараты', 'Ковалева Т.П.', '+7(495)234-56-78'),
('Биотех', 'Орлов М.С.', '+7(495)345-67-89');

INSERT INTO Medicines (Name, Manufacturer, Form, Price, Quantity, ExpiryDate, PrescriptionRequired, SupplierID) VALUES
('Парацетамол', 'Фармакор', 'Таблетки', 50.00, 100, '2025-12-31', 0, 1),
('Ибупрофен', 'Фармакор', 'Таблетки', 75.00, 80, '2025-10-31', 0, 1),
('Амоксициллин', 'Медпрепараты', 'Капсулы', 120.00, 50, '2024-08-31', 1, 2),
('Називин', 'Биотех', 'Спрей', 150.00, 60, '2024-11-30', 0, 3),
('Ношпа', 'Медпрепараты', 'Таблетки', 85.00, 90, '2025-05-31', 0, 2);