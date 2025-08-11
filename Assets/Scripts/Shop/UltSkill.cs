using UnityEngine;
using System.Collections;

public class UltSkill : MonoBehaviour
{
    public GameObject skillEffectPrefab; // 이펙트 게임오브젝트를 여기에 드래그해서 넣어주세요.
    public float skillDamage = 20f;
    public float skillCooldown = 30f;
    private float nextAvailableTime;

    public void OnUltSkillButton()
    {
        // 쿨다운 시간이 지났을 때만 코루틴을 시작합니다.
        if (Time.time >= nextAvailableTime)
        {
            StartCoroutine(UseSkillCoroutine());
        }
    }

    // 코루틴 함수는 IEnumerator 타입을 반환해야 합니다.
    IEnumerator UseSkillCoroutine()
    {
        // 1. 쿨다운 시간 설정
        nextAvailableTime = Time.time + skillCooldown;

        // 2. 스킬 이펙트 생성
        GameObject effectInstance = Instantiate(skillEffectPrefab, transform.position, Quaternion.identity);

        // 3. 이펙트를 0.75초 후에 자동으로 파괴합니다.
        Destroy(effectInstance, 0.75f);

        // 4. 여기서 0.75초 동안 기다립니다.
        yield return new WaitForSeconds(0.75f);

        // 5. 0.75초 후에 데미지를 적용합니다.
        DealDamageToEnemies();
    }

    void DealDamageToEnemies()
    {
        // 이전 예시와 동일한 데미지 처리 로직
        Debug.Log("적이 " + skillDamage + "만큼의 데미지를 입었습니다!");
        // 실제 게임에서는 Physics.OverlapSphere 등을 사용해 적을 찾아 데미지를 줘야 합니다.
    }
}