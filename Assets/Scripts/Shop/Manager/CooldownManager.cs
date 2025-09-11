using UnityEngine;
using UnityEngine.UI;

public class CooldownManager : MonoBehaviour
{
    public Slider cooltimeSlider;
    public SpawnPoint spawnPoint; // SpawnPoint 스크립트 참조
    
    private int unitIndexToSpawn; // 스폰할 유닛의 인덱스 저장
    private float currentCooldownTime = 0f;
    private float maxCooldownTime = 0f; // 버튼 클릭 시 설정될 쿨타임 시간
    private bool isCooldown = false;

    // ShopManager 참조 필요 (할당 또는 Find 등)
    private ShopManager shopManager;

    void Start()
    {
        if (cooltimeSlider != null)
        {
            cooltimeSlider.value = 0;
        }
        shopManager = FindObjectOfType<ShopManager>();
        if (shopManager == null)
            Debug.LogError("ShopManager 인스턴스를 찾을 수 없습니다.");
    }

    private void Update()
    {
        if (isCooldown)
        {
            currentCooldownTime += Time.deltaTime;
            cooltimeSlider.value = currentCooldownTime / maxCooldownTime;

            if (currentCooldownTime >= maxCooldownTime)
            {
                isCooldown = false;
                currentCooldownTime = 0;
                cooltimeSlider.value = 0;

                //여기에 스폰유닛함수를 넣어야댐
                if (spawnPoint != null)
                {
                    spawnPoint.SpawnUnit(unitIndexToSpawn);
                }

                Debug.Log("쿨타임 종료! 다시 사용 가능합니다.");

                // **쿨타임 완료 알려주기**
                if (shopManager != null)
                    shopManager.OnCooldownFinished(); // 대기큐 다음 아이템 처리 호출
            }
        }
    }

    // isCooldown 상태를 반환하는 함수
    public bool IsOnCooldown()
    {
        return isCooldown;
    }

    // 이 함수를 버튼의 OnClick() 이벤트에 연결하고, 쿨타임 시간을 매개변수로 입력합니다.
    public void StartCooldown(float buttonCooldownTime, int unitIndex)
    {
        if (!isCooldown)
        {
            Debug.Log($"쿨타임 시작: {buttonCooldownTime}초");
            isCooldown = true;
            maxCooldownTime = buttonCooldownTime; // 버튼에서 받은 쿨타임 시간으로 설정
            currentCooldownTime = 0;
            cooltimeSlider.value = 0;

            // 전달받은 unitIndex를 저장합니다.
            unitIndexToSpawn = unitIndex;
        }
        else
        {
            Debug.Log("아직 쿨타임 중입니다.");
        }
    }
}