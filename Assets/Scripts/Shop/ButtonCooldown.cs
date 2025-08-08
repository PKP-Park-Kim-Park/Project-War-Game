using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonCooldown : MonoBehaviour
{
    // 쿨타임 바 UI (Fill Image)
    // 이 스크립트가 부착된 버튼의 자식으로 설정하는 것이 좋습니다.
    public Image cooltimeBarFill;

    // 이 버튼의 쿨타임 시간. 에디터에서 설정 가능합니다.
    public float cooldownTime = 10f;

    private float currentCooldownTime = 0f;
    private bool isCooldown = false;

    void Start()
    {
        // 게임 시작 시, 쿨타임 바를 0으로 초기화하여 비어있는 상태로 만듭니다.
        if (cooltimeBarFill != null)
        {
            cooltimeBarFill.fillAmount = 1;
        }
    }

    void Update()
    {
        if (isCooldown)
        {
            currentCooldownTime += Time.deltaTime;
            cooltimeBarFill.fillAmount = currentCooldownTime / cooldownTime;

            if (currentCooldownTime >= cooldownTime)
            {
                isCooldown = false;
                currentCooldownTime = 0;
                cooltimeBarFill.fillAmount = 1;

                Debug.Log(gameObject.name + "의 쿨타임이 끝났습니다.");
            }
        }
    }

    // 이 함수를 버튼의 On Click() 이벤트에 연결합니다.
    public void OnButtonClick()
    {
        if (!isCooldown)
        {
            // 궁극기 스킬 나가는 함수 구현해야됨.
            //onSkillUsed.Invoke();

            isCooldown = true;
            currentCooldownTime = 0;
            cooltimeBarFill.fillAmount = 1;

            Debug.Log(gameObject.name + "의 궁극기 쿨타임이 " + cooldownTime + "초 동안 시작됩니다.");
        }
        else
        {
            Debug.Log(gameObject.name + "은(는) 아직 쿨타임 중입니다.");
        }
    }
}
