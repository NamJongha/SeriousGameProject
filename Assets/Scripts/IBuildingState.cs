using UnityEngine;

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector3Int gridPosition, GameObject parentObject);
    void UpdateState(Vector3Int gridPosition);
}