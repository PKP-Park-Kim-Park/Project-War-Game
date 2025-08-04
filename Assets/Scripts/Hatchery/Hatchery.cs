using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hatchery : MonoBehaviour
{
    public int maxHealth = 500;
    private int currentHealth;

    public Slider HealthBar;
    public TextMeshProUGUI text;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(25);
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
