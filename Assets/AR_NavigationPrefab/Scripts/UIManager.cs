using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("UI Root")]
    public GameObject uiRoot;   // Parent of all UI (WayfindingUI)

    [Header("Buttons")]
    public Button[] Buttons;

    [Header("Label Sets")]
    public string[] Image1Labels = { "1", "2", "3" };
    public string[] Image2Labels = { "4", "5", "6" };

    // ==============================
    // Runtime Wayfinding Cache
    // ==============================
    private Dictionary<string, GameObject> ways = new Dictionary<string, GameObject>();
    private Transform wayfindingContainer;
    private GameObject Directory;

    // Names MUST match spawned GameObjects
    private readonly string[] wayNames =
    {
        "First Direction",
        "Second Direction",
        "Third Direction"
    };

    void Start()
    {
        HideUI();
    }

    // ==============================
    // Image Tracking Hooks
    // ==============================
    public void OnImageTracked(string imageName)
    {
        ShowUI();
        // ClearWayCache(); // optional: reset ways when new image is tracked

        switch (imageName)
        {
            case "Image1":
                SetButtonTexts(Image1Labels);
                break;

            case "Image2":
                SetButtonTexts(Image2Labels);
                break;

            default:
                Debug.LogWarning($"No UI mapping for image: {imageName}");
                break;
        }
    }

    public void OnImageLost()
    {
        // HideUI();
        // DeactivateAllWays();
    }

    // ==============================
    // UI Helpers
    // ==============================
    void SetButtonTexts(string[] labels)
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            if (i >= labels.Length) return;

            TMP_Text text = Buttons[i].GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = labels[i];
        }
    }

    void ShowUI()
    {
        if (uiRoot != null)
            uiRoot.SetActive(true);
    }

    public void HideUI()
    {
        if (uiRoot != null)
            uiRoot.SetActive(false);
        DeactivateAllWays();
        ClearGameObject();
    }

    // ==============================
    // Wayfinding Runtime Logic
    // ==============================

    /// <summary>
    /// Find runtime-spawned way objects ONCE and cache them
    /// </summary>
   void CacheWaysIfNeeded()
    {
        if (ways.Count > 0) return;

        // 1. Find the active parent container by its name
        // Replace "WayfindingContainer" with the exact name of your spawned parent
        GameObject containerGo = GameObject.Find("DirectionContainer");

        if (containerGo != null)
        {
            wayfindingContainer = containerGo.transform;

            // 2. Iterate through your names and find the inactive children
            foreach (string name in wayNames)
            {
                // transform.Find works on inactive children!
                Transform childTransform = wayfindingContainer.Find(name);
                
                if (childTransform != null)
                {
                    ways[name] = childTransform.gameObject;
                    // Keep them inactive until a button is pressed
                    childTransform.gameObject.SetActive(false); 
                }
                else
                {
                    Debug.LogWarning($"Parent found, but child '{name}' is missing.");
                }
            }
        }
        else
        {
            Debug.LogWarning("Spawned parent 'WayfindingContainer' not found in scene.");
        }
    }

    /// <summary>
    /// Activate one way and disable all others
    /// Call from UI buttons
    /// </summary>
    public void SetActiveWay(string wayName)
    {
        CacheWaysIfNeeded();

        foreach (var pair in ways)
        {
            if (pair.Value != null)
                pair.Value.SetActive(pair.Key == wayName);
        }
    }

    /// <summary>
    /// Disable all wayfinding objects
    /// </summary>
    void DeactivateAllWays()
    {
        foreach (var way in ways.Values)
        {
            if (way != null)
                way.SetActive(false);
        }
    }

    /// <summary>
    /// Clear cached runtime-spawned wayfinding objects
    /// Call this before spawning a new set of ways
    /// </summary>
    public void ClearWayCache()
    {
        foreach (var way in ways.Values)
        {
            if (way != null)
                way.SetActive(false);
        }

        ways.Clear();
        Debug.Log("Wayfinding cache cleared");
    }

    public void ClearGameObject()
    {
        Directory = GameObject.FindWithTag("Directory");
        Destroy(Directory);
    }
}
