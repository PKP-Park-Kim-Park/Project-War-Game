using UnityEngine;

/// <summary>
/// 애니메이션이 끝났을 때 호출되어 스스로를 파괴하는 스크립트.
/// </summary>
public class DestroyOnAnimation : MonoBehaviour
{
    // 이 함수를 애니메이션의 마지막 프레임에 '애니메이션 이벤트'로 추가하여 호출합니다.
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
