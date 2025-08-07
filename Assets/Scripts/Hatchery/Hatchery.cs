using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hatchery : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 500;
    private int currentHealth;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

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
            Die();
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

    private void Die()
    {
        // 게임 오버 로직 추가
        Debug.Log("Hatchery가 파괴되었습니다. 게임 오버!");
        // gameObject.SetActive(false); // 예: 해처리 비활성화
    }
}
