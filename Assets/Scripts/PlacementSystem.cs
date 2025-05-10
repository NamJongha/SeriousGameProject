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
       StopPlacement(); //새로 placement시작 시 기존 placement 종료

       //매개변수로 받은 ID가 data의 ID와 일치하면 그 object의 index를 objectData로부터 불러옴
       selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
       if(selectedObjectIndex < 0)//ID에 해당하는 오브젝트가 없는 경우
       {
           Debug.LogError($"No ID found {ID}");
           return;
       }
       gridVisualization.SetActive(true);
       currentRotation = 0;
       preview.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab, database.objectsData[selectedObjectIndex].Size);

       inputManager.OnClicked += PlaceStructure; // PlaceStructure라는 메서드를 이벤트에 등록 => OnClicked 발생 시 해당 함수 호출됨
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
       
       //배치하고자 하는 가구의 현재 배치 수량이 제한 수량 이상이 아닐때만 가구를 생성 가능
       if (database.objectsData[selectedObjectIndex].limitCount > database.objectsData[selectedObjectIndex].curCount)
       {
           Vector3 mousePosition = inputManager.GetSelectedMapPosition();
           Vector3Int gridPosition = grid.WorldToCell(mousePosition);

           //배치하고자 하는 위치에 겹치는 오브젝트가 없을 때만 배치 가능
           bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
           if (!placementValidity)
           {
               return;
           }

           //배치 사운드를 넣는 경우 여기에 넣으면 됨

           //오브젝트 생성 후 floor의 자식으로 넣음(방 저장을 위함)
           GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
           database.objectsData[selectedObjectIndex].curCount += 1;
           Debug.Log(database.objectsData[selectedObjectIndex].curCount);
           newObject.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

           //생성된 오브젝트의 위치 설정
           Vector3 cellCenter = grid.CellToWorld(gridPosition);
           cellCenter.y = 0.05f;
           newObject.transform.position = cellCenter;

           newObject.transform.parent = parentObject.transform;

           //나중에 가구를 삭제할 때를 위한 저장?
           placedGameObjects.Add(newObject);

           //셀에 가구가 있는지 확인하기 위한 GridData에 새로 생성한 가구의 정보를 저장
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
