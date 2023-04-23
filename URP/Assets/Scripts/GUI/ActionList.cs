using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionList : MonoBehaviour
{
    public Action[] actions;

    // Start is called before the first frame update
    void Start()
    {
        actions = GetComponents<Action>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
