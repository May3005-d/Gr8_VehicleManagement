# 🗄️ **GR8 VEHICLE MANAGEMENT - DATABASE SCRIPTS**

## 📁 **CẤU TRÚC FILES:**

### **✅ Scripts Chính:**
- **`00_RunAll.sql`** - Chạy tất cả scripts theo thứ tự
- **`01_CreateDatabase.sql`** - Tạo database `Gr8_VehicleDealer`
- **`02_CreateTables.sql`** - Tạo 15 bảng đơn giản cho sinh viên
- **`03_SeedData.sql`** - Dữ liệu mẫu cơ bản
- **`04_UsefulQueries.sql`** - Các query hữu ích

### **📊 Database Schema:**
- **15 bảng** với cấu trúc đơn giản
- **Foreign Keys** sử dụng `ON DELETE NO ACTION` để tránh cascade conflicts
- **Constraints** cơ bản cho data integrity

## 🚀 **HƯỚNG DẪN SỬ DỤNG:**

### **Cách 1: Chạy tất cả (Khuyến nghị)**
```sql
-- Mở SQL Server Management Studio
-- Chạy file: 00_RunAll.sql
```

### **Cách 2: Chạy từng bước**
```sql
-- Bước 1: Tạo database
sqlcmd -S localhost -i "01_CreateDatabase.sql"

-- Bước 2: Tạo bảng
sqlcmd -S localhost -i "02_CreateTables.sql"

-- Bước 3: Thêm dữ liệu mẫu
sqlcmd -S localhost -i "03_SeedData.sql"
```

## 📋 **DANH SÁCH BẢNG:**

### **👥 User Management:**
1. **Users** - Người dùng hệ thống
2. **Roles** - Vai trò người dùng
3. **UserRoles** - Liên kết user-role

### **🏢 Business:**
4. **Dealers** - Đại lý
5. **Customers** - Khách hàng

### **🚗 Vehicle Management:**
6. **VehicleModels** - Mẫu xe
7. **VehicleVersions** - Phiên bản xe
8. **VehicleInventory** - Kho xe
9. **VehicleImages** - Hình ảnh xe

### **📦 Orders & Payments:**
10. **Orders** - Đơn hàng
11. **Payments** - Thanh toán

### **🎁 Marketing:**
12. **Promotions** - Khuyến mãi

### **📈 Sales & Support:**
13. **SalesTargets** - Mục tiêu bán hàng
14. **Feedbacks** - Phản hồi khách hàng

### **📊 System:**
15. **AuditLogs** - Nhật ký hệ thống

## 🔧 **CẤU TRÚC ĐƠN GIẢN:**

### **Users Table:**
```sql
- Id (GUID, Primary Key)
- Username (NVARCHAR(50), Unique)
- Email (NVARCHAR(256), Unique)
- PhoneNumber (NVARCHAR(20), Unique)
- PasswordHash (NVARCHAR(MAX))
- FullName (NVARCHAR(200))
- UserType (INT: 1=Customer, 2=DealerStaff, 3=DealerManager, 4=EVMStaff, 5=Admin)
- DealerId (GUID, Foreign Key)
- CreatedAt, UpdatedAt (DATETIME2)
```

### **VehicleModels Table:**
```sql
- Id (GUID, Primary Key)
- Name, Brand, Category (NVARCHAR)
- Description (NVARCHAR(MAX))
- EngineType (NVARCHAR(50))
- SeatingCapacity (INT)
- Range (INT) - km
- BatteryCapacity (DECIMAL(10,2)) - kWh
- ChargingTime (DECIMAL(10,2)) - hours
- MaxSpeed (INT) - km/h
- Features (NVARCHAR(MAX))
- IsActive (BIT)
- LaunchedAt (DATETIME2)
- CreatedAt, UpdatedAt (DATETIME2)
```

## 📊 **DỮ LIỆU MẪU:**

### **Users:**
- **admin** (UserType: 5) - Quản trị viên
- **customer1** (UserType: 1) - Khách hàng

### **Dealers:**
- **Đại lý Hà Nội** - Hà Nội
- **Đại lý TP.HCM** - TP.HCM

### **Vehicles:**
- **VinFast VF8** - SUV điện
- **Tesla Model 3** - Sedan điện

### **Promotions:**
- **Khuyến mãi đầu năm** - Giảm 5%
- **Ưu đãi VinFast** - Giảm 50 triệu

## ⚠️ **LƯU Ý QUAN TRỌNG:**

### **✅ Đã tối ưu cho sinh viên:**
- Cấu trúc đơn giản, dễ hiểu
- Ít field phức tạp
- Foreign Keys an toàn
- Dữ liệu mẫu cơ bản

### **🔒 Bảo mật:**
- Password được hash
- Session management
- CSRF protection (skip cho auth pages)

### **📱 Tương thích:**
- Entity Framework Core
- ASP.NET Core Razor Pages
- SQL Server 2019+

## 🎯 **KẾT QUẢ:**

Sau khi chạy scripts, bạn sẽ có:
- ✅ Database `Gr8_VehicleDealer` hoàn chỉnh
- ✅ 15 bảng với cấu trúc đơn giản
- ✅ Dữ liệu mẫu để test
- ✅ Sẵn sàng cho development

**🎉 Database sẵn sàng cho dự án sinh viên!**