using TMPro;
using UnityEngine;

[System.Serializable]
public struct MessagePanelData {
    public string levelDoneText;
    public string messageText;
}

public class MessagePanelManager : MonoBehaviour {

    [SerializeField] private MessagePanelData[] messagePanelDataArray = new MessagePanelData[5];
    [SerializeField] private TextMeshProUGUI levelDoneTMP;
    [SerializeField] private TextMeshProUGUI messageTMP;
    
    private void Awake() {
        UIManager.INSTANCE.OnMessagePanelEnabled += UIManager_OnMessagePanelEnabled;
    }

    private void UIManager_OnMessagePanelEnabled(object sender, UIManager.OnPanelEnabledEventArgs e) {
        // Set Data
        int arrayIndex = e.levelNumber - 1;
        if(arrayIndex >= messagePanelDataArray.Length) {
            return;
        }
        messageTMP.text = messagePanelDataArray[arrayIndex].messageText;
        levelDoneTMP.text = messagePanelDataArray[arrayIndex].levelDoneText;
    }

}
