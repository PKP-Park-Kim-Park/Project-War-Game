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
        // UI 컴포넌트가 할당되었는지 확인하여 실수를 방지합니다.
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
        // 데미지는 항상 양수여야 합니다.
        if (damage < 0) return;

        // TakeDamage가 호출되는지 확인하기 위한 진단용 로그입니다.
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
        // 사망 로직 추가
        Debug.Log("Hatchery가 파괴되었습니다. 게임 오버!");
        // gameObject.SetActive(false); // 예: 해처리 비활성화
    }
}
