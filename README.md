# ProductManager Projesi

*Bu proje, Docker Compose kullanarak MSSQL veritabanı ile entegre bir ProductManager uygulamasının nasıl çalıştırılacağını anlatmaktadır. Proje, CQRS, Clean Architecture, Repository Pattern, ve LunavexResult kütüphanesi gibi modern yazılım prensiplerini içermektedir.*  

## Gereksinimler

- Docker Desktop bilgisayarınıza kurulu olmalıdır.


## Kurulum Adımları

### 1.Proje Ana Klasörüne Gitme

- Terminalinizi veya komut satırını açarak projenin ana klasörüne gidin. Örnek olarak:

```
C:/Users/YourUser/repos/ProductManager

```
### 2. Docker Compose ile Uygulamayı Başlatma

- Proje dizininde aşağıdaki komutu çalıştırın:

```
docker compose up -d

```
- Bu komut, MSSQL veritabanını ve ProductManager uygulamasını Docker üzerinde başlatacaktır.

### 3. Swagger Üzerinden Uygulamayı Kullanma


- Uygulamanız başarıyla ayağa kalktığında, tarayıcınızda Swagger arayüzüne ulaşmak için şu URL'yi ziyaret edebilirsiniz:


```
http://localhost:5000/swagger/index.html

```

- Swagger üzerinden API endpoint'lerini görüntüleyebilir ve ürün yönetimi işlemlerini gerçekleştirebilirsiniz.

## Kimlik Doğrulama

- Swagger üzerinde yönetici yetkileriyle işlem yapabilmeniz için kimlik doğrulaması yapmanız gerekmektedir.

### 1. Giriş Yapmak İçin Token Alın

- api/auth endpoint'ini kullanarak aşağıdaki kimlik bilgilerini girin:

```
Kullanıcı Adı: admin@example.com
Şifre: Password123*

```
-Bu bilgilerle giriş yaparak bir JWT Token alacaksınız.

### 2. Token ile Yetkilendirme

- Swagger arayüzünde sağ üst köşedeki Authorize butonuna tıklayarak aldığınız token'ı girin. Bu sayede admin yetkileriyle sisteme giriş yapabilirsiniz.

## Yetkiler ve İşlemler

- Admin Rolüyle (Yetkilendirilmiş): Token'ı girdikten sonra Update ve Delete işlemlerini yapabilirsiniz.
- Giriş Yapmadan: Sadece Read ve Create işlemlerini gerçekleştirebilirsiniz.

# Proje Mimarisi ve Kullanılan Teknolojiler


## 1. Clean Architecture

Proje, Clean Architecture prensiplerine dayanarak katmanlar arasında bağımlılığı en aza indirgeyen ve esnekliği artıran bir yapıda tasarlanmıştır. Bu sayede uygulamanın iş kuralları ve veritabanı gibi detaylar arasında güçlü bir ayrım sağlanmıştır.

## 2. CQRS (Command Query Responsibility Segregation)

Proje, CQRS pattern'ini kullanarak komut ve sorgu işlemlerini ayırmaktadır. Bu sayede veri okuma ve yazma işlemleri daha iyi ölçeklenebilir ve yönetilebilir hale gelmiştir.


## 3. Repository Pattern

Veritabanı işlemleri, Repository Pattern kullanılarak soyutlanmış.

## LunavexResult Kütüphanesi

Bu kütüphane, başarılı ve hatalı sonuçların merkezi bir formatta yönetilmesini sağlar, böylece sonuçlar sistem genelinde tutarlı bir şekilde işlenir.



## 5. Merkezi Loglama ve Validation
Projedeki işlemler ve doğrulamalar, merkezi olarak yönetilen Behaviour sınıfları ile gerçekleştirilir. Bu sayede loglama, doğrulama ve diğer işlem davranışları katmanlar arasında tekrarlanmak yerine tek bir yerden kontrol edilir.

## 6. User ve Role Yönetimi için ASP.NET Identity

Kullanıcı ve rol yönetimi için ASP.NET Identity kütüphanesi entegre edilmiştir. Bu yapı, kullanıcı yetkilendirme ve kimlik doğrulama işlemlerinin güvenli ve esnek bir şekilde yapılmasını sağlar.


## 7. Seed Data (Öntanımlı Veriler)

İlk kurulumda, veri tabanına öntanımlı olarak bazı Product ve User verileri eklenmektedir. Bu sayede uygulamanın ayağa kalkmasıyla birlikte temel veri seti hazır olur.



## 8. Unit Testler

Application katmanında yer alan CRUD işlemleri için Unit Testler yazılmıştır. Bu testler, uygulamanın iş kurallarının doğru çalıştığını ve hata durumlarının yönetilebildiğini doğrulamak için tasarlanmıştır


