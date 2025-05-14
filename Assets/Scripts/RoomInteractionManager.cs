using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomFocus : MonoBehaviour
{
    [SerializeField] private GameObject ArchiveManager; // 방들이 배치된 부모 오브젝트
    [SerializeField] private GameObject camera2;
    private Transform roomFocusPoint; // 카메라 전방 중앙에 방이 이동할 위치
    [SerializeField] private GameObject infoPanel; // 방 정보를 보여줄 UI 패널
    [SerializeField] private TMP_Text answerText; // 방의 답변을 보여줄 UI 텍스트
    [SerializeField] private RoomPrefabSO roomDatas;

    private Camera mainCamera;
    private GameObject selectedRoom;
    private Vector3 originalPosition;
    private bool isFocused = false;
    private LightController lightController = null;

    private void Start()
    {
        mainCamera = camera2.GetComponent<Camera>();
        infoPanel.SetActive(false);
        roomFocusPoint = camera2.transform;
    }

    private void Update()
    {
        if (isFocused) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                if (hit.collider.gameObject.CompareTag("ArchiveRoom"))
                {
                    selectedRoom = hit.collider.gameObject.transform.parent.gameObject;
                    FocusOnRoom(selectedRoom);
                }
                else Debug.Log("Tag error!");
            }
        }
    }

    public void FocusOnRoom(GameObject room)
    {
        lightController = room.transform.GetChild(0).gameObject.GetComponent<LightController>();
        if(lightController!=null)
            lightController.SetLight(3f); // 방의 크기가 커짐에 따라 조명의 강도 역시 증가해야 함.

        if (isFocused) return;
        RoomData rd = roomDatas.FindDataByObject(room);

        if(rd!=null)
        {
            originalPosition = room.transform.position;
            room.transform.position = roomFocusPoint.position+roomFocusPoint.forward*10;
            room.transform.localScale = room.transform.localScale*2;
            room.transform.SetParent(roomFocusPoint); // 카메라 고정 위치로 따라오게 하기

            answerText.text = rd.answer;
            infoPanel.SetActive(true);
            isFocused = true;
        }
        else Debug.Log("data error");
    }

    public void CloseRoomView()
    {
        if (!isFocused || selectedRoom == null) return;

        selectedRoom.transform.SetParent(ArchiveManager.transform); // 원래 부모로 복귀
        selectedRoom.transform.position = originalPosition;
        selectedRoom.transform.localScale = selectedRoom.transform.localScale*0.5f;
        lightController.SetLight(1/3f); //조명 리셋
        lightController = null;

        selectedRoom = null;
        infoPanel.SetActive(false);
        isFocused = false;
    }
}
