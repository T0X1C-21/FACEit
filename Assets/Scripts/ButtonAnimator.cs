using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private RectTransform buttonRectTransform;

    private void Awake() {
        buttonRectTransform = this.GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {
        AudioManager.INSTANCE.PlaySoundEffect(SoundEffect.ButtonHover);

        buttonRectTransform.DOScale(Vector3.one * 1.1f, 0.25f);
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
        buttonRectTransform.DOScale(Vector3.one, 0.1f);
    }

}
