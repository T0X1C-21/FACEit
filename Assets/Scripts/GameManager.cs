using System;
using System.Collections;
using UnityEngine;

public enum LevelState {
    Hint,
    Combat,
    Message
}

public class GameManager : MonoBehaviour {
    public static GameManager INSTANCE;

    [SerializeField] private GameObject hintPanel;
    [SerializeField] private GameObject combatPanel;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private float animationTime;

    private void Awake() {
        INSTANCE = this;
    }

    private void Start() {
        Transitioner.INSTANCE.OpenAnimation(animationTime);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            StartCoroutine(GameManager.INSTANCE.ChangeLevelState(LevelState.Message));
        }
    }

    public IEnumerator ChangeLevelState(LevelState newLevelState) {
        Transitioner.INSTANCE.CloseAnimation(animationTime);
        yield return new WaitForSeconds(animationTime);

        switch (newLevelState) {
            case LevelState.Combat:
                hintPanel.SetActive(false);
                combatPanel.SetActive(true);
                break;
            case LevelState.Message:
                combatPanel.SetActive(false);
                messagePanel.SetActive(true);
                break;
        }

        Transitioner.INSTANCE.OpenAnimation(animationTime);
    }

    public IEnumerator ReloadLevelAnimation() {
        Transitioner.INSTANCE.CloseAnimation(animationTime);
        yield return new WaitForSeconds(animationTime);

        LevelLoader.INSTANCE.ReloadLevel();
    }

    public void EndLevelTransition(out float animationTime) {
        Transitioner.INSTANCE.CloseAnimation(this.animationTime);
        animationTime = this.animationTime;
    }

}
