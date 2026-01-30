using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct CombatPanelData {
    public string maskTitleText;
    public GameObject maskObject;
}

public class CombatPanelManager : MonoBehaviour {

    public static CombatPanelManager INSTANCE;

    public event EventHandler OnAttackButtonPressed;

    [SerializeField] private CombatPanelData[] combatPanelDataArray = new CombatPanelData[5];
    [SerializeField] private TextMeshProUGUI maskTitleTMP;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button defendButton;
    [SerializeField] private Button observeButton;
    [SerializeField] private Button talkButton;

    private bool canPressButtons = false;
    
    private void Awake() {
        INSTANCE = this;

        UIManager.INSTANCE.OnCombatPanelEnabled += UIManager_OnCombatPanelEnabled;
        attackButton.onClick.AddListener(OnAttackButtonClick);
        defendButton.onClick.AddListener(OnDefendButtonClick);
        observeButton.onClick.AddListener(OnObserveButtonClick);
        talkButton.onClick.AddListener(OnTalkButtonClick);
    }

    private void UIManager_OnCombatPanelEnabled(object sender, UIManager.OnPanelEnabledEventArgs e) {
        // Set Data
        int arrayIndex = e.levelNumber - 1;
        if(arrayIndex >= combatPanelDataArray.Length) {
            return;
        }

        maskTitleTMP.text = combatPanelDataArray[arrayIndex].maskTitleText;

        GameObject maskObject = Instantiate(combatPanelDataArray[arrayIndex].maskObject);
        maskObject.transform.SetParent(this.transform, false);
        maskObject.gameObject.SetActive(true);
        switch (e.levelNumber) {
            case 1:
                AngryMask.INSTANCE.OnIntroDialoguesFinished += OnIntroDialoguesFinished;
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
        }
    }

    private void OnIntroDialoguesFinished(object sender, System.EventArgs e) => EnableCanPressButtons();

    private void EnableCanPressButtons() => canPressButtons = true;
    private void DisableCanPressButtons() => canPressButtons = false;

    private void OnAttackButtonClick() {
        if(!canPressButtons)
            return;

        Debug.Log("Attack!");
        OnAttackButtonPressed?.Invoke(this, EventArgs.Empty);
    }

    private void OnDefendButtonClick() {
        if(!canPressButtons)
            return;

        Debug.Log("Defend!");
    }

    private void OnObserveButtonClick() {
        if(!canPressButtons)
            return;

        Debug.Log("Observe!");
    }

    private void OnTalkButtonClick() {
        if(!canPressButtons)
            return;

        Debug.Log("Talk!");
    }

}
