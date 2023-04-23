using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionListUI : MonoBehaviour
{
    public ActionList actionList;
    public ActionUI prefab;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Action a in actionList.actions)
        {
            ActionUI ui = Instantiate(prefab, transform);
            ui.SetAction(a);
        }
    }

}
