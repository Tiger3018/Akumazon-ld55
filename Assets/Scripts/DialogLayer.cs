using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogLayer : ClickDelegate
{
    [HideInInspector]
    public ObjectDialog m_initiator;
    private Animation m_animation;
    void Start()
    {
        m_animation = GetComponent<Animation>();
    }
    protected override bool onClickUpDelegate()
    {
        if (m_initiator != null)
        {
            m_initiator.CloseDialog();
        }
        else
        {
            Debug.Log("No Initiator to close.");
        }
        Destroy(gameObject);
        return false; // block deepen click event
    }
}
