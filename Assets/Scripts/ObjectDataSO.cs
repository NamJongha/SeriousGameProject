using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectDataSO : ScriptableObject {
    public List<ObjectData> objectsData;

    private void OnEnable()
    {
        ResetAllCount();
    }

    public void ResetAllCount() //배치된 수량을 0으로 초기화
    {
        foreach (var data in objectsData)
        {
            data.curCount = 0;
        }
    }
}

#region 가구 오브젝트들의 데이터를 저장할 클래스: 가구 배치를 위함
[Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public int ID { get; private set; } //각 오브젝트마다 고유의 ID: 같은 모델이더라도 다른 오브젝트임을 표시

    [field: SerializeField]
    //가구가 차지할 칸 수로 설정
    public Vector2Int Size { get; private set; } = Vector2Int.one; //default size

    [field: SerializeField]
    public GameObject Prefab { get; private set; }

    [field: SerializeField]
    public int curCount { get; set; } = 0; //현재 배치된 가구의 수량

    [field: SerializeField]
    public int limitCount { get; private set; } = 1; //배치할 수 있는 가구의 제한 수량
}
#endregion