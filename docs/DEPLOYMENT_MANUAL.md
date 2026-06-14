# คู่มือการ Deploy (Deployment Manual)
## Needis.Web — สำหรับนักพัฒนา / ผู้ดูแลระบบ

**เวอร์ชัน:** 1.0  
**อัปเดตล่าสุด:** มิถุนายน 2569  
**ระดับ:** Technical

---

## สารบัญ

1. [ภาพรวมสภาพแวดล้อม](#1-ภาพรวมสภาพแวดล้อม)
2. [Local Development Setup](#2-local-development-setup)
3. [Docker PostgreSQL (Local)](#3-docker-postgresql-local)
4. [Supabase PostgreSQL (Test Database)](#4-supabase-postgresql-test-database)
5. [Render Deployment (Web Service)](#5-render-deployment-web-service)
6. [Environment Variables ที่จำเป็น](#6-environment-variables-ที่จำเป็น)
7. [Connection String Examples](#7-connection-string-examples)
8. [การ Run Migrations](#8-การ-run-migrations)
9. [การ Seed Admin User](#9-การ-seed-admin-user)
10. [การทดสอบหลัง Deploy](#10-การทดสอบหลัง-deploy)
11. [ข้อจำกัดของ Free Hosting](#11-ข้อจำกัดของ-free-hosting)
12. [คำเตือนเรื่องไฟล์ที่อัปโหลด](#12-คำเตือนเรื่องไฟล์ที่อัปโหลด)
13. [แนะนำสำหรับอนาคต: Supabase Storage](#13-แนะนำสำหรับอนาคต-supabase-storage)

---

## 1. ภาพรวมสภาพแวดล้อม

| สภาพแวดล้อม | Web App | Database | Storage |
|---|---|---|---|
| **Local Dev** | localhost:5289 | Docker PostgreSQL | wwwroot/uploads/ |
| **Test/Staging** | Render Free | Supabase Free | wwwroot/uploads/ (ephemeral) |
| **Production** | Render Paid / VPS | Supabase Pro / RDS | Supabase Storage / S3 |

---

## 2. Local Development Setup

### ความต้องการระบบ

| Software | เวอร์ชันขั้นต่ำ | ดาวน์โหลด |
|---|---|---|
| .NET SDK | 10.0+ | [dotnet.microsoft.com](https://dotnet.microsoft.com) |
| Docker Desktop | 24.0+ | [docker.com](https://www.docker.com) |
| Node.js (optional) | 18+ | [nodejs.org](https://nodejs.org) |
| dotnet-ef (CLI tool) | 10.0+ | ดูด้านล่าง |

### ขั้นตอน Local Setup

**1. Clone หรือเปิด Project**
```bash
cd /Users/pampam/Needis
```

**2. ติดตั้ง EF Core CLI Tool**
```bash
dotnet tool install -g dotnet-ef
```

**3. เริ่ม Docker Database**
```bash
docker-compose up -d
```

ตรวจสอบว่า Container ทำงานปกติ:
```bash
docker-compose ps
```

ควรเห็น:
```
NAME         STATUS
postgres     Up
pgadmin      Up
```

**4. Run Migrations**
```bash
dotnet ef database update --project Needis.Web
```

**5. Run Application**
```bash
dotnet run --project Needis.Web
```

หรือใช้ Watch Mode (auto-reload เมื่อแก้ไขโค้ด):
```bash
dotnet watch run --project Needis.Web
```

**6. เปิด Browser**
- เว็บไซต์: `http://localhost:5289`
- Admin: `http://localhost:5289/Admin/Account/Login`
- pgAdmin: `http://localhost:5050`

---

## 3. Docker PostgreSQL (Local)

### ไฟล์ docker-compose.yml

```yaml
services:
  postgres:
    image: postgres:16
    environment:
      POSTGRES_DB: NeedisDb
      POSTGRES_USER: needis_admin
      POSTGRES_PASSWORD: needis_password
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  pgadmin:
    image: dpage/pgadmin4
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@example.com
      PGADMIN_DEFAULT_PASSWORD: admin123
    ports:
      - "5050:80"
    depends_on:
      - postgres

volumes:
  postgres_data:
```

### pgAdmin Connection Settings

เปิด `http://localhost:5050` แล้วล็อกอิน:
- Email: `admin@example.com`
- Password: `admin123`

เพิ่ม Server Connection:
- Host: `postgres` *(ใช้ชื่อ service ของ docker ไม่ใช่ localhost)*
- Port: `5432`
- Database: `NeedisDb`
- Username: `needis_admin`
- Password: `needis_password`

### คำสั่ง Docker ที่ใช้บ่อย

```bash
# เริ่ม container
docker-compose up -d

# หยุด container
docker-compose down

# ดู logs
docker-compose logs postgres

# ลบ volume (ล้างข้อมูลทั้งหมด)
docker-compose down -v

# เข้า psql โดยตรง
docker exec -it postgres psql -U needis_admin -d NeedisDb
```

---

## 4. Supabase PostgreSQL (Test Database)

Supabase ให้บริการ PostgreSQL Free Tier สำหรับ Development/Staging

### ขั้นตอนสร้าง Supabase Database

1. ไปที่ [supabase.com](https://supabase.com) และสร้างบัญชี (ฟรี)
2. สร้าง Project ใหม่
   - Project Name: `needis-test`
   - Database Password: ตั้งรหัสผ่านที่ซับซ้อน (บันทึกไว้!)
   - Region: เลือก `Southeast Asia (Singapore)` เพื่อ Latency ต่ำ
3. รอ Project สร้างเสร็จ (~2 นาที)
4. ไปที่ **Settings → Database**
5. คัดลอก **Connection String** ในรูปแบบ .NET (ADO.NET)

### Connection String จาก Supabase

ไปที่ Project Settings → Database → Connection String → .NET:

```
Host=db.xxxxxxxxxxxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true
```

> **หมายเหตุ:** แทนที่ `xxxxxxxxxxxx` ด้วย Project ID ของคุณ และ `YOUR_PASSWORD` ด้วยรหัสผ่านที่ตั้งไว้

### วิธีใช้ Supabase Connection String

เพิ่มใน `appsettings.json` หรือตั้งเป็น Environment Variable:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.xxxxxxxxxxxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
  }
}
```

---

## 5. Render Deployment (Web Service)

[Render.com](https://render.com) ให้บริการ Web Service Free Tier สำหรับ .NET

### ขั้นตอน Deploy บน Render

**1. เตรียม Repository**

- Push โค้ดขึ้น GitHub Repository
- ตรวจสอบว่า `.gitignore` ไม่ include `appsettings.Development.json` หรือไฟล์ที่มี Secret

**2. สร้าง Web Service บน Render**

1. ล็อกอิน [render.com](https://render.com)
2. คลิก **New → Web Service**
3. เชื่อมต่อ GitHub Repository
4. กำหนดค่า:
   - **Name:** `needis-web`
   - **Region:** `Singapore`
   - **Branch:** `main`
   - **Runtime:** `Docker` หรือ `.NET`
   - **Build Command:** `dotnet publish Needis.Web -c Release -o out`
   - **Start Command:** `dotnet out/Needis.Web.dll`

**3. ตั้งค่า Environment Variables**

ใน Render Dashboard → Environment:

```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Host=db.xxx.supabase.co;Port=5432;...
ASPNETCORE_URLS=http://+:10000
```

**4. Deploy**

กด **Create Web Service** แล้วรอ Build เสร็จ (~5 นาที)

### Render Free Tier ข้อจำกัด

- Web Service จะ "sleep" หลัง 15 นาทีที่ไม่มี Request
- เมื่อ Request แรกเข้ามาจะใช้เวลา ~30 วินาทีในการ "wake up"
- ไม่มี Persistent Storage (ไฟล์ที่อัปโหลดจะหายเมื่อ Redeploy)
- ใช้ได้ 750 ชั่วโมง/เดือน

---

## 6. Environment Variables ที่จำเป็น

| Variable | คำอธิบาย | ตัวอย่าง |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | Database Connection String | Host=localhost;Port=5432;... |
| `ASPNETCORE_ENVIRONMENT` | สภาพแวดล้อม | `Production` หรือ `Development` |
| `ASPNETCORE_URLS` | URL ที่ App ฟัง | `http://+:5289` (local) / `http://+:10000` (Render) |
| `AppSettings__AdminEmail` | อีเมล Admin เริ่มต้น | `admin@example.com` |
| `EmailSettings__SmtpHost` | SMTP Server | `smtp.gmail.com` |
| `EmailSettings__SmtpPort` | SMTP Port | `587` |
| `EmailSettings__SmtpUser` | อีเมลผู้ส่ง | `noreply@yourcompany.com` |
| `EmailSettings__SmtpPass` | รหัสผ่าน SMTP | `your-app-password` |
| `EmailSettings__FromName` | ชื่อผู้ส่ง | `Needis Support` |

### วิธีตั้ง Environment Variables

**บน Local (Development):**

แก้ไขไฟล์ `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=NeedisDb;Username=needis_admin;Password=needis_password"
  }
}
```

**บน Render:**

ไปที่ Dashboard → Environment → Add Environment Variable

**บน Server (Linux):**

```bash
export ConnectionStrings__DefaultConnection="Host=..."
export ASPNETCORE_ENVIRONMENT="Production"
```

---

## 7. Connection String Examples

### Local Docker

```
Host=localhost;Port=5432;Database=NeedisDb;Username=needis_admin;Password=needis_password
```

### Supabase (Free/Test)

```
Host=db.xxxxxxxxxxxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_DB_PASSWORD;SSL Mode=Require;Trust Server Certificate=true
```

### AWS RDS PostgreSQL

```
Host=yourdb.cluster-xxxxx.ap-southeast-1.rds.amazonaws.com;Port=5432;Database=NeedisDb;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require
```

---

## 8. การ Run Migrations

### คำสั่ง

```bash
# ตรวจสอบ migrations ที่มีอยู่
dotnet ef migrations list --project Needis.Web

# สร้าง migration ใหม่
dotnet ef migrations add MigrationName --project Needis.Web

# Apply migrations ไปยัง database
dotnet ef database update --project Needis.Web

# Apply ไปยัง database ที่ระบุโดยตรง (ใช้ connection string อื่น)
dotnet ef database update --project Needis.Web \
  -- --connectionstring "Host=db.xxx.supabase.co;..."

# Rollback migration ล่าสุด
dotnet ef database update PreviousMigrationName --project Needis.Web
```

### Run Migrations สำหรับ Production (Supabase)

```bash
# ตั้ง connection string เป็น Supabase ก่อน
export ConnectionStrings__DefaultConnection="Host=db.xxx.supabase.co;..."

# Run migrations
dotnet ef database update --project Needis.Web
```

### Auto-Migration เมื่อ App Start

ระบบถูกกำหนดให้ Run migrations อัตโนมัติเมื่อ Application เริ่มทำงาน (ใน `Program.cs`) ดังนั้นเมื่อ Deploy บน Render ครั้งแรก migrations จะถูก Apply อัตโนมัติ

---

## 9. การ Seed Admin User

ระบบมี `AdminSeeder` ที่ทำงานอัตโนมัติเมื่อ App เริ่มทำงาน โดยจะสร้าง Admin User เริ่มต้นถ้ายังไม่มีในระบบ

### ข้อมูล Admin Default

| Field | Value |
|---|---|
| Username | `admin` |
| Password | `Admin@123456` |
| Role | `SuperAdmin` |

### การเปลี่ยนข้อมูล Admin Default

แก้ไขไฟล์ `Data/AdminSeeder.cs` หรือตั้งค่าผ่าน Environment Variables:

```bash
AppSettings__DefaultAdminUsername=admin
AppSettings__DefaultAdminPassword=YourSecurePassword@2025
```

> **สำคัญมาก:** เปลี่ยนรหัสผ่าน Default ทันทีหลัง Deploy ครั้งแรก

---

## 10. การทดสอบหลัง Deploy

### Checklist หลัง Deploy

**1. ตรวจสอบ App เริ่มทำงาน**
- [ ] เปิดเว็บไซต์ได้ที่ URL ที่กำหนด
- [ ] ไม่มี Error 500 หรือ 502

**2. ตรวจสอบ Database**
- [ ] หน้าแรกโหลดได้ปกติ (หมายความว่า DB เชื่อมต่อสำเร็จ)
- [ ] ล็อกอิน Admin ได้ที่ `/Admin/Account/Login`

**3. ตรวจสอบ Migrations**
- [ ] ล็อกอิน Admin แล้วดู Dashboard — ไม่มี Error

**4. ตรวจสอบฟังก์ชันหลัก**
- [ ] หน้าแรก (Home) แสดงผลได้
- [ ] หน้าสินค้า (Products) แสดงผลได้
- [ ] Admin สามารถสร้างสินค้าได้
- [ ] Admin สามารถอัปโหลดรูปได้

**5. ตรวจสอบ HTTPS**
- [ ] URL เป็น `https://` (Render ออก SSL ให้อัตโนมัติ)

---

## 11. ข้อจำกัดของ Free Hosting

### Render Free Tier

| ข้อจำกัด | รายละเอียด |
|---|---|
| Cold Start | App "นอนหลับ" หลัง 15 นาที ใช้เวลา ~30 วินาทีในการ Wake Up |
| Memory | 512 MB RAM |
| CPU | Shared CPU (ช้ากว่า Paid) |
| Bandwidth | 100 GB/เดือน |
| Storage | ไม่มี Persistent Storage — ไฟล์อัปโหลดหายเมื่อ Redeploy |
| Uptime | 750 ชั่วโมง/เดือน (เพียงพอสำหรับ 1 service) |

### Supabase Free Tier

| ข้อจำกัด | รายละเอียด |
|---|---|
| Database Size | 500 MB |
| Bandwidth | 5 GB/เดือน |
| Max Connections | 60 connections |
| Project Pause | Pause อัตโนมัติหลัง 1 สัปดาห์ที่ไม่ Active |

---

## 12. คำเตือนเรื่องไฟล์ที่อัปโหลด

### ปัญหาหลัก: Ephemeral Storage บน Render Free

บน Render Free Tier ระบบไฟล์เป็นแบบ **Ephemeral** หมายความว่า:

- ไฟล์ที่อัปโหลดผ่าน Admin (รูปภาพ, วิดีโอ, PDF) จะถูกเก็บใน `/wwwroot/uploads/`
- เมื่อ Render **Redeploy** หรือ **Restart** — ไฟล์เหล่านั้น **จะหายทั้งหมด**
- รูปภาพสินค้า Banner โลโก้ ที่อัปโหลดไว้จะแสดงเป็น Broken Image

### วิธีแก้ไขชั่วคราว (สำหรับ Test)

1. อย่า Redeploy บ่อยๆ ระหว่างทดสอบ
2. เก็บ Backup ไฟล์ไว้ใน Local
3. หลัง Redeploy ให้อัปโหลดไฟล์ใหม่ทุกครั้ง

### วิธีแก้ไขถาวร (Production)

ดูหัวข้อถัดไป: [Supabase Storage](#13-แนะนำสำหรับอนาคต-supabase-storage)

---

## 13. แนะนำสำหรับอนาคต: Supabase Storage

### ทำไมต้องใช้ Supabase Storage?

- เก็บไฟล์ถาวร ไม่หายเมื่อ Redeploy
- CDN ในตัว — ไฟล์โหลดเร็วขึ้น
- รองรับ Public/Private Buckets
- Free Tier: 1 GB Storage, 2 GB/เดือน Bandwidth

### วิธี Integrate Supabase Storage (สรุป)

1. เปิด Supabase Project → Storage → สร้าง Bucket `uploads`
2. ตั้ง Bucket เป็น Public
3. ติดตั้ง Supabase .NET Client:
   ```bash
   dotnet add package Supabase --project Needis.Web
   ```
4. สร้าง Service `IStorageService` ที่ Upload ไปที่ Supabase แทน Local
5. เปลี่ยน Controller ต่างๆ จาก SaveImageAsync (Local) → IStorageService.UploadAsync
6. ไฟล์จะได้ Public URL รูปแบบ:
   ```
   https://xxxxxxxxxxxx.supabase.co/storage/v1/object/public/uploads/banners/xxx.jpg
   ```

### Timeline สำหรับ Production

| Phase | Action | เวลาโดยประมาณ |
|---|---|---|
| ทดสอบ | Render Free + Supabase Free (ยอมรับ Ephemeral Storage) | ทันที |
| Pre-Production | Implement Supabase Storage | 3-5 วันทำการ |
| Production | Render Paid ($7/เดือน) + Supabase Pro ($25/เดือน) | เมื่อพร้อม |

---

## ข้อมูลอ้างอิงด่วน

### URLs สำหรับ Local Dev

| Service | URL |
|---|---|
| Website | `http://localhost:5289` |
| Website (HTTPS) | `https://localhost:7176` |
| Admin Login | `http://localhost:5289/Admin/Account/Login` |
| pgAdmin | `http://localhost:5050` |

### คำสั่งที่ใช้บ่อย

```bash
# เริ่ม Database
docker-compose up -d

# Run App
dotnet run --project Needis.Web

# Watch Mode
dotnet watch run --project Needis.Web

# Build
dotnet build Needis.Web

# Apply Migrations
dotnet ef database update --project Needis.Web

# New Migration
dotnet ef migrations add NameHere --project Needis.Web
```

---

*เอกสารนี้จัดทำสำหรับ Needis.Web v1.0 — มิถุนายน 2569*
