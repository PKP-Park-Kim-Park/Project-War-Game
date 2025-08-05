using System.Collections.Generic;
using UnityEngine;


public class TurretSpotBuilder : MonoBehaviour
{
    [Header("Turret Spot Settings")]
    [Tooltip("터렛 스팟 프리팹")]
    public GameObject turretSpotPrefab;

    [Tooltip("첫 번째 터렛 스팟 오브젝")]
    public GameObject initialTurretSpot;

    [Tooltip("생성된 터렛 스팟의 부모 오브젝트")]
    public Transform spotParent;

    private List<GameObject> _turretSpots = new List<GameObject>();
    private const int MAX_TURRET_SPOTS = 4;

    void Start()
    {
        // 미리 배치된 첫 번째 터렛 스팟을 리스트에 추가
        if (initialTurretSpot != null)
        {
            _turretSpots.Add(initialTurretSpot);
        }
        else
        {
            Debug.LogError("초기 터렛 스팟 할당 실패..", this);
        }
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
