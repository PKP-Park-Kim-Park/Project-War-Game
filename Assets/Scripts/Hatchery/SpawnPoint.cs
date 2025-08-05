using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawning Settings")]
    [Tooltip("생성할 유닛의 프리팹")]
    public GameObject unitPrefab;

    [Tooltip("유닛이 생성될 영역의 크기")]
    public Transform spawnPosition;

    private void Update()
    {
        // 테스트용: 스페이스바를 누르면 유닛이 생성됩니다.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnUnit();
        }
    }

    // 지정된 스폰 영역 내 무작위 위치에 유닛을 생성
    public void SpawnUnit()
    {
        if (unitPrefab == null)
        {
            Debug.LogError("유닛이 프리팹에 할당되지 않음...", this.gameObject);
            return;
        }

        if (spawnPosition == null)
        {
            Debug.LogError("Spawn Position이 SpawnPoint에 할당되지 않음...", this.gameObject);
            return;
        }

        // 유닛 생성
        Instantiate(unitPrefab, spawnPosition.position, Quaternion.identity);
        Debug.Log($"유닛이 {spawnPosition.position}에 생성", this.gameObject);
    }

    /// <summary>
    /// 씬 뷰에서 스폰 지점 초록색으로 표시
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (spawnPosition == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(spawnPosition.position, 0.5f);
    }
}
