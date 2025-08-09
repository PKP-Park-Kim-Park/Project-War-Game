using UnityEngine;
using UnityEngine.UI;

public class ExpManager : MonoBehaviour
{
    public int exp = 0;
    public Text expText;

    public int expIncreaseAmount = 10;
    public float expIncreaseInterval = 1f;

    private float expTimer = 0f;

    private void Start()
    {
        UpdateExpText();
    }

    private void Update()
    {
        expTimer += Time.deltaTime;
        if (expTimer >= expIncreaseInterval)
        {
            expTimer = 0f;
            AddExp(expIncreaseAmount);
        }
    }

    public void AddExp(int amount)
    {
        exp += amount;
        UpdateExpText();
    }

    public bool HasEnoughExp(int amount) => exp >= amount;

    private void UpdateExpText()
    {
        if (expText != null)
            expText.text = $"EXP : {exp}";
    }
}
