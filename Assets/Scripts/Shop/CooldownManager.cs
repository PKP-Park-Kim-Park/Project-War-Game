using UnityEngine;
using UnityEngine.UI;

public class CooldownManager : MonoBehaviour
{
    // 쿨타임 바 UI (Fill Image)
    public Image cooltimeBarFill;

    private float currentCooldownTime = 0f; // 쿨타임이 시작된 후 경과된 시간
    private float maxCooldownTime = 0f;     // 현재 적용된 쿨타임 총 시간
    private bool isCooldown = false;        // 쿨타임 진행 여부

    // 게임 시작 시, 혹은 오브젝트가 활성화될 때 한 번 호출됩니다.
    void Start()
    {
        // 쿨타임 바를 0으로 초기화하여 비어있는 상태로 만듭니다.
        if (cooltimeBarFill != null)
        {
            cooltimeBarFill.fillAmount = 0;
        }
    }

    // 매 프레임마다 호출되어 쿨타임 상태를 업데이트합니다.
    private void Update()
    {
        if (isCooldown)
        {
            currentCooldownTime += Time.deltaTime;

            cooltimeBarFill.fillAmount = currentCooldownTime / maxCooldownTime;

            if (currentCooldownTime >= maxCooldownTime)
            {
                isCooldown = false;
                currentCooldownTime = 0;
                cooltimeBarFill.fillAmount = 0;

                //유닛 생산 코드 작성 요망
                //SpawnUnit();

                Debug.Log("쿨타임 종료! 다시 사용 가능합니다.");
            }
        }
    }

    // 각 버튼의 OnClick 이벤트에 연결될 함수입니다.
    public void StartCooldown(float buttonCooldownTime)
    {
        if (!isCooldown)
        {
            Debug.Log($"쿨타임 시작: {buttonCooldownTime}초");
            isCooldown = true;
            maxCooldownTime = buttonCooldownTime;
            currentCooldownTime = 0;
            cooltimeBarFill.fillAmount = 0;
        }
        else
        {
            Debug.Log("아직 쿨타임 중입니다.");
        }
    }
}