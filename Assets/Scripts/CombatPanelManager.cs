using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum MaskType {
    AngerMask,
    FearMask,
    SadnessMask,
    DenialMask,
    EmptyMask
}

public enum ButtonType {
    Attack,
    Defend,
    Observe,
    Talk
}

public class CombatPanelManager : MonoBehaviour {

    public static CombatPanelManager INSTANCE;

    public event EventHandler OnAttackButtonPressed;
    public event EventHandler OnDefendButtonPressed;
    public event EventHandler OnObserveButtonPressed;
    public event EventHandler OnTalkButtonPressed;
    public event EventHandler OnAcceptButtonPressed;
    public event EventHandler OnMaskDefeated;
    public event EventHandler OnStartAnimationsOver;

    [SerializeField] private Button attackButton;
    [SerializeField] private Button defendButton;
    [SerializeField] private Button observeButton;
    [SerializeField] private Button talkButton;
    [SerializeField] private Button acceptButton;
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
    [SerializeField] private Color normalHighlightColor;
    [SerializeField] private Color disabledHighlightColor;
    [SerializeField] private float perButtonCooldown;

    private RectTransform playerHealthSliderRectTransform;
    private RectTransform maskHealthSliderRectTransform;
    private RectTransform rageMeterSliderRectTransform;
    private RectTransform attackButtonRectTransform;
    private RectTransform defendButtonRectTransform;
    private RectTransform observeButtonRectTransform;
    private RectTransform talkButtonRectTransform;
    private RectTransform actionLogScrollViewRectTransform;

    private bool canPressButtons;
    private float playerHealth;
    private float maskHealth;
    private float nextPlayerDamageNullifier = 0.0f;
    private string actionLog;

    private bool canPressAttackButton = true;
    private bool canPressDefendButton = true;
    private bool canPressObserveButton = true;
    private bool canPressTalkButton = true;
    private bool canPressAcceptButton = false;

    private void Awake() {
        INSTANCE = this;

        attackButton.onClick.AddListener(OnAttackButtonClick);
        defendButton.onClick.AddListener(OnDefendButtonClick);
        observeButton.onClick.AddListener(OnObserveButtonClick);
        talkButton.onClick.AddListener(OnTalkButtonClick);
        if(maskType == MaskType.EmptyMask) {
            acceptButton.onClick.AddListener(OnAcceptButtonClick);
            acceptButton.gameObject.SetActive(false);
        }

        playerHealth = maxPlayerHealth;
        maskHealth = maxMaskHealth;

        playerHealthSlider.value = playerHealth;
        maskHealthSlider.value = maskHealth;

        playerHealthSliderRectTransform = playerHealthSlider.GetComponent<RectTransform>();
        maskHealthSliderRectTransform = maskHealthSlider.GetComponent<RectTransform>();
        if(maskType == MaskType.AngerMask) {
            rageMeterSliderRectTransform = rageMeterSlider.GetComponent<RectTransform>();
        }
        attackButtonRectTransform = attackButton.GetComponent<RectTransform>(); 
        defendButtonRectTransform = defendButton.GetComponent<RectTransform>();
        observeButtonRectTransform = observeButton.GetComponent<RectTransform>();
        talkButtonRectTransform = talkButton.GetComponent<RectTransform>();
        actionLogScrollViewRectTransform = actionLogScrollBar.GetComponent<RectTransform>();

        DisableCanPressButtons();
    }

    private void Start() {
        switch (maskType) {
            case MaskType.AngerMask:
                AngryMask.INSTANCE.OnMaskDefeated += OnMaskDefeatedGameOver;
                break;
            case MaskType.FearMask:
                FearMask.INSTANCE.OnMaskDefeated += OnMaskDefeatedGameOver;
                break;
            case MaskType.SadnessMask:
                SadnessMask.INSTANCE.OnMaskDefeated += OnMaskDefeatedGameOver;
                break;
            case MaskType.DenialMask:
                DenialMask.INSTANCE.OnMaskDefeated += OnMaskDefeatedGameOver;
                break;
            case MaskType.EmptyMask:
                EmptyMask.INSTANCE.OnMaskDefeated += OnMaskDefeatedGameOver;
                EmptyMask.INSTANCE.OnEmptyMaskInteractionsComplete += OnEmptyMaskInteractionsComplete;
                break;
        }
    }

    private void OnEmptyMaskInteractionsComplete(object sender, EventArgs e) {
        StartCoroutine(ShowAcceptButton());
    }

    private IEnumerator ShowAcceptButton() {
        DisableCanPressButtons();
        yield return new WaitForSeconds(2f);
        acceptButton.gameObject.SetActive(true);
        canPressAcceptButton = true;
        attackButton.GetComponent<CanvasGroup>().DOFade(0f, 2f);
        defendButton.GetComponent<CanvasGroup>().DOFade(0f, 2f);
        observeButton.GetComponent<CanvasGroup>().DOFade(0f, 2f);
        talkButton.GetComponent<CanvasGroup>().DOFade(0f, 2f);
        yield return new WaitForSeconds(1f);
        acceptButton.GetComponent<CanvasGroup>().DOFade(1f, 3f).OnComplete(() => {
            EnableCanPressButtons();
        });
    }

