using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoomPrefabSO : ScriptableObject
{
    public List<RoomData> rooms;

    public void OnEnable()
    {
        rooms.Clear();
    }

    public void add(RoomData room)
    {
        rooms.Add(room);
    }

    public RoomData FindDataByObject(GameObject go)
    {
        foreach(RoomData data in rooms)
        {
            if(data.room==go)
                return data;
        }
        return null;
    }
}

[Serializable]
public class RoomData
{
    [field: SerializeField]
    public int id { get; set; }
    [field: SerializeField]
    public string answer {get; set;}
    [field: SerializeField]
    public GameObject room {get; set;}
}