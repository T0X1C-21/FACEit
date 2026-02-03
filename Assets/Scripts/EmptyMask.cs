using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct EmptyMaskDialogues {
    public string[] introDialogue;
    public string[] attackFeedbackDialogue;
    public string[] defendFeedbackDialogue;
    public string[] observeFeedbackDialogue;
    public string[] talkFeedbackDialogue;
    public string[] acceptedFeedbackDialogue;
    public string[] defeatDialogue;
}

public class EmptyMask : Mask<EmptyMask> {

    public event EventHandler OnMaskDefeated;
    public event EventHandler OnEmptyMaskInteractionsComplete;

    [SerializeField] private EmptyMaskDialogues emptyMaskDialogues;
    [SerializeField] private int numberOfInteractionsToTriggerAccept;
    [SerializeField] private float timeToDefeatAfterAccepted;

    private int numberOfInteractions;

    protected override void Awake() {
        base.Awake();
    }

    protected override void Start() {
        base.Start();

        CombatPanelManager.INSTANCE.OnAcceptButtonPressed += CombatPanelManager_OnAcceptButtonPressed;
    }

    private void CombatPanelManager_OnAcceptButtonPressed(object sender, EventArgs e) {
        StartCoroutine(AcceptedSequence());
    }

    protected override void CombatPanelManager_OnStartAnimationsOver(object sender, EventArgs e) {
        InitialDialogue();
    }

    protected override void CombatPanelManager_OnAttackButtonPressed(object sender, EventArgs e) {
        CheckNumberOfInteractions();
        AttackSequence();
    }

    protected override void CombatPanelManager_OnDefendButtonPressed(object sender, EventArgs e) {
        CombatPanelManager.INSTANCE.AddToActionLog("<color=red>Defend failed!</color>");
        CheckNumberOfInteractions();
        DefendSequence();
    }

    protected override void CombatPanelManager_OnObserveButtonPressed(object sender, EventArgs e) {
        CombatPanelManager.INSTANCE.AddToActionLog("<color=red>Observe failed!</color>");
        CheckNumberOfInteractions();
        ObserveSequence();
    }

    protected override void CombatPanelManager_OnTalkButtonPressed(object sender, EventArgs e) {
        CheckNumberOfInteractions();
        TalkSequence();
    }

    protected override void CombatPanelManager_OnMaskDefeated(object sender, EventArgs e) {
        StartCoroutine(DefeatSequence());
    }

    private void InitialDialogue() {
        StartCoroutine(StringChain(emptyMaskDialogues.introDialogue));
    }

    private void AttackSequence() {
        StartCoroutine(StringSingle(emptyMaskDialogues.attackFeedbackDialogue));
    }

    private void DefendSequence() {
        StartCoroutine(StringSingle(emptyMaskDialogues.defendFeedbackDialogue));
    }

    private void ObserveSequence() {
        StartCoroutine(StringSingle(emptyMaskDialogues.observeFeedbackDialogue));
    }

    private void TalkSequence() {
        StartCoroutine(StringChain(emptyMaskDialogues.talkFeedbackDialogue));
    }

    private IEnumerator AcceptedSequence() {
        StartCoroutine(StringChain(emptyMaskDialogues.acceptedFeedbackDialogue));
        yield return new WaitForSeconds(timeToDefeatAfterAccepted);
        StartCoroutine(DefeatSequence());
    }

    private IEnumerator DefeatSequence() {
        StartCoroutine(StringChain(emptyMaskDialogues.defeatDialogue));
        
        yield return new WaitForSeconds(defeatTime);
        OnMaskDefeated?.Invoke(this, EventArgs.Empty);
    }

    private void CheckNumberOfInteractions() {
        numberOfInteractions++;
        if(numberOfInteractions == numberOfInteractionsToTriggerAccept)
            OnEmptyMaskInteractionsComplete?.Invoke(this, EventArgs.Empty);
    }

}
