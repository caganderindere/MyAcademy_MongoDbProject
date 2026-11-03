# Travelio — Yol Haritası

Her faz şu başlıklarla anlatıldı:
- **Amaç:** Bu faz neden var?
- **Ne yapılacak:** Yazılacak parçalar
- **Ben:** Claude'un yapacakları
- **Sen:** Senin yazacakların (ben chat'te satır satır anlatacağım)
- **Öğreneceğin:** Fazın asıl dersi
- **Bitti sayılır:** Fazın tamamlandığını nasıl anlayacağımız

**Genel iş bölümü:**
- **Ben:** NuGet paketleri, dil (resx) dosyaları, HTML şablonlarını Razor'a taşımak (sadece tasarım/HTML kısmı), örnek veri (seed) içerikleri, senin kodunu kontrol etmek, hata ayıklamak.
- **Sen:** Bütün C# kodu (entity, DTO, mapping, servis, validator, controller), Program.cs ayarları, view'lardaki Razor kodu (`@foreach`, `@Model`, formlar), JavaScript (Chart.js vb.), commit ve push.

Her modülde aynı kalıbı tekrar edeceğiz (hocanın kalıbı):
`Entity → DTO'lar → Mapping → Validator → Service (I + sınıf) → Program.cs kaydı → Controller → View`

---

## Faz 1 — Altyapı

**Amaç:** Dil desteği ve giriş sistemi her sayfayı etkiler. Sonradan eklemek, her sayfayı tek tek elden geçirmek demek. O yüzden en başta kurulur.

**Ne yapılacak:**
- `appsettings.json`'a 8 koleksiyon adı
- `DatabaseSettings` / `IDatabaseSettings`'e bu adların property'leri
- `Program.cs`: Localization, Cookie Authentication, QuestPDF lisansı, middleware sırası
- `_ViewImports`'a çevirici (`L`), menüde `@L["..."]` kullanımı

**Ben:** ✅ Paketler (ClosedXML, QuestPDF, BCrypt) · ✅ TR/EN resx dosyaları · ✅ `SharedResource` · ✅ `LanguageController` · ✅ `_LanguageSwitcher` menüsü

**Sen:** appsettings, settings sınıfları, Program.cs, _ViewImports, menü çevirileri

**Öğreneceğin:** Options pattern (JSON → C# sınıfı), Dependency Injection, middleware sırası, Cookie auth mantığı, resx ile çeviri

**Bitti sayılır:** Sağ üstten dil değişince menü yazıları değişiyor.

---

## Faz 2 — Entity'ler ve örnek veri

**Amaç:** Bütün sayfalar bu modellerin üstüne kurulacak. Model yanlışsa her şey yanlış olur. MongoDB'nin "gömme mi, ayırma mı" kararını burada veriyoruz.

**Ne yapılacak:**
- `LocalizedText { Tr, En }` — çok dilli alanlar için gömülü tip
- `Tour` + içine gömülü: `TourDate { StartDate, Quota }`, `TourDay { Day, Title, Description, City, Transport?, Meals? }`, `Gallery` (liste), `Highlights` (liste)
- Ayrı koleksiyonlar: `Destination`, `Category`, `Reservation`, `User`, `Comment`, `Question`, `Favorite`
- Enum'lar: `ReservationStatus { Pending, Approved, Cancelled }`, `UserRole { User, Admin }`
- `DataSeeder`: Veritabanı boşsa örnek kategori, destinasyon, tur, admin kullanıcısı ekler

**Ben:** Seed verisinin içeriği (tur adları, açıklamalar, görsel linkleri — HTML şablonlardaki örneklerden), MongoDB kurulumunda yardım

**Sen:** Bütün entity sınıfları, `DataSeeder`'ın kodu, Program.cs'te çalıştırılması

**Öğreneceğin:** Embedded vs referenced veri, `[BsonId]`, `[BsonRepresentation]`, enum'ların Mongo'da saklanması, `InsertManyAsync`

**Bitti sayılır:** Uygulama açılınca MongoDB Compass'ta 8 koleksiyon ve örnek veriler görünüyor.

---

## Faz 3 — Kategori ve Destinasyon (Admin CRUD)

**Amaç:** En basit iki modül. Hocanın kalıbını (Entity → DTO → Mapping → Validator → Service → Controller → View) yeni modellerde tek başına uygulamayı burada pekiştiriyoruz. Tur bunlara bağlı olduğu için önce bunlar gelir.

**Ne yapılacak:**
- `CategoryDtos`, `DestinationDtos` (Create / Update / Result)
- Mapping, Validator, Service, Admin controller
- Admin Index / Create / Update view'ları (TR/EN ad alanlarıyla)

**Ben:** Admin panelin ortak tasarımı (`_AdminLayout` — admin şablonlarından), tablo/form HTML iskeleti

**Sen:** Bütün C# katmanları, view'lardaki Razor kodu

**Öğreneceğin:** Kalıbı ezberlemek değil, her katmanın neden var olduğunu oturtmak

**Bitti sayılır:** Admin panelden kategori ve destinasyon eklenip, güncellenip silinebiliyor.

---

## Faz 4 — Tur Yönetimi (Admin)

**Amaç:** Projenin kalbi. İç içe (nested) bir belgeyi formdan alıp MongoDB'ye kaydetmek, case'in en önemli MongoDB dersi.

**Ne yapılacak:**
- Tur listesi: ad, destinasyon, kategori, durum filtreleri; aktif/pasif butonu; sil
- Tur ekle/güncelle formu: TR/EN sekmeleri, tarih+kontenjan satırları, gün gün program satırları, galeri, öne çıkanlar (JavaScript ile "satır ekle/çıkar")
- `TourService`: filtreli listeleme (`Builders<Tour>.Filter`), aktif/pasif için `UpdateOneAsync` + `Set`
- FluentValidation: liste içindeki her satır için kural (`RuleForEach`)

**Ben:** `admin-tours` ve `admin-tour-create` şablonlarının Razor'a taşınması (HTML)

**Sen:** Servis, DTO'lar, validator, controller, formun Razor kodu, "satır ekle" JavaScript'i

**Öğreneceğin:** Filter builder ile dinamik sorgu, Update builder (`Set`), liste bağlama (model binding: `Dates[0].Quota`), `RuleForEach`

**Bitti sayılır:** İçinde 2 tarih ve 3 günlük program olan bir tur eklenip Compass'ta tek belge olarak görünüyor.

---

## Faz 5 — Kayıt, Giriş ve Ortak Tasarım

**Amaç:** Rezervasyon, yorum, favori için "kim?" sorusunun cevabı lazım. Ayrıca site tasarımını burada oturtuyoruz.

**Ne yapılacak:**
- `AccountController`: Register, Login, Logout, AccessDenied
- `UserService`: kayıtta BCrypt ile hash, girişte doğrulama
- Giriş başarılıysa `Claim`'ler (Id, Ad, E-posta, Rol) ile cookie oluşturma
- Admin area'ya `[Authorize(Roles = "Admin")]`
- Site `_Layout`'u (şablonlardaki menü + footer), giriş durumuna göre menü (Giriş Yap / Profilim / Çıkış)

**Ben:** `register` şablonunun ve site layout'unun Razor'a taşınması (HTML)

**Sen:** Account controller, user servisi, claim'ler, authorize attribute'ları, validator'lar

**Öğreneceğin:** Hash nedir ve neden şifre düz saklanmaz, Claim/ClaimsPrincipal, `SignInAsync`, rol bazlı yetki

**Bitti sayılır:** Normal kullanıcı admin paneline giremiyor; admin girebiliyor.

---

## Faz 6 — Site Sayfaları

**Amaç:** Ziyaretçinin gördüğü yüz. Çok dilli içeriği (LocalizedText) ekranda doğru dilde göstermek burada devreye giriyor.

**Ne yapılacak:**
- Ana sayfa: popüler turlar, en çok rezervasyon alan turlar, popüler destinasyonlar, sayaçlar (`CountDocumentsAsync`)
- Tur listesi: destinasyon, kategori, fiyat aralığı, tarih filtreleri (sadece aktif turlar)
- Tur detayı: galeri, bilgiler, tarihler + kalan kontenjan, öne çıkanlar, accordion program
- `LocalizedText`'i seçili dile göre gösteren küçük bir yardımcı (`.Get()` extension)

**Ben:** `index`, `tours`, `tour-detail` şablonlarının Razor'a taşınması (HTML)

**Sen:** Servis metotları, controller'lar, view'lardaki Razor kodu, LocalizedText yardımcısı

**Öğreneceğin:** Sorgularda filtre birleştirme, sayfalama/sıralama (`Sort`, `Limit`), extension method, ViewComponent (tekrar eden bölümler için)

**Bitti sayılır:** Dil İngilizce'ye geçince tur başlıkları da İngilizce görünüyor; filtreler çalışıyor.

---

## Faz 7 — Rezervasyon

**Amaç:** Gerçek iş kuralları: fiyat hesabı, kontenjan, durum yönetimi. Atomik güncelleme dersi burada.

**Ne yapılacak:**
- Tur detayında form: tarih seçimi, yetişkin, çocuk, canlı toplam fiyat (JS)
- Sunucuda fiyatı **yeniden** hesaplamak (tarayıcıdan gelen fiyata güvenilmez)
- Kontenjanı tek atomik komutla düşürmek: "kontenjan ≥ kişi sayısıysa düşür" — düşmediyse rezervasyon alınmaz
- Durumlar: Bekliyor → Onaylandı / İptal. İptal olunca kontenjan geri eklenir
- Admin rezervasyon listesi: tur, kullanıcı, tarih, durum filtreleri; onayla / iptal et

**Ben:** Admin rezervasyon listesinin HTML iskeleti

**Sen:** Rezervasyon servisi (atomik update dahil), controller'lar, validator, fiyat JS'i

**Öğreneceğin:** Race condition nedir, `$inc` + filtre ile atomik işlem, array içindeki elemanı güncelleme (`$[]` / positional operator), "istemciye güvenme" kuralı

**Bitti sayılır:** Kontenjanı 2 olan tarihe 3 kişilik rezervasyon yapılamıyor; iptal edince kontenjan geri geliyor.

---

## Faz 8 — Yorum, Puan, Favori, Soru, Profil

**Amaç:** Kullanıcı etkileşimleri ve profil sayfası.

**Ne yapılacak:**
- Yorum + 1-5 yıldız; tur detayında ortalama puan ve yıldız dağılımı (%)
- Favoriye ekle/çıkar (aynı tur iki kez eklenmez)
- Soru gönder; admin cevaplanmamış soruları listeler, cevaplar; cevap tur detayında görünür
- Profil: Rezervasyonlarım, Favorilerim, Yorumlarım, Sorularım sekmeleri

**Ben:** `profile` şablonunun Razor'a taşınması (HTML), admin soru listesi iskeleti

**Sen:** Servisler, controller'lar, validator'lar, Razor kodu

**Öğreneceğin:** Kullanıcıya göre filtreleme (`User` claim'inden Id okumak), benzersizlik kontrolü, farklı koleksiyonlardan veri birleştirme

**Bitti sayılır:** Kullanıcı profilinde 4 sekmenin hepsi kendi verisiyle doluyor.

---

## Faz 9 — Dashboard ve Aggregation

**Amaç:** Hocanın "sadece CRUD olmasın" dediği yer. İstatistikleri veritabanına hesaplatmak.

**Ne yapılacak:**
- Sayaçlar: toplam/aktif/pasif tur, rezervasyon, kullanıcı, bekleyen soru, bu ayki rezervasyon
- Aggregation pipeline ile:
  - En çok rezervasyon alan 5 tur (`$group` + `$sort` + `$limit` + `$lookup`)
  - Kategori bazında tur sayısı
  - Destinasyon bazında rezervasyon sayısı
  - Aylık rezervasyon sayıları (son 6 ay) → Chart.js grafiği
  - En yüksek gelir sağlayan tur (`$sum`)
  - Ortalama tur puanı (`$avg`)

**Ben:** `admin-dashboard` şablonunun Razor'a taşınması (HTML)

**Sen:** `DashboardService` (bütün pipeline'lar), controller, Chart.js kodu

**Öğreneceğin:** `$match`, `$group`, `$sort`, `$limit`, `$lookup`, `$project` aşamaları; neden Find() + LINQ yerine aggregation

**Bitti sayılır:** Dashboard'daki her sayı ve grafik gerçek veriden geliyor.

---

## Faz 10 — Katılımcı Raporu (Excel + PDF)

**Amaç:** Gerçek bir işletmenin ihtiyacı: "Bu turun 1 Aralık grubunda kimler var?"

**Ne yapılacak:**
- Tur listesinde / detayında "Rapor Al" butonu; isteğe bağlı tarih seçimi
- Alanlar: Ad Soyad, E-posta, Telefon, Tur, Tur Tarihi, Yetişkin, Çocuk, Toplam Kişi, Rezervasyon Tarihi, Toplam Ücret, Durum
- Excel: ClosedXML · PDF: QuestPDF
- Dosya diske yazılmaz: `MemoryStream` → `File(...)` ile doğrudan indirilir

**Ben:** —

**Sen:** `ReportService`, Excel ve PDF üretim kodu, controller

**Öğreneceğin:** Bellekte dosya üretmek, `FileResult`, QuestPDF'in fluent tasarım API'si

**Bitti sayılır:** Seçilen tarih için Excel ve PDF iniyor, içinde sadece o tarihin katılımcıları var.

---

## Son kontrol
- Her formda FluentValidation var mı?
- Bütün metinler TR/EN çevriliyor mu?
- Admin sayfaları yetkisiz kullanıcıya kapalı mı?
- GitHub'a son push
