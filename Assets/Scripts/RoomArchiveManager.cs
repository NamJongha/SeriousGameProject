using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomArchiveManager : MonoBehaviour
{
    [SerializeField]
    private RoomPrefabSO roomDataSO;
    [SerializeField] private GameObject archiveParent;
    [SerializeField] private Camera camera2;
    private List<GameObject> roomList;
    private Vector3 roomPos;
    private Vector3 camera2Pos;
    
    private int currentMaxId = -1;

    private float width = 5.5f;
    private float heigth = 2.75f;

    public void Awake()
    {
        currentMaxId = -1;
    }

    public void Start()
    {
        roomList = new List<GameObject>();
        roomPos = Vector3.zero;
        camera2Pos = camera2.transform.position;
    }

    public void saveRoom(GameObject obj, String answer1)
    {
        RoomData room = new RoomData
        {
            //id�� 0���� ���� ������ 1�� ����
            id = GetNextId()
        };
        roomDataSO.add(room);
        
        GameObject archiveRoom = Instantiate(obj, archiveParent.transform);
        archiveRoom.transform.localPosition = roomPos;
        archiveRoom.name = "room number"+currentMaxId;
        roomList.Add(archiveRoom);
        MovePos();
    }

    private void MovePos()
    {
        if(currentMaxId%3==0)
            roomPos += new Vector3(width,heigth,0);
        if(currentMaxId%3==1)
            roomPos += new Vector3(-width,-2*heigth,-width);
        if(currentMaxId%3==2)
        {
            roomPos += new Vector3(width,heigth,0);
            camera2Pos += new Vector3(width/2,0,-width/2);
            MoveCamera();
        }
            
    }

    public int GetNextId()
    {
        currentMaxId++;
        return currentMaxId;
    }

    private void MoveCamera()
    {
        if(camera2.orthographicSize<15)
        {
            camera2.transform.position = camera2Pos;
            camera2.orthographicSize+=1;
        }
    }

    public void loadRoom()
    {

    }
}
