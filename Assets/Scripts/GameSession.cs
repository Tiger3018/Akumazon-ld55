using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityCommunity.UnitySingleton;
using Unity.Collections.LowLevel.Unsafe;

public class GameSession : MonoSingleton<GameSession>
{
    public enum SessionStage
    {
        Wait,
        Dialogue,
        Summon,
        WrongDemon,
        NoDemon,
        DesiredDemon
    }
    public enum SessionDialogue
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
    /// Session 3: Current TextId, TODO
    /// Event Behavior: Event2, Event1, EventAll
    /// </summary>
    internal int[] _m_sessionId = new int[3] { 0, 0, 0 };
    /// <summary>
    /// This is only affected by the Transcript.
    /// </summary>
    internal int _m_enumStage = 0;
    internal int _m_enumClient = 0;
    internal int _m_valueSan = 100;
    public int[] m_sessionId
    {
        get => _m_sessionId;
    private
        set => _m_sessionId = value;
    }
    public int m_enumStage
    {
        get => _m_enumStage; // TODO Warn if Get value when all event is triggered.
    private
        set {
            if (_m_enumStage != value)
            {
                m_enumStageChangedEvent?.Invoke(value);
                _m_enumStage = value;
            }
        }
    }
    public int m_enumClient
    {
        get => _m_enumClient;
    private
        set {
            if (_m_enumClient != value)
            {
                m_enumClientChangedEvent?.Invoke(value);
                _m_enumClient = value;
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
        m_sessionIdChangedEvent?.Invoke(stage, m_sessionId[1], m_sessionId[2]);
        m_sessionId[0] = stage;
    }

    public void SetEnumClient(int dialogue)
    {
        if (dialogue < 0 || dialogue > 4 || dialogue == m_sessionId[1])
        {
            Debug.LogError("Invalid dialogue: " + dialogue + " or same client as before.");
            return;
        }
        m_enumClient = dialogue;
        m_sessionIdChangedEvent?.Invoke(m_sessionId[0], dialogue, m_sessionId[2]);
        m_sessionId[1] = dialogue;
    }

    public void MinusSan()
    {
        if (m_valueSan < 20)
        {
            m_valueSan = 0;
        }
        else
        {
            m_valueSanChangedEvent?.Invoke(m_valueSan - 20);
            m_valueSan -= 20;
        }
    }
    public void PlusSan()
    {
        if (m_valueSan > 80)
        {
            m_valueSan = 100;
        }
        else
        {
            m_valueSanChangedEvent?.Invoke(m_valueSan + 20);
            m_valueSan += 20;
        }
    }

    public void DebugSessionId(int currentStage, int currentDialogue, int currentTextId)
    {
        m_enumClient = currentDialogue;
        m_enumStage = currentStage;
        m_sessionIdChangedEvent?.Invoke(currentStage, currentDialogue, currentTextId);
        m_sessionId[2] = currentTextId;
        m_sessionId[1] = currentDialogue;
        m_sessionId[0] = currentStage;
    }
    public void DebugSan(int valueSan)
    {
        m_valueSanChangedEvent?.Invoke(valueSan);
        m_valueSan = valueSan;
    }

    public delegate void SessionIdChanged(int incomingStage, int incomingDialogue, int seekTextId);
    public SessionIdChanged m_sessionIdChangedEvent;
    public delegate void EnumStageChanged(int incomingStage);
    public EnumStageChanged m_enumStageChangedEvent;
    public delegate void EnumClientChanged(int dialogueClient);
    public EnumClientChanged m_enumClientChangedEvent;
    public delegate void ValueSanChanged(int valueSan);
    public ValueSanChanged m_valueSanChangedEvent;
}
