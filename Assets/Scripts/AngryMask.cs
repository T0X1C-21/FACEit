using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public struct AngryMaskDialogues {
    public string[] introDialogue;
    public string[] attackFeedbackDialogue;
    public string[] defendFeedbackDialogue;
    public string[] observeFeedbackDialogue;
    public string[] talkFeedbackDialogue;
    public string[] zeroRageMeterDialogue;
    public string[] defeatDialogue;
}

public class AngryMask : Mask<AngryMask> {

    public event EventHandler OnIntroDialoguesFinished;
    public event EventHandler OnMaskDefeated;

    [SerializeField] private Slider angryMaskRageSlider;
    [SerializeField] private float maxRage;
    [SerializeField] private float rageIncreaseAmount;
    [SerializeField] private AngryMaskDialogues angryMaskDialoguesData;

    private bool zeroRageMode = false;
    private float currentRage;

    protected override void Awake() {
        base.Awake();
        currentRage = maxRage / 2.0f;
        angryMaskRageSlider.value = Mathf.InverseLerp(0.0f, maxRage, currentRage);
        maskDamage *= angryMaskRageSlider.value * 2.0f;
    }

    protected override void CombatPanelManager_OnAttackButtonPressed(object sender, EventArgs e) {
        if (zeroRageMode) {
            ZeroRageModeSequence();
            return;
        }
        AttackSequence();
    }

    protected override void CombatPanelManager_OnDefendButtonPressed(object sender, EventArgs e) {
        if (zeroRageMode) {
            ZeroRageModeSequence();
            return;
        }
        DefendSequence();
    }

    protected override void CombatPanelManager_OnObserveButtonPressed(object sender, EventArgs e) {
        if (zeroRageMode) {
            ZeroRageModeSequence();
            return;
        }
        ObserveSequence();
    }

    protected override void CombatPanelManager_OnTalkButtonPressed(object sender, EventArgs e) {
        if (zeroRageMode) {
            ZeroRageModeSequence();
            return;
        }
        TalkSequence();
    }

    protected override void CombatPanelManager_OnMaskDefeated(object sender, EventArgs e) {
        StartCoroutine(DefeatSequence());
    }

    protected override void CombatPanelManager_OnStartAnimationsOver(object sender, EventArgs e) {
        InitialDialogue();
    }

    private void InitialDialogue() {
        StartCoroutine(StringChain(angryMaskDialoguesData.introDialogue));
        OnIntroDialoguesFinished?.Invoke(this, EventArgs.Empty);
    }

    private void AttackSequence() {
        StartCoroutine(StringSingle(angryMaskDialoguesData.attackFeedbackDialogue));
        StartCoroutine(IncreaseRageMeter());

        StartCoroutine(AttackPlayer());
    }

    private IEnumerator IncreaseRageMeter() {
        float previousRage = currentRage;
        currentRage += rageIncreaseAmount;

        float t = 0f;
        while(t < 1f) {
            t += Time.deltaTime * 2f;
            float rageValue;
            rageValue = Mathf.Lerp(previousRage, currentRage, t);
            angryMaskRageSlider.value = Mathf.InverseLerp(0.0f, maxRage, rageValue);
            yield return null;
        }

        angryMaskRageSlider.value = Mathf.InverseLerp(0.0f, maxRage, currentRage);
        maskDamage *= angryMaskRageSlider.value * 2.0f;

        CombatPanelManager.INSTANCE.AddToActionLog("<color=red>AngryMask rage grows!</color>");
    }

    private IEnumerator DecreaseRageMeter() {
        float previousRage = currentRage;
        currentRage -= rageIncreaseAmount;
        if(currentRage <= 0.0f) {
            zeroRageMode = true;
            angryMaskRageSlider.value = 0.0f;
            yield break;
        } 

        float t = 0f;
        while(t < 1f) {
            t += Time.deltaTime * 2f;
            float rageValue;
            rageValue = Mathf.Lerp(previousRage, currentRage, t);
            angryMaskRageSlider.value = Mathf.InverseLerp(0.0f, maxRage, rageValue);
            yield return null;
        }

        angryMaskRageSlider.value = Mathf.InverseLerp(0.0f, maxRage, currentRage);
        maskDamage *= angryMaskRageSlider.value * 2.0f;

        CombatPanelManager.INSTANCE.AddToActionLog("<color=green>AngryMask calms down!</color>");
    }

    private void ZeroRageModeSequence() {
        StartCoroutine(StringSingle(angryMaskDialoguesData.zeroRageMeterDialogue));

        CombatPanelManager.INSTANCE.AddToActionLog("<color=green>AngryMask cannot attack as it has lost its rage!</color>");
    }

    private void DefendSequence() {
        StartCoroutine(StringSingle(angryMaskDialoguesData.defendFeedbackDialogue));
        StartCoroutine(DecreaseRageMeter());

        StartCoroutine(AttackPlayer());
    }

    private void ObserveSequence() {
        StartCoroutine(StringSingle(angryMaskDialoguesData.observeFeedbackDialogue));
        StartCoroutine(DecreaseRageMeter());

        StartCoroutine(AttackPlayer());
    }

    private void TalkSequence() {
        StartCoroutine(StringChain(angryMaskDialoguesData.talkFeedbackDialogue));
        
        StartCoroutine(AttackPlayer());
    }

    private IEnumerator DefeatSequence() {
        StartCoroutine(StringChain(angryMaskDialoguesData.defeatDialogue));
        
        yield return new WaitForSeconds(defeatTime);
        OnMaskDefeated?.Invoke(this, EventArgs.Empty);
    }
    
}
