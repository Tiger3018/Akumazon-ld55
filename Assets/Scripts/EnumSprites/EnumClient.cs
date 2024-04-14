using System.Collections;
using UnityEngine;

public class EnumClient : MonoBehaviour
{
    [SerializeField]
    internal GameObject[] m_enumClients;
    internal int m_nowEnumClient = 0;
    internal bool m_corotineLock = false;
    internal bool m_insistencyCheck = false;
    internal Animator m_animator;

    private void Start()
    {
        GameSession.Instance.m_enumClientChangedEvent += OnEnumChanged;
        foreach (GameObject enumClient in m_enumClients)
        {
            enumClient.SetActive(false);
        }
        m_enumClients[m_nowEnumClient].SetActive(true);
        m_animator = GetComponent<Animator>();
        m_animator.Play("ClientAnimation", 0, 1f);
    }

    private void Update()
    {
        if (m_insistencyCheck)
        {
            OnEnumChanged(GameSession.Instance.m_enumClient);
            Debug.Log("Trying to resolve inconsistency.");
        }
    }

    private IEnumerator OnEnumChangedAnimationAsync(GameObject nowEnumClient, GameObject newEnumClient)
    {
        // gameObject.GetComponent<Animation>().Play("ClientAnimation");
        // yield return new WaitUntil(() => gameObject.GetComponent<Animation>().isPlaying == false);
        m_animator.SetFloat("Speed", 1);
        m_animator.Play("ClientAnimation", 0, 0f); // Don't know why 0f is needed.
        Debug.Log("EnumClient changed: Now removing.");
        yield return new WaitUntil(() => m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        if (nowEnumClient)
            nowEnumClient.SetActive(false);
        if (newEnumClient)
            newEnumClient.SetActive(true);
        Debug.Log("EnumClient changed: Now appearing.");
        m_animator.SetFloat("Speed", -1);
        m_animator.Play("ClientAnimation");
        //  https://forum.unity.com/threads/wait-until-an-animation-is-finished.529093/#post-7539659
        yield return new WaitUntil(() => m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0f);
        // gameObject.GetComponent<Animation>().Rewind("ClientAnimation");
        // yield return new WaitUntil(() => gameObject.GetComponent<Animation>().isPlaying == false);
        m_corotineLock = false;
    }
    private void OnEnumChanged(int enumClient)
    {
        if (m_nowEnumClient == enumClient)
        {
            return; // TODO
        }
        if (enumClient < 0 || enumClient >= m_enumClients.Length)
        {
            Debug.LogError("EnumClient is out of range.");
            return;
        }
        if (gameObject.activeSelf)
        {
            if (m_corotineLock)
            {
                m_insistencyCheck = true;
                Debug.Log("Coroutine is locked. No task will be done.");
                return;
            }
            m_corotineLock = true;
            StartCoroutine(OnEnumChangedAnimationAsync(m_enumClients[m_nowEnumClient], m_enumClients[enumClient]));
        }
        else
        {
            m_enumClients[m_nowEnumClient].SetActive(false);
            m_enumClients[enumClient].SetActive(true);
        }
        m_insistencyCheck = false;
        m_nowEnumClient = enumClient;
    }
}
