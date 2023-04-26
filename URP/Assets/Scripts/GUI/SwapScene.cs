using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwapScene : MonoBehaviour
{
    [SerializeField] private Transform canvas;
    public void OnSceneChange()
    {
        canvas.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
