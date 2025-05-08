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

    public void ResetAllCount() //��ġ�� ������ 0���� �ʱ�ȭ
    {
        foreach (var data in objectsData)
        {
            data.curCount = 0;
        }
    }
}

#region ���� ������Ʈ���� �����͸� ������ Ŭ����: ���� ��ġ�� ����
[Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public int ID { get; private set; } //�� ������Ʈ���� ������ ID: ���� ���̴��� �ٸ� ������Ʈ���� ǥ��

    [field: SerializeField]
    //������ ������ ĭ ���� ����
    public Vector2Int Size { get; private set; } = Vector2Int.one; //default size

    [field: SerializeField]
    public GameObject Prefab { get; private set; }

    [field: SerializeField]
    public int curCount { get; set; } = 0; //���� ��ġ�� ������ ����

    [field: SerializeField]
    public int limitCount { get; private set; } = 1; //��ġ�� �� �ִ� ������ ���� ����
}
#endregion