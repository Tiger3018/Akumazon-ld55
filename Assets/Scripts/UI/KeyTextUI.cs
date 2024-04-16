using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class KeyTextUI : MonoBehaviour
{
    public GameObject m_inputFieldObject;
    private TMP_InputField m_inputField;
    // public GameObject m_parentObject;
    private CanvasGroup m_parentCanvasGroup;
    private RectTransform m_parentRectTransform;
    // public GameObject m_overlayObject;
    // private RectTransform m_overlayRectTransform;
    public float m_marginWidth;
    public float m_fadeTime = 0.5f;
    private TextMeshProUGUI m_textMeshProUGUI;
    private RectTransform m_rectTransform;
    // private uint m_keyLength;
    // private string originalStr; // TODO Deprecated
    private AudioSource m_audioSource;
    private int m_enumStageLastSummon;

    private void Awake()
    {
        m_inputField = m_inputFieldObject.GetComponent<TMP_InputField>();
        // m_parentObject = ;
        m_parentCanvasGroup = GetComponentInParent<CanvasGroup>();
        m_parentRectTransform = m_parentCanvasGroup.transform.GetComponent<RectTransform>();
        // m_overlayRectTransform = m_overlayObject.GetComponent<RectTransform>();
        m_textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        m_rectTransform = GetComponent<RectTransform>();
        m_audioSource = GetComponent<AudioSource>();
        m_parentCanvasGroup.alpha = 0;
        m_parentCanvasGroup.interactable = false;
        m_parentCanvasGroup.blocksRaycasts = false;
    }

    private void Start()
    {
        GameSession.Instance.m_enumStageChangedEvent += UpdateFocus;
    }

    private void Update()
    {
        // Borrow idea from
        // https://forum.unity.com/threads/how-to-auto-resize-inputfield-according-to-text-inside.297812/
        // https://forum.unity.com/threads/does-the-content-size-fitter-work.484678/
        m_parentRectTransform.sizeDelta =
            new Vector2(m_textMeshProUGUI.preferredWidth + m_marginWidth, // + m_rectTransform.sizeDelta.y,
                        m_parentRectTransform.sizeDelta.y);
        // m_overlayRectTransform.localScale =
        //     new Vector2(m_fieldRectTransform.sizeDelta.x, m_fieldRectTransform.sizeDelta.y);
    }

    private void UpdateFocus(int stage)
    {
        if (CheckIfAnySummonState(stage))
        {
            Debug.Log("KeyTextUI is handling: " + stage);
            m_inputField.text = GameSession.Instance.m_selectedKey;
            m_parentCanvasGroup.interactable = true;
            m_inputField.ActivateInputField();
            Assert.IsTrue(m_inputField.IsActive());
            // m_inputField.OnPointerClick(null);
        }
        else if (m_parentCanvasGroup.interactable) // might be not isfocused
        {
            ClearKey(); // simply failed
            m_inputField.DeactivateInputField();
            StartCoroutine(UIAnimation.FadeAlpha(m_parentCanvasGroup, 1, 0, m_fadeTime));
            m_parentCanvasGroup.interactable = false;
            m_parentCanvasGroup.blocksRaycasts = false;
        }
    }

    static public bool CheckIfAnySummonState(int stage)
    {
        if (stage == (int)GameSession.SessionStage.Summon || stage == (int)GameSession.SessionStage.SummonSecondTry ||
            stage == (int)GameSession.SessionStage.SummonValidRiddle)
        {
            return true;
        }
        return false;
    }
    public void UpdateWhenSelect(string str)
    {
        if (!CheckIfAnySummonState(GameSession.Instance.m_enumStage))
        {
            Debug.LogError("This ReplayInput should not be selected.");
            m_inputField.DeactivateInputField();
        }
        m_inputField.caretPosition = m_inputField.text.Length;
    }

    public void UpdateWhenDeselect(string str)
    {
        Debug.Log("Key Deselected: " + str);
        if (CheckIfAnySummonState(GameSession.Instance.m_enumStage))
        {
            if (GameSession.Instance.m_selectedKey.Length == 0)
            {
                ClearKey(); // TODO now it can't get into there
            }
            m_inputField.ActivateInputField();
        }
    }

    public void UpdateWhenChanged(string givenStr)
    {
        Debug.Log("Key Changed: " + givenStr);
        string originalStr = GameSession.Instance.m_selectedKey; // TODO const
        if (givenStr.Length == originalStr.Length)
        {
            // m_rectTransform.sizeDelta = new Vector2(0, 0);
            return; // Avoid Callback Twice.
        }
        string replaceStr = givenStr.ToUpper();
        if (givenStr.Length < originalStr.Length)
        {
            replaceStr = originalStr;
        }
        else if (givenStr.Length == 1 && replaceStr[0] >= 'A' && replaceStr[0] <= 'Z')
        {
            StartCoroutine(UIAnimation.FadeAlpha(m_parentCanvasGroup, 0, 1, m_fadeTime));
            m_parentCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            for (int i = 0; i < originalStr.Length; i++)
            {
                if (replaceStr[i] != originalStr[i])
                {
                    if (replaceStr[i] >= 'A' && replaceStr[i] <= 'Z')
                    {
                        replaceStr = originalStr + replaceStr[i];
                    }
                    else
                    {
                        replaceStr = originalStr;
                    }
                    break;
                }
            }
            if (replaceStr.Length > originalStr.Length)
            {

                if (replaceStr[replaceStr.Length - 1] >= 'A' && replaceStr[replaceStr.Length - 1] <= 'Z')
                {
                }
                else
                {
                    replaceStr = originalStr;
                }
            }
        }
        if (originalStr !=
            replaceStr) // && m_audioSource.clip != null && m_audioSource.clip.loadState == AudioDataLoadState.Loaded)
        {
            m_audioSource.Play();
        }
        GameSession.Instance.m_selectedKey = replaceStr;
        m_inputField.text = replaceStr;
        // https://discussions.unity.com/t/inputfield-disable-selecting/196793/2
        //  m_inputField.caretPosition = m_inputField.text.Length; // Seems to be no affect
        m_textMeshProUGUI.text = replaceStr;

        if (replaceStr.Length == 10) // TODO
        {
            m_enumStageLastSummon = GameSession.Instance.m_enumStage;
            GameSession.Instance.SetStage((int)GameSession.SessionStage.SummonValidRiddle);
            m_textMeshProUGUI.color = new Color(107f / 255, 22f / 255, 22f / 255, 1);
        }
        else if (replaceStr.Length > 10)
        {
            if (GameSession.Instance.m_enumStage == (int)GameSession.SessionStage.SummonValidRiddle)
            {
                GameSession.Instance.SetStage(m_enumStageLastSummon);
            }
            m_textMeshProUGUI.color = new Color(50f / 255, 50f / 255, 50f / 255, 0.15f);
        }
        else
        {
            if (GameSession.Instance.m_enumStage == (int)GameSession.SessionStage.SummonValidRiddle)
            {
                GameSession.Instance.SetStage(m_enumStageLastSummon);
            }
            m_textMeshProUGUI.color = new Color(50f / 255, 50f / 255, 50f / 255, 0.7f);
        }
    }

    public void ClearKey()
    {
        if (GameSession.Instance.m_selectedKey.Length == 0)
        {
            return;
        }
        StartCoroutine(UIAnimation.FadeAlpha(m_parentCanvasGroup, 1, 0, m_fadeTime));
        GameSession.Instance.m_selectedKey = "";
        m_inputField.text = "";
        StartCoroutine(UIAnimation.WaitSecondsThenClear(m_fadeTime, m_textMeshProUGUI));
        m_parentCanvasGroup.blocksRaycasts = false;
    }
}