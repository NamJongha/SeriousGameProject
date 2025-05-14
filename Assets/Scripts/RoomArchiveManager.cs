using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class RoomArchiveManager : MonoBehaviour
{
    [SerializeField]
    private RoomPrefabSO roomDataSO;
    [SerializeField] private GameObject archiveParent;
    [SerializeField] private Camera camera2;
    [SerializeField] private Camera2MoveController moveController;
    [SerializeField] private GameObject RoomInteractable;
    private List<string> answerList;
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
        answerList = new List<string>();
        roomPos = Vector3.zero;
        if(camera2!=null)
            camera2Pos = camera2.transform.position;
        else camera2Pos = new Vector3(18.4f,15.74f,18.4f); //set default position

        if(RoomInteractable==null)
            Debug.Log("error!");
    }

    public void saveRoom(GameObject obj, string answer1)
    {
        GameObject archiveRoom = Instantiate(obj, archiveParent.transform);
        RoomData room = new RoomData
        {
            //id�� 0���� ���� ������ 1�� ����
            id = GetNextId(),
            answer = answer1,
            room = archiveRoom
        };
        roomDataSO.add(room);
        archiveRoom.transform.localPosition = roomPos;
        archiveRoom.name = "room number" + currentMaxId;
        GameObject roomInteractable = Instantiate(RoomInteractable,archiveRoom.transform);
        
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
            moveController.maxX += width*0.7f;
        }
    }

    public int GetNextId()
    {
        currentMaxId++;
        return currentMaxId;
    }
}
