using TMPro;
using UnityEngine;

public class Mask<T> : MonoBehaviour where T : MonoBehaviour{

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

    protected virtual void Awake() {
        INSTANCE = this as T;
    }

}
