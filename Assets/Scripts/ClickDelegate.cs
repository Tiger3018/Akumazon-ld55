using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClickDelegate : MonoBehaviour
{
    // Return True if this doen't block deepen click event
    public delegate bool ClickUpEventDelegate();


    private ClickDelegate m_clickScript;
    protected Transform[] m_childTransformAtStart;
    public event ClickUpEventDelegate m_clickEvent;

    // Start is called before the first frame update
    void Start()
    {
        ClickDelegate[] clickScriptList = GetComponents<ClickDelegate>();
        foreach (ClickDelegate clickScript in clickScriptList)
        { 
            if (clickScript.GetType().ToString() != "ClickDelegate") // child will also use this code
            {
                if(m_clickScript != null)
                {
                    Debug.Log("Multiple click script detected. Will omit" + m_clickScript.GetType().ToString());
                }
                m_clickScript = clickScript;
            }
        }
        if (m_clickScript != null)
        {
            ClickUpEventDelegate delegateDef = m_clickScript.onClickUpDelegate;
            m_clickEvent = delegateDef;
        }
        else
        {
            Debug.Log("No click script detected!");
        }

        m_childTransformAtStart = GetComponentsInChildren<Transform>(true);
        Array.Reverse(m_childTransformAtStart);
        Array.Resize(ref m_childTransformAtStart, m_childTransformAtStart.Length - 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public static Vector2 ReadValueVector2(InputAction actionName)
    //{
    //    return actionName.ReadValue<Vector2>();
    //}

    public bool ClickUpProcess()
    {
        if (m_clickEvent != null)
        {
            return m_clickEvent.Invoke();
        } else
        {
            Debug.Log("Null ClickEvent");
            return true;
        }
    }

    protected virtual bool onClickUpDelegate() { return true; }

}
