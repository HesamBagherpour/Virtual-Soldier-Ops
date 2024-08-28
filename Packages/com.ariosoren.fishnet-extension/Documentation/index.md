
<div dir=rtl style="text-align: center;">


# مستندات فنی برای استفاده از Fish-Networking 
<br><br>
</div>

<div dir=rtl>


## 1. مقدمه
---

<br>

### معرفی

این مستندات به منظور راهنمایی توسعه‌دهندگان در پیاده‌سازی مکانیک‌های پروژه هایی است که می خواهند از فریم‌ورک Fish-Networking در موتور بازی‌سازی یونیتی استفاده کنند تهیه شده است. هدف ما ارائه راهکارها و کانونشن‌هایی است که توسعه را تسهیل کرده و اطمینان حاصل کند که تمامی بخش‌های کد به صورت یکپارچه و کارآمد با مفاهیم شبکه‌ای هماهنگ هستند.


<br>

### معرفی Fish-Networking

Fish-Networking یک فریم‌ورک قدرتمند و انعطاف‌پذیر برای پیاده‌سازی قابلیت‌های چندنفره در بازی‌های یونیتی است. این فریم‌ورک امکانات گسترده‌ای برای مدیریت ارتباطات سرور و کلاینت، همگام‌سازی داده‌ها، و اجرای کدهای شبکه‌ای فراهم می‌کند. ویژه گی بارز این فریمورک پیاده سازی Server Authority می باشد.


<br>

## 2. مفاهیم پایه
---

<br>

### معماری سرور | کلاینت | هاست

در معماری پروژه های چند نفره تحت شبکه، سه نقش اصلی وجود دارد:

- **سرور (Server)**: مسئول مدیریت وضعیت بازی، اعتبارسنجی اقدامات بازیکنان و هماهنگ‌سازی اطلاعات بین کلاینت‌ها است. سرور به عنوان منبع اصلی حقیقت در بازی در معماری Server Authority عمل می‌کند.
- **کلاینت (Client)**: نماینده بازیکنان است که درخواست‌ها را به سرور ارسال کرده و به‌روزرسانی‌های بازی را دریافت می‌کند. کلاینت‌ها رابط کاربری و تجربه بازی را برای بازیکنان فراهم می‌کنند.
	- **Local Client**: بر اساس `Owner` کدی که اجرا می شود به کلاینتی گفته می شود که Owner آن آبجکت باشد.  `OwnerId == LocalClientId`
	- **Remote Client**: بر اساس `Owner` کدی که اجرا می شود به کلاینتی گفته می شود که Owner آن آبجکت نباشد. `OwnerId != LocalClientId`
- **هاست (Host)**: ترکیبی از سرور و کلاینت است که معمولاً در بازی‌های `P2P` استفاده می‌شود. در پروژه ما، ممکن است از هاست برای تست‌ها و توسعه محلی استفاده شود.



