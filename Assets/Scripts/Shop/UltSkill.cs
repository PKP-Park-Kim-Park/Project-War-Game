using UnityEngine;
using System.Collections;

public class UltSkill : MonoBehaviour
{
    public GameObject skillEffectPrefab; // 이펙트 게임오브젝트를 여기에 드래그해서 넣어주세요.
    public int skillDamage = 50;
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

        //번쩍하는 이펙트 만들고  지우고

        // 5. 0.75초 후에 데미지를 적용합니다.
        DealDamageToEnemies();
    }

    void DealDamageToEnemies()
    {
        // 1. 플레이어 주변에 있는 적들을 찾기 위한 범위를 설정합니다.
        // 여기서는 플레이어 위치를 중심으로 반경 5f(미터) 안에 있는 모든 Collider들을 찾습니다.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 50f);

        // 2. 찾아낸 Collider들을 하나씩 검사합니다.
        foreach (var hitCollider in hitColliders)
        {
            // 3. 충돌한 오브젝트의 태그가 "Enemy"인지 확인합니다.
            if (hitCollider.CompareTag("Enemy"))
            {
                // 4. "Enemy" 태그를 가진 오브젝트의 Health 스크립트를 가져옵니다.
                // (적 캐릭터에 Health 스크립트가 있다고 가정합니다.)
                UnitController enemyHealth = hitCollider.GetComponent<UnitController>();

                // 5. 만약 Health 스크립트가 존재하면 데미지를 적용합니다.
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(skillDamage);
                    Debug.Log(hitCollider.name + "가 " + skillDamage + "만큼의 데미지를 입었습니다!");
                }
            }
        }
    }
}