using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    public  int PlaceObject(GameObject prefab, Vector3 position, GameObject floorObject)
    {
        GameObject newObject = Instantiate(prefab);

        //생성된 오브젝트의 위치 설정
        Vector3 cellCenter = position;
        cellCenter.y = 0.05f;
        newObject.transform.position = cellCenter;

        //오브젝트 생성 후 floor의 자식으로 넣음(방 저장을 위함)
        newObject.transform.parent = floorObject.transform;

        //나중에 가구를 삭제할 때를 위한 저장?
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
}
