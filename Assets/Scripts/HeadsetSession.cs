using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Goes on an object in MainMenu. It's kept when changing scenes (DontDestroyOnLoad)
// so CaseA and CaseB use the same settings and the same saved data.
public class HeadsetSession : MonoBehaviour
{
    public static HeadsetSession Instance { get; private set; }

    [Header("Saved data")]
    [Tooltip("True: when the game opens, the saved data is deleted and each headset starts with its Inspector values. " +
             "False: the reservations from the previous session are kept")]
    public bool resetValuesAtStart = true;
    [Tooltip("Name of the MainMenu button that deletes the saved data")]
    public string resetButtonName = "Reset";

    [Header("Random occupied hours (same in CaseA and CaseB)")]
    public bool randomDemoData = true;
    [Range(0f, 1f)] public float randomOccupiedChance = 0.2f;
    [Tooltip("Each headset uses this seed + its number, so its occupied hours are different")]
    public int randomSeed = 1234;

    void Awake()
    {
        // When returning to MainMenu one already exists: the original stays.
        // Only this component is destroyed, not the object, in case it shares the object
        // with other scripts (for example the SceneChange buttons)
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Only happens once, when the game opens
        if (resetValuesAtStart)
            ResetSavedValues();

        // Every time a scene loads (including MainMenu when returning) the Reset button is hooked up
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject resetObject = GameObject.Find(resetButtonName);
        if (resetObject == null) return;

        Button resetButton = resetObject.GetComponent<Button>();
        if (resetButton == null) return;

        resetButton.onClick.RemoveListener(ResetSavedValues); // in case it was already hooked up
        resetButton.onClick.AddListener(ResetSavedValues);
    }

    // Deletes all saved data: the next time CaseA or CaseB is entered,
    // the headsets start with their Inspector values
    public void ResetSavedValues()
    {
        HeadsetDataStore.Clear();
        Debug.Log("Headset data reset.");
    }
}
