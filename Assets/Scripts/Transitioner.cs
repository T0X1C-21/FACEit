using DG.Tweening;
using UnityEngine;

public class Transitioner : MonoBehaviour {

    public static Transitioner INSTANCE;

    [SerializeField] private RectTransform leftRectTransform;
    [SerializeField] private RectTransform rightRectTransform;

    private void Awake() {
        INSTANCE = this;
    }

    public void OpenAnimation(float animationTime) {
        leftRectTransform.DOAnchorPosX(-1080f, animationTime);
        rightRectTransform.DOAnchorPosX(1080f, animationTime);
    }

    public void CloseAnimation(float animationTime) {
        leftRectTransform.DOAnchorPosX(0f, animationTime);
        rightRectTransform.DOAnchorPosX(0f, animationTime);
    }

}
