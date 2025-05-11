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

    public void Rotate()
    {
        rotation = (rotation + 90) % 360;
        previewSystem.RotatePreview(rotation);

        // ȸ���� size ���
        Vector2Int originalSize = database.objectsData[selectedObjectIndex].Size;
        Vector2Int rotatedSize = GetRotatedSize(originalSize, rotation);

        // Ŀ�� ������ ������Ʈ
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

    //���� ��ġ �� ȣ��
    public void OnAction(Vector3Int gridPosition, GameObject parentObject)
    {
        // ��ġ�ϰ��� �ϴ� ������Ʈ�� ���� ���� �̻��� �ƴ϶�� ��ġ ����
        if (database.objectsData[selectedObjectIndex].limitCount > database.objectsData[selectedObjectIndex].curCount)
        {
            Vector2Int originalSize = database.objectsData[selectedObjectIndex].Size;
            Vector2Int rotatedSize = GetRotatedSize(originalSize, rotation);

            // ��ġ�ϰ��� �ϴ� ��ġ�� ��ġ�� ������Ʈ�� ���� ���� ��ġ ����
            bool placementValidity = CheckPlacementValidity(gridPosition, rotatedSize);
            if (!placementValidity)
            {
                return;
            }

            // �������� �����Ͽ� ������Ʈ ��ġ�� ��ġ ��� (ȸ���� ������ ��������)
            Vector3 finalWorldPosition = grid.CellToWorld(gridPosition);
            finalWorldPosition += GetPlacementOffset(rotatedSize, rotation); // ȸ���� ����� �������� ������ ����

            // ���� ���� ��ġ�� ������Ʈ ��ġ
            int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, finalWorldPosition, rotation, parentObject);

            // ������Ʈ ī��Ʈ ����
            database.objectsData[selectedObjectIndex].curCount += 1;

            // GridData�� ��ġ�� ������ ��ġ ���� ���� (���⼭�� gridPosition�� �״�� ���)
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
