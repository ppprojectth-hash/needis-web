export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=NeedisDb;Username=needis_admin;Password=needis_password;SSL Mode=Disable"

dotnet ef database update

export ConnectionStrings__DefaultConnection="Host=119.59.102.62;Port=5432;Database=NeedisDb;Username=needis_admin;Password=รหัสจริง;SSL Mode=Prefer;Trust Server Certificate=true"


ถ้าเคย export เป็นตัวจริงไว้ ต้องล้างก่อน

เช็กก่อน:

echo $ConnectionStrings__DefaultConnection


ถ้ามันขึ้น Host=119.59.102.62 แปลว่าตอนนี้ยังชี้ตัวจริงอยู่ ให้ล้าง:

unset ConnectionStrings__DefaultConnection



git status
git add .
git commit -m "Prepare production deployment"
git push origin main

git status
git add .
git commit -m "Fix admin UI issues and media interactions"
git push origin main