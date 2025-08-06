using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 터렛 스팟 건설을 관리하는 클래스
/// </summary>
public class TurretSpotBuilder : MonoBehaviour
{
    [Header("Turret Spot Settings")]
    [Tooltip("터렛 스팟 프리팹")]
    public GameObject turretSpotPrefab;

    [Tooltip("생성된 터렛 스팟의 부모 오브젝트")]
    public Transform spotParent;

    [Tooltip("첫 번째 터렛 스팟이 생성될 위치 (spotParent 기준 로컬 좌표)")]
    public Vector3 initialSpotPosition = Vector3.zero;

    private List<GameObject> _turretSpots = new List<GameObject>();
    private const int MAX_TURRET_SPOTS = 4;

    void Start()
    {
        // 게임 시작 시 첫 번째 터렛 스팟을 지정된 위치에 생성
        if (turretSpotPrefab == null)
        {
            Debug.LogError("터렛 스팟 프리팹이 할당되지 않았습니다. 초기 스팟을 생성할 수 없습니다.", this);
            return;
        }

        // spotParent가 지정되지 않았다면, 이 컴포넌트의 Transform을 부모로 사용합니다.
        Transform parent = spotParent != null ? spotParent : this.transform;
        if (spotParent == null)
        {
            Debug.LogWarning($"'Spot Parent'가 지정되지 않아 이 오브젝트({this.name})를 부모로 사용합니다.", this);
        }

        GameObject initialSpot = Instantiate(turretSpotPrefab, parent.TransformPoint(initialSpotPosition), parent.rotation, parent);
        _turretSpots.Add(initialSpot);
        Debug.Log($"<color=cyan>초기 터렛 스팟 '{initialSpot.name}' 생성 완료.</color>", initialSpot);
    }

    void Update()
    {
        // 테스트용: T 키로 스팟 건설
        if (Input.GetKeyDown(KeyCode.T))
        {
            BuildTurretSpot();
        }
    }

    /// <summary>
    /// 터렛 스팟을 건설
    /// </summary>
    public void BuildTurretSpot()
    {

        if (turretSpotPrefab == null)
        {
            Debug.LogError("터렛 스팟 프리팹 할당X", this);
            return;
        }

        if (_turretSpots.Count == 0)
        {
            Debug.LogError("터렛 스팟 부모 지정 필요..", this);
            return;
        }

        if (_turretSpots.Count >= MAX_TURRET_SPOTS)
        {
            Debug.Log("터렛 스팟이 최대 개수에 도달.. (최대: " + MAX_TURRET_SPOTS + "개)", this);
            return;
        }

        // 마지막 스팟 위에 다음 스팟 건설
        GameObject lastSpot = _turretSpots[_turretSpots.Count - 1];

        Transform nextSpotTransform = lastSpot.transform.Find("NextSpotPosition");

        if (nextSpotTransform == null)
        {
            Debug.LogError("'NextSpotPosition' 다음 위치 찾기 실패..", lastSpot);
            return;
        }

        Vector3 spawnPosition = nextSpotTransform.position;

        GameObject newSpot = Instantiate(turretSpotPrefab, spawnPosition, Quaternion.identity, spotParent);
        _turretSpots.Add(newSpot);
        Debug.Log($"{_turretSpots.Count}번째 터렛 스팟 생성 완료.", newSpot);
    }
}
