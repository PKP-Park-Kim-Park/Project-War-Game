using UnityEngine;

public class CamreController : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] private float _dragSpeed = 0.01f;  
    [SerializeField] private bool _invert = false;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float edgeThreshold = 0.2f; // 화면 비율(20%) 기준

    [Header("Bounds (World Space)")]
    [SerializeField] private Transform _leftEnd;      
    [SerializeField] private Transform _rightEnd;       

    private Vector2 _lastPointerPos;
    private bool _isDragging = false;

    private void Update()
    {
        //HandleMouseDrag();

        HandleTouchDrag(); // 모바일
    }

    private void HandleMouseDrag()
    {
        Vector3 pos = transform.position;

        // 마우스 위치를 0~1 비율로 변환
        float mouseXNormalized = Input.mousePosition.x / Screen.width;

        // 왼쪽 이동
        if (mouseXNormalized < edgeThreshold)
        {
            pos.x -= moveSpeed * Time.deltaTime;
        }
        // 오른쪽 이동
        else if (mouseXNormalized > 1f - edgeThreshold)
        {
            pos.x += moveSpeed * Time.deltaTime;
        }

        // 이동 제한
        pos.x = Mathf.Clamp(pos.x, _leftEnd.localPosition.x, _rightEnd.localPosition.x);

        transform.position = pos;
    }

    private void HandleTouchDrag()
    {
        if (Input.touchCount <= 0)
        {
            _isDragging = false;
            return;
        }

        Touch t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began)
        {
            _lastPointerPos = t.position;
            _isDragging = true;
            return;
        }

        if ((t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary) && _isDragging)
        {
            Vector2 currentPos = t.position;
            Vector2 direction = currentPos - _lastPointerPos;

            float sign = _invert ? 1f : -1f;
            float moveX = sign * direction.x * _dragSpeed;

            Vector3 next = transform.position + new Vector3(moveX, 0f, 0f);

            float minX = (_leftEnd != null) ? _leftEnd.position.x : next.x;
            float maxX = (_rightEnd != null) ? _rightEnd.position.x : next.x;
            if (minX > maxX)
            {
                float tmp = minX;
                minX = maxX;
                maxX = tmp;
            }

            float clampedX = Mathf.Clamp(next.x, minX, maxX);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

            _lastPointerPos = currentPos;
            return;
        }

        if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
        {
            _isDragging = false;
            return;
        }
    }
}
