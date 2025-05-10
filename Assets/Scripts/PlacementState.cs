using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectDataSO database;
    GridData furnitureData;
    ObjectPlacer objectPlacer;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectDataSO database,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        //�Ű������� ���� ID�� data�� ID�� ��ġ�ϸ� �� object�� index�� objectData�κ��� �ҷ���
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);

        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab, database.objectsData[selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"No object with id {iD}");
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    //���� ��ġ �� ȣ��
    public void OnAction(Vector3Int gridPosition, GameObject parentObject)
    {
        //��ġ�ϰ��� �ϴ� ������Ʈ�� ���� ���� �̻��� �ƴ϶�� ��ġ ����
        if (database.objectsData[selectedObjectIndex].limitCount > database.objectsData[selectedObjectIndex].curCount)
        {
            //��ġ�ϰ��� �ϴ� ��ġ�� ��ġ�� ������Ʈ�� ���� ���� ��ġ ����
            bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
            if (!placementValidity)
            {
                return;
            }

            //��ġ ���带 �ִ� ��� ���⿡ ������ ��

            int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition), parentObject);

            database.objectsData[selectedObjectIndex].curCount += 1;

            //���� ������ �ִ��� Ȯ���ϱ� ���� GridData�� ���� ������ ������ ������ ����
            furnitureData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size, database.objectsData[selectedObjectIndex].ID, index);

            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
        }
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        return furnitureData.CheckObjectPlacableAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        Vector3 cellCentor = grid.CellToWorld(gridPosition);
        cellCentor.y = 0.05f;

        previewSystem.UpdatePosition(cellCentor, placementValidity);
    }
}
