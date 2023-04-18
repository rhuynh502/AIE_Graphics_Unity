using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationTesting : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Slider sliderForward;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void WalkForward()
    {
        animator.SetFloat("Forward", sliderForward.value);
    }

    public void Jump()
    {
        animator.SetTrigger("Jump");
        /*if(!animator.GetBool("Jump"))
            StartCoroutine(JumpCooldown_CR());*/
    }

    IEnumerator JumpCooldown_CR()
    {
        animator.SetBool("Jump", true);
        yield return new WaitForSeconds(2);
        animator.SetBool("Jump", false);
    }
}
