using UnityEngine;

public class MixedAnimator : MonoBehaviour
{
    [SerializeField] private Animator[] animators;

    public void SetFloat(string name, float value){
        for (int i = 0; i < animators.Length; i++) {
            animators[i].SetFloat(name, value);
        }
    }
    
    public void SetInteger(string name, int value){
        for (int i = 0; i < animators.Length; i++) {
            animators[i].SetInteger(name, value);
        }
    }

    public void SetBool(string name, bool value){
        for (int i = 0; i < animators.Length; i++) {
            animators[i].SetBool(name, value);
        }
    }

    public void SetTriger(string name){
        for (int i = 0; i < animators.Length; i++) {
            animators[i].SetTrigger(name);
        }
    }
    
    public void CrossFade(string name, float normalizedTransitionDuration){
        for (int i = 0; i < animators.Length; i++) {
            animators[i].CrossFade(name, normalizedTransitionDuration);
        }
    }
    
    public void CrossFadeInFixedTime(string name, float fixedTransitionDuration){
        for (int i = 0; i < animators.Length; i++) {
            animators[i].CrossFadeInFixedTime(name, fixedTransitionDuration);
        }
    }
}
