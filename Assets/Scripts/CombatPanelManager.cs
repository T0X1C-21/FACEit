using UnityEngine;

public class CombatPanelManager : MonoBehaviour {
    
    private void Awake() {
        UIManager.INSTANCE.OnCombatPanelEnabled += UIManager_OnCombatPanelEnabled; ;
    }

    private void UIManager_OnCombatPanelEnabled(object sender, UIManager.OnPanelEnabledEventArgs e) {
        // Set Data
        int arrayIndex = e.levelNumber - 1;

    }

}
