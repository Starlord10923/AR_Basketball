using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ET = UnityEngine.InputSystem.EnhancedTouch;

public class SwipeThrow : MonoBehaviour
{
    public List<GameObject> ballPrefabs; // Assign in Inspector
    public float minThrowForce = 1f;  // Minimum force
    public float maxThrowForce = 20f; // Maximum force
    public float spawnDistance = 0.5f; // Distance in front of camera

    private Vector2 touchStartPos, touchEndPos;
    private float swipeStartTime, swipeEndTime;
    private bool isSwipe;
    // Pixel threshold to cancel swipe if downward movement is too steep
    private float minSwipeVerticalThreshold = 30f;

    void Start()
    {
        // EnhancedTouchSupport.Enable();
    }

    void Update()
    {
        // Process only touch input
        if (ET.Touch.activeTouches.Count > 0)
            HandleTouchInput();
    }

    void HandleTouchInput()
    {
        var touch = ET.Touch.activeTouches[0];
        switch (touch.phase)
        {
            case UnityEngine.InputSystem.TouchPhase.Began:
                touchStartPos = touch.screenPosition;
                swipeStartTime = Time.time;
                isSwipe = true;
                Debug.Log("Swipe started at: " + touchStartPos);
                break;

            case UnityEngine.InputSystem.TouchPhase.Moved:
                Debug.Log("Touch moving at: " + touch.screenPosition);
                if (touch.screenPosition.y < touchStartPos.y - minSwipeVerticalThreshold)
                {
                    isSwipe = false;
                    Debug.Log("Swipe cancelled due to downward movement.");
                }
                break;

            case UnityEngine.InputSystem.TouchPhase.Ended:
                if (isSwipe)
                {
                    touchEndPos = touch.screenPosition;
                    swipeEndTime = Time.time;
                    Debug.Log("Swipe ended at: " + touchEndPos);
                    ThrowBall();
                }
                break;
        }
    }

    void ThrowBall()
    {
        // Only throw if the hoop has been placed.
        if (GetComponent<PlaceHoop>().spawnedHoop == null)
            return;

        Transform cameraTransform = Camera.main.transform;
        Vector3 spawnPosition = cameraTransform.position + cameraTransform.forward * spawnDistance;
        spawnPosition.y -= 0.2f; // Adjust height for improved feel

        int randomIndex = Random.Range(0, ballPrefabs.Count);
        GameObject ball = Instantiate(ballPrefabs[randomIndex], spawnPosition, cameraTransform.rotation);
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        // Calculate swipe duration (minimum value prevents division by zero)
        float swipeDuration = Mathf.Max(swipeEndTime - swipeStartTime, 0.01f);
        float swipeDistance = (touchEndPos - touchStartPos).magnitude;
        float swipeSpeed = swipeDistance / swipeDuration; // Pixels per second

        // Normalize swipe values
        float normalizedDistance = Mathf.Clamp01(swipeDistance / (Screen.height * 0.5f));
        float normalizedSpeed = Mathf.Clamp01(Mathf.InverseLerp(0, 2000f, swipeSpeed));

        // Combine normalized factors to compute throw force
        float combinedFactor = (normalizedDistance + normalizedSpeed) * 0.5f;
        float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, combinedFactor);

        // Apply force: forward plus upward component for a natural arc
        Vector3 force = cameraTransform.forward * throwForce + Vector3.up * (throwForce * 0.5f);
        rb.AddForce(force, ForceMode.Impulse);

        Debug.Log($"Ball thrown with force: {force} (SwipeDistance: {swipeDistance}, SwipeSpeed: {swipeSpeed}, CombinedFactor: {combinedFactor})");

        Destroy(ball, 6f);
    }
}
