using System;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class RemovingState : IBuildingState
{
    //objectPlacer�� gameobject list�� �ε���
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

        //������ ������ �ִٸ�
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
            //���� ���� �� ���� ������ 1 ���ҽ�Ŵ
            ID = selectedData.GetRepresentationID(gridPosition);
            database.objectsData[ID].curCount -= 1;

            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }
        Vector3 cursorPosition = grid.CellToWorld(gridPosition);
        cursorPosition.y = 0.05f;

        Vector3 previewPosition = cursorPosition + new Vector3(0, previewSystem.previewYOffset, 0); // ����

        previewSystem.UpdatePosition(cursorPosition, previewPosition, Vector2Int.one, CheckIfSelectionIsValid(gridPosition));
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        //�ش� ��ġ�� ������ ���ٸ� false ��ȯ
        return !furnitureData.CheckObjectPlacableAt(gridPosition, Vector2Int.one);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        Vector3 cursorPosition = grid.CellToWorld(gridPosition);
        cursorPosition.y = 0.05f;

        Vector3 previewPosition = cursorPosition + new Vector3(0, previewSystem.previewYOffset, 0); // ����

        previewSystem.UpdatePosition(cursorPosition, previewPosition, Vector2Int.one, validity);
    }
}
