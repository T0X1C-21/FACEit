using DG.Tweening;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ShakeManager : MonoBehaviour {

    public static ShakeManager INSTANCE { get; private set; }

    [SerializeField] private CinemachineBasicMultiChannelPerlin cinemachineShakeComponent;

    [SerializeField] private GameObject greenImageGameObject;
    [SerializeField] private GameObject redImageGameObject;
    [SerializeField] private CanvasGroup greenCanvasGroup;
    [SerializeField] private CanvasGroup redCanvasGroup;
    [SerializeField] private float colorFadeTime;
    [SerializeField] private float colorWaitTime;
    [SerializeField] private float shakeAmplitude;

    private void Awake() {
        cinemachineShakeComponent.AmplitudeGain = 0f;
        INSTANCE = this;
    }

    public void TriggerPositiveShake() {
        StartPositiveShakeSequence();
    }

    public void TriggerNegativeShake() {
        StartNegativeShakeSequence();
    }

    private void StartPositiveShakeSequence() {
        cinemachineShakeComponent.AmplitudeGain = shakeAmplitude;
        greenCanvasGroup.alpha = 0f;
        greenImageGameObject.SetActive(true);
        greenCanvasGroup.DOFade(1f, colorFadeTime).OnComplete(() => {
            StartCoroutine(EndPositiveShakeSequence());
            DOTween.To(() => cinemachineShakeComponent.AmplitudeGain, x => cinemachineShakeComponent.AmplitudeGain = x, 
                0f, colorWaitTime);
        });
    }

    private IEnumerator EndPositiveShakeSequence() {
        yield return new WaitForSeconds(colorWaitTime);
        greenCanvasGroup.DOFade(0f, colorFadeTime).OnComplete(() => {
            greenImageGameObject.SetActive(false);
        });
    }

    private void StartNegativeShakeSequence() {
        cinemachineShakeComponent.AmplitudeGain = shakeAmplitude;
        redCanvasGroup.alpha = 0f;
        redImageGameObject.SetActive(true);
        redCanvasGroup.DOFade(1f, colorFadeTime).OnComplete(() => {
            StartCoroutine(EndNegativeShakeSequence());
            DOTween.To(() => cinemachineShakeComponent.AmplitudeGain, x => cinemachineShakeComponent.AmplitudeGain = x, 
                0f, colorWaitTime);
        });
    }
    
    private IEnumerator EndNegativeShakeSequence() {
        yield return new WaitForSeconds(colorWaitTime);
        redCanvasGroup.DOFade(0f, colorFadeTime).OnComplete(() => {
            redImageGameObject.SetActive(false);
        });
    }

}
