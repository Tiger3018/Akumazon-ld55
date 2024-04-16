using UnityEngine;

public class Demon : MonoBehaviour
{
    private AudioSource m_audioSource;
    private GameObject m_demonBody, m_demonFace;
    private bool m_currentShownDemon = false;
    private void Start()
    {
        GameSession.Instance.m_enumStageChangedEvent += ControlDemon;
        m_audioSource = GetComponent<AudioSource>();
        Transform[] demonTransform = GetComponentsInChildren<Transform>();
        m_demonBody = demonTransform[1].gameObject;
        m_demonFace = demonTransform[2].gameObject;
        m_demonBody.SetActive(false);
        m_demonFace.SetActive(false);
        m_audioSource = GetComponent<AudioSource>();
        // Debug.Log(m_demonBody.name + " " + m_demonFace.name);
    }

    private void ControlDemon(int stage)
    {
        GameObject animationAdded;
        switch (stage)
        {
        case (int)GameSession.SessionStage.DesiredDemon:
        goto case (int)GameSession.SessionStage.BadEnd; case (int)GameSession.SessionStage.GoodEnd:
        goto case (int)GameSession.SessionStage.BadEnd; case (int)GameSession.SessionStage.BadEnd:
            animationAdded = Instantiate(Resources.Load<GameObject>("Animation/CFXR2 WW Explosion (Omni)"),
                                         transform.position, transform.rotation, transform);
            animationAdded.transform.localScale = new Vector3(200f, 200f, 200f);
            ShowDemon(GameSession.Instance.m_demonSelected);
            break;
        case (int)GameSession.SessionStage.WrongDemon:
            animationAdded = Instantiate(Resources.Load<GameObject>("Animation/CFXR2 Ground Hit"), transform.position,
                                         transform.rotation, transform);
            animationAdded.transform.localScale = new Vector3(200f, 200f, 200f);
            ShowDemon(GameSession.Instance.m_demonSelected);
            break;
        default:
            if (m_currentShownDemon)
                HideDemon();
            break;
        }
    }

    private void ShowDemon(int demonId)
    {
        int demonRect = demonId & 0b11, demonFace = (demonId & 0b1100) >> 2, demonColor = (demonId & 0b110000) >> 4;
        Debug.Log("Demon: " + demonRect + " " + demonFace + " " + demonColor);
        m_demonBody.GetComponent<SpriteRenderer>().sprite =
            Resources.Load<Sprite>("Sprite/Demon/demon00b" + demonColor + "x" + demonRect);
        m_demonFace.GetComponent<SpriteRenderer>().sprite =
            Resources.Load<Sprite>("Sprite/Demon/DemonFace" + demonFace);
        m_demonBody.SetActive(true);
        m_demonFace.SetActive(true);
        if (m_audioSource != null)
        {
            m_audioSource.clip = Resources.Load<AudioClip>("Audio/summon");
            m_audioSource.Play();
        }
        m_currentShownDemon = true;
    }

    private void HideDemon()
    {
        m_demonBody.SetActive(false);
        m_demonFace.SetActive(false);
        m_currentShownDemon = false;
    }
}