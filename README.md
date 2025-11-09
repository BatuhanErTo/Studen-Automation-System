## Kurulum
- Projeyi kendi makinenize klon ettikten sonra sırasıyla aşağıdaki işlemleri gerçekleştiriniz:
- Projenin içerisinde bulunan **etc/docker** klasörüne gidin ve terminalden **.\run-docker.ps1** adlı komutu çalıştırın. 
- Bu işlem sonunda postgre ve PgAdmin kullanımıza hazır olacak, **localhost:5050** adresinden PgAdmin’e giriş yapabilirsiniz. 
  - Kullanıcı adı ve şifre bilgileri: **Kullanıcı adı: spaghetti@spaghetti.com.tr**, **Şifre: spaghettiErol**
- PgAdmin’e giriş yaptıktan sonra, sol üstteki **"Servers"** menüsüne sağ tıklayın ve **Register > Server** seçeneğini seçin.
  - General sekmesinde Name alanına localhost yazın.
  - Connection sekmesinde ise Host name/address kısmına **host.docker.internal**, kullanıcı adına **postgres** ve şifreye **1234** yazın. Ardından kaydedin.
  - En son olarak Visual Studio’da projeyi açın ve **DBMigrator** projesini çalıştırın. Automation veri tabanı oluşacaktır.
  - Projenin başlangıç projeisini **Blazor** seçin ve çalıştırın.
  - Blazor Web projesi açıldığınında Web Sayfasının dil seçeneğini **İngilizce** yapın çünkü tüm Lokalizasyon ingilizce için yapıldı.
  - Açılan web sayfasının linkinin yanına '**/swagger**' eklersenin EndPointlere swagger dökümantasyonu aracılığıyla erişebilirsiniz.

## Domain Dizaynı
- Domain Katmanı temel olarak **5 temel AggregateRoot** üzerinde tasarlanmıştır. Bunlar:
  - StudentEntity (maalesef visual studio proje isimlendirmemden dolayı Student ismini kabul etmedi, import ederken sıkıntılar yaşattı o yüzden StudentEntity olarak isimlendirilmiştir.)
  - Teacher
  - Department
  - Enrollment
  - Course
- Course ve Enrollment Root varlıkları tarafından yaşam döngüsü yönetilen child varlıklar tanımlanmıştır.
  - Course içerisinde:
    - **CourseSession**
      - bir dersin hangi gün ve saatler arasında olacağını yönetir
    - **GradeComponent** 
      - bir derse notlamalar eklemek içindir, buradan kasıt puanlama değildir. Mesela bir dersin 4 tane sınavı varken başka bir dersin iki sınavı vardır. Bunu yönetmek için oluşturulmuş. Puan yönetimini yapmaz.
  - Enrollment içerisinde:
    - **AttendanceEntry**
      - bir derse kayıtlı öğrencinin derse katılımını yönetmek için
    - **GradeEntry**
      - öğrencinin kayıtlı olduğu bir ders içerisinde bulunun **GradeComponent**'lerin puanlama yönetimini sağlar
    - **TeacherComment**
      - öğretmenin kendi dersine kayıtlı olan öğrenciye yorum yapmasını sağlar
- Bu dizaynın altındaki temel mantık şudur:
  - Course varlığı ile derslerin takvimini ve o ders süresince yapılacak olan puanlama sistematiğini yönetmek
  - Enrollment varlığı ile bir derse kayıtlı olan öğrencinin sınav vs. sonucu almış olduğu puanların, derse katılım takibini ve öğretmen tarafından öğrenciye yapılan yorumları yönetmek
  - Böylelikle de **temel Domain Driven Designe uygun bir Domain katmanı oluşturulmuştur**.

## Kullanıcı Yönetimi
- Projede üç tip rol vardır. Bunlar halihazırda abp ile gelen **admin** ve bunun yanında benim tarafımdan eklenmiş olan **teacher** ve **student** 'dır.
- Student ve Teacher varlıkları birer kullanıcı olarak değerlendiriğinden içerisinde Guid tiğinde IdentityUserId alanı bulunmaktadır. Bu alan 'foreign key' olarak ABP içerisinde bulunan AbpUsers tablosuna referans verir. Bu planlama sayesinde sisteme öğretmen ve öğrenci olarak giriş yapılıp gerekli işlemler gerçekleştirilebilmektedir.
- Domain Katmanı içerisinde bulunan StudentAutomationDataSeederContributor.cs adlı Data Seed işlemini sağlayan koda girerseniz bazı hazır kullanıcıları göreceksiniz.
- Proje kurulumunu başarılı bir şekilde yaparsınız bu kullanıcılar sisteminizde tanımlı olacaktır.
- Erişim rahatlığı olması açısından üç tane kullanıcıyı aşağıya kullanıcı adı ve şifre olarak ekledim:
  - ADMIN
    - kullanıcı adı: admin
    - şifre: 1q2w3E*
  - TEACHER
    - kullanıcı adı: ahmet@university.edu
    - şifre: Ahmet123!
  - STUDENT
    - kullanıcı adı: ali.kirici@example.com
    - şifre: AliPassword123!
