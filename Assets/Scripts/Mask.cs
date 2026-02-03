using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Mask<T> : MonoBehaviour where T : MonoBehaviour{

    public static T INSTANCE;

    [SerializeField] protected TextMeshProUGUI dialogueBoxOneTMP;
    [SerializeField] protected TextMeshProUGUI dialogueBoxTwoTMP;
    [SerializeField] protected float dialogueWaitTime_min;
    [SerializeField] protected float dialogueWaitTime_max;
    [SerializeField] protected float cooldownBetweenDialogues_min;
    [SerializeField] protected float cooldownBetweenDialogues_max;
    [SerializeField] protected float defeatTime;
    [SerializeField] protected float maskDamage;
    [SerializeField] protected float attackCooldown_min;
    [SerializeField] protected float attackCooldown_max;
    [SerializeField] protected float animationTime;
    [SerializeField] protected float typeWritingSpeed;

    protected RectTransform maskRectTransform;

    protected virtual void Awake() {
        INSTANCE = this as T;

        maskRectTransform = this.GetComponent<RectTransform>();
        float targetUp = maskRectTransform.anchoredPosition.y + 10f;
        float targetDown = maskRectTransform.anchoredPosition.y - 10f;
        float targetReset = maskRectTransform.anchoredPosition.y;
        Tween upAnimation = maskRectTransform.DOAnchorPosY(targetUp, 0.5f);
        Tween downAnimation = maskRectTransform.DOAnchorPosY(targetDown, 0.5f);
        Tween resetAnimation = maskRectTransform.DOAnchorPosY(targetReset, 0.5f);

        Sequence animationSequence = DOTween.Sequence();
        animationSequence.Append(upAnimation);
        animationSequence.Append(downAnimation);
        animationSequence.Append(resetAnimation);
        animationSequence.SetLoops(-1);
    }

    protected virtual void Start() {
        CombatPanelManager.INSTANCE.OnAttackButtonPressed += CombatPanelManager_OnAttackButtonPressed;
        CombatPanelManager.INSTANCE.OnDefendButtonPressed += CombatPanelManager_OnDefendButtonPressed;
        CombatPanelManager.INSTANCE.OnObserveButtonPressed += CombatPanelManager_OnObserveButtonPressed;
        CombatPanelManager.INSTANCE.OnTalkButtonPressed += CombatPanelManager_OnTalkButtonPressed;
        CombatPanelManager.INSTANCE.OnMaskDefeated += CombatPanelManager_OnMaskDefeated;
        CombatPanelManager.INSTANCE.OnStartAnimationsOver += CombatPanelManager_OnStartAnimationsOver;
    }

    protected abstract void CombatPanelManager_OnAttackButtonPressed(object sender, EventArgs e);
    protected abstract void CombatPanelManager_OnDefendButtonPressed(object sender, EventArgs e);
    protected abstract void CombatPanelManager_OnObserveButtonPressed(object sender, EventArgs e);
    protected abstract void CombatPanelManager_OnTalkButtonPressed(object sender, EventArgs e);
    protected abstract void CombatPanelManager_OnMaskDefeated(object sender, EventArgs e);
    protected abstract void CombatPanelManager_OnStartAnimationsOver(object sender, EventArgs e);

    protected TextMeshProUGUI GetRandomDialogueTMP() {
        int i = Random.Range(0, 2);
        return (i == 1) ? dialogueBoxOneTMP : dialogueBoxTwoTMP;
    }

    protected IEnumerator AttackPlayer() {
        float attackCooldown = Random.Range(attackCooldown_min, attackCooldown_max);
        yield return new WaitForSeconds(attackCooldown);
        StartCoroutine(CombatPanelManager.INSTANCE.DealDamageToPlayer(maskDamage));
    }

    protected void DialogueFadeInAnimation(CanvasGroup canvasGroup) {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, animationTime).SetEase(Ease.Linear);
    }

    protected void DialogueFadeOutAnimation(CanvasGroup canvasGroup) {
        canvasGroup.alpha = 1f;
        canvasGroup.DOFade(0f, animationTime).SetEase(Ease.Linear);
    }

    protected IEnumerator Typewriter(TextMeshProUGUI tmp, string text, Action onComplete) {
        DialogueFadeInAnimation(tmp.GetComponent<CanvasGroup>());
        string currentString = "";
        foreach(char ch in text) {
            if(!char.IsWhiteSpace(ch))
                AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.TypeWriter);

            currentString += ch;
            tmp.text = currentString;
            yield return new WaitForSeconds(typeWritingSpeed);
        }
        onComplete?.Invoke();
    }

    protected IEnumerator StringChain(string[] stringArray) {
        CombatPanelManager.INSTANCE.DisableCanPressButtons();
        TextMeshProUGUI dialogueTMP = GetRandomDialogueTMP();

        for(int i = 0; i < stringArray.Length; i++) {
            bool typeWritingOver = false;
            StartCoroutine(Typewriter(dialogueTMP, stringArray[i], () => {
                typeWritingOver = true;
            }));
            yield return new WaitUntil(() => typeWritingOver);

            if(i == stringArray.Length - 1) {
                continue;
            }

            float cooldownBetweenDialogues = Random.Range(cooldownBetweenDialogues_min, cooldownBetweenDialogues_max);
            yield return new WaitForSeconds(cooldownBetweenDialogues);
            DialogueFadeOutAnimation(dialogueTMP.GetComponent<CanvasGroup>());
        }

        float dialogueWaitTime = Random.Range(dialogueWaitTime_min, dialogueWaitTime_max);
        yield return new WaitForSeconds(dialogueWaitTime);
        DialogueFadeOutAnimation(dialogueTMP.GetComponent<CanvasGroup>());
        CombatPanelManager.INSTANCE.EnableCanPressButtons();
    }

    protected IEnumerator StringSingle(string[] stringArray) {
        CombatPanelManager.INSTANCE.DisableCanPressButtons();
        TextMeshProUGUI dialogueTMP = GetRandomDialogueTMP();

        int randomIndex = Random.Range(0, stringArray.Length);
        string randomAttackFeedbackText = stringArray[randomIndex];
        bool typeWritingOver = false;
        StartCoroutine(Typewriter(dialogueTMP, randomAttackFeedbackText, () => {
            typeWritingOver = true;
        }));
        yield return new WaitUntil(() => typeWritingOver);

        float dialogueWaitTime = Random.Range(dialogueWaitTime_min, dialogueWaitTime_max);
        yield return new WaitForSeconds(dialogueWaitTime);
        DialogueFadeOutAnimation(dialogueTMP.GetComponent<CanvasGroup>());
        CombatPanelManager.INSTANCE.EnableCanPressButtons();
    }

}
