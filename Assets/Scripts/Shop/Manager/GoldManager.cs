using UnityEngine;
using UnityEngine.UI;

public class GoldManager : MonoBehaviour
{
    public static GoldManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int gold = 10000;
    public Text goldText;

    private void Start()
    {
        UpdateGoldText();
    }

    public bool HasEnoughGold(int amount) => gold >= amount;

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldText();
    }

    public bool SpendGold(int amount)
    {
        if (HasEnoughGold(amount))
        {
            gold -= amount;
            UpdateGoldText();
            return true;
        }
        return false;
    }

    private void UpdateGoldText()
    {
        if (goldText != null)
            goldText.text = $"Gold : {gold}";
    }
}
