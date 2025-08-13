using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 적 AI 터렛 건설 관리
/// </summary>
public class EnemyAIController : MonoBehaviour
{
    [Header("터렛 건설 전략(순서)")]
    [Tooltip("AI는 이 순서대로 터렛을 건설함..")]
    public List<GameObject> turretBuildSequence;

    [Tooltip("AI가 건설할 최대 터렛 수")]
    public int maxTurrets = 2;

    [Header("Timing")]
    [Tooltip("터렛 건설을 시도하는 주기 (초)")]
    public float buildTime = 30f;

    [Tooltip("AI가 터렛 스팟을 건설하는 주기 (초). 이 값은 TurretSpotBuilder로 전달됩니다.")]
    public float spotBuildInterval = 10f;

    [Tooltip("게임 시작 후 첫 터렛 건설까지의 대기 시간 (초)")]
    public float initBuildTime = 10f;

    [Header("터렛 설치 최대 개수")]
    [Tooltip("첫 터렛 건설 후 밑의 시간이 지나면 최대 터렛 수 1 증가")]
    public float increaseMaxTurretTime = 480f; // 8 * 60 = 480 seconds

    private Queue<EnemyTurretSlot> turretQueue = new Queue<EnemyTurretSlot>();
    private bool flag = false;
    private Hatchery enemyHatchery;

    IEnumerator Start()
    {
        if (turretBuildSequence == null || turretBuildSequence.Count == 0)
        {
            Debug.LogWarning("AI의 터렛 건설 순서가 비어서 터렛 건설 AI가 비활성화", this);
            yield break;
        }

        // AI 자신의 해처리를 찾습니다.
        GameObject hatcheryObj = GameObject.FindGameObjectWithTag("EnemyHatchery");
        if (hatcheryObj != null)
        {
            enemyHatchery = hatcheryObj.GetComponent<Hatchery>();
        }
        if (enemyHatchery == null) Debug.LogError("AI가 적 해처리를 찾을 수 없어 업그레이드 로직이 작동하지 않습니다.", this);

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
    /// 다음 건설 단계를 실행합니다. 우선순위: 1.구시대 터렛 교체 -> 2.빈 슬롯에 건설 -> 3.가장 오래된 터렛 교체
    /// </summary>
    private void ExecuteNextBuildStep()
    {
        if (enemyHatchery == null) return;

        // 우선순위 1: 현재 시대보다 낮은 시대에 지어진 터렛을 찾아 최신 시대의 같은 터렛으로 교체합니다.
        var occupiedSlots = FindObjectsOfType<EnemyTurretSlot>().Where(s => s.IsOccupied && s.MountedTurret != null).ToList();
        foreach (var slot in occupiedSlots)
        {
            var turretFire = slot.MountedTurret.GetComponent<TurretFire>();
            if (turretFire != null && turretFire.BuiltInAge < enemyHatchery.CurrAge)
            {
                UpgradeTurretForAge(slot);
                return; // 이번 턴의 건설/교체 작업 완료
            }
        }

        // 우선순위 2: 가장 오래된 터렛 중 업그레이드 가능한 터렛을 찾아 다음 등급으로 업그레이드합니다.
        EnemyTurretSlot slotToUpgradeInSequence = FindOldestUpgradableTurret();
        if (slotToUpgradeInSequence != null)
        {
            UpgradeTurretInSequence(slotToUpgradeInSequence);
            return; // 이번 턴의 건설/교체 작업 완료
        }

        // 우선순위 3: 빈 슬롯이 있고, 최대 터렛 수에 도달하지 않았다면 기본 터렛을 신규 건설합니다.
        if (turretQueue.Count < maxTurrets)
        {
            var emptySlots = FindObjectsOfType<EnemyTurretSlot>().Where(slot => !slot.IsOccupied).ToList();
            if (emptySlots.Count > 0)
            {
                BuildNewTurret(emptySlots);
                return;
            }
        }

        Debug.Log("AI: 모든 터렛이 최고 등급이며, 빈 슬롯이 없습니다. 다음 주기에 다시 시도", this);
    }

    private void UpgradeTurretForAge(EnemyTurretSlot slotToUpgrade)
    {
        GameObject currentTurretPrefab = slotToUpgrade.MountedTurretPrefab;
        if (currentTurretPrefab == null)
        {
            Debug.LogWarning($"AI: 시대를 업그레이드하려 했으나 '{slotToUpgrade.gameObject.name}' 슬롯의 터렛 프리팹 정보를 찾을 수 없습니다.", this);
            return;
        }
        Debug.Log($"AI: 구시대 터렛 발견. '{slotToUpgrade.gameObject.name}' 슬롯의 터렛을 최신 시대로 교체합니다.", this);
        slotToUpgrade.DestroyTurret();
        slotToUpgrade.BuildTurretByAI(currentTurretPrefab);
    }

    private EnemyTurretSlot FindOldestUpgradableTurret()
    {
        // 큐를 순회하여 업그레이드 가능한 가장 오래된 터렛을 찾습니다.
        foreach (EnemyTurretSlot slot in turretQueue)
        {
            if (slot == null || !slot.IsOccupied || slot.MountedTurretPrefab == null) continue;

            int currentIndex = turretBuildSequence.IndexOf(slot.MountedTurretPrefab);

            // 터렛이 시퀀스에 있고 최고 등급이 아닌 경우
            if (currentIndex != -1 && currentIndex < turretBuildSequence.Count - 1)
            {
                return slot; // 업그레이드할 가장 오래된 터렛을 찾았습니다.
            }
        }
        return null; // 업그레이드 가능한 터렛이 없습니다.
    }

    private void UpgradeTurretInSequence(EnemyTurretSlot slotToUpgrade)
    {
        int currentIndex = turretBuildSequence.IndexOf(slotToUpgrade.MountedTurretPrefab);
        GameObject nextTurretPrefab = turretBuildSequence[currentIndex + 1];
        Debug.Log($"AI: '{slotToUpgrade.gameObject.name}' 슬롯의 '{slotToUpgrade.MountedTurretPrefab.name}'을(를) '{nextTurretPrefab.name}'(으)로 업그레이드합니다.", this);

        slotToUpgrade.DestroyTurret();
        slotToUpgrade.BuildTurretByAI(nextTurretPrefab);
    }

    private void BuildNewTurret(List<EnemyTurretSlot> emptySlots)
    {
        GameObject turretToBuild = turretBuildSequence[0]; // 신규 건설은 항상 기본 터렛으로
        EnemyTurretSlot slotToBuildIn = emptySlots[Random.Range(0, emptySlots.Count)];

        Debug.Log($"AI: 빈 슬롯 '{slotToBuildIn.gameObject.name}'에 기본 터렛 '{turretToBuild.name}' 신규 건설을 시도합니다.", this);
        slotToBuildIn.BuildTurretByAI(turretToBuild);

        turretQueue.Enqueue(slotToBuildIn);
        HandleFirstBuild();
    }

    private void HandleFirstBuild()
    {
        if (!flag)
        {
            flag = true;
            TurretSpotBuilder spotBuilder = FindObjectOfType<TurretSpotBuilder>();
            if (spotBuilder != null)
            {
                spotBuilder.StartBuildingProcess(spotBuildInterval);
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