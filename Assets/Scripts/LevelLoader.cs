using UnityEngine;

public class LevelLoader : MonoBehaviour {

    public static LevelLoader INSTANCE;

    [SerializeField] private GameObject[] levels;

    private GameObject currentLevel;
    private int levelIndex = 0;

    private void Awake() {
        INSTANCE = this;
        SpawnNextLevel();
    }

    public void SpawnNextLevel() {
        if(levelIndex >= levels.Length) {
            return;
        }

        if(currentLevel != null)
            Destroy(currentLevel);

        currentLevel = Instantiate(levels[levelIndex++]);
        currentLevel.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public void ReloadLevel() {
        if(currentLevel != null)
            Destroy(currentLevel);

        currentLevel = Instantiate(levels[--levelIndex]);
        currentLevel.GetComponent<Canvas>().worldCamera = Camera.main;
    }

}
