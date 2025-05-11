using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    public  int PlaceObject(GameObject prefab, Vector3 position, int rotation, GameObject floorObject)
    {
        GameObject newObject = Instantiate(prefab);

        //������ ������Ʈ�� ��ġ ����
        Vector3 cellCenter = position;
        cellCenter.y = 0.05f;
        newObject.transform.position = cellCenter;

        newObject.transform.rotation = Quaternion.Euler(0, rotation, 0);

        //������Ʈ ���� �� floor�� �ڽ����� ����(�� ������ ����)
        newObject.transform.parent = floorObject.transform;

        //���߿� ������ ������ ���� ���� ����?
        placedGameObjects.Add(newObject);

        return placedGameObjects.Count - 1;
    }

    public void RemoveObjectAt(int gameObjectIndex)
    {
        if(placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
        {
            return;
        }
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }

    public void RemoveAllObjects()
    {
        foreach(GameObject go in placedGameObjects)
        {
            Destroy(go);
        }
    }
}
