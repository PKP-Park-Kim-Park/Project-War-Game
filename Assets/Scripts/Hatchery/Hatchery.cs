using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hatchery : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 500;
    private int currentHealth;

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
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (damage < 0) return;

        Debug.Log($"Hatchery가 {damage}의 데미지를 받았습니다. 이전 체력: {currentHealth}", this);

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            DestroyHatchery();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth}";
        }
    }

    private void DestroyHatchery()
    {
        // 적 해처리인지 태그로 확인
        if (gameObject.CompareTag(enemyHatcheryTag))
        {
            // --- 승리 조건 ---
            Debug.Log("적 해처리가 파괴되었습니다. 게임 승리!");
            if (winUIPanel != null)
            {
                winUIPanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("승리 UI 패널이 할당되지 않음", this);
            }
            // 3초 후 게임 종료 시퀀스 시작
            StartCoroutine(SequenceCoroutine());
        }
        else if (gameObject.CompareTag(playerHatcheryTag))
        {
            // --- 패배 조건 (플레이어 해처리) ---
            Debug.Log("적 해처리가 파괴되었습니다. 게임 승리!");
            if (winUIPanel != null)
            {
                loseUIPanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("패배 UI 패널이 할당되지 않음", this);
            }
            // 3초 후 게임 종료 시퀀스 시작
            StartCoroutine(SequenceCoroutine());
        }
    }

    /// <summary>
    /// 승리 연출을 위한 코루틴. 3초 대기 후 게임을 종료합니다.
    /// </summary>
    private IEnumerator SequenceCoroutine()
    {
        yield return new WaitForSeconds(3f);
        if (GameManager.instance != null)
        {
            GameManager.instance.EndGame();
        }
    }
}
