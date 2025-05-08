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
}

[Serializable]
public class RoomData
{
    [field: SerializeField]
    public int id { get; set; }
}