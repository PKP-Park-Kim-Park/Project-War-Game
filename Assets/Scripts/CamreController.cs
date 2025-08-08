using UnityEngine;

public class CamreController : MonoBehaviour
{
    [SerializeField] private float _dragSpeed = 0.01f;
    [SerializeField] private Transform _leftEnd;
    [SerializeField] private Transform _rightEnd;

    private Vector2 _lastTouchPos;
    private bool _isDragging = false;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _lastTouchPos = touch.position;
                _isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && _isDragging)
            {
                Vector2 currentPos = touch.position;
                Vector2 diraction = currentPos - _lastTouchPos;

                float moveX = -diraction.x * _dragSpeed;

                Vector3 nextPosition = transform.position + new Vector3(moveX, 0f, 0f);

                float clampedX = Mathf.Clamp(nextPosition.x, _leftEnd.localPosition.x, _rightEnd.localPosition.x);

                transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

                _lastTouchPos = currentPos;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                _isDragging = false;
            }
        }
    }
}