> [!Note]
> - آبجکت های که `OwnerId` آنها برابر با -1 است هیچ مالکی ندارند.
> - آبجکت هایی که در `Scene` از ابتدا وجود دارند `Networked Scene Objects` گفته می شود. برای اطلاع از محدودیت های این آبجکت ها به قسمت [محدودیت ها](#networked-scene-objects) رجوع کنید.
> - کلاینت ها برای کنترل آبجکت هایی که صاحب آن نیستند ابتدا باید توسط سرور مالکیت آن آبجکت را بگیرند.


<br>

### اصطلاحات کلیدی

- **NetworkBehaviour**: کلاسی که از `MonoBehaviour` مشتق شده و قابلیت‌های شبکه‌ای را فراهم می‌کند. تمامی اسکریپت‌هایی که نیاز به تعامل با شبکه دارند باید از این کلاس مشتق شوند.
- **NetworkObject**: آبجکتی که قابلیت شبکه‌ای دارد و می‌تواند بین سرور و کلاینت‌ها همگام‌سازی شود. دارای یک آیدی منحصر به فرد که میتوان آن را در همه کلاینت ها و سرور شناسایی کرد. تمامی آبجکت‌هایی که باید در شبکه به اشتراک گذاشته شوند باید به عنوان `NetworkObject` تعریف شوند.
- **RPC (Remote Procedure Call)**: مکانیزمی برای فراخوانی متدها در سمت دیگر شبکه (مثلاً از کلاینت به سرور یا بالعکس).
- **SyncVar**: متغیری که مقدار آن به طور خودکار بین سرور و کلاینت‌ها همگام‌سازی می‌شود. این همگام سازی باید از سمت سرور انجام گیرد تا در همه کلاینت ها آپدیت شود.

<br>

## 3. قواعد و کانونشن‌های کدنویسی
---

برای حفظ یکنواختی و خوانایی کدها، باید از قواعد و کانونشن‌های زیر پیروی شود:

<br>

### نام‌گذاری

- **متدها**:
  - متدهایی که **فقط** در سرور اجرا می‌شوند باید با پیشوند `Server` شروع شوند. مثال: `ServerSpawnEnemy()`
	- این متد ها دارای اتریبیوت `[Server]` می باشند. یعنی این متد ها تنها از سمت کدی اجرا می شوند که در **سرور** اجرا شده باشد.

  - متدهایی که **فقط** در کلاینت اجرا می‌شوند باید با پیشوند `Client` شروع شوند. مثال: `ClientPlayAnimation()`
	- این متد ها دارای اتریبیوت `[Client]` می باشند. یعنی این متد ها تنها از سمت کدی اجرا می شوند که در **کلاینت** اجرا شده باشد.

  - متدهای RPC باید با پسوند `Rpc` **ختم** شوند و با توجه به نوعشان نام‌گذاری شوند. مثال:
	- `UpdateDamageServerRpc()`
	- `BroadcastScoreUpdateObserversRpc()`
	- `kickReasonTargetRpc()`

> [!Note]
> بهتر است تا حد امکان، استفاده از متد های مولتی تارکت را کم کنید و از چند پیاده سازی جداگانه استفاده نمایید.


- **متغیرها**:
  - متغیرهایی که با SyncVar همگام‌سازی می‌شوند باید با پیشوند `sync` شروع شوند. مثال: `syncHealth`, `syncScore`


- **کلاس‌ها**:
  - نام کلاس‌ها باید بیانگر وظیفه و نقش شبکه‌ای آنها باشد. مثال: `PlayerNetworkController`, `EnemySpawnerNetwork`

> [!Note]
> تمام متد ها، پراپرتی ها و متغییر های پابلیک در کلاس باید رفتار مشخص شده در شبکه داشته باشند تا برای توسعه دهنده نحوه استفاده از آنها و رفتار آنها در شبکه شفاف باشد.


<br>

### ساختار کلاس‌ها

- تمامی کلاس‌هایی که با شبکه تعامل دارند باید از `NetworkBehaviour` مشتق شوند. مگر اینکه کلاس معمولی C# باشند در این صورت در متد سازنده کلاس NetworkBehaviour یا NetworkObject والد خود را دریافت می کنند.
- متدهای مرتبط با شبکه باید در بخش‌های جداگانه‌ای از کلاس قرار گیرند و با کامنت‌های مناسب مشخص شوند.
  
<br>

**مثال**:

 
</div>

```csharp
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;

public class PlayerNetworkController : NetworkBehaviour
{
    // SyncVars
    [SyncVar]
    public int syncHealth;

    // Server Methods
   [Server]
    public void ServerTakeDamage(int amount)
    {
        if (!IsServer) return;
        syncHealth -= amount;
    }

    // Client Methods
   [Client]
    public void ClientUpdateUI()
    {
        if (!IsClient) return;
        // Update UI elements
    }

    // ServerRpc Methods
    [ServerRpc]
    public void TakeDamageServerRpc(int amount)
    {
        ServerTakeDamage(amount);
    }

    // ObserversRpc Methods
    [ObserversRpc]
    public void UpdateScoreObserversRpc(int newScore)
    {
        // Update score for all clients
    }
}
```
<div dir=rtl>

<br>

### ملاحظات امنیتی

- تمامی ورودی‌های دریافت‌شده از کلاینت‌ها باید در سمت سرور اعتبارسنجی شوند. مگر اینکه از مهماری Client Aithority استفاده گردد.

> [!Note]
> در معماری Client Authority هم حتی اعتبار سنجی و اجرای بعضی از لاجیک ها که مربوط به آبجکت های Global است یا اینکه یک کلاینت خاص توانایی تصمیم گیری در مورد وضعیت آن را ندارد در سمت `سرور` هندل خواهد شد.
> مانند لاجیک مرگ یک پلیر یا NPC : در این حالت هیچ کلاینتی را نمیتوان برای بررسی آن اولویت داد جز خود سرور.

- از اجرای منطق حساس در سمت کلاینت خودداری کنید و مسئولیت‌های اصلی را به سرور واگذار کنید.
- دسترسی به متدها و متغیرها را بر اساس نیاز محدود کنید و از modifiers مناسب استفاده کنید (public, private, protected).


<br>

## 4. کار با NetworkBehaviour و NetworkObject
---

<br>

### ایجاد و مدیریت NetworkBehaviour

برای ایجاد اسکریپت‌هایی که قابلیت‌های شبکه‌ای دارند، مراحل زیر را دنبال کنید:

1. **ایجاد اسکریپت جدید**: یک اسکریپت جدید ایجاد کرده و آن را از `NetworkBehaviour` مشتق کنید.

</div>

```csharp
using FishNet.Object;

public class EnemyController : NetworkBehaviour
{
    // کدهای مربوط به دشمن
}
```

<div dir=rtl>

<br>

2. **تعریف متغیرهای شبکه‌ای**: از انواع سینک مانند `SyncVar` برای همگام‌سازی متغیرها بین سرور و کلاینت‌ها استفاده کنید.

> [!Note]
> در صورتی که تعداد متغییر های بالایی دارید و دوره بروز رسانی آنها نزدیک به هم می باشد از یک Struct یا Class برای مدیریت همه آنها استفاده کنید.
> برای سینک داده های کاستوم خود از `Custom Serializers` استفاده کنید.

</div>

```csharp
[SyncVar]
public int syncHealth = 100;
```

<div dir=rtl>

<br>

3. **استفاده از RPCها**: برای ارسال درخواست‌ها و به‌روزرسانی‌ها بین سرور و کلاینت‌ها از متدهای RPC استفاده کنید.

</div>

```csharp
[ServerRpc]
public void TakeDamageServerRpc(int amount)
{
    syncHealth -= amount;
    if (syncHealth <= 0)
    {
        DestroyEnemy();
    }
}
```
<div dir=rtl>

4. **استفاده از کالبک‌های شبکه‌ای**: از کالبک‌هایی مانند `OnStartServer` و `OnStartClient` برای اجرای کدهای خاص در زمان مناسب استفاده کنید.
- برای مقدار دهی و تنطیمات اولیه از کالبک های NetworkBehaviour استفاده کنید
- اگر می خواهید تنظیماتی برای آن آبجکت در همه کلاینت ها و سرور از قبل مشخص شود می تواند در متد Awake قرار دهید.
- از مقدار Owner , OwnerId , LocalClientId و دیگر مقادیر در این کالبک ها برای مدیریت کد استفاده کنید.

</div>

```csharp
public override void OnStartServer()
{
    base.OnStartServer();
    // کدهای اولیه‌سازی در سمت سرور
}

public override void OnStartClient()
{
    base.OnStartClient();
    // کدهای اولیه‌سازی در سمت کلاینت
}
```
<div dir=rtl>

### اسپاون و دی‌اسپاون آبجکت‌ها

برای ایجاد و حذف آبجکت‌های شبکه‌ای، از روش‌های زیر استفاده کنید:

**اسپاون کردن آبجکت**:

قبل از spawn کردن آبجکت باید یک نمونه از آن را با استفاده از Instantiate ایجاد کنید.

</div>

```csharp
[Server]
public void ServerSpawnEnemy(Vector3 position)
{
    GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
    Spawn(enemy);
}
```
<div dir=rtl>

**دی‌اسپاون کردن آبجکت**:

</div>

```csharp
[Server]
public void ServerDespawnEnemy(NetworkObject enemyNetworkObject)
{
    Despawn(enemyNetworkObject);
}
```
<div dir=rtl>

> [!Note]
> اطمینان حاصل کنید که عملیات اسپاون و دی‌اسپاون فقط در سمت سرور انجام می‌شود و تمامی کلاینت‌ها به درستی از این تغییرات مطلع می‌شوند.

<br>

## 5. محدودیت ها و نکات Fish Net: 
---

<br>

### در متد `OnStartNetwork` برای دست یابی  به مالک آبجکت باید از `base.Owner.IsLocalClient` استفاده کنید.

</div>

```csharp
public override void OnStartNetwork()
{
    /* If you wish to check for ownership inside
    * this method do not use base.IsOwner, use
    * the code below instead. This difference exist
    * to support a clientHost condition. */
    if (base.Owner.IsLocalClient)
        SetupCamera();
}

```
<div dir=rtl>

<br>

### Networked Scene Objects:

وقتی یک Scene Object شبکه‌ای می‌شود، ممکن است رفتار متفاوتی داشته باشد.

- یک Networked Scene Object زمانی که صحنه بارگذاری می‌شود غیرفعال خواهد بود و تا زمانی که کلاینت یا سرور راه‌اندازی نشده، فعال نخواهد شد. به طور خاص برای کلاینت‌ها، Networked Scene Objects تنها زمانی فعال می‌شوند که سرور مطمئن شود که کلاینت صحنه را بارگذاری کرده است؛ این کار به‌طور خودکار از طریق Scene Manager در Fish-Networking انجام می‌شود.
  
- Scene Manager ما اجازه می‌دهد تا اشیاء شبکه‌ای ایجاد شده بین صحنه‌ها جابه‌جا شوند، اما Networked Scene Objects نمی‌توانند به این شکل عمل کنند. یونیتی نمی‌تواند جزئیات یک Scene Object را بدون بارگذاری صحنه بداند، بنابراین تلاش برای اسپاون کردن یک Scene Object بدون بارگذاری صحنه توسط کلاینت باعث ایجاد خطا می‌شود.

- به دلیل محدودیت‌های ذکر شده در جابه‌جایی، Networked Scene Objects نمی‌توانند به‌عنوان `DontDestroyOnLoad` علامت‌گذاری شوند و همچنین نمی‌توان از ویژگی `NetworkObject.IsGlobal` استفاده کرد. هر دو این موارد Scene Object را در یک صحنه جدید قرار می‌دهند که باعث بروز خطا می‌شود.

- زمانی که یک Networked Scene Object دی‌اسپاون می‌شود، همیشه غیرفعال می‌شود، نه اینکه نابود شود. این کار به این دلیل است که بتوانید آن را در زمانی دیگر اسپاون کنید. به صورت دستی نابود کردن یک Scene Object در سرور ممکن است و در نتیجه آن شیء هرگز در کلاینت‌ها اسپاون نمی‌شود.

<br>

### Nested NetworkObjects و NetworkBehaviours:

یک `NetworkObject` تو در تو ممکن است از والد خود جدا شود، اما حتی در صورت جدا شدن از والد، باز هم با دی‌اسپاون شدن والد، آن نیز دی‌اسپاون می‌شود. این اتفاق به این دلیل می‌افتد که والد یک مرجع کش شده از رفتارها و اشیاء تو در تو خود دارد و این مراجع نمی‌توانند بدون هزینه عملکردی غیرقابل قبول به صورت خودکار به‌روزرسانی شوند.


در نهایت پیشنهاد می شود که حتما مستندات FIsh NEtworking مطالعه گردد.

<br>

https://fish-networking.gitbook.io/docs

</div>