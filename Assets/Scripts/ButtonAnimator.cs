using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    private RectTransform buttonRectTransform;

    private void Awake() {
        buttonRectTransform = this.GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {
        AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonHover);
        ScaleUpAnimation();
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
        ScaleDownAnimation();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnPointerClick(PointerEventData eventData) {
        OnPointerExit(eventData);
    }

    private void ScaleUpAnimation() {
        buttonRectTransform.DOScale(Vector3.one * 1.1f, 0.25f);
    }

    private void ScaleDownAnimation() {
        buttonRectTransform.DOScale(Vector3.one, 0.25f);
    }

}
