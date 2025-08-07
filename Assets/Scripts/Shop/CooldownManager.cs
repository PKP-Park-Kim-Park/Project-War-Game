using UnityEngine;
using UnityEngine.UI;

public class CooldownManager : MonoBehaviour
{
    public Image cooltimeBarFill;

    private float currentCooldownTime = 0f;
    private float maxCooldownTime = 0f; // 버튼 클릭 시 설정될 쿨타임 시간
    private bool isCooldown = false;

    void Start()
    {
        if (cooltimeBarFill != null)
        {
            cooltimeBarFill.fillAmount = 0;
        }
    }

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

                //SpawnUnit();

                Debug.Log("쿨타임 종료! 다시 사용 가능합니다.");
            }
        }
    }

    // isCooldown 상태를 반환하는 함수
    public bool IsOnCooldown()
    {
        return isCooldown;
    }

    // 이 함수를 버튼의 OnClick() 이벤트에 연결하고, 쿨타임 시간을 매개변수로 입력합니다.
    public void StartCooldown(float buttonCooldownTime)
    {
        if (!isCooldown)
        {
            Debug.Log($"쿨타임 시작: {buttonCooldownTime}초");
            isCooldown = true;
            maxCooldownTime = buttonCooldownTime; // 버튼에서 받은 쿨타임 시간으로 설정
            currentCooldownTime = 0;
            cooltimeBarFill.fillAmount = 0;
        }
        else
        {
            Debug.Log("아직 쿨타임 중입니다.");
        }
    }
}







