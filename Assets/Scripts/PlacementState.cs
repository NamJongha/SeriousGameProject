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

        //매개변수로 받은 ID가 data의 ID와 일치하면 그 object의 index를 objectData로부터 불러옴
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

    //가구 배치 시 호출
    public void OnAction(Vector3Int gridPosition, GameObject parentObject)
    {
        //배치하고자 하는 오브젝트가 제한 수량 이상이 아니라면 배치 가능
        if (database.objectsData[selectedObjectIndex].limitCount > database.objectsData[selectedObjectIndex].curCount)
        {
            //배치하고자 하는 위치에 겹치는 오브젝트가 없을 때만 배치 가능
            bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
            if (!placementValidity)
            {
                return;
            }

            //배치 사운드를 넣는 경우 여기에 넣으면 됨

            int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition), parentObject);

            database.objectsData[selectedObjectIndex].curCount += 1;

            //셀에 가구가 있는지 확인하기 위한 GridData에 새로 생성한 가구의 정보를 저장
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
