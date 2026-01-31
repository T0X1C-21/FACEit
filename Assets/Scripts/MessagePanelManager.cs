using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanelManager : MonoBehaviour {

    [SerializeField] private RectTransform messageTextRectTransform;
    [SerializeField] private RectTransform levelDoneRectTransform;
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
        messageTextRectTransform.DOScale(Vector3.zero, animationTime).SetEase(Ease.InElastic);
        levelDoneRectTransform.DOScale(Vector3.zero, animationTime + 0.2f).SetEase(Ease.InElastic).OnComplete(() => {
            StartCoroutine(LevelCloseAnimation());
        });
        continueButtonRectTransform.DOAnchorPosY(-100f, animationTime).SetEase(Ease.InElastic);
        foreach(CanvasGroup canvasGroup in canvasGroupToFadeArray) {
            canvasGroup.DOFade(0f, animationTime).SetEase(Ease.Linear);
        }
    }

    private void OnEnable() {
        // Reset texts
        messageTextRectTransform.localScale = Vector3.zero;
        levelDoneRectTransform.localScale = Vector3.zero;

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
        messageTextRectTransform.DOScale(Vector3.one, animationTime).SetEase(Ease.OutElastic);
        levelDoneRectTransform.DOScale(Vector3.one, animationTime + 0.2f).SetEase(Ease.OutElastic);
        continueButtonRectTransform.DOAnchorPosY(100f, animationTime).SetEase(Ease.OutElastic);
        foreach(CanvasGroup canvasGroup in canvasGroupToFadeArray) {
            canvasGroup.DOFade(1f, animationTime).SetEase(Ease.Linear);
        }
    }

    private IEnumerator LevelCloseAnimation() {
        float animationTime;
        GameManager.INSTANCE.EndLevelTransition(out animationTime);
        yield return new WaitForSeconds(animationTime);

        LevelLoader.INSTANCE.SpawnNextLevel();
    }

}
