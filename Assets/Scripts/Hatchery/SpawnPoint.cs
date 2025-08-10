using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawning Settings")]
    [Tooltip("생성할 유닛 프리팹 목록. 1, 2, 3키에 순서대로 매핑")]
    public List<GameObject> unitPrefabs = new List<GameObject>();

    [Tooltip("유닛 생성 영역")]
    public Transform spawnTransform;

    // 지정된 인덱스의 유닛을 스폰 영역에 생성합니다.
    public void SpawnUnit(int unitIndex)
    {
        // 유닛 프리팹 목록이 비어있거나 인덱스가 범위를 벗어나는지 확인합니다.
        if (unitPrefabs == null || unitPrefabs.Count == 0)
        {
            Debug.LogError("유닛 프리팹 목록 없음..", this.gameObject);
            return;
        }

        if (unitIndex < 0 || unitIndex >= unitPrefabs.Count)
        {
            Debug.LogError($"유효하지 않은 유닛 인덱스: {unitIndex}. 프리팹 목록이 비어있음..", this.gameObject);
            return;
        }

        GameObject unitPrefab = unitPrefabs[unitIndex];

        if (unitPrefab == null)
        {
            Debug.LogError($"인덱스 {unitIndex}에 해당하는 유닛 프리팹이 할당X", this.gameObject);
            return;
        }

        if (spawnTransform == null)
        {
            Debug.LogError("Spawn Position이 SpawnPoint에 할당X", this.gameObject);
            return;
        }

        // 유닛 생성
        Instantiate(unitPrefab, spawnTransform.position, Quaternion.identity);
        Debug.Log($"'{unitPrefab.name}' 유닛 생성", this.gameObject);
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
