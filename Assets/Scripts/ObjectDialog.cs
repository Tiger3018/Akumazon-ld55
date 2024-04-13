using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectDialog : ClickDelegate
{
    public GameObject m_dialogPrefab;
    public GameObject m_rayLayer;

    public void CloseDialog()
    {
        foreach (Transform childTransform in m_childTransformAtStart)
        {
            childTransform.gameObject.SetActive(false);
        }
    }
    private void OpenDialog()
    {
        foreach (Transform childTransform in m_childTransformAtStart)
        {
            //childTransform.position = new Vector3(-transform.position.y/transform.localScale.y, -transform.position.x/transform.localScale.x, 0);
            childTransform.root.position = new Vector3(0, 0, 0);
            childTransform.gameObject.SetActive(true);
        }
    }
    protected override bool onClickUpDelegate()
    {
        GameObject dialog = Instantiate(m_dialogPrefab, m_rayLayer.transform);
        dialog.GetComponent<DialogLayer>().m_initiator = this;
        dialog.SetActive(true);

        OpenDialog();
        return true;
    }
}
