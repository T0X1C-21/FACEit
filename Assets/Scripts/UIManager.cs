using System;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public static UIManager INSTANCE;

    public class OnPanelEnabledEventArgs : EventArgs {
        public int levelNumber;
    }
    public event EventHandler<OnPanelEnabledEventArgs> OnHintPanelEnabled;
    public event EventHandler<OnPanelEnabledEventArgs> OnCombatPanelEnabled;
    public event EventHandler<OnPanelEnabledEventArgs> OnMessagePanelEnabled;

    [SerializeField] private GameObject hintPanel;
    [SerializeField] private GameObject combatPanel;
    [SerializeField] private GameObject messagePanel;

    private void Awake() {
        INSTANCE = this;
    }

    private void Start() {
        GameManager.INSTANCE.OnLevelStateChanged += GameManager_OnLevelStateChanged;
    }

    private void GameManager_OnLevelStateChanged(object sender, GameManager.OnLevelStateChangedEventArgs e) {
        DisableAllPanels();
        switch (e.levelState) {
            case LevelState.Hint:
                hintPanel.gameObject.SetActive(true);
                OnHintPanelEnabled?.Invoke(this, new OnPanelEnabledEventArgs {
                    levelNumber = e.levelNumber
                });
                break;
            case LevelState.Combat:
                combatPanel.gameObject.SetActive(true);
                OnCombatPanelEnabled?.Invoke(this, new OnPanelEnabledEventArgs {
                    levelNumber = e.levelNumber
                });
                break;
            case LevelState.Message:
                messagePanel.gameObject.SetActive(true);
                OnMessagePanelEnabled?.Invoke(this, new OnPanelEnabledEventArgs {
                    levelNumber = e.levelNumber
                });
                break;
        }
    }

    private void DisableAllPanels() {
        hintPanel.SetActive(false);
        combatPanel.SetActive(false);
        messagePanel.SetActive(false);
    }

}
