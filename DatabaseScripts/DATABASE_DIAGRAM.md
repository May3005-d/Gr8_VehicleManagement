# 📊 GR8_VEHICLEDEALER - DATABASE DIAGRAM

## 🗂️ ENTITY RELATIONSHIP DIAGRAM (ERD)

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                         GR8_VEHICLEDEALER DATABASE                               │
│                              15 TABLES SCHEMA                                    │
└─────────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────┐
│                       MODULE 1: AUTHENTICATION & ROLES                           │
└─────────────────────────────────────────────────────────────────────────────────┘

        ┌─────────────────┐
        │     Roles       │
        ├─────────────────┤
        │ • Id (PK)       │
        │   Name          │
        │   Description   │
        │   IsSystemRole  │
        └────────┬────────┘
                 │
                 │ Many-to-Many
                 │
        ┌────────▼────────┐
        │   UserRoles     │
        ├─────────────────┤
        │ • UserId (FK)   │
        │ • RoleId (FK)   │
        │   AssignedAt    │
        └────────┬────────┘
                 │
                 │
        ┌────────▼──────────────────────────┐
        │           Users                    │◄──────┐
        ├────────────────────────────────────┤       │
        │ • Id (PK)                          │       │
        │   Email (unique)                   │       │
        │   PhoneNumber (unique)             │       │
        │   PasswordHash                     │       │
        │   FullName                         │       │
        │   UserType (enum)                  │       │
        │   DealerId (FK) ────┐              │       │
        │   MfaEnabled        │              │       │
        │   IsLocked          │              │       │
        └──────────┬──────────┼──────────────┘       │
                   │          │                       │
                   │          │                       │
                   │          │   ┌───────────────────┘
                   │          │   │ (many relationships)
                   │          │   │

┌─────────────────────────────────────────────────────────────────────────────────┐
│                          MODULE 2: DEALERS                                       │
└─────────────────────────────────────────────────────────────────────────────────┘

                 ┌────────▼─────────────────────┐
                 │         Dealers              │◄──────┐
                 ├──────────────────────────────┤       │
                 │ • Id (PK)                    │       │
                 │   Code (unique)              │       │
                 │   Name                       │       │
                 │   Email, PhoneNumber         │       │
                 │   Address, City, Province    │       │
                 │   Status, Region             │       │
                 │   ContractNumber             │       │
                 │   SalesTarget                │       │
                 │   CommissionRate             │       │
                 │   CurrentDebt, CreditLimit   │       │
                 └──────────┬───────────────────┘       │
                            │                           │
                            │ One-to-Many               │

┌─────────────────────────────────────────────────────────────────────────────────┐
│                       MODULE 3: VEHICLES                                         │
└─────────────────────────────────────────────────────────────────────────────────┘

        ┌──────────────────────────┐
        │    VehicleModels         │
        ├──────────────────────────┤
        │ • Id (PK)                │
        │   Code (unique)          │
        │   Name                   │
        │   Category               │
        │   Description            │
        │   Specifications (JSON)  │
        │   IsActive               │
        └────────┬─────────────────┘
                 │
                 │ One-to-Many
                 │
        ┌────────▼──────────────────────────┐
        │      VehicleVersions              │
        ├───────────────────────────────────┤
        │ • Id (PK)                         │
        │   ModelId (FK)                    │
        │   VersionName                     │
        │   ColorName, ColorCode            │
        │   BatteryCapacity, Range          │
        │   MaxSpeed, ChargingTime          │
        │   SeatingCapacity                 │
        │   Features (JSON)                 │
        │   BasePrice, SellingPrice         │
        └────────┬──────────────────────────┘
                 │
                 │ One-to-Many
                 │
        ┌────────▼──────────────────────────┐
        │     VehicleInventory              │
        ├───────────────────────────────────┤
        │ • Id (PK)                         │
        │   VIN (unique)                    │
        │   VersionId (FK)                  │
        │   DealerId (FK, nullable)         │
        │   Status (enum)                   │
        │   Location                        │
        │   ManufacturedDate                │
        │   ReservedAt, SoldAt              │
        └────────┬──────────────────────────┘
                 │
                 │
        ┌────────▼──────────────────────────┐
        │      VehicleImages                │
        ├───────────────────────────────────┤
        │ • Id (PK)                         │
        │   VehicleModelId (FK, nullable)   │
        │   VehicleVersionId (FK, nullable) │
        │   VehicleInventoryId (FK, null)   │
        │   ImageUrl                        │
        │   ImageType                       │
        │   DisplayOrder, IsDefault         │
        └───────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────┐
