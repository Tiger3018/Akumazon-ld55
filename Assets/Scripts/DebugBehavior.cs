using UnityEngine;

public class DebugSession : MonoBehaviour
{
    [SerializeField]
    internal int m_currentStage, m_currentDialogue, m_upmostDialogue, m_valueSan = 100;
    [HideInInspector]
    internal int m_enumClient;
    private void DebugUpdateSessionId(int currentStage, int currentDialogue, int upmostDialogue)
    {
        m_currentStage = currentStage;
        m_currentDialogue = currentDialogue;
        m_upmostDialogue = upmostDialogue;
    }
    private void DebugUpdateEnumClient(int enumClient)
    {
        m_enumClient = enumClient;
    }
    private void DebugUpdateValueSan(int valueSan)
    {
        m_valueSan = valueSan;
    }
    private void Start()
    {
        GameSession.Instance.DebugSessionId(m_currentStage, m_currentDialogue, m_upmostDialogue);
        GameSession.Instance.DebugSan(m_valueSan);
        GameSession.Instance.m_sessionIdChangedEvent += DebugUpdateSessionId;
        GameSession.Instance.m_enumClientChangedEvent += DebugUpdateEnumClient;
        GameSession.Instance.m_valueSanChangedEvent += DebugUpdateValueSan;
    }

    private void Update()
    {
        if (m_currentStage != GameSession.Instance.m_sessionId[0] ||
            m_currentDialogue != GameSession.Instance.m_sessionId[1] ||
            m_upmostDialogue != GameSession.Instance.m_sessionId[2])
        {
            GameSession.Instance.m_sessionIdChangedEvent -= DebugUpdateSessionId;
            GameSession.Instance.DebugSessionId(m_currentStage, m_currentDialogue, m_upmostDialogue);
            GameSession.Instance.m_sessionIdChangedEvent += DebugUpdateSessionId;
        }
        if (m_valueSan != GameSession.Instance.m_valueSan)
        {
            GameSession.Instance.m_valueSanChangedEvent -= DebugUpdateValueSan;
            GameSession.Instance.DebugSan(m_valueSan);
            GameSession.Instance.m_valueSanChangedEvent += DebugUpdateValueSan;
        }
    }
}
