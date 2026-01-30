using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct AngryMaskDialogues {
    public string[] introDialogue;
    public string[] attackFeedbackDialogue;
    public string[] defendFeedbackDialogue;
    public string[] observeFeedbackDialogue;
    public string zeroRageMeterDialogue;
    public string defeatDialogue;
}

public class AngryMask : Mask<AngryMask> {

    public event EventHandler OnIntroDialoguesFinished;

    [SerializeField] private AngryMaskDialogues angryMaskDialoguesData;
    [SerializeField] private TextMeshProUGUI[] dialogueBoxArray = new TextMeshProUGUI[2];
    [SerializeField] private float dialogueWaitTime_min;
    [SerializeField] private float dialogueWaitTime_max;
    [SerializeField] private float cooldownBetweenDialogues_min;
    [SerializeField] private float cooldownBetweenDialogues_max;

    private void Start() {
        CombatPanelManager.INSTANCE.OnAttackButtonPressed += CombatPanelManager_OnAttackButtonPressed;

        StartCoroutine(InitialDialogue());
    }

    private void CombatPanelManager_OnAttackButtonPressed(object sender, EventArgs e) {
        StartCoroutine(AttackDialogue());
    }

    private IEnumerator AttackDialogue() {
        TextMeshProUGUI dialogueTMP = GetRandomDialogueTMP();
        int randomIndex = Random.Range(0, angryMaskDialoguesData.attackFeedbackDialogue.Length);
        string randomAttackFeedbackText = angryMaskDialoguesData.attackFeedbackDialogue[randomIndex];
        dialogueTMP.text = randomAttackFeedbackText;
        dialogueTMP.gameObject.SetActive(true);

        float dialogueWaitTime = Random.Range(dialogueWaitTime_min, dialogueWaitTime_max);
        yield return new WaitForSeconds(dialogueWaitTime);
        DisableAllDialogueTMPs();
    }

    private IEnumerator InitialDialogue() {
        foreach(string text in angryMaskDialoguesData.introDialogue) {
            TextMeshProUGUI dialogueTMP = GetRandomDialogueTMP();
            dialogueTMP.text = text;
            dialogueTMP.gameObject.SetActive(true);

            float dialogueWaitTime = Random.Range(dialogueWaitTime_min, dialogueWaitTime_max);
            yield return new WaitForSeconds(dialogueWaitTime);
            DisableAllDialogueTMPs();

            float cooldownBetweenDialogues = Random.Range(cooldownBetweenDialogues_min, cooldownBetweenDialogues_max);
            yield return new WaitForSeconds(cooldownBetweenDialogues);
        }
        OnIntroDialoguesFinished?.Invoke(this, EventArgs.Empty);
    }

    private TextMeshProUGUI GetRandomDialogueTMP() {
        int i = Random.Range(0, 2);
        return dialogueBoxArray[i];
    }

    private void DisableAllDialogueTMPs() {
        foreach(TextMeshProUGUI tmp in dialogueBoxArray) {
            tmp.gameObject.SetActive(false);
        }
    }
    
}
