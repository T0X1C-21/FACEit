using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct HintLevelData {
    public string levelText;
    public string hintText;
}

public class HintPanelManager : MonoBehaviour {

    [SerializeField] private HintLevelData[] hintLevelDataArray = new HintLevelData[5];
    [SerializeField] private TextMeshProUGUI hintTMP;
    [SerializeField] private TextMeshProUGUI levelTMP;
    [SerializeField] private Button continueButton;

    private void Awake() {
        UIManager.INSTANCE.OnHintPanelEnabled += UIManager_OnHintPanelEnabled;
        continueButton.onClick.AddListener(OnClickContinueButton);
    }

    private void UIManager_OnHintPanelEnabled(object sender, UIManager.OnPanelEnabledEventArgs e) {
        // Set Data
        int arrayIndex = e.levelNumber - 1;
        if(arrayIndex >= hintLevelDataArray.Length) {
            return;
        }
        hintTMP.text = hintLevelDataArray[arrayIndex].hintText;
        levelTMP.text = hintLevelDataArray[arrayIndex].levelText;
    }

    private void OnClickContinueButton() {
        GameManager.INSTANCE.ChangeToCombatLevelState();
    }

}
