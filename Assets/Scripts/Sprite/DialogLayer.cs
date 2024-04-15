using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogLayer : ClickDelegate
{
    [HideInInspector]
    public RiddleDialog m_initiator;
    private Animation m_animation;
    private void Start()
    {
        m_animation = GetComponent<Animation>();
    }
    protected override bool onClickUpLeftDelegate()
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
