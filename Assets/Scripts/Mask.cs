using UnityEngine;

public class Mask<T> : MonoBehaviour where T : MonoBehaviour{

    public static T INSTANCE;

    protected virtual void Awake() {
        INSTANCE = this as T;
    }

}
