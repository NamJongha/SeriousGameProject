using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectDataSO database;

    [SerializeField]
    private GameObject gridVisualization;

    //오브젝트가 속하게 될 floor prefab -> 초기화할 경우 초기화 시마다 다시 reference 등록 필요
    [SerializeField]
    private GameObject parentObject;

    private GridData furnitureData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    [SerializeField]
    IBuildingState buildingState;

    private void Start()
    {
        StopPlacement();
        furnitureData = new(); //new GridData()
    }

    public void StartPlacement(int ID)
    {
        StopPlacement(); //새로 placement시작 시 기존 placement 종료
        gridVisualization.SetActive(true);

        buildingState = new PlacementState(ID, grid, preview, database, furnitureData, objectPlacer);
        
        inputManager.OnClicked += PlaceStructure; // PlaceStructure라는 메서드를 이벤트에 등록 => OnClicked 발생 시 해당 함수 호출됨
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, furnitureData, database, objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUi())
        {
            return;
        }
        
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition, parentObject);
    }

    //private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    //{
    //    return furnitureData.CheckObjectPlacableAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    //}

    private void StopPlacement()
    {
        if(buildingState == null)
        {
            return;
        }
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if(buildingState == null)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);


        if(lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);

            lastDetectedPosition = gridPosition;
        }
    }
}
