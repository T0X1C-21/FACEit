using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct FearMaskDialogues {
    public string[] introDialogue;
    public string[] attackFeedbackDialogue;
    public string[] defendFeedbackDialogue;
    public string[] observeFeedbackDialogue;
    public string[] talkFeedbackDialogue;
    public string[] successfulObserveDialogue;
    public string[] defeatDialogue;
}

public class FearMask : Mask<FearMask> {

    public event EventHandler OnMaskDefeated;

    [SerializeField] private FearMaskDialogues fearMaskDialogues;
    [SerializeField] private int timesToObserve_min;
    [SerializeField] private int timesToObserve_max;

    private bool successfullyObserved = false;
    private int numberOfTimesObserved = 0;
    private int timesToObserve;

    protected override void Awake() {
        base.Awake();
        timesToObserve = Random.Range(timesToObserve_min, timesToObserve_max + 1);
    }

    protected override void CombatPanelManager_OnStartAnimationsOver(object sender, EventArgs e) {
        InitialDialogue();
    }

    protected override void CombatPanelManager_OnAttackButtonPressed(object sender, EventArgs e) {
        if (successfullyObserved) {
            SuccessfullyObservedSequence();
            return;
        }
        AttackSequence();
    }

    protected override void CombatPanelManager_OnDefendButtonPressed(object sender, EventArgs e) {
        if (successfullyObserved) {
            SuccessfullyObservedSequence();
            return;
        }
        DefendSequence();
    }

    protected override void CombatPanelManager_OnObserveButtonPressed(object sender, EventArgs e) {
        if (successfullyObserved) {
            SuccessfullyObservedSequence();
            return;
        }
        IncrementNumberOfTimesObserved();
        ObserveSequence();
    }

    protected override void CombatPanelManager_OnTalkButtonPressed(object sender, EventArgs e) {
        if (successfullyObserved) {
            SuccessfullyObservedSequence();
            return;
        }
        TalkSequence();
    }

    protected override void CombatPanelManager_OnMaskDefeated(object sender, EventArgs e) {
        StartCoroutine(DefeatSequence());
    }

    private void InitialDialogue() {
        StartCoroutine(StringChain(fearMaskDialogues.introDialogue));
    }
    
    private void AttackSequence() {
        StartCoroutine(StringSingle(fearMaskDialogues.attackFeedbackDialogue));
        StartCoroutine(AttackPlayer());
    }

    private void DefendSequence() {
        StartCoroutine(StringSingle(fearMaskDialogues.defendFeedbackDialogue));
        StartCoroutine(AttackPlayer());
    }

    private void ObserveSequence() {
        StartCoroutine(StringSingle(fearMaskDialogues.observeFeedbackDialogue));
        StartCoroutine(AttackPlayer());

        CombatPanelManager.INSTANCE.AddToActionLog("<color=green>FearMask's attacks seems more clear!</color>");
    }

    private void TalkSequence() {
        StartCoroutine(StringChain(fearMaskDialogues.talkFeedbackDialogue));
        StartCoroutine(AttackPlayer());
    }

    private IEnumerator DefeatSequence() {
        StartCoroutine(StringChain(fearMaskDialogues.defeatDialogue));
        
        yield return new WaitForSeconds(defeatTime);
        OnMaskDefeated?.Invoke(this, EventArgs.Empty);
    }

    private void SuccessfullyObservedSequence() {
        StartCoroutine(StringSingle(fearMaskDialogues.successfulObserveDialogue));

        CombatPanelManager.INSTANCE.AddToActionLog("<color=green>FearMask cannot attack as it has been fully observed!</color>");
    }

    private void IncrementNumberOfTimesObserved() {
        numberOfTimesObserved++;
        if(numberOfTimesObserved == timesToObserve) {
            successfullyObserved = true;
        }
    }

    public bool IsSuccessfullyObserved() {
        return successfullyObserved;
    }

}
