using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct SadnessMaskDialogues {
    public string[] introDialogue;
    public string[] attackFeedbackDialogue;
    public string[] defendFeedbackDialogue;
    public string[] observeFeedbackDialogue;
    public string[] talkFeedbackDialogue;
    public string[] passiveFeedbackDialogue;
    public string[] defeatDialogue;
}

public class SadnessMask : Mask<SadnessMask> {

    public event EventHandler OnMaskDefeated;

    [SerializeField] private SadnessMaskDialogues sadnessMaskDialogues;

    private bool isTalkedTo = false;

    protected override void CombatPanelManager_OnStartAnimationsOver(object sender, EventArgs e) {
        InitialDialogue();
    }

    protected override void CombatPanelManager_OnAttackButtonPressed(object sender, EventArgs e) {
        if (isTalkedTo) {
            PassiveSequence();
            return;
        }

        AttackSequence();
    }

    protected override void CombatPanelManager_OnDefendButtonPressed(object sender, EventArgs e) {
        if (isTalkedTo) {
            PassiveSequence();
            return;
        }

        DefendSequence();
    }

    protected override void CombatPanelManager_OnObserveButtonPressed(object sender, EventArgs e) {
        if (isTalkedTo) {
            PassiveSequence();
            return;
        }

        ObserveSequence();
    }

    protected override void CombatPanelManager_OnTalkButtonPressed(object sender, EventArgs e) {
        if (isTalkedTo) {
            PassiveSequence();
            return;
        }

        TalkSequence();
    }

    protected override void CombatPanelManager_OnMaskDefeated(object sender, EventArgs e) {
        StartCoroutine(DefeatSequence());
    }

    private void InitialDialogue() {
        StartCoroutine(StringChain(sadnessMaskDialogues.introDialogue));
    }

    private void AttackSequence() {
        StartCoroutine(StringSingle(sadnessMaskDialogues.attackFeedbackDialogue));
        StartCoroutine(AttackPlayer());
    }

    private void DefendSequence() {
        StartCoroutine(StringSingle(sadnessMaskDialogues.defendFeedbackDialogue));
        StartCoroutine(AttackPlayer());
    }

    private void ObserveSequence() {
        StartCoroutine(StringSingle(sadnessMaskDialogues.observeFeedbackDialogue));
        StartCoroutine(AttackPlayer());
    }

    private void TalkSequence() {
        StartCoroutine(StringChain(sadnessMaskDialogues.talkFeedbackDialogue));
        isTalkedTo = true;
    }

    private void PassiveSequence() {
        StartCoroutine(StringSingle(sadnessMaskDialogues.passiveFeedbackDialogue));
    }

    private IEnumerator DefeatSequence() {
        StartCoroutine(StringChain(sadnessMaskDialogues.defeatDialogue));
        
        yield return new WaitForSeconds(defeatTime);
        OnMaskDefeated?.Invoke(this, EventArgs.Empty);
    }

    public bool IsTalkedTo() {
        return isTalkedTo;
    }

}
