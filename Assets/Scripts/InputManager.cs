using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action OnClicked, OnExit, OnRotate;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //버튼이 눌렸을 때 이벤트 리스너가 동작했다면 정상적으로 실행됨
            OnClicked?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            OnRotate?.Invoke();
        }
    }

    public bool IsPointerOverUi()
    {
        //마우스 포인터가 UI Object 위에 있다면 true 반환
        return EventSystem.current.IsPointerOverGameObject();
    }

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane; // 카메라에 렌더링되지 않은 오브젝트 선택 불가능하게 하기 위함
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}
