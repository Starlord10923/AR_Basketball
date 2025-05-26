using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.InputSystem.EnhancedTouch;
using ET = UnityEngine.InputSystem.EnhancedTouch;

public class PlaceHoop : MonoBehaviour
{
    public GameObject hoopPrefab; // Assign in Inspector
    public GameObject spawnedHoop;
    // Offset to move the hoop further away (horizontally) from the camera
    public float placementOffset = 1.0f; 
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool hoopPlaced = false; // Prevents further placement

    void Start()
    {
        EnhancedTouchSupport.Enable();
        raycastManager = FindObjectOfType<ARRaycastManager>();
        if (raycastManager == null)
            Debug.LogWarning("ARRaycastManager not found! Running in XR Simulation mode.");
    }

    void Update()
    {
        if (hoopPlaced) return;

        // Process only touch input
        if (ET.Touch.activeTouches.Count > 0)
            HandleTouchInput();
    }

    void HandleTouchInput()
    {
        var touch = ET.Touch.activeTouches[0];
        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            // Use AR raycast if available, else fallback to physics raycast
            if (raycastManager != null)
            {
                if (raycastManager.Raycast(touch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    PlaceHoopAt(hitPose.position);
                }
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                    PlaceHoopAt(hit.point);
            }
        }
    }

    void PlaceHoopAt(Vector3 position)
    {
        // Calculate horizontal direction from the camera to the hit point.
        Vector3 camPos = Camera.main.transform.position;
        Vector3 horizontalDirection = position - camPos;
        horizontalDirection.y = 0; // Ignore vertical difference.
        if(horizontalDirection == Vector3.zero)
            horizontalDirection = Vector3.forward;
        horizontalDirection = horizontalDirection.normalized;

        // Offset only horizontally to avoid placing below the plane.
        Vector3 adjustedPosition = position + horizontalDirection * placementOffset;
        
        Quaternion facingRotation = GetFacingRotation(adjustedPosition);
        if (spawnedHoop == null)
        {
            spawnedHoop = Instantiate(hoopPrefab, adjustedPosition, facingRotation);
            hoopPlaced = true;
            Debug.Log("Hoop placed at: " + adjustedPosition);
        }
    }

    Quaternion GetFacingRotation(Vector3 position)
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 directionToCamera = (cameraPosition - position).normalized;
        directionToCamera.y = 0; // Maintain vertical alignment
        return Quaternion.LookRotation(directionToCamera);
    }
}
