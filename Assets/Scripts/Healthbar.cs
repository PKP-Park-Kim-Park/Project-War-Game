using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public int maxHealth = 500;
    private int currentHealth;

    public Slider HealthBar;
    public TextMeshProUGUI text;

    void Awake()
    {
        Application.targetFrameRate = 60;
        currentHealth = maxHealth;
    }

    // 테스트를 위해 'H' 키를 누르면 데미지를 입도록 수정했습니다.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HealthBar.value -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (text != null)
        {
            text.text = $"{currentHealth}";
        }

        if (currentHealth <= 0)
        {
            // 사망 로직 추가
            Debug.Log("게임 오버");
        }
    }
}
