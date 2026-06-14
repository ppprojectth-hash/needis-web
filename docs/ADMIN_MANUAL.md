# คู่มือการใช้งานระบบหลังบ้าน (Admin Manual)
## Needis.Web — Admin Back Office

**เวอร์ชัน:** 1.0  
**อัปเดตล่าสุด:** มิถุนายน 2569  
**ระดับ:** สำหรับผู้ดูแลระบบ

---

## สารบัญ

1. [การเข้าสู่ระบบ Admin](#1-การเข้าสู่ระบบ-admin)
2. [หน้า Dashboard](#2-หน้า-dashboard)
3. [จัดการ Site Setting / โลโก้](#3-จัดการ-site-setting--โลโก้)
4. [จัดการ Home Banner](#4-จัดการ-home-banner)
5. [จัดการหมวดหมู่สินค้า (Categories)](#5-จัดการหมวดหมู่สินค้า-categories)
6. [จัดการสินค้า (Products)](#6-จัดการสินค้า-products)
7. [จัดการสเปคสินค้า (Product Specifications)](#7-จัดการสเปคสินค้า-product-specifications)
8. [จัดการบริการ (Services)](#8-จัดการบริการ-services)
9. [จัดการกิจกรรม/ข่าวสาร (Activities)](#9-จัดการกิจกรรมข่าวสาร-activities)
10. [จัดการข้อความติดต่อ (Contact Messages)](#10-จัดการข้อความติดต่อ-contact-messages)
11. [จัดการคำขอใบเสนอราคา (Quotations)](#11-จัดการคำขอใบเสนอราคา-quotations)
12. [จัดการ Media Library](#12-จัดการ-media-library)
13. [จัดการ SEO](#13-จัดการ-seo)
14. [จัดการผู้ใช้งาน Admin (Admin Users)](#14-จัดการผู้ใช้งาน-admin-admin-users)
15. [จัดการ Roles / Permissions](#15-จัดการ-roles--permissions)
16. [สถิติการใช้งาน (Usage Statistics)](#16-สถิติการใช้งาน-usage-statistics)
17. [Email Logs](#17-email-logs)
18. [Audit Logs](#18-audit-logs)
19. [คำแนะนำการอัปโหลดไฟล์](#19-คำแนะนำการอัปโหลดไฟล์)
20. [ความหมายของสถานะ (Status)](#20-ความหมายของสถานะ-status)
21. [Best Practices](#21-best-practices)

---

## 1. การเข้าสู่ระบบ Admin

เส้นทาง: `/Admin/Account/Login`

### ขั้นตอน

1. เปิด Browser แล้วไปที่ `/Admin/Account/Login`
2. กรอก **Username** (ชื่อผู้ใช้)
3. กรอก **Password** (รหัสผ่าน)
4. คลิกปุ่ม **Login / เข้าสู่ระบบ**

### ข้อมูลเข้าสู่ระบบ (Default — สำหรับทดสอบเท่านั้น)

| ฟิลด์ | ค่า |
|---|---|
| Username | admin |
| Password | Admin@123456 |

> **คำเตือน:** กรุณาเปลี่ยนรหัสผ่านทันทีหลังจากเข้าสู่ระบบครั้งแรก

### กรณีลืมรหัสผ่าน

ติดต่อผู้ดูแลระบบ (Developer) เพื่อรีเซ็ตรหัสผ่านในฐานข้อมูล

---

## 2. หน้า Dashboard

เส้นทาง: `/Admin/Dashboard`

หน้า Dashboard แสดงภาพรวมระบบ ประกอบด้วย:

- **จำนวนสินค้า** — สินค้าที่ Active / ทั้งหมด
- **จำนวนหมวดหมู่** — หมวดหมู่ที่ Active
- **คำขอใบเสนอราคาใหม่** — รายการที่ยังไม่ได้ดำเนินการ
- **ข้อความติดต่อใหม่** — ข้อความที่ยังไม่ได้อ่าน
- **กราฟสถิติ** — การเข้าชมเว็บไซต์

### เมนูด้านซ้าย (Sidebar)

เมนูหลักแบ่งเป็นกลุ่ม:

```
Dashboard
├── Content Management
│   ├── Banners
│   ├── Categories
│   ├── Products
│   ├── Services
│   └── Activities
├── Customer
│   ├── Quotations
│   └── Contact Messages
├── Media
│   └── Media Library
├── SEO
│   └── SEO Settings
├── System
│   ├── Site Settings
│   ├── Admin Users
│   ├── Roles & Permissions
│   ├── Usage Statistics
│   ├── Email Logs
│   └── Audit Logs
└── Account
    ├── Profile
    └── Logout
```

---

## 3. จัดการ Site Setting / โลโก้

เส้นทาง: `/Admin/SiteSetting`

### ข้อมูลที่จัดการได้

**ข้อมูลบริษัท**
- ชื่อบริษัท (ภาษาไทย / อังกฤษ)
- โลโก้บริษัท
- Favicon (ไอคอนที่แสดงบน Browser Tab)
- สีหลักของเว็บไซต์ (Main Color)

**ข้อมูลติดต่อ**
- อีเมลติดต่อ
- เบอร์โทรศัพท์
- ที่อยู่ (ภาษาไทย / อังกฤษ)

**Social Media**
- Facebook URL
- Line URL
- LinkedIn URL

### วิธีเปลี่ยนโลโก้

1. ไปที่ `/Admin/SiteSetting`
2. คลิกปุ่ม **Choose File** ในส่วน Logo
3. เลือกไฟล์รูปภาพ (PNG หรือ SVG แนะนำ)
4. คลิก **Save / บันทึก**

> **คำแนะนำโลโก้:** ขนาดแนะนำ 200×60 px, รูปแบบ PNG พื้นหลังโปร่งใส

---

## 4. จัดการ Home Banner

เส้นทาง: `/Admin/Banner`

Home Banner คือแบนเนอร์ที่แสดงบนหน้าแรกของเว็บไซต์ รองรับ 3 ประเภท

### 4.1 ประเภทของ Banner

| ประเภท | คำอธิบาย |
|---|---|
| **Image** | แบนเนอร์รูปภาพนิ่ง |
| **Video** | แบนเนอร์วิดีโอ (อัปโหลดไฟล์หรือ URL) |
| **YouTube** | แบนเนอร์วิดีโอ YouTube |

### 4.2 วิธีเพิ่ม Banner ใหม่

1. คลิก **Add New Banner**
2. กรอกข้อมูล:

**ส่วน Banner Content (เนื้อหา)**
- Title (EN) * — ชื่อหัวข้อภาษาอังกฤษ (จำเป็น)
- Title (TH) — ชื่อหัวข้อภาษาไทย
- Subtitle (EN/TH) — คำบรรยาย
- Button Text (EN/TH) — ข้อความบนปุ่ม
- Button URL — ลิงก์ที่ปุ่มจะพาไป (เช่น `/Product`)

**ส่วน Media (สื่อ)**

สำหรับ **Image Banner:**
- เลือก Media Type = **Image**
- อัปโหลดรูปภาพหลัก (แนะนำ 1920×700 px, JPG/PNG/WebP, ไม่เกิน 5 MB)
- อัปโหลด Mobile Image (ตัวเลือก — แสดงบนมือถือ)

สำหรับ **Video Banner:**
- เลือก Media Type = **Video**
- อัปโหลดไฟล์วิดีโอ (MP4, WebM, MOV, ไม่เกิน 50 MB)
- หรือกรอก URL วิดีโอตรง (Direct Link)
- อัปโหลด Poster Image (รูปที่แสดงขณะโหลดวิดีโอ)
- ตั้งค่า Autoplay / Muted / Loop / Show Controls

สำหรับ **YouTube Banner:**
- เลือก Media Type = **YouTube**
- วาง YouTube URL * (เช่น `https://www.youtube.com/watch?v=xxxxx`)

**ส่วน Appearance (รูปลักษณ์)**
- Overlay Style — ความมืดของพื้นหลัง (Dark / Light / Gradient / None)
- Text Position — ตำแหน่งข้อความ (Left / Center / Right)
- Slide Duration — ระยะเวลาที่แสดง (3-20 วินาที)

**ส่วน Schedule & Display (กำหนดการแสดง)**
- Start Date — วันเริ่มแสดง (ว่างหมายถึงแสดงเสมอ)
- End Date — วันสิ้นสุดแสดง
- Display Order — ลำดับการแสดง (น้อย = แสดงก่อน)
- Active — เปิด/ปิดการแสดง

3. คลิก **Create Banner**

### 4.3 วิธีแก้ไข Banner

1. ในหน้ารายการ Banner คลิกปุ่ม ✏️ (Edit)
2. แก้ไขข้อมูลที่ต้องการ
3. หากต้องการเปลี่ยนรูปภาพ ให้อัปโหลดใหม่ (ไม่ต้องทำอะไรถ้าต้องการใช้รูปเดิม)
4. คลิก **Save Changes**

### 4.4 วิธีลบ Banner

1. ในหน้ารายการ Banner คลิกปุ่ม 🗑️ (Delete)
2. ยืนยันการลบ

> **หมายเหตุ:** Banner แสดงบนหน้าแรกเฉพาะเมื่อ: IsActive = เปิด และวันที่ปัจจุบันอยู่ในช่วง Start-End Date (ถ้ากำหนด)

---

## 5. จัดการหมวดหมู่สินค้า (Categories)

เส้นทาง: `/Admin/Category`

### วิธีเพิ่มหมวดหมู่

1. คลิก **Add New Category**
2. กรอก:
   - Name (EN) * — ชื่อภาษาอังกฤษ
   - Name (TH) — ชื่อภาษาไทย
   - Slug * — URL-friendly name (ตัวอักษรพิมพ์เล็ก, ใช้ `-` แทนเว้นวรรค เช่น `measuring-instruments`)
   - Short Description (EN/TH) — คำอธิบายสั้น
   - Image — รูปภาพหมวดหมู่ (แนะนำ 600×400 px)
   - Display Order — ลำดับการแสดง
   - Active — เปิด/ปิด
3. คลิก **Save**

> **Slug:** ต้องไม่ซ้ำกัน และเป็นตัวอักษรภาษาอังกฤษ, ตัวเลข, และ `-` เท่านั้น

---

## 6. จัดการสินค้า (Products)

เส้นทาง: `/Admin/Product`

### วิธีเพิ่มสินค้า

1. คลิก **Add New Product**
2. กรอกข้อมูล:

**ข้อมูลพื้นฐาน**
- Name (EN) * — ชื่อสินค้าภาษาอังกฤษ
- Name (TH) — ชื่อสินค้าภาษาไทย
- Slug * — URL-friendly name
- Category * — หมวดหมู่สินค้า
- Brand — แบรนด์
- Model — รุ่น
- SKU — รหัสสินค้า

**คำอธิบาย**
- Short Description (EN/TH) — คำอธิบายสั้น (แสดงบนการ์ดสินค้า)
- Full Description (EN/TH) — คำอธิบายเต็ม
- Specification (EN/TH) — สเปคทางเทคนิค (แบบ Text หรือใช้ Specification Manager)

**ราคาและการแสดง**
- Price — ราคา
- Is Price Visible — เปิด/ปิดการแสดงราคา
- Display Order — ลำดับการแสดง
- Is Featured — แสดงบนหน้าแรก (สินค้าแนะนำ)
- Active — เปิด/ปิดสินค้า

**รูปภาพและไฟล์**
- Main Image — รูปภาพหลัก (แนะนำ 800×800 px)
- Additional Images — รูปภาพเพิ่มเติม
- Brochure File — ไฟล์โบรชัวร์ PDF

3. คลิก **Save Product**

### การจัดการรูปภาพสินค้า

- รองรับรูปภาพหลายรูป
- คลิก **Add Image** เพื่อเพิ่มรูปเพิ่มเติม
- ลากเพื่อเรียงลำดับรูป
- คลิก ❌ เพื่อลบรูปภาพ

### การเปิด/ปิดสินค้า (Activate/Deactivate)

1. ในหน้ารายการสินค้า คลิกปุ่ม Toggle หรือปุ่ม Active/Inactive
2. ระบบจะอัปเดตสถานะทันที

---

## 7. จัดการสเปคสินค้า (Product Specifications)

เส้นทาง: `/Admin/Product/{id}/Specifications`

สเปคแบบโครงสร้างช่วยให้แสดงตารางสเปคที่อ่านง่าย

### วิธีเพิ่มสเปค

1. ไปที่หน้าแก้ไขสินค้า
2. คลิกแท็บ **Specifications**
3. คลิก **Add Spec Row**
4. กรอก:
   - Spec Group — กลุ่มสเปค (เช่น "ทั่วไป", "ไฟฟ้า", "Physical")
   - Spec Name (EN/TH) — ชื่อรายการสเปค
   - Spec Value (EN/TH) — ค่าสเปค
   - Unit — หน่วย (เช่น V, A, mm, kg)
   - Is Highlight — เน้นแสดงเป็นไฮไลท์
   - Display Order — ลำดับ
   - Active — เปิด/ปิด
5. คลิก **Save**

### ตัวอย่างสเปค

| Group | Name | Value | Unit |
|---|---|---|---|
| General | Measurement Range | 0–100 | °C |
| General | Accuracy | ±0.1 | °C |
| Electrical | Supply Voltage | 220 | V AC |
| Physical | Weight | 1.2 | kg |

---

## 8. จัดการบริการ (Services)

เส้นทาง: `/Admin/Service`

### วิธีเพิ่มบริการ

1. คลิก **Add New Service**
2. กรอก:
   - Service Name (EN/TH) *
   - Slug *
   - Short Description (EN/TH)
   - Full Description (EN/TH)
   - Icon — ชื่อ Bootstrap Icon (เช่น `bi-tools`)
   - Image — รูปภาพบริการ
   - Display Order
   - Is Featured — แสดงบนหน้าแรก
   - Active
3. คลิก **Save**

---

## 9. จัดการกิจกรรม/ข่าวสาร (Activities)

เส้นทาง: `/Admin/Activity`

### วิธีเพิ่มกิจกรรม

1. คลิก **Add New Activity**
2. กรอก:
   - Title (EN/TH) *
   - Slug *
   - Short Description (EN/TH)
   - Full Content (EN/TH) — เนื้อหาเต็ม
   - Cover Image — รูปภาพปก
   - Activity Date — วันที่จัดกิจกรรม
   - Is Published — เผยแพร่หรือไม่
   - Active
3. คลิก **Save**

---

## 10. จัดการข้อความติดต่อ (Contact Messages)

เส้นทาง: `/Admin/Contact`

### การดูข้อความ

1. คลิกเมนู Contact Messages
2. รายการจะแสดงข้อความใหม่ที่ลูกค้าส่งมา
3. คลิกที่ข้อความเพื่อดูรายละเอียด

### สถานะข้อความ

| สถานะ | ความหมาย |
|---|---|
| New | ข้อความใหม่ ยังไม่ได้อ่าน |
| Read | อ่านแล้ว |
| Replied | ตอบกลับแล้ว |
| Closed | ปิดแล้ว |

---

## 11. จัดการคำขอใบเสนอราคา (Quotations)

เส้นทาง: `/Admin/Quotation`

### การดูคำขอ

1. คลิกเมนู Quotations
2. รายการแสดงคำขอทั้งหมด พร้อมชื่อ อีเมล วันที่ส่ง และสถานะ
3. คลิกที่รายการเพื่อดูรายละเอียดสินค้าที่ขอ

### สถานะคำขอ

| สถานะ | ความหมาย |
|---|---|
| New | คำขอใหม่ |
| In Progress | กำลังดำเนินการ |
| Quoted | ส่งใบเสนอราคาแล้ว |
| Completed | เสร็จสิ้น |
| Cancelled | ยกเลิก |

### วิธีอัปเดตสถานะ

1. เปิดรายการคำขอ
2. เลือกสถานะใหม่จาก Dropdown
3. เพิ่มหมายเหตุ (ถ้าต้องการ)
4. คลิก **Update**

---

## 12. จัดการ Media Library

เส้นทาง: `/Admin/Media`

### คำอธิบาย

Media Library เก็บไฟล์ทั้งหมดที่อัปโหลดเข้าระบบ เช่น รูปภาพ PDF วิดีโอ

### การใช้งาน

1. คลิกเมนู Media Library
2. ดูรายการไฟล์ที่อัปโหลดทั้งหมด
3. คลิกที่ไฟล์เพื่อดู URL หรือคัดลอก Path
4. ลบไฟล์ที่ไม่ใช้งาน

> **คำเตือน:** อย่าลบไฟล์ที่ยังใช้งานอยู่ในสินค้า บริการ หรือ Banner

---

## 13. จัดการ SEO

เส้นทาง: `/Admin/Seo`

### ข้อมูล SEO ที่จัดการได้

- Meta Title — ชื่อหน้าที่แสดงใน Google
- Meta Description — คำอธิบายหน้า
- Meta Keywords — คำค้นหา
- Open Graph Image — รูปที่แสดงเมื่อแชร์บน Social Media
- Google Analytics ID — รหัส GA4 (ถ้ามี)
- Google Site Verification — รหัสยืนยัน Google Search Console

---

## 14. จัดการผู้ใช้งาน Admin (Admin Users)

เส้นทาง: `/Admin/AdminUser`

### วิธีเพิ่มผู้ใช้งานใหม่

1. คลิก **Add New User**
2. กรอก:
   - Username * — ชื่อผู้ใช้
   - Display Name — ชื่อที่แสดง
   - Email *
   - Password * — รหัสผ่าน (ขั้นต่ำ 8 ตัว, ผสมตัวอักษรพิมพ์ใหญ่-เล็ก, ตัวเลข, อักขระพิเศษ)
   - Role — บทบาท (SuperAdmin, Admin, Editor, Viewer)
   - Active
3. คลิก **Save**

> **ข้อสำคัญ:** รหัสผ่านถูก Hash ก่อนบันทึก ไม่มีการเก็บรหัสผ่านแบบ Plain Text

### วิธีเปลี่ยนรหัสผ่าน

1. คลิก Edit ที่ผู้ใช้งานที่ต้องการ
2. กรอก New Password
3. กรอก Confirm Password
4. คลิก **Update**

---

## 15. จัดการ Roles / Permissions

เส้นทาง: `/Admin/Role`

### บทบาทมาตรฐาน

| Role | คำอธิบาย |
|---|---|
| SuperAdmin | เข้าถึงได้ทุกฟังก์ชัน |
| Admin | จัดการเนื้อหาและผู้ใช้งาน |
| Editor | จัดการเนื้อหาเท่านั้น |
| Viewer | ดูได้อย่างเดียว ไม่สามารถแก้ไข |

### Permissions ที่จัดการได้

แต่ละ Role สามารถกำหนด Permission ได้ในรูปแบบ:

```
[Module].[Action]
ตัวอย่าง:
  Product.View
  Product.Create
  Product.Edit
  Product.Delete
  Banner.View
  Banner.Create
  ...
```

---

## 16. สถิติการใช้งาน (Usage Statistics)

เส้นทาง: `/Admin/UsageStat`

### ข้อมูลที่แสดง

- จำนวนการเข้าชมแต่ละหน้า
- ภาษาที่ผู้ใช้เลือก (TH/EN)
- IP Address ผู้เข้าชม
- อุปกรณ์ที่ใช้ (User Agent)
- วันและเวลาที่เข้าชม
- หน้าที่เข้าชมมากที่สุด
- กราฟแสดงแนวโน้มการเข้าชม

### การกรองข้อมูล

- กรองตามช่วงวันที่
- กรองตามหน้า
- กรองตามภาษา

---

## 17. Email Logs

เส้นทาง: `/Admin/EmailLog`

แสดงประวัติอีเมลที่ระบบส่งออก เช่น:
- อีเมลยืนยันคำขอใบเสนอราคา
- อีเมลแจ้งเตือนข้อความติดต่อ

| คอลัมน์ | ความหมาย |
|---|---|
| To | ผู้รับอีเมล |
| Subject | หัวข้อ |
| Status | Sent / Failed |
| Sent At | วันเวลาที่ส่ง |

---

## 18. Audit Logs

เส้นทาง: `/Admin/AuditLog`

บันทึกกิจกรรมทั้งหมดของ Admin ใช้สำหรับตรวจสอบย้อนหลัง

| คอลัมน์ | ความหมาย |
|---|---|
| User | ผู้ดำเนินการ |
| Action | การกระทำ (Create, Edit, Delete) |
| Entity | ข้อมูลที่ถูกดำเนินการ |
| Details | รายละเอียดการเปลี่ยนแปลง |
| Timestamp | วันเวลา |

---

## 19. คำแนะนำการอัปโหลดไฟล์

### รูปภาพ

| ประเภทรูป | ขนาดแนะนำ | รูปแบบ | ขนาดไฟล์สูงสุด |
|---|---|---|---|
| โลโก้บริษัท | 200×60 px | PNG (พื้นหลังโปร่งใส) | 1 MB |
| Favicon | 32×32 px | ICO หรือ PNG | 100 KB |
| Banner หน้าแรก | 1920×700 px | JPG, PNG, WebP | 5 MB |
| Banner มือถือ | 768×500 px | JPG, PNG, WebP | 3 MB |
| รูปภาพสินค้า | 800×800 px | JPG, PNG, WebP | 5 MB |
| รูปภาพหมวดหมู่ | 600×400 px | JPG, PNG, WebP | 2 MB |
| รูปภาพบริการ | 600×400 px | JPG, PNG, WebP | 2 MB |
| รูปภาพกิจกรรม | 1200×630 px | JPG, PNG, WebP | 3 MB |

### วิดีโอ

| ประเภท | รูปแบบ | ขนาดไฟล์สูงสุด |
|---|---|---|
| Banner Video | MP4, WebM, MOV | 50 MB |
| Mobile Banner Video | MP4, WebM | 30 MB |

### ไฟล์เอกสาร

| ประเภท | รูปแบบ | ขนาดไฟล์สูงสุด |
|---|---|---|
| Brochure | PDF | 20 MB |

> **คำเนะนำ:** บีบอัดรูปภาพก่อนอัปโหลดโดยใช้เครื่องมือเช่น [TinyPNG](https://tinypng.com) หรือ [Squoosh](https://squoosh.app)

---

## 20. ความหมายของสถานะ (Status)

### สินค้า / หมวดหมู่ / บริการ / Banner

| สถานะ | สัญลักษณ์ | ความหมาย |
|---|---|---|
| Active | 🟢 | แสดงบนเว็บไซต์ |
| Inactive | ⚫ | ซ่อนจากเว็บไซต์ |
| Featured | ⭐ | แสดงบนหน้าแรกในส่วนสินค้าแนะนำ |
| Deleted | 🔴 | ถูกลบ (Soft Delete, ไม่แสดงในระบบ) |

### Banner

| เงื่อนไข | ผล |
|---|---|
| Active = ✅ และไม่ได้กำหนดวันที่ | แสดงตลอด |
| Active = ✅ และกำหนดวันที่ | แสดงเฉพาะในช่วงวันที่กำหนด |
| Active = ❌ | ไม่แสดง ไม่ว่าวันที่จะเป็นอะไร |

---

## 21. Best Practices

### การจัดการเนื้อหา

1. **เสมอกรอกทั้ง 2 ภาษา** — กรอกทั้ง TH และ EN เพื่อให้ผู้ใช้ทั้งสองภาษาได้รับประสบการณ์ที่ดี
2. **Slug ต้องไม่ซ้ำ** — ตรวจสอบก่อนบันทึกทุกครั้ง
3. **Optimize รูปภาพก่อนอัปโหลด** — ช่วยให้เว็บโหลดเร็วขึ้น
4. **ตั้ง Display Order อย่างสม่ำเสมอ** — ใช้ตัวเลขห่างกัน เช่น 10, 20, 30 เพื่อให้แทรกได้ง่าย

### ความปลอดภัย

1. **เปลี่ยนรหัสผ่าน Default ทันที** หลังจากเข้าสู่ระบบครั้งแรก
2. **ใช้รหัสผ่านที่ซับซ้อน** — ขั้นต่ำ 12 ตัว ผสมตัวอักษรพิมพ์ใหญ่-เล็ก ตัวเลข และอักขระพิเศษ
3. **Logout ทุกครั้ง** หลังใช้งานบนคอมพิวเตอร์สาธารณะ
4. **ไม่แชร์รหัสผ่าน** — ทุกคนควรมี Account เป็นของตัวเอง

### การจัดการข้อมูล

1. **ตรวจสอบ Active Status** ก่อนบันทึก เพื่อหลีกเลี่ยงการแสดงข้อมูลที่ยังไม่พร้อม
2. **ใช้ Preview** เพื่อตรวจสอบหน้าเว็บก่อน Active
3. **บันทึกงานบ่อยๆ** — ระบบไม่มี Auto-save
4. **ตรวจสอบ Audit Log** เมื่อพบข้อมูลที่ผิดปกติ

---

*เอกสารนี้จัดทำสำหรับ Needis.Web v1.0 — มิถุนายน 2569*
