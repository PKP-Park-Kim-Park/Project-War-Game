using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 해처리의 시대를 정의합니다.
/// </summary>
public enum Age
{
    Primitive,   // 원시 시대
    Developed,   // 발전 시대
    Advanced     // 진보 시대
}

public class Hatchery : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 500;
    private int currHealth;

    [Header("Era Settings")]
    [Tooltip("현재 해처리 시대")]
    [SerializeField] private Age currAge = Age.Primitive;
    public Age CurrAge => currAge;

    [Header("UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    [Tooltip("게임 승패 시 활성화할 UI 패널")]
    [SerializeField] private GameObject winUIPanel;
    [SerializeField] private GameObject loseUIPanel;

    [Header("Tags")]
    [Tooltip("적 해처리 태그")]
    [SerializeField] private string enemyHatcheryTag = "EnemyHatchery";
    [SerializeField] private string playerHatcheryTag = "PlayerHatchery";

    void Awake()
    {
        if (healthBar == null)
        {
            Debug.LogWarning("Hatchery: HealthBar가 Inspector에 할당되지 않았습니다.", this);
        }
        if (healthText == null)
        {
            Debug.LogWarning("Hatchery: HealthText가 Inspector에 할당되지 않았습니다.", this);
        }
        if (winUIPanel != null)
        {
            winUIPanel.SetActive(false);
        }
        currHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (damage < 0) return;

        Debug.Log($"Hatchery가 {damage}의 데미지를 받았습니다. 이전 체력: {currHealth}", this);

        currHealth = Mathf.Clamp(currHealth - damage, 0, maxHealth);

        UpdateHealthUI();

        if (currHealth <= 0)
        {
            DestroyHatchery();
        }
    }

    /// <summary>
    /// 해처리 시대 업글
    /// </summary>
    public void UpgradeAge()
    {
        if (currAge < Age.Advanced)
        {
            currAge++;
            Debug.Log($"해처리 시대가 {currAge}(으)로 업그레이드되었습니다!", this);

            // 최대 체력을 2배로 늘립니다.
            maxHealth *= 2;

            // 현재 체력을 새로운 최대 체력으로 회복
            currHealth = currHealth + (maxHealth / 2);

            // 체력 UI를 업데이트합니다.
            UpdateHealthUI();

            Debug.Log($"해처리 체력이 {maxHealth}(으)로 증가했습니다!", this);
        }
        else
        {
            Debug.Log("이미 최종 시대입니다.", this);
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currHealth}";
        }
    }

    private void DestroyHatchery()
    {
        bool isGameOver = false;
        // 적 해처리인지 태그로 확인
        if (gameObject.CompareTag(enemyHatcheryTag))
        {
            // --- 승리 조건 ---
            Debug.Log("적 해처리가 파괴되었습니다. 게임 승리!");
            isGameOver = true;
            if (winUIPanel != null)
            {
                winUIPanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("승리 UI 패널이 할당되지 않음", this);
            }
        }
        else if (gameObject.CompareTag(playerHatcheryTag))
        {
            // --- 패배 조건 (플레이어 해처리) ---
            Debug.Log("플레이어 해처리가 파괴되었습니다. 게임 패배!");
            isGameOver = true;
            if (loseUIPanel != null)
            {
                loseUIPanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("패배 UI 패널이 할당되지 않음", this);
            }
        }

        if (isGameOver)
        {
            // 게임 정지 및 BGM 끄기
            Time.timeScale = 0f;

            BGM bgmManager = FindObjectOfType<BGM>();
            if (bgmManager != null)
            {
                bgmManager.StopMusic();
            }

            // 3초 후 게임 종료 시퀀스 시작 (Time.timeScale에 영향을 받지 않음)
            StartCoroutine(SequenceCoroutine());
        }
    }

    /// <summary>
    /// 승리 연출을 위한 코루틴. 3초 대기 후 게임을 종료합니다.
    /// </summary>
    private IEnumerator SequenceCoroutine()
    {
        yield return new WaitForSecondsRealtime(3f);
        if (GameManager.instance != null)
        {
            GameManager.instance.EndGame();
        }
    }
}
