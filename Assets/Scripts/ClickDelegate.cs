using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClickDelegate : MonoBehaviour
{
    // Return True if this doen't block deepen click event
    public delegate bool ClickUpLeftEventDelegate();
    public delegate bool ClickUpRightEventDelegate();

    private ClickDelegate m_clickScript;
    protected Transform[] m_childTransformAtStart;
    public event ClickUpLeftEventDelegate m_clickLeftEvent;
    public event ClickUpRightEventDelegate m_clickRightEvent;

    // Start is called before the first frame update
    protected void Start()
    {
        ClickDelegate[] clickScriptList = GetComponents<ClickDelegate>();
        foreach (ClickDelegate clickScript in clickScriptList)
        {
            if (clickScript.GetType().ToString() != "ClickDelegate") // child will also use this code
            {
                if (m_clickScript != null)
                {
                    Debug.Log("Multiple click script detected. Will omit" + m_clickScript.GetType().ToString());
                }
                m_clickScript = clickScript;
            }
        }
        if (m_clickScript != null)
        {
            ClickUpLeftEventDelegate delegateLeftDef = m_clickScript.onClickUpLeftDelegate;
            ClickUpRightEventDelegate delegateRightDef = m_clickScript.onClickUpRightDelegate;
            m_clickLeftEvent = delegateLeftDef;
            m_clickRightEvent = delegateRightDef;
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

    // public static Vector2 ReadValueVector2(InputAction actionName)
    //{
    //     return actionName.ReadValue<Vector2>();
    // }

    public bool ClickUpProcess(bool isLeft)
    {
        if (isLeft)
        {
            return ClickUpLeftProcess();
        }
        else
        {
            if (m_clickRightEvent != null)
            {
                return m_clickRightEvent.Invoke();
            }
            else
            {
                Debug.Log("Null ClickRightEvent");
                return true;
            }
        }
    }
    public bool ClickUpLeftProcess()
    {

        if (m_clickLeftEvent != null)
        {
            return m_clickLeftEvent.Invoke();
        }
        else
        {
            Debug.Log("Null ClickLeftEvent");
            return true;
        }
    }

    protected virtual bool onClickUpLeftDelegate()
    {
        return true;
    }
    protected virtual bool onClickUpRightDelegate()
    {
        return true;
    }
}