## İzinler
- Projede izin yapısı her bir ana varlık ve child varlıklar için yapılmıştır. Admin olarak sisteme girersiniz izinleri görüntüleyebilirsiniz veya **AutomationPermissions.cs** dosyasında ayrıntıları bulabilirsiniz.

## Projenin Kullanımı Frontend Görselleriyle

### Admin Kullanıcı için
<img width="1896" height="466" alt="image" src="https://github.com/user-attachments/assets/55bff009-eaf9-49cc-884e-f2b67bf73fba" />

#### Admin Öğretmen Listeleyebilir, Filtreleyebilir, Güncelleyebilir, Silebilir ve Oluşturabilir
<img width="1873" height="905" alt="image" src="https://github.com/user-attachments/assets/065bca35-5b86-485d-99ba-49eb80c406fd" />

<img width="1890" height="878" alt="image" src="https://github.com/user-attachments/assets/62ce384c-7d12-41f2-b5c9-fc4e0cd10caf" />

<img width="1459" height="238" alt="image" src="https://github.com/user-attachments/assets/07f515b7-6ef0-456c-be36-813782d06944" />

<img width="1882" height="903" alt="image" src="https://github.com/user-attachments/assets/ae431ca0-819b-435f-bd03-3d653cfcdb85" />

#### Admin Ders Listeleyebilir, Filtreleyebilir, Güncelleyebilir, Silebilir ve Oluşturabilir
<img width="1874" height="889" alt="image" src="https://github.com/user-attachments/assets/e8d23f72-e036-427c-ac46-7c70387745a5" />

<img width="1549" height="919" alt="image" src="https://github.com/user-attachments/assets/c0b4bf42-6d29-46f7-8aae-55b2d23b125b" />

<img width="1883" height="874" alt="image" src="https://github.com/user-attachments/assets/844c7d1a-f214-4471-b1f4-681bfdc34beb" />

<img width="1464" height="159" alt="image" src="https://github.com/user-attachments/assets/cddb8788-834e-4277-ba34-cb795eeb8698" />

#### Admin Öğrenci Listeleyebilir, Filtreleyebilir, Güncelleyebilir, Silebilir ve Oluşturabilir
<img width="1902" height="928" alt="image" src="https://github.com/user-attachments/assets/4c50edc5-cd77-4cb5-ae68-68c8bf64cac8" />

- Yapı genel olarak her üç sayfa için aynıdır. Admin bu sayfalara filtreleme yapabilir, ders, öğrenci, öğretmen ekleyebilir güncelleyebilir ve silebilir.
### Teacher Kullanıcı için
<img width="1905" height="519" alt="image" src="https://github.com/user-attachments/assets/d0ea46a5-b3dd-4195-a987-c5827bab320a" />

### Teacher kullanıcı derslerini görüntüleyebilir, detaylarını görüntüleyebilir, ders durumlarını güncelleyebilir, dersine sınav türleri (GradeComponent ekleyebilir), ders takvimi (CourseSession) ekleyebilir, öğrenciyi derse ekleyebilir
<img width="1919" height="567" alt="image" src="https://github.com/user-attachments/assets/743a2c83-0318-4af4-881d-71a827a5491d" />

<img width="1408" height="797" alt="image" src="https://github.com/user-attachments/assets/50b1bb78-dae1-4c90-943c-9f11515d6831" />

<img width="1456" height="641" alt="image" src="https://github.com/user-attachments/assets/e417553c-96fa-40a8-a0a7-cc80727fd0a4" />

<img width="1560" height="771" alt="image" src="https://github.com/user-attachments/assets/e67f79b0-2151-4cb6-8d64-15d3fa003b80" />

<img width="1305" height="877" alt="image" src="https://github.com/user-attachments/assets/543f029d-5c68-4a9f-b025-22de8ccb26a3" />

<img width="1288" height="699" alt="image" src="https://github.com/user-attachments/assets/845611bb-0f8f-416e-b0a7-ee012ced553b" />

- Enroll Student formu açılınca ilk olarak search tuşuna basınız, form açılır açılmaz verileri getirmeyi yapmamışım. Bug diyemem o detay gözümden kaçmış.

