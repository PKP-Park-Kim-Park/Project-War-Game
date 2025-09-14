using System.Collections.Generic;
using System.Collections;
using System;
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

    [Tooltip("건설할 수 있는 최대 터렛 스팟의 수")]
    [SerializeField] private int maxTurretSpots = 4;

    // 터렛 스팟이 생성될 때 발생하는 이벤트. TurretManager가 이를 구독함.
    public static event Action<TurretSlot> OnTurretSpotBuilt;

    private List<GameObject> _turretSpots = new List<GameObject>();
    private float _buildInterval = 10f; // AI 컨트롤러로부터 값을 받기 전의 기본값
    private bool _isBuildingProcessStarted = false;


    void Start()
    {
        // 게임 시작 시 첫 번째 터렛 스팟을 지정된 위치에 생성
        if (turretSpotPrefab == null)
        {
            Debug.LogError("터렛 스팟 프리팹이 할당되지 않았습니다. 초기 스팟을 생성할 수 없습니다.", this);
            return;
        }

        Transform parent = spotParent != null ? spotParent : this.transform;
        if (spotParent == null)
        {
            Debug.LogWarning($"'Spot Parent'가 지정되지 않아 이 오브젝트({this.name})를 부모로 사용합니다.", this);
        }

        GameObject initialSpot = Instantiate(turretSpotPrefab, parent.TransformPoint(initialSpotPosition), parent.rotation, parent);
        _turretSpots.Add(initialSpot);

        // 초기 스팟에 대한 이벤트도 발생시켜 TurretManager가 등록할 수 있도록 함.
        TurretSlot initialSlot = initialSpot.GetComponentInChildren<TurretSlot>();
        if (initialSlot != null)
        {
            OnTurretSpotBuilt?.Invoke(initialSlot);
            Debug.Log($"<color=cyan>초기 터렛 스팟 '{initialSpot.name}' 생성 및 등록 완료.</color>", initialSpot);
        }
    }

    /// <summary>
    /// 적 터렛 설치 AI에서 사용
    /// </summary>
    public void StartBuildingProcess(float intervalFromAI)
    {
        if (_isBuildingProcessStarted) return;
        _isBuildingProcessStarted = true;
        _buildInterval = intervalFromAI;

        Debug.Log($"첫 터렛 건설 후 {_buildInterval}초마다 터렛 스팟 건설을 시작합니다.");
        StartCoroutine(BuildSpotsOverTime());
    }

    /// <summary>
    /// 일정 시간(1분)마다 터렛 스팟을 추가로 건설
    /// </summary>
    private IEnumerator BuildSpotsOverTime()
    {
        // 최대 스팟 수에 도달할 때까지 반복합니다.
        while (_turretSpots.Count < maxTurretSpots)
        {
            yield return new WaitForSeconds(_buildInterval);

            // BuildTurretSpot()은 내부적으로 최대 개수 체크를 하므로 여기서 호출만 하면 됩니다.
            BuildTurretSpot();
        }
        Debug.Log("최대 터렛 스팟 수에 도달하여 추가 건설을 중단합니다.");
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

        if (_turretSpots.Count >= maxTurretSpots)
        {
            Debug.Log("터렛 스팟이 최대 개수에 도달.. (최대: " + maxTurretSpots + "개)", this);
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

        // 새로 생성된 터렛 스팟의 TurretSlot을 이벤트로 알림.
        TurretSlot newSlot = newSpot.GetComponentInChildren<TurretSlot>();
        OnTurretSpotBuilt?.Invoke(newSlot);

        Debug.Log($"{_turretSpots.Count}번째 터렛 스팟 생성 완료.", newSpot);
    }
}
