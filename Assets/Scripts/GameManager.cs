using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private InterstitialAd _interstitialAd;
    private Action _onAdClosedCallback;

    // 테스트용
    //private string _adUnitId = "ca-app-pub-3940256099942544/1033173712";

    // 실제 적용 키
    private string _adUnitId = "";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });
    }

    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Application.targetFrameRate = 60;
        LoadInterstitialAd();
    }

    public void LoadInterstitialAd()
    {
        // 이전에 로드된 광고가 있는지 확인하고 있다면 제거하고 해제한다.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        // 새로 광고를 로드하기위한 요청을 생성한다.
        var adRequest = new AdRequest();

        // 광고단위 ID _adUnitId와 adRequest 객체를 전달받아 광고를 로드한다.
        InterstitialAd.Load(_adUnitId, adRequest,
        (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("interstitial ad failed to load an ad " +
                                       "with error : " + error);
                return;
            }

            Debug.Log("Interstitial ad loaded with response : "
                    + ad.GetResponseInfo());

            _interstitialAd = ad;
            // 성공한 경우 로드된 광고에 대한 이벤트 핸들러를 등록한다.
            RegisterEventHandlers(_interstitialAd);
        });
    }

    // 이벤트 핸들러 구현
    private void RegisterEventHandlers(InterstitialAd ad)
    {
        // 1. 광고가 정상적으로 닫혔을 때
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("광고가 닫혔습니다.");

            // 메인 스레드에서 실행 (안전장치)
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                // 저장해둔 행동(씬 이동)이 있다면 실행!
                if (_onAdClosedCallback != null)
                {
                    _onAdClosedCallback.Invoke();
                    _onAdClosedCallback = null; // 실행했으니 비우기
                }

                // 다음 판을 위해 새 광고 로드
                LoadInterstitialAd();
            });
        };

        // 2. 광고 열기 실패했을 때 (인터넷 끊김 등)
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("광고 열기 실패: " + error);

            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                // 실패했어도 게임은 계속 진행되어야 함!
                if (_onAdClosedCallback != null)
                {
                    _onAdClosedCallback.Invoke();
                    _onAdClosedCallback = null;
                }

                LoadInterstitialAd();
            });
        };
    }

    // 광고 화면 표시 함수 (게임 오버 등의 상황에서 호출)
    public void ShowInterstitialAd(Action onClosed)
    {
        // 끝나고 할 일을 변수에 저장해둡니다.
        _onAdClosedCallback = onClosed;

        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.Show();
        }
        else
        {
            Debug.Log("광고가 준비되지 않았습니다. 바로 행동을 실행합니다.");
            // 광고가 없으면 기다리지 않고 바로 실행!
            onClosed.Invoke();
            _onAdClosedCallback = null;
        }
    }

    public void StartGame()
    {
        Debug.Log("게임 시작!");
    }

    public void EndGame()
    {
        Debug.Log("게임 종료");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }
}