### Teacher kullanıcı derslerine kayıtlı olan öğrencilerine ayarlamış olduğu GradeEntry'e göre puan verebilir
<img width="1916" height="666" alt="image" src="https://github.com/user-attachments/assets/794c83ac-5b3c-4cec-afed-9d415a1322ec" />

### Teacher kullanıcı derslerine kayıtlı olan öğrencilerin derse katılım durumlarını ekleyebilir
<img width="1905" height="790" alt="image" src="https://github.com/user-attachments/assets/073c4f3f-4a94-40a4-9193-ce1639a0d722" />

### Teacher kullanıcı derslerine kayıtlı olan öğrencilerine yorum yapabilir ve o öğrencilere daha önce atmış olduğu mesajları listeli halde görebilir
<img width="1881" height="763" alt="image" src="https://github.com/user-attachments/assets/b2454ec6-9366-4628-b695-5034b0b931cd" />

<img width="1919" height="594" alt="image" src="https://github.com/user-attachments/assets/7c288ef0-d64b-40d0-af77-993938a474f9" />

### Teacher kullanıcı Öğrenci Listeleyebilir, ekleyebilir, öğrenci bilgilerini güncelleyebilir
<img width="1905" height="868" alt="image" src="https://github.com/user-attachments/assets/9aba5a0d-f08b-4326-9aba-1c04eeba94ff" />

### Student Kullanıcı için
<img width="1917" height="392" alt="image" src="https://github.com/user-attachments/assets/7ba65520-27fe-4ea0-9428-ee43f95de50b" />

### Student kullanıcı kayıtlı olduğu derslere ait puanlamaları ve ortalamayı görebilir
<img width="1894" height="390" alt="image" src="https://github.com/user-attachments/assets/b56808e2-de75-49a9-9b99-6e56ce5f052b" />

### Student kullanıcı kayıtlı olduğu derslere ait öğretmen notlarını görebilir
<img width="1894" height="545" alt="image" src="https://github.com/user-attachments/assets/0a2b2f4c-b2ca-4c9f-90d0-ebfe61cb22d6" />

## Endpointler
- Ben ayrıyeten [RemoteService(false)] kullanıp Controller'ı kendim ekleyip özelleştirme yapmadım, AppService içerisinde bulunan methodlar aracılığıyla ABP tarafından otomatik oluşturulmasını istedim.
- Aşağıda bu projede bulunan endpoinlerin swagger dökümanstasyonundan almış olduğum ekran görüntüleri vardır:

<img width="1536" height="827" alt="image" src="https://github.com/user-attachments/assets/b0d64994-f605-415c-92a3-b7e164f5e00e" />

<img width="1037" height="825" alt="image" src="https://github.com/user-attachments/assets/79d2d18e-e553-4da3-82b6-49b640d923e2" />

<img width="987" height="700" alt="image" src="https://github.com/user-attachments/assets/f5c5af58-1bd0-41c7-92b0-a41fd2969b9a" />

- Projede tüm bu endpointleri kullanmadım, bazıları ileriyi düşünerek bırakıldı. Amacım tüm endpointlerle sistemin ileriye dönükte hazır hale gelmesi.

## Eksik Kalan Yerler ve İlerisi İçin Yapılacaklar
- Backend kısmında bazı method, değişken isimleri çok kötü bırakılmış bunları yer yer tekrardan yazmış olduğum kodu okuyunca yapılacaklar olarak ileride düzeltmek için ekledim. Baz yerlerde yorum satırlarında TODO olarak not düştüm.
- Backend logic olarak İş kuralları arasında eksikler içeriyor olabilir. Oradalarda gerekli notları yorum satırında TODO olarak aldım.Ayrıca issue olarak ekliyecem.
- Projeye loglama için elasticsearhc, cache için redis eklenebilirdi. Vakit kısıtlması olduğu için yetişmedi. Bunları içinde gerekli notlarımı aldım.
- Projede rabbitmq en çok öğretmenler not, yorum eklediğinde öğrencilere bildirim yapmak için kurulabilirdi. İleride bunu yapmak için not aldım.
- İlerisi için projeye CI pipeline ve Unit Test ekleyeceğim.
- **İlerisi için yapacağım görevleri şu şekilde sıralacağım. Bu sıralamanın amacı akademi programının eğitim çıktılarını doğru bir düzende takip etmek içindir**:
  - Projeye ElasticSearch ve Redis kullanarak gerekli featur'lar eklenecek
  - RabbitMq yapısı kurulacak
  - Unit Test eklenecek
  - CI pipeline kurulacak

- **Vaktinizi ayırdığınız için teşekkür ederim**




  
  





