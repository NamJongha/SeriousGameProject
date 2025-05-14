using UnityEngine;
using UnityEngine.UI;

public class Camera2MoveController : MonoBehaviour
{
[Header("Camera Movement")]
    [SerializeField] private Camera camera2;
    public float minX = 0f;              // 카메라 이동 최소 X 값
    public float maxX = 1f;             // 카메라 이동 최대 X 값 (방 개수에 따라 동적으로 설정됨)

    [Header("UI Scrollbar")]
    public Scrollbar scrollbar;

    private Vector3 camera2OriginPos;

    private void Start()
    {
        scrollbar.onValueChanged.AddListener(OnScrollChanged);
        camera2OriginPos = camera2.transform.position;
    }

    private void OnScrollChanged(float value)
    {
        // 스크롤 값은 0 ~ 1 사이의 값이므로, 이를 minX ~ maxX 범위로 변환
        float targetX = Mathf.Lerp(minX, maxX, value);
        Vector3 newPos = camera2OriginPos + new Vector3(targetX, 0, -targetX);
        camera2.transform.position = newPos;
    }
}
