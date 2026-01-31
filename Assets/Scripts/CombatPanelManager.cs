using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum MaskType {
    AngryMask
}

public class CombatPanelManager : MonoBehaviour {

    public static CombatPanelManager INSTANCE;

    public event EventHandler OnAttackButtonPressed;
    public event EventHandler OnDefendButtonPressed;
    public event EventHandler OnObserveButtonPressed;
    public event EventHandler OnTalkButtonPressed;
    public event EventHandler OnMaskDefeated;
    public event EventHandler OnStartAnimationsOver;

    [SerializeField] private Button attackButton;
    [SerializeField] private Button defendButton;
    [SerializeField] private Button observeButton;
    [SerializeField] private Button talkButton;
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private Slider maskHealthSlider;
    [SerializeField] private Slider rageMeterSlider;
    [SerializeField] private float playerDamage;
    [SerializeField] private float maxPlayerHealth;
    [SerializeField] private float maxMaskHealth;
    [SerializeField] private MaskType maskType;
    [SerializeField] private TextMeshProUGUI actionLogTMP;
    [SerializeField] private Scrollbar actionLogScrollBar;
    [SerializeField] private CanvasGroup[] canvasGroupToFadeArray;
    [SerializeField] private float animationTime;
    [SerializeField] private float stepAnimationTime;

    private RectTransform playerHealthSliderRectTransform;
    private RectTransform maskHealthSliderRectTransform;
    private RectTransform rageMeterSliderRectTransform;
    private RectTransform attackButtonRectTransform;
    private RectTransform defendButtonRectTransform;
    private RectTransform observeButtonRectTransform;
    private RectTransform talkButtonRectTransform;
    private RectTransform actionLogScrollViewRectTransform;

    private bool canPressButtons = false;
    private float playerHealth;
    private float maskHealth;
    private float nextPlayerDamageNullifier = 0.0f;
    private string actionLog;
    
    private void Awake() {
        INSTANCE = this;

        attackButton.onClick.AddListener(OnAttackButtonClick);
        defendButton.onClick.AddListener(OnDefendButtonClick);
        observeButton.onClick.AddListener(OnObserveButtonClick);
        talkButton.onClick.AddListener(OnTalkButtonClick);

        playerHealth = maxPlayerHealth;
        maskHealth = maxMaskHealth;

        playerHealthSlider.value = playerHealth;
        maskHealthSlider.value = maskHealth;

        playerHealthSliderRectTransform = playerHealthSlider.GetComponent<RectTransform>();
        maskHealthSliderRectTransform = maskHealthSlider.GetComponent<RectTransform>();
        if(maskType == MaskType.AngryMask) {
            rageMeterSliderRectTransform = rageMeterSlider.GetComponent<RectTransform>();
        }
        attackButtonRectTransform = attackButton.GetComponent<RectTransform>(); 
        defendButtonRectTransform = defendButton.GetComponent<RectTransform>();
        observeButtonRectTransform = observeButton.GetComponent<RectTransform>();
        talkButtonRectTransform = talkButton.GetComponent<RectTransform>();
        actionLogScrollViewRectTransform = actionLogScrollBar.GetComponent<RectTransform>();
    }

    private void Start() {
        switch (maskType) {
            case MaskType.AngryMask:
                AngryMask.INSTANCE.OnIntroDialoguesFinished += OnIntroDialoguesFinished;
                AngryMask.INSTANCE.OnMaskDefeated += OnMaskDefeatedGameOver;
                break;
        }
    }

    private void OnMaskDefeatedGameOver(object sender, EventArgs e) {
        playerHealthSliderRectTransform.DOScale(Vector3.zero, animationTime).SetEase(Ease.InElastic);
        maskHealthSliderRectTransform.DOScale(Vector3.zero, animationTime + (1 * stepAnimationTime)).SetEase(Ease.InElastic);
        if(maskType == MaskType.AngryMask) {
            rageMeterSliderRectTransform.DOScale(Vector3.zero, animationTime + (1 * stepAnimationTime)).SetEase(Ease.InElastic);
        }
        actionLogScrollViewRectTransform.DOScale(Vector3.zero, animationTime + (2 * stepAnimationTime)).SetEase(Ease.InElastic);
        attackButtonRectTransform.DOAnchorPosY(-275f, animationTime+ (3 * stepAnimationTime)).SetEase(Ease.InElastic);
        defendButtonRectTransform.DOAnchorPosY(-275f, animationTime+ (4 * stepAnimationTime)).SetEase(Ease.InElastic);
        observeButtonRectTransform.DOAnchorPosY(-412.5f, animationTime+ (5 * stepAnimationTime)).SetEase(Ease.InElastic);
        talkButtonRectTransform.DOAnchorPosY(-412.5f, animationTime+ (6 * stepAnimationTime)).SetEase(Ease.InElastic).
            OnComplete(() => {
                StartCoroutine(GameManager.INSTANCE.ChangeLevelState(LevelState.Message));
            });

        foreach(CanvasGroup canvasGroup in canvasGroupToFadeArray) {
            canvasGroup.DOFade(0f, animationTime).SetEase(Ease.Linear);
        }
    }

