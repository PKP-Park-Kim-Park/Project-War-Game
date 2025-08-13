using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 이 스크립트는 씬의 다른 모든 스크립트가 임시 메시지를 표시하는 데 사용하는 단일 시스템입니다.
public class MessageManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static MessageManager Instance { get; private set; }

    [Tooltip("메시지를 표시할 UI Text 컴포넌트를 여기에 연결하세요.")]
    public Text messageText;

    private Coroutine currentMessageCoroutine;

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 필요에 따라 씬 전환 시 오브젝트 유지
        }
        else
        {
            Destroy(gameObject);
        }

        // 초기 상태: 텍스트 숨기기
        if (messageText != null)
        {
            messageText.enabled = false;
        }
    }

    /// <summary>
    /// UI에 임시 메시지를 표시합니다.
    /// 기존 메시지가 있다면 즉시 멈추고 새 메시지를 표시합니다.
    /// </summary>
    /// <param name="message">표시할 메시지 문자열.</param>
    /// <param name="duration">메시지가 표시될 시간(초).</param>
    /// <param name="color">메시지 텍스트의 색상.</param>
    public void ShowTemporaryText(string message, float duration = 1.0f, Color? color = null)
    {
        if (messageText == null)
        {
            Debug.LogError("MessageManager: messageText가 연결되지 않았습니다.");
            return;
        }

        // 기존 코루틴이 실행 중이라면 중지
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
        }

        // 메시지 및 색상 설정
        messageText.text = message;
        messageText.color = color ?? Color.white; // 색상이 지정되지 않으면 흰색으로 설정

        messageText.enabled = true;

        // 새 메시지 표시 코루틴 시작
        currentMessageCoroutine = StartCoroutine(HideTextAfterTime(duration));
    }

    private IEnumerator HideTextAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);

        messageText.enabled = false;
        currentMessageCoroutine = null;
    }
}
