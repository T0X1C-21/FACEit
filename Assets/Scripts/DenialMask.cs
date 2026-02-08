using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct DenialMaskDialogues {
    public string[] introDialogue;
    public string[] attackFeedbackDialogue;
    public string[] defendFeedbackDialogue;
    public string[] observeFeedbackDialogue;
    public string[] talkFeedbackDialogue;
    public string[] maskCrackedFeedbackDialogue;
    public string[] defeatDialogue;
}

public class DenialMask : Mask<DenialMask> {

    public event EventHandler OnMaskDefeated;

    [SerializeField] private DenialMaskDialogues denialMaskDialogues;
    [SerializeField] private int timesToAttackToCrackMask_min;
    [SerializeField] private int timesToAttackToCrackMask_max;

    private bool isMaskCracked = false;
    private int numberOfTimesAttacked;
    private int timesToAttackToCrackMask;

    protected override void Awake() {
        base.Awake();
        timesToAttackToCrackMask = Random.Range(timesToAttackToCrackMask_min, timesToAttackToCrackMask_max + 1);
    }

    protected override void CombatPanelManager_OnStartAnimationsOver(object sender, EventArgs e) {
        InitialDialogue();
    }

    protected override void CombatPanelManager_OnAttackButtonPressed(object sender, EventArgs e) {
        if (isMaskCracked) {
            MaskCrackedSequence();
            return;
        }

        AttackSequence();
    }

    protected override void CombatPanelManager_OnDefendButtonPressed(object sender, EventArgs e) {
        if (isMaskCracked) {
            MaskCrackedSequence();
            return;
        }

        DefendSequence();
    }

    protected override void CombatPanelManager_OnObserveButtonPressed(object sender, EventArgs e) {
        if (isMaskCracked) {
            MaskCrackedSequence();
            return;
        }

        ObserveSequence();
    }

    protected override void CombatPanelManager_OnTalkButtonPressed(object sender, EventArgs e) {
        if (isMaskCracked) {
            MaskCrackedSequence();
            return;
        }

        TalkSequence();
    }

    protected override void CombatPanelManager_OnMaskDefeated(object sender, EventArgs e) {
        StartCoroutine(DefeatSequence());
    }

    private void InitialDialogue() {
        StartCoroutine(StringChain(denialMaskDialogues.introDialogue));
    }

    private void AttackSequence() {
        numberOfTimesAttacked++;

        StartCoroutine(StringSingle(denialMaskDialogues.attackFeedbackDialogue));
        StartCoroutine(AttackPlayer());
    }

    private void DefendSequence() {
        StartCoroutine(StringSingle(denialMaskDialogues.defendFeedbackDialogue));
        StartCoroutine(AttackPlayer());
    }

    private void ObserveSequence() {
        StartCoroutine(StringSingle(denialMaskDialogues.observeFeedbackDialogue));
        StartCoroutine(AttackPlayer());
    }

    private void TalkSequence() {
        StartCoroutine(StringChain(denialMaskDialogues.talkFeedbackDialogue));
        StartCoroutine(AttackPlayer());
    }

    private void MaskCrackedSequence() {
        StartCoroutine(StringSingle(denialMaskDialogues.maskCrackedFeedbackDialogue));
    }

    private IEnumerator DefeatSequence() {
        StartCoroutine(StringChain(denialMaskDialogues.defeatDialogue));
        
        yield return new WaitForSeconds(defeatTime);
        OnMaskDefeated?.Invoke(this, EventArgs.Empty);
    }

}
