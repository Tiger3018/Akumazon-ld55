using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class Summon : MonoBehaviour
{

    private string[] m_demonColor =
        new string[3] { "AOBDHSERLH", "IICODIDDAL", "CRAROMRRWE" }; // Also change in switch case
    private string[] m_demonLike = new string[3] { "11011", "10101", "01010" };
    public TMPro.TextMeshProUGUI m_keyTextUI;
    public GameObject m_badEndDialogue, m_goodEndDialogue;
    private GameObject m_continueGameObject, m_retryGameObject;
    private CanvasGroup m_continueCanvasGroup, m_retryCanvasGroup;
    private AudioSource m_audioSource;

    protected void Start()
    {
        GameSession.Instance.m_selectedViewEnum = GameSession.ViewClick.None;
        GameSession.Instance.m_enumStageChangedEvent += ControlButton;
        m_continueGameObject = GameObject.Find("ProceedButton");
        m_retryGameObject = GameObject.Find("RetryButton");
        m_continueCanvasGroup = m_continueGameObject.GetComponent<CanvasGroup>();
        m_retryCanvasGroup = m_retryGameObject.GetComponent<CanvasGroup>();
        // m_continueCanvasGroup.alpha = 0;
        m_retryCanvasGroup.alpha = 0;
        m_audioSource = GetComponent<AudioSource>();
    }

    private void ControlButton(int stage)
    {
        switch (stage)
        {
        case (int)GameSession.SessionStage.Wait:
        goto case (int)GameSession.SessionStage.DesiredDemon; case (int)GameSession.SessionStage.SummonValidRiddle:
        goto case (int)GameSession.SessionStage.DesiredDemon; case (int)GameSession.SessionStage.DesiredDemon:
            if (m_retryCanvasGroup.alpha > 0)
            {
                StartCoroutine(UIAnimation.FadeAlpha(m_retryCanvasGroup, 1, 0, .3f));
            }
            StartCoroutine(UIAnimation.FadeAlpha(m_continueCanvasGroup, 0, 1, .3f));
            break;
        case (int)GameSession.SessionStage.WrongDemon:
        goto case (int)GameSession.SessionStage.NoDemon; case (int)GameSession.SessionStage.SummonSecondTry:
        goto case (int)GameSession.SessionStage.NoDemon; case (int)GameSession.SessionStage.NoDemon:
            if (m_continueCanvasGroup.alpha > 0)
            {
                StartCoroutine(UIAnimation.FadeAlpha(m_continueCanvasGroup, 1, 0, .3f));
            }
            StartCoroutine(UIAnimation.FadeAlpha(m_retryCanvasGroup, 0, 1, .3f));
            break;
        default:
            if (m_retryCanvasGroup.alpha > 0)
            {
                StartCoroutine(UIAnimation.FadeAlpha(m_retryCanvasGroup, 1, 0, .3f));
            }
            if (m_continueCanvasGroup.alpha > 0)
            {
                StartCoroutine(UIAnimation.FadeAlpha(m_continueCanvasGroup, 1, 0, .3f));
            }
            break;
        }
    }

    public void onClickUpLeftDelegate()
    {
        // if (m_keyTextUI == null)
        //{
        //     Debug.LogError("Key Text UI is null");
        //     return;
        // }
        switch (GameSession.Instance.m_enumStage)
        {
        case (int)GameSession.SessionStage.Wait:
            GameSession.Instance.SetStage((int)GameSession.SessionStage.Dialogue);
            break;
        case (int)GameSession.SessionStage.SummonValidRiddle:
            if (GameSession.Instance.m_selectedView == -1 || GameSession.Instance.m_selectedKey.Length != 10)
            {
                Debug.Log("Summon Failed - May play music here");
                if (m_audioSource != null)
                {
                    m_audioSource.clip = Resources.Load<AudioClip>("Audio/sanloss");
                    m_audioSource.Play();
                }
                GameSession.Instance.m_selectedKey = "";
                GameSession.Instance.SetStage((int)GameSession.SessionStage.SimplyFailedSummon);
                GameSession.Instance.SetStage((int)GameSession.SessionStage.Summon);
                return;
            }
            CheckSummonResult();
            break;
        case (int)GameSession.SessionStage.SummonSecondTry:
            GameSession.Instance.SetStage((int)GameSession.SessionStage.Dialogue);
            break;
        case (int)GameSession.SessionStage.DesiredDemon:
            GameSession.Instance.SetStage((int)GameSession.SessionStage.Dialogue);
            break;
        case (int)GameSession.SessionStage.WrongDemon:
        case (int)GameSession.SessionStage.NoDemon:
            GameSession.Instance.SetStage((int)GameSession.SessionStage.SummonSecondTry);
            break;
        }
        return; // false;
    }
    private void CheckSummonResult()
    {
        int checkValidRiddle = -1, checkValidCandle = -1, currentClient = GameSession.Instance.m_enumClient;
        string candleString = "";

        // Check for Riddle
        switch (GameSession.Instance.m_selectedKey[0])
        {
        case 'A':
            checkValidRiddle =
                GameSession.Instance.m_selectedKey.Equals(m_demonColor[0]) ? (int)GameSession.ColorRiddle.RedFire : -1;
            break;
        case 'I':
            checkValidRiddle = GameSession.Instance.m_selectedKey.Equals(m_demonColor[1])
                                   ? (int)GameSession.ColorRiddle.BlueDisease
                                   : -1;
            break;
        case 'C':
            checkValidRiddle = GameSession.Instance.m_selectedKey.Equals(m_demonColor[2])
                                   ? (int)GameSession.ColorRiddle.GreenPlant
                                   : -1;
            break;
        default:
            Debug.Log("No Summon There - Riddle Failed");
            if (m_audioSource != null)
            {
                m_audioSource.clip = Resources.Load<AudioClip>("Audio/fault");
                m_audioSource.Play();
            }
            GameSession.Instance.SetStage((int)GameSession.SessionStage.NoDemon);
            return;
        }

        // Check for Candle
        for (int i = 0; i < 5; i++)
        {
            candleString += GameSession.Instance.m_selectedCandle[i] ? "1" : "0";
        }
        if (candleString.Equals(m_demonLike[0]))
        {
            checkValidCandle = (int)GameSession.LikeCandle.Love;
        }
        else if (candleString.Equals(m_demonLike[1]))
        {
            checkValidCandle = (int)GameSession.LikeCandle.Trail;
        }
        else if (candleString.Equals(m_demonLike[2]))
        {
            checkValidCandle = (int)GameSession.LikeCandle.Dollar;
        }
        else
        {
            Debug.Log("No Summon There - Candle Failed: " + candleString);
            if (m_audioSource != null)
            {
                m_audioSource.clip = Resources.Load<AudioClip>("Audio/fault");
                m_audioSource.Play();
            }
            GameSession.Instance.SetStage((int)GameSession.SessionStage.NoDemon);
            return;
        }

        int selectedDemon = GameSession.Instance.m_selectedView | checkValidCandle << 2 | checkValidRiddle << 4,
            wantDemon = GameSession.Instance.m_demonAnswer[currentClient][0][0] |
                        GameSession.Instance.m_demonAnswer[currentClient][0][1] << 2 |
                        GameSession.Instance.m_demonAnswer[currentClient][0][2] << 4;
        GameSession.Instance.m_demonSelected = selectedDemon;
        if (currentClient == 3 && checkValidRiddle == GameSession.Instance.m_demonAnswer[currentClient][1][2] &&
            checkValidCandle == GameSession.Instance.m_demonAnswer[currentClient][1][1] &&
            GameSession.Instance.m_selectedView == GameSession.Instance.m_demonAnswer[currentClient][1][0])
        {
            Debug.Log("Summon Success End BAD");
            GameSession.Instance.SetStage((int)GameSession.SessionStage.BadEnd);
            StartCoroutine(
                UIAnimation.WaitSecondsThenEnd(3.5f, m_badEndDialogue)); // GameObject.Find("badEndDialogue")));
            return;
        }
        else if (selectedDemon != wantDemon)
        {
            if (m_audioSource != null)
            {
                m_audioSource.clip = Resources.Load<AudioClip>("Audio/sanloss");
                m_audioSource.Play();
            }
            Debug.Log("Summon Compare " + selectedDemon + " with desired " + wantDemon + " Failed");
            GameSession.Instance.SetStage((int)GameSession.SessionStage.WrongDemon);
            return;
        }
        else if (currentClient == 3)
        {
            Debug.Log("Summon Success End GOOD");
            GameSession.Instance.SetStage((int)GameSession.SessionStage.GoodEnd);
            StartCoroutine(
                UIAnimation.WaitSecondsThenEnd(3.5f, m_goodEndDialogue)); // GameObject.Find("badEndDialogue")));
            return;
        }
        else
        {
            GameSession.Instance.SetEnumClient(currentClient + 1);
            GameSession.Instance.SetStage((int)GameSession.SessionStage.DesiredDemon);
        }
    }
}
