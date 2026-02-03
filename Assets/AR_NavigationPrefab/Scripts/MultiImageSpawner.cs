using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class MultiImageSpawner : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;

    [SerializeField] private UIManager uiManager;

    [Header("Prefabs (match by image name)")]
    public GameObject image1Prefab;
    public GameObject image2Prefab;

    // Spawned objects by image name
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

    // Track last active image (for easy cleanup)
    private string lastTrackedImage = null;

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

        foreach (var trackedImage in eventArgs.removed)
            OnImageRemoved(trackedImage);
    }

    void ProcessImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        GameObject prefabToSpawn = GetPrefab(imageName);

        if (prefabToSpawn == null) return;

        // --- THE FIX ---
        // Check if the dictionary contains the key, 
        // AND if the object assigned to that key is actually missing/destroyed.
        bool needsSpawn = !spawnedObjects.ContainsKey(imageName) || spawnedObjects[imageName] == null;

        if (needsSpawn)
        {
            GameObject spawned = Instantiate(
                prefabToSpawn,
                trackedImage.transform.position,
                trackedImage.transform.rotation
            );

            spawned.transform.parent = null;

            // Update or add the entry in the dictionary
            spawnedObjects[imageName] = spawned;

            Debug.Log($"Spawned/Respawned persistent object for {imageName}");
        }

        // Update pose ONLY while tracking
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            // Always check for null before moving, just in case it was destroyed this frame
            if (spawnedObjects[imageName] != null)
            {
                spawnedObjects[imageName].transform.SetPositionAndRotation(
                    trackedImage.transform.position,
                    trackedImage.transform.rotation
                );
            }

            lastTrackedImage = imageName;
            uiManager.OnImageTracked(imageName);
        }
        else
        {
            uiManager.OnImageLost();
        }
    }

    void OnImageRemoved(ARTrackedImage trackedImage)
    {
        uiManager.OnImageLost();
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

    // --- PUBLIC API ---

    public void DestroySpawnedObject(string imageName)
    {
        if (spawnedObjects.ContainsKey(imageName) && spawnedObjects[imageName] != null)
        {
            Destroy(spawnedObjects[imageName]);
            // Important: Explicitly remove from dictionary so the "ProcessImage" 
            // logic sees it as ready to spawn again.
            spawnedObjects.Remove(imageName);
            
            if (lastTrackedImage == imageName) lastTrackedImage = null;
            Debug.Log($"Destroyed {imageName}");
        }
    }

    public void DestroyAllSpawnedObjects()
    {
        foreach (var key in new List<string>(spawnedObjects.Keys))
        {
            DestroySpawnedObject(key);
        }
        spawnedObjects.Clear();
        lastTrackedImage = null;
    }

    public void DestroyLastTrackedObject()
    {
        if (!string.IsNullOrEmpty(lastTrackedImage))
            DestroySpawnedObject(lastTrackedImage);
    }
}