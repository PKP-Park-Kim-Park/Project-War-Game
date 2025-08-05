using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawning Settings")]
    [Tooltip("생성할 유닛 프리팹")]
    public GameObject unitPrefab;

    [Tooltip("유닛 생성 영역")]
    public Transform spawnTransform;

    private void Update()
    {
        // 테스트용: 스페이스바를 누르면 유닛이 생성됩니다.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnUnit();
        }
    }

    // 스폰 영역에 유닛 생성
    public void SpawnUnit()
    {
        if (unitPrefab == null)
        {
            Debug.LogError("유닛이 프리팹에 할당되지 않음...", this.gameObject);
            return;
        }

        if (spawnTransform == null)
        {
            Debug.LogError("Spawn Position이 SpawnPoint에 할당되지 않음...", this.gameObject);
            return;
        }

        // 유닛 생성
        Instantiate(unitPrefab, spawnTransform.position, Quaternion.identity);
        Debug.Log($"유닛이 {spawnTransform.position}에 생성", this.gameObject);
    }

    /// <summary>
    /// 씬 뷰에서 스폰 지점 초록색으로 표시
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (spawnTransform == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(spawnTransform.position, 0.5f);
    }
}