│                       MODULE 4: CUSTOMERS & CRM                                  │
└─────────────────────────────────────────────────────────────────────────────────┘

        ┌──────────────────────────────────┐
        │          Customers               │
        ├──────────────────────────────────┤
        │ • Id (PK)                        │
        │   UserId (FK) ──► Users          │
        │   CustomerCode (unique)          │
        │   FullName, DateOfBirth          │
        │   IdNumber, Address              │
        │   CustomerType                   │
        │   AssignedDealerId (FK)          │
        │   AssignedStaffId (FK)           │
        │   Interactions (JSON)            │
        │   TestDriveHistory (JSON)        │
        └────────┬─────────────────────────┘
                 │
                 │ One-to-Many
                 │

┌─────────────────────────────────────────────────────────────────────────────────┐
│                    MODULE 5: SALES & ORDERS                                      │
└─────────────────────────────────────────────────────────────────────────────────┘

        ┌────────▼───────────────────────────┐
        │           Orders                   │
        ├────────────────────────────────────┤
        │ • Id (PK)                          │
        │   OrderNumber (unique)             │
        │   CustomerId (FK)                  │
        │   DealerId (FK)                    │
        │   CreatedBy (FK) ──► Users         │
        │   VersionId (FK)                   │
        │   VehicleInventoryId (FK)          │
        │   BasePrice, DiscountAmount        │
        │   TotalAmount                      │
        │   DepositAmount, RemainingAmount   │
        │   OrderDate                        │
        │   OrderStatus, PaymentStatus       │
        │   PaymentMethod                    │
        │   ContractNumber (merged)          │
        │   CustomerSignature                │
        │   DeliveryNotes, DeliveryPhotos    │
        │   InstallmentProvider, Term        │
        └────────┬───────────────────────────┘
                 │
                 │ One-to-Many
                 │
        ┌────────▼───────────────────────────┐
        │          Payments                  │
        ├────────────────────────────────────┤
        │ • Id (PK)                          │
        │   PaymentNumber (unique)           │
        │   OrderId (FK)                     │
        │   CustomerId (FK)                  │
        │   Amount                           │
        │   PaymentType, PaymentMethod       │
        │   PaymentDate, Status              │
        │   TransactionId, PaymentProof      │
        │   RefundReason (merged)            │
        │   RefundApprovedBy (FK)            │
        │   InstallmentNumber (merged)       │
        │   DueDate, LateFee                 │
        └────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────┐
│                       MODULE 6: PROMOTIONS                                       │
└─────────────────────────────────────────────────────────────────────────────────┘

        ┌─────────────────────────────────┐
        │        Promotions               │
        ├─────────────────────────────────┤
        │ • Id (PK)                       │
        │   Code (unique)                 │
        │   Name, Description             │
        │   DiscountType, DiscountValue   │
        │   ApplicableModels (JSON)       │
        │   ApplicableDealers (JSON)      │
        │   MinOrderAmount                │
        │   StartDate, EndDate            │
        │   Quota, UsedCount              │
        │   Priority, CanCombine          │
        └─────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────┐
│                   MODULE 7: SALES TARGETS & COMMISSION                           │
└─────────────────────────────────────────────────────────────────────────────────┘

        ┌─────────────────────────────────┐
        │       SalesTargets              │
        ├─────────────────────────────────┤
        │ • Id (PK)                       │
        │   TargetFor (Dealer/Staff)      │
        │   DealerId (FK, nullable)       │
        │   StaffId (FK, nullable)        │
        │   Year, Month, Quarter          │
        │   TargetQuantity, TargetRevenue │
        │   ActualQuantity, ActualRevenue │
        │   AchievementRate               │
        │   CommissionEarned              │
        └─────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────┐
