//��ġ�� ������ ���� ������ �����ϱ� ���� ��ü
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class GridData
{
    //Vector3Int: �׸����� ��ġ�� ������Ʈ ������ ���� ����
    //new() : �ڵ������� ��ü ����(��ü�� �����ϴ� ���� ����) new Dictinary<>()�� ���� �ƶ�
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
    {
        //��ü�� ���� ��ġ�� ����Ͽ� �̸� ���� Vector3Int�� ������ x, y, z�� ��ġ 3������ �����ϱ� �����̸�
        //List�� ������ ������ �ϳ��� ��ü�� �������� ��(�׸��� ĭ)�� ������ �� �ֱ� ����
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);

        //������ ������ �ϴ� ��ġ�� �̹� �ٸ� ������ �ִ��� Ȯ��
        foreach(var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                Debug.Log($"Dictinary already contains this cell position {pos}");
                break;
            }
            //��ġ�� ��ġ�� ������ pos ��ġ�� �ش��ϴ� �κп� data�� �־� ��ü�� ������ ������
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

    //��ü�� �ΰ��� �ϴ� ��ġ�� �� �� �ִ� �� Ȯ��, AddObjectAt ���� foreach���� ���� ����
    //AddObjectAt�Լ������� placedObjects[pos]�� foreach�� ������ �����ؾ� �ϱ� ������ ���� ����
    //��ü�� ������ true ��ȯ
    public bool CheckObjectPlacableAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach(var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
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

    //�� ��ġ�� �ִ� ������Ʈ ���� ID�� ������(database�� ������ ID)
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

//Vector3Int�� ��ġ�� ��ġ�� ������Ʈ�� ����
public class PlacementData
{
    public List<Vector3Int> occupiedPositions;//��ü�� ������ x, y ��ġ

    public int ID { get; private set; }

    //��ġ�� ������Ʈ ���� �� ���� ����
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}