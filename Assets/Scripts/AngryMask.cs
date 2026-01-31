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
    [SerializeField] private float animationTime;

    private RectTransform maskRectTransform;

    private bool zeroRageMode = false;
    private float currentRage;

    protected override void Awake() {
        base.Awake();
        currentRage = maxRage / 2.0f;
        angryMaskRageSlider.value = Mathf.InverseLerp(0.0f, maxRage, currentRage);
        maskDamage *= angryMaskRageSlider.value * 2.0f;

        maskRectTransform = this.GetComponent<RectTransform>();
        Tween upAnimation = maskRectTransform.DOAnchorPosY(-5f, 0.5f);
        Tween downAnimation = maskRectTransform.DOAnchorPosY(-25f, 0.5f);
        Tween resetAnimation = maskRectTransform.DOAnchorPosY(-15f, 0.5f);

        Sequence animationSequence = DOTween.Sequence();
        animationSequence.Append(upAnimation);
        animationSequence.Append(downAnimation);
        animationSequence.Append(resetAnimation);
        animationSequence.SetLoops(-1);
    }

    private void Start() {
        CombatPanelManager.INSTANCE.OnAttackButtonPressed += CombatPanelManager_OnAttackButtonPressed;
        CombatPanelManager.INSTANCE.OnDefendButtonPressed += CombatPanelManager_OnDefendButtonPressed;
        CombatPanelManager.INSTANCE.OnObserveButtonPressed += CombatPanelManager_OnObserveButtonPressed;
        CombatPanelManager.INSTANCE.OnTalkButtonPressed += CombatPanelManager_OnTalkButtonPressed;
        CombatPanelManager.INSTANCE.OnMaskDefeated += CombatPanelManager_OnMaskDefeated;
        CombatPanelManager.INSTANCE.OnStartAnimationsOver += CombatPanelManager_OnStartAnimationsOver;
    }

    private void CombatPanelManager_OnStartAnimationsOver(object sender, EventArgs e) {
        StartCoroutine(InitialDialogue());
    }

    private void CombatPanelManager_OnMaskDefeated(object sender, EventArgs e) {
        StartCoroutine(DefeatSequence());
    }

    private void DialogueFadeInAnimation(CanvasGroup canvasGroup) {
        canvasGroup.DOFade(1f, animationTime).SetEase(Ease.Linear);
    }

    private IEnumerator DefeatSequence() {
        CombatPanelManager.INSTANCE.DisableCanPressButtons();

        foreach(string text in angryMaskDialoguesData.defeatDialogue) {
            float cooldownBetweenDialogues = Random.Range(cooldownBetweenDialogues_min, cooldownBetweenDialogues_max);
            yield return new WaitForSeconds(cooldownBetweenDialogues);

            TextMeshProUGUI dialogueTMP = GetRandomDialogueTMP();
            dialogueTMP.text = text;
            DialogueFadeInAnimation(dialogueTMP.GetComponent<CanvasGroup>());

            float dialogueWaitTime = Random.Range(dialogueWaitTime_min, dialogueWaitTime_max);
            yield return new WaitForSeconds(dialogueWaitTime);
            DialogueFadeOutAnimation(dialogueTMP.GetComponent<CanvasGroup>());
        }
        
        yield return new WaitForSeconds(defeatTime);
        OnMaskDefeated?.Invoke(this, EventArgs.Empty);
    }

    private void CombatPanelManager_OnTalkButtonPressed(object sender, EventArgs e) {
        if (zeroRageMode) {
            StartCoroutine(ZeroRageModeSequence());
            return;
        }

        StartCoroutine(TalkSequence());
    }

    private IEnumerator TalkSequence() {
        CombatPanelManager.INSTANCE.DisableCanPressButtons();

        foreach(string text in angryMaskDialoguesData.talkFeedbackDialogue) {
            TextMeshProUGUI dialogueTMP = GetRandomDialogueTMP();
            dialogueTMP.text = text;
            DialogueFadeInAnimation(dialogueTMP.GetComponent<CanvasGroup>());

            float dialogueWaitTime = Random.Range(dialogueWaitTime_min, dialogueWaitTime_max);
            yield return new WaitForSeconds(dialogueWaitTime);
            DialogueFadeOutAnimation(dialogueTMP.GetComponent<CanvasGroup>());
        }
        
        StartCoroutine(AttackPlayer());
    }

    private void CombatPanelManager_OnObserveButtonPressed(object sender, EventArgs e) {
        if (zeroRageMode) {
            StartCoroutine(ZeroRageModeSequence());
            return;
        }

        StartCoroutine(ObserveSequence());
    }

    private IEnumerator ObserveSequence() {
        CombatPanelManager.INSTANCE.DisableCanPressButtons();

        TextMeshProUGUI dialogueTMP = GetRandomDialogueTMP();
        int randomIndex = Random.Range(0, angryMaskDialoguesData.observeFeedbackDialogue.Length);
        string randomAttackFeedbackText = angryMaskDialoguesData.observeFeedbackDialogue[randomIndex];
        dialogueTMP.text = randomAttackFeedbackText;
        DialogueFadeInAnimation(dialogueTMP.GetComponent<CanvasGroup>());

        float dialogueWaitTime = Random.Range(dialogueWaitTime_min, dialogueWaitTime_max);
        yield return new WaitForSeconds(dialogueWaitTime);
        DialogueFadeOutAnimation(dialogueTMP.GetComponent<CanvasGroup>());

        StartCoroutine(AttackPlayer());
    }

    private void CombatPanelManager_OnAttackButtonPressed(object sender, EventArgs e) {
        if (zeroRageMode) {
            StartCoroutine(ZeroRageModeSequence());
            return;
        }

        StartCoroutine(AttackSequence());
    }

    private IEnumerator ZeroRageModeSequence() {
        CombatPanelManager.INSTANCE.DisableCanPressButtons();

        TextMeshProUGUI dialogueTMP = GetRandomDialogueTMP();
        int randomIndex = Random.Range(0, angryMaskDialoguesData.zeroRageMeterDialogue.Length);
        string randomAttackFeedbackText = angryMaskDialoguesData.zeroRageMeterDialogue[randomIndex];
        dialogueTMP.text = randomAttackFeedbackText;
        DialogueFadeInAnimation(dialogueTMP.GetComponent<CanvasGroup>());

        float dialogueWaitTime = Random.Range(dialogueWaitTime_min, dialogueWaitTime_max);
        yield return new WaitForSeconds(dialogueWaitTime);
        DialogueFadeOutAnimation(dialogueTMP.GetComponent<CanvasGroup>());

        CombatPanelManager.INSTANCE.AddToActionLog("<color=green>AngryMask cannot attack as it has lost its rage!</color>");
        CombatPanelManager.INSTANCE.EnableCanPressButtons();
    }

    private IEnumerator AttackSequence() {
        CombatPanelManager.INSTANCE.DisableCanPressButtons();
        IncreaseRageMeter();

        TextMeshProUGUI dialogueTMP = GetRandomDialogueTMP();
        int randomIndex = Random.Range(0, angryMaskDialoguesData.attackFeedbackDialogue.Length);
        string randomAttackFeedbackText = angryMaskDialoguesData.attackFeedbackDialogue[randomIndex];
        dialogueTMP.text = randomAttackFeedbackText;
        DialogueFadeInAnimation(dialogueTMP.GetComponent<CanvasGroup>());

        float dialogueWaitTime = Random.Range(dialogueWaitTime_min, dialogueWaitTime_max);
        yield return new WaitForSeconds(dialogueWaitTime);
        DialogueFadeOutAnimation(dialogueTMP.GetComponent<CanvasGroup>());

        StartCoroutine(AttackPlayer());
    }

    private void IncreaseRageMeter() {
        currentRage += rageIncreaseAmount;
        angryMaskRageSlider.value = Mathf.InverseLerp(0.0f, maxRage, currentRage);
        maskDamage *= angryMaskRageSlider.value * 2.0f;

        CombatPanelManager.INSTANCE.AddToActionLog("<color=red>AngryMask rage grows!</color>");
    }

    private IEnumerator AttackPlayer() {
        float attackCooldown = Random.Range(attackCooldown_min, attackCooldown_max);
        yield return new WaitForSeconds(attackCooldown);
        CombatPanelManager.INSTANCE.DealDamageToPlayer(maskDamage);
        CombatPanelManager.INSTANCE.EnableCanPressButtons();
    }

    private void CombatPanelManager_OnDefendButtonPressed(object sender, EventArgs e) {
        if (zeroRageMode) {
            StartCoroutine(ZeroRageModeSequence());
            return;
        }

        StartCoroutine(DefendSequence());
    }

    private IEnumerator DefendSequence() {
        CombatPanelManager.INSTANCE.DisableCanPressButtons();
        DecreaseRageMeter();

        TextMeshProUGUI dialogueTMP = GetRandomDialogueTMP();
        int randomIndex = Random.Range(0, angryMaskDialoguesData.defendFeedbackDialogue.Length);
        string randomDefendFeedbackText = angryMaskDialoguesData.defendFeedbackDialogue[randomIndex];
        dialogueTMP.text = randomDefendFeedbackText;
        DialogueFadeInAnimation(dialogueTMP.GetComponent<CanvasGroup>());

        float dialogueWaitTime = Random.Range(dialogueWaitTime_min, dialogueWaitTime_max);
        yield return new WaitForSeconds(dialogueWaitTime);
        DialogueFadeOutAnimation(dialogueTMP.GetComponent<CanvasGroup>());

        StartCoroutine(AttackPlayer());
    }

    private void DecreaseRageMeter() {
        currentRage -= rageIncreaseAmount;
        if(currentRage <= 0.0f) {
            zeroRageMode = true;
            angryMaskRageSlider.value = 0.0f;
        } else {
            angryMaskRageSlider.value = Mathf.InverseLerp(0.0f, maxRage, currentRage);
            maskDamage *= angryMaskRageSlider.value * 2.0f;
            CombatPanelManager.INSTANCE.AddToActionLog("<color=green>AngryMask calms down!</color>");
        }
    }

    private IEnumerator InitialDialogue() {
        foreach(string text in angryMaskDialoguesData.introDialogue) {
            float cooldownBetweenDialogues = Random.Range(cooldownBetweenDialogues_min, cooldownBetweenDialogues_max);
            yield return new WaitForSeconds(cooldownBetweenDialogues);

            TextMeshProUGUI dialogueTMP = GetRandomDialogueTMP();
            dialogueTMP.text = text;
            DialogueFadeInAnimation(dialogueTMP.GetComponent<CanvasGroup>());

            float dialogueWaitTime = Random.Range(dialogueWaitTime_min, dialogueWaitTime_max);
            yield return new WaitForSeconds(dialogueWaitTime);
            DialogueFadeOutAnimation(dialogueTMP.GetComponent<CanvasGroup>());
        }
        OnIntroDialoguesFinished?.Invoke(this, EventArgs.Empty);
    }

    private TextMeshProUGUI GetRandomDialogueTMP() {
        int i = Random.Range(0, 2);
        return (i == 1) ? dialogueBoxOneTMP : dialogueBoxTwoTMP;
    }

    private void DialogueFadeOutAnimation(CanvasGroup canvasGroup) {
        canvasGroup.DOFade(0f, animationTime).SetEase(Ease.Linear);
    }
    
}
