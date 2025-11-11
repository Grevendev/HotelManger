# 🏨 Hotel Booking Console App

A **scalable console-based hotel management system** built with C# (.NET 9).  
Designed for **receptionists** and **administrators** to manage hotel room bookings, user access, and hotel operations with **file-based storage** and a clear separation of services for future scalability.

---

## 🌟 Features (Implemented)

### 👤 User Management
| Feature | Description |
|---------|-------------|
| Login system | Supports roles: **Admin** and **Receptionist** |
| User creation | Admin can create new users with defined roles |
| Temporary permissions | Admin can grant temporary permissions to receptionists for specific actions |
| Input validation | Secure validation for usernames, roles, and other user input |

---

### 🛏 Room Management
| Feature | Description |
|---------|-------------|
| View rooms | Show all, available, and unavailable rooms |
| Detailed room info | Room type, capacity, price per night, bed type/count, description, amenities, current guest, check-in date |
| Booking | Book a room for a guest, calculates **total price** automatically |
| Checkout | Checkout guest, calculates **total cost** and updates room status |
| Unavailability | Mark room as temporarily unavailable |
| Add/Remove rooms | Admin can add or remove rooms (removal only if not occupied) |
| Update rooms | Admin or authorized Receptionist can update **type, capacity, price, bed info, description, amenities** |
| Filtering & Sorting | Filter rooms by type, status, price, capacity, amenities; sort by price, capacity, type |

**Room Status Color Coding:**  
- 🟢 **Green** = Available  
- 🔴 **Red** = Occupied  
- 🟡 **Yellow** = Unavailable  

---

### 📜 History Logging
| Feature | Description |
|---------|-------------|
| Logging | All bookings, checkouts, room availability changes, and system changes |
| View history | Show full hotel activity logs |
| Filtering | Filter history by **keyword** and **date range** |
| Format | Each log includes **timestamp** and **event type** (SYSTEM, BOOKED, CHECKOUT, LOGIN, LOGOUT, etc.) |

---

### 🎨 UI and Input Enhancements
- Color-coded room status for easy identification  
- Improved **menu layout** with headers and separators  
- Comprehensive **input validation** for:
  - Room numbers, capacities, prices  
  - Dates (check-in/check-out)  
  - Enum values (RoomType, RoomStatus, UserRole)  
- Centralized validation logic using `InputHelper.cs`

---

### 💾 Persistence
- File-based storage:
  - Rooms → `rooms.json`  
  - Users → `users.txt`  
  - History → `history.txt`  
- Automatic saving after any change

---

## 🔮 Future Improvements
- Database integration (e.g., **MySQL**) for scalable persistent storage  
- WPF or another GUI for user-friendly interface  
- Enhanced reporting and analytics  
- Advanced permissions system with more granular control

---

### ✅ Summary
This console app provides a **realistic hotel management simulation** with admin/receptionist roles, full room lifecycle handling, detailed logging, filtering, sorting, and robust input validation. Ideal for learning or as a starting point for more advanced systems.
