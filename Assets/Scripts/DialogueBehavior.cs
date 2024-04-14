using UnityEngine;
using Yarn.Unity;

public class DialogueBehavior : MonoBehaviour
{
    internal DialogueRunner m_dialogueRunner;

    private void Awake()
    {
        m_dialogueRunner = GetComponent<DialogueRunner>();
        m_dialogueRunner.startAutomatically = false;
    }
    private void Start()
    {
        GameSession.Instance.m_sessionIdChangedEvent += CheckDialogueAndUpdate;
        if (m_dialogueRunner == null)
        {
            Debug.LogError("DialogueRunner is not found.");
        }
    }

    private void CheckDialogueAndUpdate(int stage, int dialogue, int textId)
    {
        if (dialogue < 0 || dialogue > 3) // TODO
        {
            Debug.LogError("Invalid dialogue: " + dialogue);
            return;
        }
        if (m_dialogueRunner.IsDialogueRunning)
        {
            if (stage != (int)GameSession.SessionStage.Dialogue)
            {
                m_dialogueRunner.Stop();
            }
            if (dialogue != GameSession.Instance.m_sessionId[1])
            {
                m_dialogueRunner.Stop();
                m_dialogueRunner.StartDialogue("Client_" + (dialogue + 1).ToString());
            }
        }
        if (!m_dialogueRunner.IsDialogueRunning && stage == (int)GameSession.SessionStage.Dialogue)
        {
            m_dialogueRunner.StartDialogue("Client_" + (dialogue + 1).ToString());
        }
    }
}