│                    MODULE 8: CUSTOMER SERVICE                                    │
└─────────────────────────────────────────────────────────────────────────────────┘

        ┌─────────────────────────────────┐
        │         Feedbacks               │
        ├─────────────────────────────────┤
        │ • Id (PK)                       │
        │   FeedbackNumber (unique)       │
        │   CustomerId (FK)               │
        │   OrderId (FK, nullable)        │
        │   DealerId (FK)                 │
        │   Type (Feedback/Complaint)     │
        │   Category, Subject, Content    │
        │   Evidence (JSON)               │
        │   Rating                        │
        │   Status, Priority              │
        │   AssignedTo (FK) ──► Users     │
        │   SLA, Resolution               │
        └─────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────┐
│                       MODULE 9: AUDIT & LOGGING                                  │
└─────────────────────────────────────────────────────────────────────────────────┘

        ┌─────────────────────────────────┐
        │        AuditLogs                │
        ├─────────────────────────────────┤
        │ • Id (PK)                       │
        │   UserId (FK, nullable)         │
        │   UserEmail                     │
        │   Action                        │
        │   EntityType, EntityId          │
        │   OldValue (JSON)               │
        │   NewValue (JSON)               │
        │   IpAddress, UserAgent          │
        │   Result, ErrorMessage          │
        │   Timestamp                     │
        └─────────────────────────────────┘

```

## 🔗 KEY RELATIONSHIPS

### **One-to-Many Relationships:**
```
Users          ──► Customers (UserId)
Users          ──► Orders (CreatedBy)
Users          ──► SalesTargets (StaffId)
Dealers        ──► Users (DealerId)
Dealers        ──► Customers (AssignedDealerId)
Dealers        ──► Orders (DealerId)
Dealers        ──► VehicleInventory (DealerId)
Dealers        ──► SalesTargets (DealerId)
VehicleModels  ──► VehicleVersions (ModelId)
VehicleModels  ──► VehicleImages (VehicleModelId)
VehicleVersions ──► VehicleInventory (VersionId)
VehicleVersions ──► VehicleImages (VehicleVersionId)
VehicleVersions ──► Orders (VersionId)
VehicleInventory ──► VehicleImages (VehicleInventoryId)
VehicleInventory ──► Orders (VehicleInventoryId)
Customers      ──► Orders (CustomerId)
Customers      ──► Payments (CustomerId)
Customers      ──► Feedbacks (CustomerId)
Orders         ──► Payments (OrderId)
Orders         ──► Feedbacks (OrderId)
```

### **Many-to-Many Relationships:**
```
Users ◄──► Roles (via UserRoles)
```

### **Self-Referencing:**
```
Users.AssignedBy ──► Users.Id (trong UserRoles)
Payments.RefundApprovedBy ──► Users.Id
Feedbacks.AssignedTo ──► Users.Id
```

## 📋 TABLE GROUPING

### **Core Tables (Must Have Data):**
1. Users
2. Roles
3. Dealers
4. VehicleModels
5. VehicleVersions

### **Operational Tables:**
6. VehicleInventory
7. Customers
8. Orders
9. Payments

### **Supporting Tables:**
10. VehicleImages
11. Promotions
12. SalesTargets
13. Feedbacks

### **System Tables:**
14. UserRoles
15. AuditLogs

## 🎨 COLOR CODING (for ER diagrams)

- 🔵 **Blue**: Authentication & Users
- 🟢 **Green**: Vehicles & Inventory
- 🟡 **Yellow**: Sales & Orders
- 🟠 **Orange**: Customers & CRM
- 🔴 **Red**: Finance & Payments
- ⚪ **Gray**: System & Audit

---

## 📈 DATA FLOW

```
1. USER REGISTRATION
   Users → UserRoles → Roles

2. VEHICLE CATALOG
   VehicleModels → VehicleVersions → VehicleImages

3. INVENTORY MANAGEMENT
   VehicleVersions → VehicleInventory → Dealers

4. SALES PROCESS
   Users (Customer) → Customers → Orders → Payments

5. DELIVERY
   Orders → VehicleInventory (assign VIN) → Update Status

6. CUSTOMER SERVICE
   Customers → Orders → Feedbacks → Users (assigned staff)

7. REPORTING
   Orders + Payments → SalesTargets (update actual)
   All tables → AuditLogs (tracking)
```

---


