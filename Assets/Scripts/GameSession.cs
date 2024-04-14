using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityCommunity.UnitySingleton;
using Unity.Collections.LowLevel.Unsafe;

public class GameSession : MonoSingleton<GameSession>
{
    enum SessionStage
    {
        Wait,
        Dialogue,
        Summon,
        WrongDemon,
        NoDemon,
        DesiredDemon
    }
    enum SessionDialogue
    {
        AGentleman,
        BTownsman,
        CGirl,
        DFather,
        EasyFreeExplore
    }
    /// <summary>
    /// Session 1: Current Stage, 0 for wait, 1 for dialogue, 2 for summon, 3 for wrong demon
    ///            4 for no demon, 5 for desired demon
    /// Session 2: Current Dialogue, see SessionDialogue, 0 - 4
    /// Session 3: Upmost Dialogue, 0 - 4
    /// </summary>
    internal int[] _m_sessionId = new int[3] { 0, 0, 0 };
    /// <summary>
    /// This is only affected by the Transcript.
    /// </summary>
    internal int _m_enumClient = 0;
    internal int _m_valueSan = 100;
    public int[] m_sessionId
    {
        get => _m_sessionId;
    private
        set => _m_sessionId = value;
    }
    public int m_enumClient
    {
        get => _m_enumClient;
    // private
        set {
            if (_m_enumClient != value)
            {
                _m_enumClient = value;
                m_enumClientChangedEvent?.Invoke(value);
            }
        }
    }
    public int m_valueSan
    {
        get => _m_valueSan;
    private
        set => _m_valueSan = value;
    }

    public void SetStage(int stage)
    {
        if (stage < 0 || stage > 5 || stage == m_sessionId[0])
        {
            Debug.LogError("Invalid stage: " + stage + " or same stage as before.");
            return;
        }
        m_sessionId[0] = stage;
        m_sessionIdChangedEvent?.Invoke(stage, m_sessionId[1], m_sessionId[2]);
    }

    public void SetEnumClient(int dialogue)
    {
        if (dialogue < 0 || dialogue > 4 || dialogue == m_sessionId[1])
        {
            Debug.LogError("Invalid dialogue: " + dialogue + " or same client as before.");
            return;
        }
        m_sessionId[1] = dialogue;
        m_enumClient = dialogue;
        m_sessionIdChangedEvent?.Invoke(m_sessionId[0], dialogue, m_sessionId[2]);
    }

    public void MinusSan()
    {
        m_valueSan -= 20;
        if (m_valueSan < 0)
        {
            m_valueSan = 0;
        }
        else
        {
            m_valueSanChangedEvent?.Invoke(m_valueSan);
        }
    }
    public void PlusSan()
    {
        m_valueSan += 20;
        if (m_valueSan > 100)
        {
            m_valueSan = 100;
        }
        else
        {
            m_valueSanChangedEvent?.Invoke(m_valueSan);
        }
    }

    public void DebugSessionId(int currentStage, int currentDialogue, int upmostDialogue)
    {
        m_sessionId[0] = currentStage;
        m_sessionId[1] = currentDialogue;
        m_sessionId[2] = upmostDialogue;
        m_enumClient = currentDialogue;
        m_sessionIdChangedEvent?.Invoke(currentStage, currentDialogue, upmostDialogue);
    }
    public void DebugSan(int valueSan)
    {
        m_valueSan = valueSan;
        m_valueSanChangedEvent?.Invoke(valueSan);
    }

    public delegate void SessionIdChanged(int currentStage, int currentDialogue, int upmostDialogue);
    public SessionIdChanged m_sessionIdChangedEvent;
    public delegate void EnumClientChanged(int enumClient);
    public EnumClientChanged m_enumClientChangedEvent;
    public delegate void ValueSanChanged(int valueSan);
    public ValueSanChanged m_valueSanChangedEvent;
}
