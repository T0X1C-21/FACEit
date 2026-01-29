using System;
using UnityEngine;

public enum LevelState {
    Hint,
    Combat,
    Message
}

public class GameManager : MonoBehaviour {
    public static GameManager INSTANCE;

    public class OnLevelStateChangedEventArgs : EventArgs {
        public LevelState levelState;
        public int levelNumber;
    }

    public event EventHandler<OnLevelStateChangedEventArgs> OnLevelStateChanged;

    private LevelState currentlevelState;
    private int currentlevelNumber;

    private void Awake() {
        INSTANCE = this;
    }

    private void Start() {
        // Initialization
        ChangeLevelState(LevelState.Hint);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            ChangeLevelState(LevelState.Hint);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            ChangeLevelState(LevelState.Combat);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            ChangeLevelState(LevelState.Message);
        }
    }

    private void ChangeLevelState(LevelState newLevelState) {
        currentlevelState = newLevelState;

        if(newLevelState == LevelState.Hint) {
            currentlevelNumber++;
        }

        OnLevelStateChanged?.Invoke(this, new OnLevelStateChangedEventArgs {
            levelState = currentlevelState,
            levelNumber = currentlevelNumber
        });
    }
}
