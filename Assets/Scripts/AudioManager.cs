using UnityEngine;

public enum SoundEffect {
    TransitionStart,
    TransitionEnd,
    ButtonHover,
    ButtonClick,
    ButtonError,
    TypeWriter,
    GameOver,
    DefeatedMask
}

public class AudioManager : MonoBehaviour {

    public static AudioManager INSTANCE;

    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSourceTwo;
    [SerializeField] private AudioSource typeWriterSoundEffectAudioSource;
    [SerializeField] private AudioClip transitionStartAudioClip;
    [SerializeField] private AudioClip transitionEndAudioClip;
    [SerializeField] private AudioClip buttonHoverAudioClip;
    [SerializeField] private AudioClip buttonClickAudioClip;
    [SerializeField] private AudioClip buttonErrorAudioClip;
    [SerializeField] private AudioClip gameOverAudioClip;
    [SerializeField] private AudioClip defeatedMaskAudioClip;

    private void Awake() {
        INSTANCE = this;
    }

    private void Start() {
        musicAudioSource.Play();
    }

    public void PlaySoundEffect(SoundEffect soundEffect) {
        switch (soundEffect) {
            case SoundEffect.TransitionStart:
                GetFreeAudioSource().PlayOneShot(transitionStartAudioClip);
                break;
            case SoundEffect.TransitionEnd:
                GetFreeAudioSource().PlayOneShot(transitionEndAudioClip);
                break;
            case SoundEffect.ButtonHover:
                GetFreeAudioSource().PlayOneShot(buttonHoverAudioClip);
                break;
            case SoundEffect.ButtonClick:
                GetFreeAudioSource().PlayOneShot(buttonClickAudioClip);
                break;
            case SoundEffect.ButtonError:
                GetFreeAudioSource().PlayOneShot(buttonErrorAudioClip);
                break;
            case SoundEffect.TypeWriter:
                typeWriterSoundEffectAudioSource.Play();
                break;
            case SoundEffect.GameOver:
                GetFreeAudioSource().PlayOneShot(gameOverAudioClip);
                break;
            case SoundEffect.DefeatedMask:
                GetFreeAudioSource().PlayOneShot(defeatedMaskAudioClip);
                break;
        }
    }

    private AudioSource GetFreeAudioSource() {
        return (soundEffectAudioSource.isPlaying) ? soundEffectAudioSourceTwo : soundEffectAudioSource;
    }

}
