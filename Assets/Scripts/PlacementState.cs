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
    int rotation;
    private Vector3Int currentGridPosition;

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
        this.rotation = 0;

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

    public void Rotate()
    {
        rotation = (rotation + 90) % 360;
        previewSystem.RotatePreview(rotation);

        // 회전된 size 계산
        Vector2Int originalSize = database.objectsData[selectedObjectIndex].Size;
        Vector2Int rotatedSize = GetRotatedSize(originalSize, rotation);

        // 커서 사이즈 업데이트
        previewSystem.UpdateCursorSize(rotatedSize);

        Vector3 cursorPosition = grid.CellToWorld(currentGridPosition);
        cursorPosition.y = 0.05f;

        Vector3 previewPosition = cursorPosition + GetPlacementOffset(rotatedSize, rotation);

        bool placementValidity = CheckPlacementValidity(currentGridPosition, rotatedSize);
        previewSystem.UpdatePosition(cursorPosition, previewPosition, rotatedSize, placementValidity);
    }

    private Vector2Int GetRotatedSize(Vector2Int originalSize, int rotation)
    {
        return (rotation % 180 == 0) ? originalSize : new Vector2Int(originalSize.y, originalSize.x);
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    //가구 배치 시 호출
    public void OnAction(Vector3Int gridPosition, GameObject parentObject)
    {
        // 배치하고자 하는 오브젝트가 제한 수량 이상이 아니라면 배치 가능
        if (database.objectsData[selectedObjectIndex].limitCount > database.objectsData[selectedObjectIndex].curCount)
        {
            Vector2Int originalSize = database.objectsData[selectedObjectIndex].Size;
            Vector2Int rotatedSize = GetRotatedSize(originalSize, rotation);

            // 배치하고자 하는 위치에 겹치는 오브젝트가 없을 때만 배치 가능
            bool placementValidity = CheckPlacementValidity(gridPosition, rotatedSize);
            if (!placementValidity)
            {
                return;
            }

            // 오프셋을 적용하여 오브젝트 배치할 위치 계산 (회전된 사이즈 기준으로)
            Vector3 finalWorldPosition = grid.CellToWorld(gridPosition);
            finalWorldPosition += GetPlacementOffset(rotatedSize, rotation); // 회전된 사이즈를 기준으로 오프셋 적용

            // 실제 월드 위치에 오브젝트 배치
            int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, finalWorldPosition, rotation, parentObject);

            // 오브젝트 카운트 증가
            database.objectsData[selectedObjectIndex].curCount += 1;

            // GridData에 배치된 가구의 위치 정보 저장 (여기서는 gridPosition을 그대로 사용)
            furnitureData.AddObjectAt(gridPosition, rotatedSize, database.objectsData[selectedObjectIndex].ID, index);

            Vector3 cursorPosition = grid.CellToWorld(gridPosition);
            cursorPosition.y = 0.05f;

            previewSystem.UpdatePosition(cursorPosition, finalWorldPosition, rotatedSize, false);
        }
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, Vector2Int size)
    {
        return furnitureData.CheckObjectPlacableAt(gridPosition, size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        currentGridPosition = gridPosition;

        Vector2Int originalSize = database.objectsData[selectedObjectIndex].Size;
        Vector2Int rotatedSize = GetRotatedSize(originalSize, rotation);

        bool placementValidity = CheckPlacementValidity(gridPosition, rotatedSize);

        Vector3 cursorPosition = grid.CellToWorld(gridPosition);
        cursorPosition.y = 0.05f;

        Vector3 previewPosition = cursorPosition + GetPlacementOffset(rotatedSize, rotation);

        previewSystem.UpdatePosition(cursorPosition, previewPosition, rotatedSize, placementValidity);
    }

    private Vector3 GetPlacementOffset(Vector2Int size, int rotation)
    {
        switch (rotation % 360)
        {
            case 0:
                return Vector3.zero;

            case 90:
                return new Vector3(0f, 0f, size.y);

            case 180:
                return new Vector3(size.x, 0f, size.y);

            case 270:
                return new Vector3(size.x, 0f, 0f);

            default:
                return Vector3.zero;
        }
    }
}