    private void OnAcceptButtonClick() {
        if (!canPressAcceptButton) {
            return;
        }

        if (!canPressButtons) {
            AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonError);
            return;
        }

        OnAcceptButtonPressed?.Invoke(this, EventArgs.Empty);
        acceptButton.interactable = false;
        DisableCanPressButtons();
    }

    private void OnMaskDefeatedGameOver(object sender, EventArgs e) {
        StartCoroutine(LevelCloseAnimation());
    }

    private IEnumerator LevelCloseAnimation() {
        AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.DefeatedMask);
        yield return new WaitForSeconds(5f);

        playerHealthSliderRectTransform.DOScale(Vector3.zero, animationTime).SetEase(Ease.InElastic);
        maskHealthSliderRectTransform.DOScale(Vector3.zero, animationTime + (1 * stepAnimationTime)).SetEase(Ease.InElastic);
        if(maskType == MaskType.AngerMask) {
            rageMeterSliderRectTransform.DOScale(Vector3.zero, animationTime + (1 * stepAnimationTime)).SetEase(Ease.InElastic);
        }
        actionLogScrollViewRectTransform.DOScale(Vector3.zero, animationTime + (2 * stepAnimationTime)).SetEase(Ease.InElastic);
        if(maskType == MaskType.EmptyMask) {
            acceptButton.GetComponent<RectTransform>().DOAnchorPosY
                (-412.5f, animationTime+ (3 * stepAnimationTime)).SetEase(Ease.InElastic);
        }
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
        if(maskType == MaskType.AngerMask) {
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
        if(maskType == MaskType.AngerMask) {
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

    public void EnableCanPressButtons() {
        ColorBlock colorBlock = ColorBlock.defaultColorBlock;
        colorBlock.highlightedColor = normalHighlightColor;
        attackButton.colors = colorBlock;
        defendButton.colors = colorBlock;
        observeButton.colors = colorBlock;
        talkButton.colors = colorBlock;

        canPressButtons = true;
    }
    public void DisableCanPressButtons() {
        ColorBlock colorBlock = ColorBlock.defaultColorBlock;
        colorBlock.highlightedColor = disabledHighlightColor;
        attackButton.colors = colorBlock;
        defendButton.colors = colorBlock;
        observeButton.colors = colorBlock;
        talkButton.colors = colorBlock;

        canPressButtons = false;
    }

    private void OnAttackButtonClick() {
        if (!canPressButtons) {
            AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonError);
            return;
        }

        if (!canPressAttackButton) {
            AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonError);
            return;
        }
        canPressAttackButton = false;
        StartCoroutine(AttackAnimation());
        StartCoroutine(ButtonCooldown(ButtonType.Attack));
    }

    private IEnumerator ButtonCooldown(ButtonType buttonType) {
        yield return new WaitForSeconds(perButtonCooldown);
        switch (buttonType) {
            case ButtonType.Attack:
                canPressAttackButton = true;
                break;
            case ButtonType.Defend:
                canPressDefendButton = true;
                break;
            case ButtonType.Observe:
                canPressObserveButton = true;
                break;
            case ButtonType.Talk:
                canPressTalkButton = true;
                break;
        }
    }

    private IEnumerator AttackAnimation() {
        if (!canPressButtons) {
            AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonError);
            yield break;
        }

        AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonClick);

        float damageAmount = 0f;
        switch (maskType) {
            case MaskType.AngerMask:
                damageAmount = AngerMaskAttackCalculator();
                break;
            case MaskType.FearMask:
                damageAmount = FearMaskAttackCalculator();
                break;
            case MaskType.SadnessMask:
                damageAmount = SadnessMaskAttackCalculator();
                break;
            case MaskType.DenialMask:
                damageAmount = NormalAttackCalculator();
                break;
            case MaskType.EmptyMask:
                damageAmount = EmptyMaskAttackCalculator();
                break;
        }

        float previousHealth = maskHealth;
        maskHealth -= damageAmount;

        float t = 0f;
        while(t < 1f) {
            t += Time.deltaTime * 2f;
            float healthValue = Mathf.Lerp(previousHealth, maskHealth, t);
            maskHealthSlider.value = Mathf.InverseLerp(0f, maxMaskHealth, healthValue);
            yield return null;
        }

        maskHealthSlider.value = Mathf.InverseLerp(0f, maxMaskHealth, maskHealth);
        if(maskHealth <= 0f) {
            OnMaskDefeated?.Invoke(this, EventArgs.Empty);
            yield break;
        }
        OnAttackButtonPressed?.Invoke(this, EventArgs.Empty);
    }

    private float NormalAttackCalculator() {
        int randomNumber = Random.Range(1, 11);
        float randomDamage = 0f;
        if(randomNumber < 4) { // 1, 2, 3
            randomDamage = playerDamage * 2.5f;
            AddToActionLog($"<color=orange>Considerable damage applied for {randomDamage.ToString("F1")}!</color>");
        } else if(randomNumber < 9) { // 4, 5, 6, 7, 8
            randomDamage = playerDamage * 1.0f;
            AddToActionLog($"<color=green>Moderate damage applied for {randomDamage.ToString("F1")}!</color>");
        } else { // 9, 10
            randomDamage = playerDamage * 5.0f;
            AddToActionLog($"<color=yellow>Critical damage applied for {randomDamage.ToString("F1")}!</color>");
        }
        return randomDamage;
    }

    private float AngerMaskAttackCalculator() {
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
        return randomDamage;
    }

    private float FearMaskAttackCalculator() {
        if (!FearMask.INSTANCE.IsSuccessfullyObserved()) {
            AddToActionLog($"<color=red>Attacks seem to be missing!</color>");
            return 0f;
        }
        return NormalAttackCalculator();
    }

    private float SadnessMaskAttackCalculator() {
        if (!SadnessMask.INSTANCE.IsTalkedTo()) {
            int randomNumber = Random.Range(0, 3);
            if(randomNumber == 0)
                AddToActionLog($"<color=red>Attack failed!</color>");
            else
                AddToActionLog($"<color=green>Moderate damage applied for {playerDamage.ToString("F1")}!</color>");
            return (randomNumber == 0) ? 0f : playerDamage;
        }
        return NormalAttackCalculator();
    }

    private float EmptyMaskAttackCalculator() {
        AddToActionLog($"<color=green>Attack doesn't seem to be affecting the EmptyMask</color>");
        return 0f;
    }

    public IEnumerator DealDamageToPlayer(float damage) {
        float previousHealth = playerHealth;
        float maskDamage = damage * (1.0f - nextPlayerDamageNullifier);
        playerHealth -= maskDamage;
        nextPlayerDamageNullifier = 0.0f;

        AddToActionLog($"<color=purple>Received damage from {maskType.ToString()} for {maskDamage.ToString("F1")}!</color>");
        
        float t = 0f;
        while(t < 1f) {
            t += Time.deltaTime * 2f;
            float healthValue = Mathf.Lerp(previousHealth, playerHealth, t);
            playerHealthSlider.value = Mathf.InverseLerp(0f, maxPlayerHealth, healthValue);
            yield return null;
        }

        playerHealthSlider.value = Mathf.InverseLerp(0f, maxPlayerHealth, playerHealth);
        if(playerHealth <= 0f) {
            StartCoroutine(PlayerDefeatAnimation());
        }
    }

    private IEnumerator PlayerDefeatAnimation() {   
        AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.GameOver);
        yield return new WaitForSeconds(6f);

        playerHealthSliderRectTransform.DOScale(Vector3.zero, animationTime).SetEase(Ease.InElastic);
        maskHealthSliderRectTransform.DOScale(Vector3.zero, animationTime + (1 * stepAnimationTime)).SetEase(Ease.InElastic);
        if(maskType == MaskType.AngerMask) {
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
        if (!canPressButtons) {
            AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonError);
            return;
        }

        if (!canPressDefendButton) {
            AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonError);
            return;
        }
        canPressDefendButton = false;
        StartCoroutine(ButtonCooldown(ButtonType.Defend));

        if(maskType == MaskType.DenialMask || maskType == MaskType.EmptyMask) {
            nextPlayerDamageNullifier = 0.0f;
            OnDefendButtonPressed?.Invoke(this, EventArgs.Empty);
            AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonClick);
            return;
        }

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

        AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonClick);
    }

    private void OnObserveButtonClick() {
        if (!canPressButtons) {
            AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonError);
            return;
        }

        if (!canPressObserveButton) {
            AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonError);
            return;
        }
        canPressObserveButton = false;
        StartCoroutine(ButtonCooldown(ButtonType.Observe));

        AddToActionLog($"<#964B00>Observing {maskType.ToString()}</color>");
        OnObserveButtonPressed?.Invoke(this, EventArgs.Empty);

        AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonClick);
    }

    private void OnTalkButtonClick() {
        if (!canPressButtons) {
            AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonError);
            return;
        }

        if (!canPressTalkButton) {
            AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonError);
            return;
        }
        canPressTalkButton = false;
        StartCoroutine(ButtonCooldown(ButtonType.Talk));

        AddToActionLog($"<color=blue>Talking to {maskType.ToString()}</color>");
        OnTalkButtonPressed?.Invoke(this, EventArgs.Empty);

        AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonClick);
    }

    public void AddToActionLog(string actionStatement) {
        actionLog += actionStatement;
        actionLog += "\n";
        actionLogTMP.text = actionLog;
        Canvas.ForceUpdateCanvases();
        actionLogScrollBar.value = 0.0f;
    }

}
