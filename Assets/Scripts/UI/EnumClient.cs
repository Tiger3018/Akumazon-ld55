using System.Collections;
using UnityEngine;

public class EnumClient : MonoBehaviour
{
    [SerializeField]
    internal GameObject[] m_enumClients;
    internal bool m_corotineLock = false;
    internal bool m_insistencyCheck = false;
    internal Animator m_animator;

    private void Start()
    {
        GameSession.Instance.m_enumStageChangedEvent += OnStageChanged;
        GameSession.Instance.m_enumClientChangedEvent += OnClientChanged;
        foreach (GameObject enumClient in m_enumClients)
        {
            enumClient.SetActive(false);
        }
        m_enumClients[GameSession.Instance.m_enumClient].SetActive(true);
        m_animator = GetComponent<Animator>();
        m_animator.Play("ClientAnimation", 0, 1f);
    }

    private void Update()
    {
        if (m_insistencyCheck)
        {
            OnClientChanged(GameSession.Instance.m_enumClient);
            Debug.Log("Trying to resolve inconsistency.");
        }
    }

    private IEnumerator OnEnumChangedAnimationAsync(GameObject oldEnumClient, GameObject newEnumClient)
    {
        // gameObject.GetComponent<Animation>().Play("ClientAnimation");
        // yield return new WaitUntil(() => gameObject.GetComponent<Animation>().isPlaying == false);
        if (oldEnumClient)
        {
            m_animator.SetFloat("Speed", 1);
            m_animator.Play("ClientAnimation", 0, 0f); // Don't know why 0f is needed.
            // Debug.Log("EnumClient changed: Now removing.");
            yield return new WaitUntil(() => m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
            oldEnumClient.SetActive(false);
        }
        if (newEnumClient)
        {
            newEnumClient.SetActive(true);
            // Debug.Log("EnumClient changed: Now appearing.");
            m_animator.SetFloat("Speed", -1);
            m_animator.Play("ClientAnimation", 0, 1f);
            //  https://forum.unity.com/threads/wait-until-an-animation-is-finished.529093/#post-7539659
            yield return new WaitUntil(() => m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0f);
        }
        // gameObject.GetComponent<Animation>().Rewind("ClientAnimation");
        // yield return new WaitUntil(() => gameObject.GetComponent<Animation>().isPlaying == false);
        m_corotineLock = false;
    }

    private void OnStageChanged(int incomingStage)
    {
        int enumClient = GameSession.Instance.m_enumClient;
        if (enumClient >= m_enumClients.Length)
        {
            Debug.LogError("EnumClient is out of range.");
            return;
        }
        if (incomingStage == (int)GameSession.SessionStage.Dialogue)
        {
            if (m_corotineLock)
            {
                m_insistencyCheck = true;
                Debug.Log("Coroutine is locked. No task will be done.");
                return;
            }
            m_corotineLock = true;
            StartCoroutine(OnEnumChangedAnimationAsync(null, m_enumClients[enumClient]));
        }
        else
        {
            if (m_corotineLock)
            {
                m_insistencyCheck = true;
                Debug.Log("Coroutine is locked. No task will be done.");
                return;
            }
            m_corotineLock = true;
            StartCoroutine(OnEnumChangedAnimationAsync(m_enumClients[enumClient], null));
        }
    }
    private void OnClientChanged(int dialogueClient)
    {
        int oldEnumClient = GameSession.Instance.m_enumClient;
        if (dialogueClient >= m_enumClients.Length || oldEnumClient >= m_enumClients.Length)
        {
            Debug.LogError("EnumClient is out of range.");
            return;
        }
        if (GameSession.Instance.m_enumStage == (int)GameSession.SessionStage.Dialogue)
        {
            if (m_corotineLock)
            {
                m_insistencyCheck = true;
                Debug.Log("Coroutine is locked. No task will be done.");
                return;
            }
            m_corotineLock = true;
            StartCoroutine(OnEnumChangedAnimationAsync(m_enumClients[oldEnumClient], m_enumClients[dialogueClient]));
        }
        else
        {
            m_enumClients[oldEnumClient].SetActive(false);
            m_enumClients[dialogueClient].SetActive(true);
        }
        m_insistencyCheck = false;
        oldEnumClient = dialogueClient;
    }
}
