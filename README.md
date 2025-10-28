# Hotel Booking Console App

A scalable console-based hotel management system built with C# (.NET 9).  
Designed for receptionists and administrators to manage hotel room bookings and user access, with file-based storage and a clear separation of services for future scalability.

---

## **Features (Implemented)**

### **User Management**
- Login system with roles (`Admin` and `Receptionist`)  
- Admin can create new users with defined roles  

### **Room Management**
- View all rooms in the hotel  
- View available rooms  
- View unavailable rooms  
- Book a room for a guest  
- Checkout a guest from a room  
- Mark a room as temporarily unavailable  
- Admin can add new rooms  
- Admin can remove rooms (only if not occupied)  

### **History Logging**
- Logs all bookings, checkouts, and room availability changes  
- View full history of hotel activity  

### **Persistence**
- File-based storage for rooms and users (`rooms.json` and `users.txt`)  
- Automatic saving after any change  
- History stored in `history.txt`  

---

## **Getting Started**




