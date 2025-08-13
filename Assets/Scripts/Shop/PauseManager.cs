using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    // Inspector에서 연결할 Image UI
    public Image pauseImage;

    // 일시정지 상태를 저장하는 변수
    private bool isPaused = false;

    // 버튼 클릭 시 호출될 함수 (게임 일시정지 전용)
    public void PauseGame()
    {
        // 게임이 아직 일시정지되지 않았을 때만 실행
        if (!isPaused)
        {
            Time.timeScale = 0f; // 시간 정지
            pauseImage.gameObject.SetActive(true); // 이미지 활성화

            // 일시정지 상태로 변경
            isPaused = true;
        }
    }

    // 게임 일시정지 해제
    public void ResumeGame()
    {
        // 일시정지 상태일 때만 실행
        if (isPaused)
        {
            Time.timeScale = 1f; // 시간 재생
            pauseImage.gameObject.SetActive(false); // 이미지 비활성화

            // 일시정지 상태 해제
            isPaused = false;
        }
    }

    // 게임 초기화하고 게임시작씬으로 이동
    public void GoToMainMenu()
    {
        // 게임 상태 초기화
        Time.timeScale = 1f; // 게임 시간을 정상 상태로 되돌림

        // 초기 화면 씬 로드
        SceneManager.LoadScene("Title"); // 빌드 설정에서 0번 인덱스에 있는 씬을 로드
    }
}