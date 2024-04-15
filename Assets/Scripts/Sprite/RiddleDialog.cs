using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RiddleDialog : ClickDelegate
{
    public GameObject m_dialogPrefab;
    public GameObject m_rayLayer;
    private AudioSource m_audioSource;

    void Start()
    {
        base.Start();
        m_audioSource = GetComponent<AudioSource>();
    }

    public void CloseDialog()
    {
        foreach (Transform childTransform in m_childTransformAtStart)
        {
            childTransform.gameObject.SetActive(false);
        }
    }
    private void OpenDialog()
    {
        if (m_audioSource != null)
        {
            m_audioSource.Play();
        }
        foreach (Transform childTransform in m_childTransformAtStart)
        {
            // childTransform.position = new Vector3(-transform.position.y/transform.localScale.y,
            // -transform.position.x/transform.localScale.x, 0);
            // childTransform.root.position = new Vector3(0, 0, 0);
            // childTransform.localScale = new Vector3(0, 0, 0);
            childTransform.gameObject.SetActive(true);
            // childTransform.gameObject.GetComponent<Animation>().Play("ObjectDialogShow");
        }
    }
    protected override bool onClickUpLeftDelegate()
    {
        GameObject dialog = Instantiate(m_dialogPrefab, m_rayLayer.transform);
        dialog.GetComponent<DialogLayer>().m_initiator = this;
        dialog.transform.position = m_rayLayer.transform.position;

        OpenDialog();
        return true;
    }
}
