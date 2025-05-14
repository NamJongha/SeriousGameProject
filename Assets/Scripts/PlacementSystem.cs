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

    //      ?      ?     floor prefab ->  ? ?        ? ?  ©ª     ?  reference      ? 
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
        furnitureData.SetGridBounds(0, 5, 0, 5);
    }

    public void StartPlacement(int ID)
    {
        StopPlacement(); //     placement             placement     
        gridVisualization.SetActive(true);

        buildingState = new PlacementState(ID, grid, preview, database, furnitureData, objectPlacer);
        
        inputManager.OnClicked += PlaceStructure; // PlaceStructure     ?  ?  ? ?       => OnClicked  ?      ?   ?  ?   
        inputManager.OnExit += StopPlacement;
        inputManager.OnRotate += RotateStructure;
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

    private void RotateStructure()
    {
        if (buildingState is PlacementState placementState)
        {
            placementState.Rotate();
        }
    }

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
        inputManager.OnRotate -= RotateStructure;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    public void ResetPlacement()
    {
        StopPlacement();
        objectPlacer.RemoveAllObjects();
        database.ResetAllCount();
        furnitureData.ClearAll();
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
