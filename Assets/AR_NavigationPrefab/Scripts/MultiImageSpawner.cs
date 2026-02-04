using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class MultiImageSpawner : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;

    [SerializeField] private UIManager uiManager;

    [Header("Prefabs (match by reference image name)")]
    public GameObject image1Prefab;
    public GameObject image2Prefab;

    // Only ONE object allowed at any time
    private GameObject currentSpawnedObject = null;
    private string currentImageName = null;

    void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
            ProcessImage(trackedImage);

        foreach (var trackedImage in eventArgs.updated)
            ProcessImage(trackedImage);
    }

    void ProcessImage(ARTrackedImage trackedImage)
    {
        // Only react when image is actively tracked
        if (trackedImage.trackingState != TrackingState.Tracking)
            return;

        string imageName = trackedImage.referenceImage.name;
        GameObject prefabToSpawn = GetPrefab(imageName);
        if (prefabToSpawn == null)
            return;

        // 🔥 HARD RULE: destroy existing object if image changed
        if (currentSpawnedObject != null && currentImageName != imageName)
        {
            Destroy(currentSpawnedObject);
            currentSpawnedObject = null;
            currentImageName = null;
        }

        // Spawn if nothing exists
        if (currentSpawnedObject == null)
        {
            currentSpawnedObject = Instantiate(
                prefabToSpawn,
                trackedImage.transform.position,
                trackedImage.transform.rotation
            );

            currentImageName = imageName;
            Debug.Log($"Spawned prefab for {imageName}");
        }

        // Always update pose
        currentSpawnedObject.transform.SetPositionAndRotation(
            trackedImage.transform.position,
            trackedImage.transform.rotation
        );

        uiManager.OnImageTracked(imageName);
    }

    GameObject GetPrefab(string imageName)
    {
        switch (imageName)
        {
            case "Image1": return image1Prefab;
            case "Image2": return image2Prefab;
            default:
                Debug.LogWarning($"No prefab assigned for image: {imageName}");
                return null;
        }
    }

    // ---------- PUBLIC API ----------

    /// <summary>
    /// Destroy whatever object is currently spawned (no matter what image)
    /// </summary>
    public void DestroyCurrentObject()
    {
        if (currentSpawnedObject != null)
        {
            Destroy(currentSpawnedObject);
            currentSpawnedObject = null;
            currentImageName = null;
            uiManager.OnImageLost();

            Debug.Log("Destroyed current spawned object");
        }
    }
}
