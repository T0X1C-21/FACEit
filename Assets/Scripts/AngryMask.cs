using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

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
        maskDamage = maskDamage * (angryMaskRageSlider.value * 2.0f);
    }

    protected override void CombatPanelManager_OnAttackButtonPressed(object sender, EventArgs e) {
        if (zeroRageMode) {
            ZeroRageModeSequence();
            return;
        }

        CombatPanelManager.INSTANCE.AddToActionLog($"<color=#FFFFC5>The AngerMask's rage increases!.</color>");
        CombatPanelManager.INSTANCE.AddToActionLog("");

        StartCoroutine(AttackSequence());
    }

    protected override void CombatPanelManager_OnDefendButtonPressed(object sender, EventArgs e) {
        if (zeroRageMode) {
            ZeroRageModeSequence();
            return;
        }

        CombatPanelManager.INSTANCE.AddToActionLog($"<color=#FFFFC5>The AngerMask's rage decreases!.</color>");
        CombatPanelManager.INSTANCE.AddToActionLog("");

        DefendSequence();
    }

    protected override void CombatPanelManager_OnObserveButtonPressed(object sender, EventArgs e) {
        if (zeroRageMode) {
            ZeroRageModeSequence();
            return;
        }

        CombatPanelManager.INSTANCE.AddToActionLog($"<color=#ADD8E6>---------- TURN {CombatPanelManager.turnNumber++} ----------</color>");
        CombatPanelManager.INSTANCE.AddToActionLog($"<color=#32CD32>You observed the AngerMask.</color>");
        CombatPanelManager.INSTANCE.AddToActionLog("");

        CombatPanelManager.INSTANCE.AddToActionLog($"<color=#FFFFC5>The AngerMask's rage decreases!.</color>");
        CombatPanelManager.INSTANCE.AddToActionLog("");

        ObserveSequence();
    }

    protected override void CombatPanelManager_OnTalkButtonPressed(object sender, EventArgs e) {
        if (zeroRageMode) {
            ZeroRageModeSequence();
            return;
        }

        CombatPanelManager.INSTANCE.AddToActionLog($"<color=#ADD8E6>---------- TURN {CombatPanelManager.turnNumber++} ----------</color>");
        CombatPanelManager.INSTANCE.AddToActionLog($"<color=#32CD32>You talk to the AngerMask.</color>");
        CombatPanelManager.INSTANCE.AddToActionLog("");

        CombatPanelManager.INSTANCE.AddToActionLog($"<color=#FFFFC5>The AngerMask's rage decreases!.</color>");
        CombatPanelManager.INSTANCE.AddToActionLog("");

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
    }

    private IEnumerator AttackSequence() {
        StartCoroutine(IncreaseRageMeter());
        yield return new WaitForSeconds(2f);
        StartCoroutine(StringSingle(angryMaskDialoguesData.attackFeedbackDialogue));

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

    }

    private IEnumerator DecreaseRageMeter() {
        float previousRage = currentRage;
        currentRage -= rageIncreaseAmount;
        if(currentRage <= 0.0f) {
            zeroRageMode = true;
            angryMaskRageSlider.value = 0.0f;
            typeWritingSpeed = 0.2f;
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
    }

    private void ZeroRageModeSequence() {

        CombatPanelManager.INSTANCE.AddToActionLog("<color=#CB4C4F>The AngerMask cannot attack as it's rage exhausted!</color>");
        CombatPanelManager.INSTANCE.AddToActionLog("<color=#ADD8E6>---------------------------------</color>");

        StartCoroutine(StringSingle(angryMaskDialoguesData.zeroRageMeterDialogue));
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
        StartCoroutine(DecreaseRageMeter());
        
        StartCoroutine(AttackPlayer());
    }

    private IEnumerator DefeatSequence() {
        StartCoroutine(StringChain(angryMaskDialoguesData.defeatDialogue));
        
        yield return new WaitForSeconds(defeatTime);
        OnMaskDefeated?.Invoke(this, EventArgs.Empty);
    }
    
}
