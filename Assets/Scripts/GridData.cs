//배치한 가구에 대한 정보를 저장하기 위한 객체
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class GridData
{
    //Vector3Int: 그리드의 위치와 오브젝트 저장을 위해 선언
    //new() : 자동적으로 객체 생성(전체를 정의하는 것을 생략) new Dictinary<>()와 같은 맥락
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    private int minX, minZ, maxX, maxZ;

    public void SetGridBounds(int minX, int maxX, int minZ, int maxZ)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minZ = minZ;
        this.maxZ = maxZ;
    }

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
    {
        //물체가 놓일 위치를 계산하여 이를 저장 Vector3Int인 이유는 x, y, z의 위치 3가지를 고려하기 때문이며
        //List로 선언한 이유는 하나의 물체가 여러개의 셀(그리드 칸)를 차지할 수 있기 때문
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);

        //가구를 놓고자 하는 위치에 이미 다른 가구가 있는지 확인
        foreach(var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                Debug.Log($"Dictinary already contains this cell position {pos}");
                break;
            }
            //위치가 겹치지 않으면 pos 위치에 해당하는 부분에 data를 넣어 물체가 있음을 저장함
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> result = new();

        for(int x = 0; x < objectSize.x; x++)
        {
            for(int y = 0; y < objectSize.y; y++)
            {
                result.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }

        return result;
    }

    //물체를 두고자 하는 위치에 둘 수 있는 지 확인, AddObjectAt 내의 foreach문과 같은 내용
    //AddObjectAt함수에서는 placedObjects[pos]를 foreach문 내에서 실행해야 하기 때문에 따로 놔둠
    //물체가 없으면 true 반환
    public bool CheckObjectPlacableAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach(var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                return false;
            }

            if (pos.x < minX || pos.x >= maxX || pos.z < minZ || pos.z >= maxZ)
            {
                return false;
            }
        }
        return true;
    }

    public int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if(placedObjects.ContainsKey(gridPosition) == false)
        {
            return -1;
        }
        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    //셀 위치에 있는 오브젝트 고유 ID를 갖고옴(database의 설정된 ID)
    public int GetRepresentationID(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
        {
            return -1;
        }
        return placedObjects[gridPosition].ID;
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach(var position in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(position);
        }
    }

    public void ClearAll()
    {
        placedObjects.Clear();
    }
}

//Vector3Int의 위치에 위치한 오브젝트의 정보
public class PlacementData
{
    public List<Vector3Int> occupiedPositions;//물체가 차지한 x, y 위치

    public int ID { get; private set; }

    //배치한 오브젝트 삭제 시 사용될 정보
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}