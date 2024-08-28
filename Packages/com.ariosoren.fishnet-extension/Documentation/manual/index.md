
#### کلاس مدیریت بازیکن (`PlayerNetworkController`)

```csharp
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;

public class PlayerNetworkController : NetworkBehaviour
{
    // متغیرهای همگام‌سازی شده با شبکه
    // فقط از سمت سرور آپدیت می شوند
    // البته کلاینت هم میتواند در سمت خود مقدار را تغییر دهد اما در شبکه سینک نخواهد شد.
    [SyncVar]
    public int syncHealth;

    // این مقدار توسط rpc آپدیت می شود
    // به همین دلیل نباید این متغببر بخ صورت پابلیک باشد
    // که باعث سردرگمی توسعه دهنگان می شود.
    private int score;

    // متدهایی که فقط در سرور اجرا می‌شوند
    [Server]
    public void ServerTakeDamage(int amount)
    {
        syncHealth -= amount;
        if (syncHealth <= 0)
        {
            ServerDespawnPlayer();
        }
    }

    [Server]
    public void ServerDespawnPlayer()
    {
        // دی‌اسپاون کردن بازیکن
        NetworkObject playerNetworkObject = base.NetworkObject;
        if (playerNetworkObject != null)
        {
            Despawn(playerNetworkObject);
        }
    }

    // متدهایی که فقط در کلاینت اجرا می‌شوند
    // می توان به کدهایی که فقط مربوط به ویژوال می شود یا
    // کد هایی مربوط به player input
    [Client]
    public void ClientUpdateUI()
    {
        if (!IsClient) return;
        // به‌روزرسانی عناصر رابط کاربری
    }

    // متدهای ServerRpc برای ارسال درخواست به سرور
    // از سمت کلاینت ها بر اساس اکشنی که فقط شمت کلاینت است
    // درخواستی به سرور ارسال می شود
    [ServerRpc]
    public void TakeDamageServerRpc(int amount)
    {
        // باید در سمت سرور ولیدیشن وجود داشته باشد
        ServerTakeDamage(amount);
    }
    
    // این متد دقیقا مقدار ضربه را در سرور محاسبه خواهد کرد
    // ولی داده های ارسالی از سمت کلاینت مستقیما مقدار صئمه نیست
    // و در این حالت سرور بهتر می تواند با داده های دریافتی از سمت کلاینت
    // ولیدیشن را انجام دهد
    [ServerRpc]
    public void TakeDamageServerRpc(Transform player, Transform target, Position hitPos)
    {
        // باید در سمت سرور ولیدیشن وجود داشته باشد
        if(validationDamageData(player,target,hitPos)}{
            ServerTakeDamage(amount);
        }
    }

    // متدهای ObserversRpc برای به‌روزرسانی کلاینت‌ها
    // باید از سمت سرور صدا زده شود
    [ObserversRpc]
    public void UpdateScoreObserversRpc(int newScore)
    {
        score = newScore;
        // به‌روزرسانی امتیاز برای تمامی کلاینت‌ها
    }

    // کالبک برای شروع در سمت سرور
    public override void OnStartServer()
    {
        base.OnStartServer();
        syncHealth = 100;
    }

    // کالبک برای شروع در سمت کلاینت
    public override void OnStartClient()
    {
        base.OnStartClient();
        ClientUpdateUI();
    }
}
```

#### کلاس مدیریت دشمن (`EnemyController`)

```csharp
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class EnemyController : NetworkBehaviour
{
    // متغیرهای همگام‌سازی شده با شبکه
    [SyncVar]
    public int syncHealth = 100;


    // متدی که فقط در سرور اجرا می‌شود
    [Server]
    private void ServerTakeDamage(int amount)
    {
        syncHealth -= amount;
        if (syncHealth <= 0)
        {
            DestroyEnemy();
        }
    }

    // متدی برای از بین بردن دشمن
    [Server]
    private void DestroyEnemy()
    {
        NetworkObject enemyNetworkObject = base.NetworkObject;
        if (enemyNetworkObject != null)
        {
            Despawn(enemyNetworkObject);
        }
    }

}
```
