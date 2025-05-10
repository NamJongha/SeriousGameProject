using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
   [SerializeField]
   private GameObject mouseIndicator;

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

   private List<GameObject> placedGameObjects = new();

   [SerializeField]
   private PreviewSystem preview;

   private Vector3Int lastDetectedPosition = Vector3Int.zero;

   private int currentRotation = 0;

   private void Start()
   {
       StopPlacement();
       furnitureData = new(); //new GridData()
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
       currentRotation = 0;
       preview.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab, database.objectsData[selectedObjectIndex].Size);

       inputManager.OnClicked += PlaceStructure; // PlaceStructure��� �޼��带 �̺�Ʈ�� ��� => OnClicked �߻� �� �ش� �Լ� ȣ���
       inputManager.OnExit += StopPlacement;
       //inputManager.OnRotate += RotateStructure;
   }

   //private void RotateStructure()
   //{
   //    currentRotation = (currentRotation + 90) % 360;
   //
   //    Vector2Int originalSize = database.objectsData[selectedObjectIndex].Size;
   //    Vector2Int rotatedSize = GetRotatedSize(originalSize, currentRotation);
   //
   //    preview.UpdatePreviewRotation(rotatedSize, currentRotation);
   //
   //    Vector3 mousePosition = inputManager.GetSelectedMapPosition();
   //    Vector3Int gridPosition = grid.WorldToCell(mousePosition);
   //    Vector3 cellCenter = grid.GetCellCenterWorld(gridPosition);
   //    cellCenter.y = 0.05f;
   //
   //    bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
   //    preview.UpdatePosition(cellCenter, currentRotation, placementValidity);
   //}
   //
   //private Vector2Int GetRotatedSize(Vector2Int originalSize, int rotation)
   //{
   //    if(rotation % 180 != 0)
   //    {
   //        return new Vector2Int(originalSize.y, originalSize.x);
   //    }
   //
   //    return originalSize;
   //}

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
           newObject.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

           //������ ������Ʈ�� ��ġ ����
           Vector3 cellCenter = grid.CellToWorld(gridPosition);
           cellCenter.y = 0.05f;
           newObject.transform.position = cellCenter;

           newObject.transform.parent = parentObject.transform;

           //���߿� ������ ������ ���� ���� ����?
           placedGameObjects.Add(newObject);

           //���� ������ �ִ��� Ȯ���ϱ� ���� GridData�� ���� ������ ������ ������ ����
           furnitureData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size, database.objectsData[selectedObjectIndex].ID, placedGameObjects.Count - 1);

           preview.UpdatePosition(grid.CellToWorld(gridPosition), false);
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
       preview.StopShowingPreview();
       inputManager.OnClicked -= PlaceStructure;
       inputManager.OnExit -= StopPlacement;
       //inputManager.OnRotate -= RotateStructure;
       lastDetectedPosition = Vector3Int.zero;
   }

   private void Update()
   {
       if(selectedObjectIndex < 0)
       {
           return;
       }
       Vector3 mousePosition = inputManager.GetSelectedMapPosition();
       Vector3Int gridPosition = grid.WorldToCell(mousePosition);

       mouseIndicator.transform.position = mousePosition;

       if(lastDetectedPosition != gridPosition)
       {
           bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

           mouseIndicator.transform.position = mousePosition;

           Vector3 cellCentor = grid.CellToWorld(gridPosition);
           cellCentor.y = 0.05f;

           preview.UpdatePosition(cellCentor, placementValidity);

           lastDetectedPosition = gridPosition;
       }
   }
}