    private void OnEnable() {
        // reset
        playerHealthSliderRectTransform.localScale = Vector3.zero;
        maskHealthSliderRectTransform.localScale = Vector3.zero;
        if(maskType == MaskType.AngryMask) {
            rageMeterSliderRectTransform.localScale = Vector3.zero;
        }
        actionLogScrollViewRectTransform.localScale = Vector3.zero;
        attackButtonRectTransform.anchoredPosition = new Vector2(attackButtonRectTransform.anchoredPosition.x, -275f);
        defendButtonRectTransform.anchoredPosition = new Vector2(defendButtonRectTransform.anchoredPosition.x, -275f);
        observeButtonRectTransform.anchoredPosition = new Vector2(observeButtonRectTransform.anchoredPosition.x, -412.5f);
        talkButtonRectTransform.anchoredPosition = new Vector2(talkButtonRectTransform.anchoredPosition.x, -412.5f);

        foreach(CanvasGroup canvasGroup in canvasGroupToFadeArray) {
            canvasGroup.alpha = 0f;
        }

        // animate
        playerHealthSliderRectTransform.DOScale(Vector3.one, animationTime).SetEase(Ease.OutElastic);
        maskHealthSliderRectTransform.DOScale(Vector3.one, animationTime + (1 * stepAnimationTime)).SetEase(Ease.OutElastic);
        if(maskType == MaskType.AngryMask) {
            rageMeterSliderRectTransform.DOScale(Vector3.one, animationTime + (1 * stepAnimationTime)).SetEase(Ease.OutElastic);
        }
        actionLogScrollViewRectTransform.DOScale(Vector3.one, animationTime + (2 * stepAnimationTime)).SetEase(Ease.OutElastic);
        attackButtonRectTransform.DOAnchorPosY(12.5f, animationTime+ (3 * stepAnimationTime)).SetEase(Ease.OutElastic);
        defendButtonRectTransform.DOAnchorPosY(12.5f, animationTime+ (4 * stepAnimationTime)).SetEase(Ease.OutElastic);
        observeButtonRectTransform.DOAnchorPosY(-137.5f, animationTime+ (5 * stepAnimationTime)).SetEase(Ease.OutElastic);
        talkButtonRectTransform.DOAnchorPosY(-137.5f, animationTime+ (6 * stepAnimationTime)).SetEase(Ease.OutElastic).
            OnComplete(() => {
                OnStartAnimationsOver?.Invoke(this, EventArgs.Empty);
            });

        foreach(CanvasGroup canvasGroup in canvasGroupToFadeArray) {
            canvasGroup.DOFade(1f, animationTime).SetEase(Ease.Linear);
        }
    }

    private void OnIntroDialoguesFinished(object sender, System.EventArgs e) => EnableCanPressButtons();

    public void EnableCanPressButtons() => canPressButtons = true;
    public void DisableCanPressButtons() => canPressButtons = false;

    private void OnAttackButtonClick() {
        if(!canPressButtons)
            return;

        int randomNumber = Random.Range(1, 11);
        float randomDamage;
        if(randomNumber < 4) { // 1, 2, 3
            randomDamage = playerDamage * 2.5f;
            AddToActionLog($"<color=orange>Considerable damage applied for {randomDamage.ToString("F1")}!</color>");
        } else if(randomNumber < 9) { // 4, 5, 6, 7, 8
            randomDamage = playerDamage * 1.0f;
            AddToActionLog($"<color=green>Moderate damage applied for {randomDamage.ToString("F1")}!</color>");
        } else if(randomNumber < 10){ // 9
            randomDamage = playerDamage * 0.0f;
            AddToActionLog("<color=red>Attack failed!</color>");
        } else { // 10
            randomDamage = playerDamage * 5.0f;
            AddToActionLog($"<color=yellow>Critical damage applied for {randomDamage.ToString("F1")}!</color>");
        }
        
        maskHealth -= randomDamage;
        maskHealthSlider.value = Mathf.InverseLerp(0f, maxMaskHealth, maskHealth);
        if(maskHealth <= 0f) {
            OnMaskDefeated?.Invoke(this, EventArgs.Empty);
            return;
        }
        OnAttackButtonPressed?.Invoke(this, EventArgs.Empty);
    }

