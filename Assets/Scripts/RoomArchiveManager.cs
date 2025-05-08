using Unity.VisualScripting;
using UnityEngine;

public class RoomArchiveManager : MonoBehaviour
{
    [SerializeField]
    private RoomPrefabSO roomDataSO;
    
    private int currentMaxId = -1;

    public void Awake()
    {
        currentMaxId = -1;
    }

    public void saveRoom(GameObject obj)
    {
        RoomData room = new RoomData
        {
            //id�� 0���� ���� ������ 1�� ����
            id = GetNextId()
        };

        roomDataSO.add(room);
    }

    public int GetNextId()
    {
        currentMaxId++;
        return currentMaxId;
    }

    public void loadRoom()
    {

    }
}
