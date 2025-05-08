using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator, cellIndicator;

    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectDataSO database;
    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private GameObject parentObject;

    private GridData furnitureData;

    private Renderer[] previewRenderer;

    private List<GameObject> placedGameObjects = new();

    private void Start()
    {
        StopPlacement();
        furnitureData = new(); //new GridData()
        previewRenderer = cellIndicator.GetComponentsInChildren<Renderer>();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement(); //���� placement���� �� ���� placement ����

        //�Ű������� ���� ID�� data�� ID�� ��ġ�ϸ� �� object�� index�� objectData�κ��� �ҷ���
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if(selectedObjectIndex < 0)//ID�� �ش��ϴ� ������Ʈ�� ���� ���
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }
        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure; // PlaceStructure��� �޼��带 �̺�Ʈ�� ��� => OnClicked �߻� �� �ش� �Լ� ȣ���
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUi())
        {
            return;
        }
        
        //��ġ�ϰ��� �ϴ� ������ ���� ��ġ ������ ���� ���� �̻��� �ƴҶ��� ������ ���� ����
        if (database.objectsData[selectedObjectIndex].limitCount > database.objectsData[selectedObjectIndex].curCount)
        {
            Vector3 mousePosition = inputManager.GetSelectedMapPosition();
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);

            //��ġ�ϰ��� �ϴ� ��ġ�� ��ġ�� ������Ʈ�� ���� ���� ��ġ ����
            bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
            if (!placementValidity)
            {
                return;
            }

            //��ġ ���带 �ִ� ��� ���⿡ ������ ��

            //������Ʈ ���� �� floor�� �ڽ����� ����(�� ������ ����)
            GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
            database.objectsData[selectedObjectIndex].curCount += 1;
            Debug.Log(database.objectsData[selectedObjectIndex].curCount);
            newObject.transform.parent = parentObject.transform;

            //������ ������Ʈ�� ��ġ ����
            Vector3 cellCenter = grid.GetCellCenterWorld(gridPosition);
            cellCenter.y = 0.05f;
            newObject.transform.position = cellCenter;

            //���߿� ������ ������ ���� ���� ����?
            placedGameObjects.Add(newObject);

            //���� ������ �ִ��� Ȯ���ϱ� ���� GridData�� ���� ������ ������ ������ ����
            furnitureData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size, database.objectsData[selectedObjectIndex].ID, placedGameObjects.Count - 1);
        }
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        return furnitureData.CheckObjectPlacableAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }

    private void Update()
    {
        if(selectedObjectIndex < 0)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        foreach(var renderer in previewRenderer)
        {
            //true�� ��� cursorIndicator�� ���� ���, false�� ��� ���������� ��Ÿ��
            renderer.material.color = placementValidity ? Color.white : Color.red;
        }

        mouseIndicator.transform.position = mousePosition;

        Vector3 cellCentor = grid.GetCellCenterWorld(gridPosition);
        cellCentor.y = 0.05f;
        cellIndicator.transform.position = cellCentor;
    }
}
