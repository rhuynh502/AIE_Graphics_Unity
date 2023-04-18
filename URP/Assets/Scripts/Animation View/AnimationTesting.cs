using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class AnimationTesting : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Slider sliderForward;
    [SerializeField] private Slider sliderRight;
    [SerializeField] private Slider sliderZoom;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    
    private float blinkTime = 0;
    private bool canBlink = true;
    private new Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        if (blinkTime > 0.35f)
        {
            blinkTime = 0;
            StartCoroutine(Blink_CR());
        }

        if(canBlink)
            blinkTime += Time.deltaTime;
    }

    public void WalkForward()
    {
        animator.SetFloat("Forward", sliderForward.value / 10);
    }

    public void WalkRight()
    {
        animator.SetFloat("Right", sliderRight.value / 10);
    }

    public void Jump()
    {
        animator.SetTrigger("Jump");
    }

    public void Zoom()
    {
        camera.transform.Translate(new float3(0, 0, sliderZoom.value));
    }

    IEnumerator Blink_CR()
    {
        canBlink = false;
        skinnedMeshRenderer.SetBlendShapeWeight(0, Mathf.Lerp(0, 100, blinkTime / 0.35f));
        yield return new WaitForSeconds(8);
        skinnedMeshRenderer.SetBlendShapeWeight(0, Mathf.Lerp(100, 0, blinkTime / 0.35f));
        canBlink = true;
    }

    private void ResetFacial()
    {
        for(int i = 6; i < 10; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, 0);
        }
    }
}
