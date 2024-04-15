using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class KeyReplayTextUI : MonoBehaviour
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
    private uint m_keyLength;
    private string m_keySaved;
    private string[] m_demonColor =
        new string[3] { "AOBDHSERLH", "IICODIDDAL", "CRAROMRRWE" }; // RBG, Fire Disease Plant

    private void Awake()
    {
        m_inputField = m_inputFieldObject.GetComponent<TMP_InputField>();
        // m_parentObject = ;
        m_parentCanvasGroup = GetComponentInParent<CanvasGroup>();
        m_parentRectTransform = m_parentCanvasGroup.transform.GetComponent<RectTransform>();
        // m_overlayRectTransform = m_overlayObject.GetComponent<RectTransform>();
        m_textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        m_rectTransform = GetComponent<RectTransform>();
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
        if (stage == (int)GameSession.SessionStage.Summon)
        {
            // Debug.Log("Summon");
            m_keyLength = 0;
            m_keySaved = "";
            m_inputField.text = "";
            m_textMeshProUGUI.text = "";
            m_parentCanvasGroup.interactable = true;
            m_inputField.ActivateInputField();
            // m_inputField.OnPointerClick(null);
        }
        else if (m_parentCanvasGroup.interactable) // might be not isfocused
        {
            m_inputField.DeactivateInputField();
            StartCoroutine(UIAnimation.FadeAlpha(m_parentCanvasGroup, 1, 0, m_fadeTime));
            m_parentCanvasGroup.interactable = false;
            m_parentCanvasGroup.blocksRaycasts = false;
        }
    }
    public void UpdateWhenSelect(string str)
    {
        if (GameSession.Instance.m_enumStage != (int)GameSession.SessionStage.Summon)
        {
            Debug.LogError("This ReplayInput should not be selected.");
            m_inputField.DeactivateInputField();
        }
        m_inputField.caretPosition = m_inputField.text.Length;
    }

    public void UpdateWhenDeselect(string str)
    {
        if (GameSession.Instance.m_enumStage == (int)GameSession.SessionStage.Summon)
        {
            m_inputField.ActivateInputField();
        }
    }

    public void UpdateWhenChanged(string givenStr)
    {
        Debug.Log("Changed: " + givenStr);
        if (givenStr.Length == m_keyLength)
        {
            // m_rectTransform.sizeDelta = new Vector2(0, 0);
            return; // Avoid Callback Twice.
        }
        string replaceStr = givenStr.ToUpper();
        if (givenStr.Length < m_keyLength)
        {
            replaceStr = m_keySaved;
        }
        else if (givenStr.Length == 1)
        {
            StartCoroutine(UIAnimation.FadeAlpha(m_parentCanvasGroup, 0, 1, m_fadeTime));
            m_parentCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            for (int i = 0; i < m_keySaved.Length; i++)
            {
                if (replaceStr[i] != m_keySaved[i])
                {
                    if (replaceStr[i] >= 'A' && replaceStr[i] <= 'Z')
                    {
                        replaceStr = m_keySaved + replaceStr[i];
                    }
                    else
                    {
                        replaceStr = m_keySaved;
                    }
                    break;
                }
            }
            if (replaceStr.Length > m_keySaved.Length)
            {

                if (replaceStr[replaceStr.Length - 1] >= 'A' && replaceStr[replaceStr.Length - 1] <= 'Z')
                {
                }
                else
                {
                    replaceStr = m_keySaved;
                }
            }
        }
        m_keySaved = replaceStr;
        m_keyLength = (uint)replaceStr.Length;
        m_inputField.text = replaceStr;
        // https://discussions.unity.com/t/inputfield-disable-selecting/196793/2
        //  m_inputField.caretPosition = m_inputField.text.Length; // Seems to be no affect
        m_textMeshProUGUI.text = replaceStr;

        if (replaceStr.Length == 10)
        {
            m_textMeshProUGUI.color = new Color(107f / 255, 22f / 255, 22f / 255, 1);
        }
        else if (replaceStr.Length > 10)
        {
            m_textMeshProUGUI.color = new Color(50f / 255, 50f / 255, 50f / 255, 0.15f);
        }
        else
        {
            m_textMeshProUGUI.color = new Color(50f / 255, 50f / 255, 50f / 255, 0.7f);
        }
    }

    public void ClearKey()
    {
        StartCoroutine(UIAnimation.FadeAlpha(m_parentCanvasGroup, 1, 0, m_fadeTime));
        m_keySaved = "";
        m_keyLength = 0;
        m_inputField.text = "";
        StartCoroutine(UIAnimation.WaitSecondsThenClear(m_fadeTime, m_textMeshProUGUI));
        // m_textMeshProUGUI.text = "";
        m_parentCanvasGroup.blocksRaycasts = false;
    }
}