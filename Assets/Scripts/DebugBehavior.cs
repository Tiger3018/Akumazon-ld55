using UnityEngine;

public class DebugBehavior : MonoBehaviour
{
    [SerializeField]
    internal int m_debugStage, m_debugDialogue, m_debugTextId, m_valueSan = 100;
    [HideInInspector]
    internal int m_enumClient;
    private void DebugUpdateSessionId(int currentStage, int currentDialogue, int currentTextId)
    {
        m_debugTextId = currentTextId;
        m_debugDialogue = currentDialogue;
        m_debugStage = currentStage;
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
        GameSession.Instance.DebugSessionId(m_debugStage, m_debugDialogue, m_debugTextId);
        GameSession.Instance.DebugSan(m_valueSan);
        GameSession.Instance.m_sessionIdChangedEvent += DebugUpdateSessionId;
        GameSession.Instance.m_enumClientChangedEvent += DebugUpdateEnumClient;
        GameSession.Instance.m_valueSanChangedEvent += DebugUpdateValueSan;
    }

    private void Update()
    {
        if (m_debugStage != GameSession.Instance.m_sessionId[0] ||
            m_debugDialogue != GameSession.Instance.m_sessionId[1] ||
            m_debugTextId != GameSession.Instance.m_sessionId[2])
        {
            GameSession.Instance.m_sessionIdChangedEvent -= DebugUpdateSessionId;
            GameSession.Instance.DebugSessionId(m_debugStage, m_debugDialogue, m_debugTextId);
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