    public void DealDamageToPlayer(float damage) {
        float maskDamage = damage * (1.0f - nextPlayerDamageNullifier);
        playerHealth -= maskDamage;
        nextPlayerDamageNullifier = 0.0f;

        AddToActionLog($"<color=purple>Received damage from {maskType.ToString()} for {maskDamage.ToString("F1")}!</color>");

        playerHealthSlider.value = Mathf.InverseLerp(0f, maxPlayerHealth, playerHealth);
        if(playerHealth <= 0f) {
            PlayerDefeatAnimation();
            return;
        }
    }

    private void PlayerDefeatAnimation() {
        playerHealthSliderRectTransform.DOScale(Vector3.zero, animationTime).SetEase(Ease.InElastic);
        maskHealthSliderRectTransform.DOScale(Vector3.zero, animationTime + (1 * stepAnimationTime)).SetEase(Ease.InElastic);
        if(maskType == MaskType.AngryMask) {
            rageMeterSliderRectTransform.DOScale(Vector3.zero, animationTime + (1 * stepAnimationTime)).SetEase(Ease.InElastic);
        }
        actionLogScrollViewRectTransform.DOScale(Vector3.zero, animationTime + (2 * stepAnimationTime)).SetEase(Ease.InElastic);
        attackButtonRectTransform.DOAnchorPosY(-275f, animationTime+ (3 * stepAnimationTime)).SetEase(Ease.InElastic);
        defendButtonRectTransform.DOAnchorPosY(-275f, animationTime+ (4 * stepAnimationTime)).SetEase(Ease.InElastic);
        observeButtonRectTransform.DOAnchorPosY(-412.5f, animationTime+ (5 * stepAnimationTime)).SetEase(Ease.InElastic);
        talkButtonRectTransform.DOAnchorPosY(-412.5f, animationTime+ (6 * stepAnimationTime)).SetEase(Ease.InElastic).
            OnComplete(() => {
                StartCoroutine(GameManager.INSTANCE.ReloadLevelAnimation());
            });

        foreach(CanvasGroup canvasGroup in canvasGroupToFadeArray) {
            canvasGroup.DOFade(0f, animationTime).SetEase(Ease.Linear);
        }
    }

    private void OnDefendButtonClick() {
        if(!canPressButtons)
            return;

        int randomNumber = Random.Range(1, 11);
        if(randomNumber < 4) { // 1, 2, 3
            nextPlayerDamageNullifier = 1.0f;
            AddToActionLog("<color=green>Full defense applied</color>");
        } else if(randomNumber < 9) { // 4, 5, 6, 7, 8
            nextPlayerDamageNullifier = 0.5f;
            AddToActionLog("<color=orange>Half defense applied</color>");
        } else { // 9, 10
            nextPlayerDamageNullifier = 0.0f;
            AddToActionLog("<color=red>Failed to apply defense</color>");
        }

        OnDefendButtonPressed?.Invoke(this, EventArgs.Empty);
    }

    private void OnObserveButtonClick() {
        if(!canPressButtons)
            return;

        AddToActionLog($"<#964B00>Observing {maskType.ToString()}</color>");
        OnObserveButtonPressed?.Invoke(this, EventArgs.Empty);
    }

    private void OnTalkButtonClick() {
        if(!canPressButtons)
            return;

        AddToActionLog($"<color=blue>Talking to {maskType.ToString()}</color>");
        OnTalkButtonPressed?.Invoke(this, EventArgs.Empty);
    }

    public void AddToActionLog(string actionStatement) {
        actionLog += actionStatement;
        actionLog += "\n";
        actionLogTMP.text = actionLog;
        Canvas.ForceUpdateCanvases();
        actionLogScrollBar.value = 0.0f;
    }

}
