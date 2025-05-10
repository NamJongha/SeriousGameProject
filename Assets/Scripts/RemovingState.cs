using System;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class RemovingState : IBuildingState
{
    //objectPlacer의 gameobject list의 인덱스
    private int gameObjectIndex = -1;
    private int ID;
    Grid grid;
    PreviewSystem previewSystem;
    GridData furnitureData;
    ObjectDataSO database;
    ObjectPlacer objectPlacer;

    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         GridData furnitureData,
                         ObjectDataSO database,
                         ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.furnitureData = furnitureData;
        this.database = database;
        this.objectPlacer = objectPlacer;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition, GameObject parentObject)
    {
        GridData selectedData = null;

        //공간에 가구가 있다면
        if (furnitureData.CheckObjectPlacableAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = furnitureData;
        }

        if(selectedData == null)
        {
            //nothing to remove: maybe play some sound effects
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if(gameObjectIndex == -1)
            {
                return;
            }
            //제한 수량 중 현재 수량을 1 감소시킴
            ID = selectedData.GetRepresentationID(gridPosition);
            database.objectsData[ID].curCount -= 1;

            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }
        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        cellPosition.y = 0.05f;
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        //해당 위치에 가구가 없다면 false 반환
        return !furnitureData.CheckObjectPlacableAt(gridPosition, Vector2Int.one);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        cellPosition.y = 0.05f;
        previewSystem.UpdatePosition(cellPosition, validity);
    }
}
