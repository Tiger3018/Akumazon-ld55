using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

public class BackgroundDialogue : DialogueViewBase
{
    private Effects.CoroutineInterruptToken m_currentStopToken = new Effects.CoroutineInterruptToken();
    private bool m_checkStatus = false;
    [SerializeField]
    private float m_fadeTime, m_showTime, m_checkTime;
    [SerializeField]
    private bool m_isPlayerView;

    internal CanvasGroup m_canvasGroup;

    private void Start()
    {
        // gameObject.SetActive(false);
        // m_canvasGroup = GetComponentInParent<CanvasGroup>();
        if (m_canvasGroup == null)
        {
            Debug.LogError("CanvasGroup is not found.");
        }
    }
    private void Awake()
    {
        m_canvasGroup = GetComponentInParent<CanvasGroup>();
        //  if (!m_checkStatus)
        {
            m_canvasGroup.alpha = 0;
            m_canvasGroup.blocksRaycasts = false;
        }
    }

    private void Reset()
    {
        m_canvasGroup = GetComponentInParent<CanvasGroup>();
        if (m_canvasGroup == null)
        {
            Debug.LogError("CanvasGroup is not found.");
        }
    }

    public static bool IsNotMyViewCheck(string characterName, bool isPlayerView, System.Action onAction)
    {
        if (characterName == "您" || characterName == "You")
        {
            if (!isPlayerView)
            {
                onAction();
                return true;
            }
        }
        else if (isPlayerView)
        {
            onAction();
            return true;
        }
        return false;
    }

    public override void RunLine(LocalizedLine dialogueLine, System.Action onDialogueLineFinished)
    {
        if (IsNotMyViewCheck(dialogueLine.CharacterName, m_isPlayerView, onDialogueLineFinished))
        {
            return;
        }
        if (!m_isPlayerView) // Is Client View
        {
            if (dialogueLine.CharacterName == null || dialogueLine.CharacterName.Length == 0 ||
                dialogueLine.CharacterName == "Error")
            {
                Debug.LogError("Error parse assert, because CharacterName is " + dialogueLine.CharacterName);
                return;
            }
        }
    }

    public override void DialogueStarted()
    {
        // m_checkStatus = true;
        if (true)
        {
            if (m_currentStopToken.CanInterrupt)
            {
                m_currentStopToken.Interrupt();
            }
            StartCoroutine(Effects.FadeAlpha(m_canvasGroup, m_canvasGroup.alpha, 1, m_showTime, m_currentStopToken));
            m_canvasGroup.blocksRaycasts = true;
        }
    }

    public override void DialogueComplete()
    {
        if (true)
        {
            // m_checkStatus = false;
            if (m_currentStopToken.CanInterrupt)
            {
                m_currentStopToken.Interrupt();
            }
            StartCoroutine(Effects.FadeAlpha(m_canvasGroup, m_canvasGroup.alpha, 0, m_fadeTime, m_currentStopToken));
            m_canvasGroup.blocksRaycasts = false;
        }
    }

    private IEnumerator DelayedDeactivate() // No need, since all dialogue process will need me.
    {
        yield return new WaitForSeconds(m_checkTime);
        if (m_checkStatus || !gameObject.activeSelf)
        {
            yield break;
        }
        yield return Effects.FadeAlpha(m_canvasGroup, 1, 0, m_fadeTime, m_currentStopToken);
        // gameObject.SetActive(false);
    }
}
