using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // FSM için durumlarımız
    private enum EnemyState
    {
        Patrol,
        Chase
    }

    [Header("References")]
    [Tooltip("Devriye sol sınırı")]
    public Transform leftPoint;
    [Tooltip("Devriye sağ sınırı")]
    public Transform rightPoint;
    [Tooltip("Kovalayacağımız Player Transform (elle atayabilirsiniz veya kodla bulun)")]
    public Transform player;

    [Header("Settings")]
    [Tooltip("Hareket hızı (devriye ve kovalama)")]
    public float moveSpeed = 2f;
    [Tooltip("Devriye esnasında hedef noktaya yakınlık eşiği")]
    public float patrolArriveThreshold = 0.2f;

    private EnemyState currentState;
    private bool movingRight = true; // Şu an sağa doğru mu devriye yapılıyor
    private Transform targetPatrolPoint; // Hangi noktaya doğru ilerliyoruz (leftPoint / rightPoint)
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Başlangıçta devriye modundayız
        currentState = EnemyState.Patrol;
        // İlk devriye hedefimiz rightPoint olsun (isterseniz leftPoint de yapabilirsiniz)
        targetPatrolPoint = rightPoint;
    }

    private void Update()
    {
        // 1) Öncelikle, her framede Player devriye alanında mı diye bakıyoruz
        if (IsPlayerInPatrolArea())
        {
            // Oyuncu devriye alanında ise Chase moduna geç
            currentState = EnemyState.Chase;
        }
        else
        {
            // Oyuncu devriye alanının dışında ise Patrol moduna geç
            currentState = EnemyState.Patrol;
        }

        // 2) FSM durumuna göre davranış sergile
        switch (currentState)
        {
            case EnemyState.Patrol:
                PatrolMovement();
                break;

            case EnemyState.Chase:
                ChasePlayer();
                break;
        }
    }

    /// <summary>
    /// Devriye alanını basitçe X ekseninde leftPoint ile rightPoint arasındaki bölge olarak kabul eder.
    /// Oyuncunun x pozisyonu bu aralık içindeyse, "alan içinde" demektir.
    /// </summary>
    private bool IsPlayerInPatrolArea()
    {
        if (player == null) return false; // Player referansı yoksa

        float leftX = leftPoint.position.x;
        float rightX = rightPoint.position.x;
        float playerX = player.position.x;

        // Eğer leftPoint sağPoint'ten büyükse, yer değiştirelim (olası sahne kurulum hatası)
        if (leftX > rightX)
        {
            float temp = leftX;
            leftX = rightX;
            rightX = temp;
        }

        // Player x bu aralıktaysa true döner
        return (playerX >= leftX && playerX <= rightX);
    }

    /// <summary>
    /// Devriye hareketi: düşman leftPoint ve rightPoint arasında gidip gelir.
    /// Hedefe ulaştığında yön değiştirir.
    /// </summary>
    private void PatrolMovement()
    {
        // Hedef noktaya doğru ilerle
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPatrolPoint.position,
            moveSpeed * Time.deltaTime
        );

        // Sprite flip işlemi (hedef nokta sağdaysa sağa bak, soldaysa sola bak)
        if (targetPatrolPoint.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true; // sola bak
        }
        else
        {
            spriteRenderer.flipX = false; // sağa bak
        }

        // Hedefe yaklaştık mı?
        float distance = Vector2.Distance(transform.position, targetPatrolPoint.position);
        if (distance <= patrolArriveThreshold)
        {
            // Mevcut hedefimiz rightPoint ise, sonraki hedef leftPoint olsun
            if (targetPatrolPoint == rightPoint)
            {
                targetPatrolPoint = leftPoint;
            }
            else
            {
                // Tersi: eğer mevcut hedef leftPoint ise, sonraki hedef rightPoint
                targetPatrolPoint = rightPoint;
            }
        }
    }

    /// <summary>
    /// Kovalama hareketi: düşman Player transform'una doğru hareket eder.
    /// (Sadece X ekseninde de kısıtlayabilirsiniz, örn. 2D platformer'e göre.)
    /// </summary>
    private void ChasePlayer()
    {
        if (player == null) return;

        // Player pozisyonuna doğru ilerle
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );

        // Sprite flip (oyuncu soldaysa sola, sağdaysa sağa dön)
        if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true; // sola bak
        }
        else
        {
            spriteRenderer.flipX = false; // sağa bak
        }
    }
}
