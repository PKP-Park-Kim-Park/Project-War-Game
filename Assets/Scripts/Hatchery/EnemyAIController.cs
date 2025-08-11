using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 적 AI 터렛 건설 관리
/// </summary>
public class EnemyAIController : MonoBehaviour
{
    [Header("Turret Building Strategy")]
    [Tooltip("AI는 이 순서대로 터렛을 건설함..")]
    public List<GameObject> turretBuildSequence;

    [Tooltip("AI가 건설할 최대 터렛 수")]
    public int maxTurrets = 2;

    [Header("Timing")]
    [Tooltip("터렛 건설을 시도하는 주기 (초)")]
    public float buildTime = 30f;

    [Tooltip("게임 시작 후 첫 터렛 건설까지의 대기 시간 (초)")]
    public float initBuildTime = 10f;

    [Header("Turret Capacity Upgrade")]
    [Tooltip("첫 터렛 건설 후 밑의 시간이 지나면 최대 터렛 수 1 증가")]
    public float increaseMaxTurretTime = 480f; // 8 * 60 = 480 seconds

    private Queue<EnemyTurretSlot> turretQueue = new Queue<EnemyTurretSlot>();
    private int turretSeq = 0;
    private bool flag = false;

    IEnumerator Start()
    {
        if (turretBuildSequence == null || turretBuildSequence.Count == 0)
        {
            Debug.LogWarning("AI의 터렛 건설 순서가 비어서 터렛 건설 AI가 비활성화", this);
            yield break;
        }

        yield return new WaitForSeconds(initBuildTime);

        // 주기적으로 터렛 건설/교체를 시도
        while (true)
        {
            ExecuteNextBuildStep();
            // 건설 주기만큼 대기
            yield return new WaitForSeconds(buildTime);
        }
    }

    /// <summary>
    /// `turretBuildSequence` 순서대로 EnemyTurretSlot 컴포넌트를 가진 오브젝트에만 설치
    /// 건설된 터렛 수가 최대치(현재 기본값 2개)에 도달하면 가장 오래된 터렛을 파괴, 재건설
    /// </summary>
    private void ExecuteNextBuildStep()
    {
        // 1. 최대 건설 수에 도달하면 가장 오래된 터렛을 제거
        if (turretQueue.Count >= maxTurrets)
        {
            EnemyTurretSlot oldestSlot = turretQueue.Dequeue();
            if (oldestSlot != null)
            {
                Debug.Log($"AI: 최대 터렛 수({maxTurrets})에 도달하여 가장 오래된 터렛이 설치된 '{oldestSlot.gameObject.name}' 슬롯의 터렛을 파괴", this);
                oldestSlot.DestroyTurret();
            }
        }

        // 2. 씬에 있는 모든 빈 적 터렛 슬롯을 찾기
        var emptySlots = FindObjectsOfType<EnemyTurretSlot>()
            .Where(slot => !slot.IsOccupied)
            .ToList();

        if (emptySlots.Count == 0)
        {
            Debug.Log("AI: 건설 가능한 빈 터렛 슬롯이 없습니다. 다음 주기에 다시 시도", this);
            return;
        }

        // 3. 건설할 터렛을 turretBuildSequence에서 순서 가져옴
        // 큐에서 순환
        if (turretSeq >= turretBuildSequence.Count)
        {
            turretSeq = 0; // 처음부터 다시 시작
        }
        GameObject turretToBuild = turretBuildSequence[turretSeq];

        // 4. 사용 가능한 슬롯 중 하나를 무작위로 선택
        EnemyTurretSlot slotToBuildIn = emptySlots[Random.Range(0, emptySlots.Count)];

        // 5. 터렛 건설
        string turretNameToBuild = turretToBuild != null ? turretToBuild.name : "무작위 터렛";
        Debug.Log($"AI가 동적으로 찾은 빈 슬롯 '{slotToBuildIn.gameObject.name}'에 순서에 따른 '{turretNameToBuild}' 건설을 시도", this);
        slotToBuildIn.BuildTurretByAI(turretToBuild);

        // 6. 건설된 터렛 정보를 큐에 추가 다음 인덱스 증가
        turretQueue.Enqueue(slotToBuildIn);
        turretSeq++;

        // 7. 첫 터렛 건설 시, 필요한 프로세스들을 시작합니다.
        if (!flag)
        {
            flag = true;

            // 터렛 스팟 자동 건설 프로세스 시작
            TurretSpotBuilder spotBuilder = FindObjectOfType<TurretSpotBuilder>();
            if (spotBuilder != null)
            {
                spotBuilder.StartBuildingProcess();
                Debug.Log("AI가 첫 터렛을 건설하여 터렛 스팟 자동 건설 시작", this);
            }
            else
            {
                Debug.LogWarning("씬에서 TurretSpotBuilder를 못 찾겠음..", this);
            }

            // 최대 터렛 수 증가 타이머 시작
            if (increaseMaxTurretTime > 0)
            {
                StartCoroutine(IncreaseMaxTurretsAfterDelay());
            }
        }
    }

    /// <summary>
    /// 지정된 시간(8분)이 지나면 최대 터렛 수를 1 증가시킵니다.
    /// </summary>
    private IEnumerator IncreaseMaxTurretsAfterDelay()
    {
        Debug.Log($"첫 터렛 건설. {increaseMaxTurretTime}초 후에 최대 터렛 수가 1 증가합니다.", this);
        yield return new WaitForSeconds(increaseMaxTurretTime);
        maxTurrets++;
        Debug.Log($"시간이 경과하여 AI의 최대 터렛 수가 {maxTurrets}(으)로 증가했습니다.", this);
    }
}