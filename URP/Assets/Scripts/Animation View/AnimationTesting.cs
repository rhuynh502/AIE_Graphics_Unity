using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnimationTesting : MonoBehaviour
{
    [SerializeField] private ParticleSystem petalParticle;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Player")]
    [SerializeField] private Player player;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform pauseScreen;
    [SerializeField] private Animator animator;
    [SerializeField] private Material shirtColor;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Sliders")]
    [SerializeField] private Slider sliderForward;
    [SerializeField] private Slider sliderRight;
    [SerializeField] private Slider sliderRed;
    [SerializeField] private Slider sliderGreen;
    [SerializeField] private Slider sliderBlue;
    [SerializeField] private Slider sliderMetallic;
    [SerializeField] private Slider sliderSmoothness;
    [SerializeField] private Slider sliderFace;


    private float blinkTime = 0;
    private bool canBlink = true;
    private bool petalIsPlaying = true;
    private new Camera camera;

    private void Awake()
    {
        // This will take the materials saved values and put them into the slider
        sliderRed.SetValueWithoutNotify(shirtColor.GetFloat("_ColorRed"));
        sliderGreen.SetValueWithoutNotify(shirtColor.GetFloat("_ColorGreen"));
        sliderBlue.SetValueWithoutNotify(shirtColor.GetFloat("_ColorBlue"));
        sliderMetallic.SetValueWithoutNotify(shirtColor.GetFloat("_Metallic"));
        sliderSmoothness.SetValueWithoutNotify(shirtColor.GetFloat("_Smoothness"));

    }

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        // If the character can blink, the coroutine is called.
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

    public void ActivatePlayerInput()
    {
        virtualCamera.Priority = 11;

        StartCoroutine(DelayInput_CR());

    }

    public void DeActivatePlayerInput()
    {
        // Turns on this canvas
        gameObject.SetActive(true);
        // Toggles player input
        player.enabled = !player.enabled;
        playerInput.enabled = !playerInput.enabled;

        player.UnPause();

        pauseScreen.gameObject.SetActive(false);
        virtualCamera.Priority = 9;
        
    }

    public void OnChangeFacialExpression()
    {
        ResetFacial();
        if(sliderFace.value == 0)
            ResetFacial();       
        else
            skinnedMeshRenderer.SetBlendShapeWeight((int)sliderFace.value, 100);
    }

    public void SetMaterialColor()
    {
        // This changes the values of the material everytime a slider is moved
        shirtColor.SetFloat("_ColorRed", sliderRed.value);
        shirtColor.SetFloat("_ColorGreen", sliderGreen.value);
        shirtColor.SetFloat("_ColorBlue", sliderBlue.value);

        shirtColor.SetFloat("_Metallic", sliderMetallic.value);
        shirtColor.SetFloat("_Smoothness", sliderSmoothness.value);
    }

    // This Coroutine determines the time it takes for the model to blink again
    IEnumerator Blink_CR()
    {
        canBlink = false;
        skinnedMeshRenderer.SetBlendShapeWeight(0, Mathf.Lerp(0, 100, blinkTime / 1.2f));
        yield return new WaitForSeconds(8);
        skinnedMeshRenderer.SetBlendShapeWeight(0, Mathf.Lerp(100, 0, blinkTime / 1.2f));
        canBlink = true;
    }

    // This Coroutine prevents the player from inputting while camera is moving
    IEnumerator DelayInput_CR()
    {
        Cursor.lockState = CursorLockMode.Locked;

        yield return new WaitForSeconds(2.5f);

        // Toggles player input
        player.enabled = !player.enabled;
        playerInput.enabled = !playerInput.enabled;

        // Turns off this canvas
        gameObject.SetActive(false);
    }

    public void TogglePetals()
    {
        // Checks if the particle system is playing
        if(petalIsPlaying)
            // This will stop the particle system from emitting and clear already existing particles
            petalParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        else
            petalParticle.Play();
        
        petalIsPlaying = !petalIsPlaying;
    }

    private void ResetFacial()
    {
        // This resets all facial expressions back to default
        for(int i = 1; i < 10; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, 0);
        }
    }
}
