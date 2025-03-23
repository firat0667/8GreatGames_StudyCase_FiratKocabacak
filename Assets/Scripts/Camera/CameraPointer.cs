using GreatGames.CaseLib.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPointer : MonoBehaviour
{
    [SerializeField] private float _baseFOV = 60f;
    [SerializeField] private float _fowMultiply = 3f;
    [SerializeField] private float _baseWidth = 6f; 
    [SerializeField] private float _cameraXOffset = -0.5f;
    [SerializeField] private float _cameraXMultiply = 1f;

    private Camera _cam;

    public void AdjustCameraByLevelData(LevelConfigSO levelData)
    {
        if (levelData == null)
        {
            Debug.LogWarning("CameraPointer: LevelData null!");
            return;
        }

        int upperWidth = levelData.UpperGridSize.x;
        int lowerWidth = levelData.LowerGridSize.x;
        int gridWidth = Mathf.Max(upperWidth, lowerWidth);
        float scaleFactor = 0;
        if (gridWidth%2==0)
        {
            scaleFactor = (gridWidth / 2) - (_cameraXOffset);
        }
        else
        {
            scaleFactor = (gridWidth / 2)+ (_cameraXOffset);
        }
        
        _cam = Camera.main != null ? Camera.main : GetComponent<Camera>();
        Vector3 currentPos = _cam.transform.position;
        _cam.transform.position = new Vector3((_cameraXMultiply * scaleFactor)-_cameraXOffset, currentPos.y, currentPos.z);

        _cam.fieldOfView = _baseFOV + (gridWidth - _baseWidth) * _fowMultiply;
    }
}
