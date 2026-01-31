using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintPanelManager : MonoBehaviour {

    [SerializeField] private RectTransform levelTextRectTransform;
    [SerializeField] private RectTransform hintTextRectTransform;
    [SerializeField] private Button continueButton;
    [SerializeField] private float animationTime;
    [SerializeField] private CanvasGroup[] canvasGroupToFadeArray;

    private bool doOnce;

    private void Awake() {
        continueButton.onClick.AddListener(OnClickContinueButton);
    }

    private void OnClickContinueButton() {
        if (!doOnce) {
            doOnce = true;
        } else {
            return;
        }

        RectTransform continueButtonRectTransform = continueButton.GetComponent<RectTransform>();
        // animate
        levelTextRectTransform.DOScale(Vector3.zero, animationTime).SetEase(Ease.InElastic);
        hintTextRectTransform.DOScale(Vector3.zero, animationTime + 0.2f).SetEase(Ease.InElastic).OnComplete(() => {
            StartCoroutine(GameManager.INSTANCE.ChangeLevelState(LevelState.Combat));
        });
        continueButtonRectTransform.DOAnchorPosY(-100f, animationTime).SetEase(Ease.InElastic);
        foreach(CanvasGroup canvasGroup in canvasGroupToFadeArray) {
            canvasGroup.DOFade(0f, animationTime).SetEase(Ease.Linear);
        }
    }

    private void OnEnable() {
        // Reset texts
        levelTextRectTransform.localScale = Vector3.zero;
        hintTextRectTransform.localScale = Vector3.zero;

        // Reset button
        RectTransform continueButtonRectTransform = continueButton.GetComponent<RectTransform>();
        Vector2 anchoredPosition = continueButtonRectTransform.anchoredPosition;
        anchoredPosition.y = -100f;
        continueButtonRectTransform.anchoredPosition = anchoredPosition;

        // Reset fade valyues
        foreach(CanvasGroup canvasGroup in canvasGroupToFadeArray) {
            canvasGroup.alpha = 0f;
        }

        // animate
        levelTextRectTransform.DOScale(Vector3.one, animationTime).SetEase(Ease.OutElastic);
        hintTextRectTransform.DOScale(Vector3.one, animationTime + 0.2f).SetEase(Ease.OutElastic);
        continueButtonRectTransform.DOAnchorPosY(100f, animationTime).SetEase(Ease.OutElastic);
        foreach(CanvasGroup canvasGroup in canvasGroupToFadeArray) {
            canvasGroup.DOFade(1f, animationTime).SetEase(Ease.Linear);
        }
    }

}
