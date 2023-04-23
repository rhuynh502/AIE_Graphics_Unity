
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionUI : MonoBehaviour
{
    public Action action;

    [Header("Child Components")]
    public Image icon;
    public TextMeshProUGUI nameTag;
    public TextMeshProUGUI descriptionTag;

    private void Start()
    {
        SetAction(action);
    }

    public void SetAction(Action a)
    {
        if (a == null)
            return;

        action = a;
        if (nameTag)
            nameTag.text = action.actionName;
        if (descriptionTag) 
            descriptionTag.text = action.description;
        if(icon)
        {
            icon.sprite = action.icon;
            icon.color = action.color;
        }
    }
}
