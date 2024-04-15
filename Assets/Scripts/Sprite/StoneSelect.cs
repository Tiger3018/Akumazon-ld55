using System.Linq;
using UnityEngine;

class StoneSelect : ClickDelegate
{
    [SerializeField]
    private int m_stoneId;
    [SerializeField]
    private bool _m_stoneSelected = false;
    public bool m_stoneSelected
    {
        get => _m_stoneSelected;
        set {
            _m_stoneSelected = value;
            if (_m_stoneSelected)
            {
                VoidOtherSelected();
                m_stoneSelectedTransform.gameObject.SetActive(true);
                GameSession.Instance.m_selectedViewEnum = (GameSession.ViewClick)m_stoneId;
            }
            else
            {
                m_stoneSelectedTransform.gameObject.SetActive(false);
                if (GameSession.Instance.m_selectedViewEnum == (GameSession.ViewClick)m_stoneId)
                {
                    GameSession.Instance.m_selectedViewEnum = GameSession.ViewClick.None;
                }
            }
        }
    }
    private Transform m_stoneSelectedTransform;
    private StoneSelect[] m_stoneSiblingSelectable = new StoneSelect[0];

    private void Start()
    {
        if (m_stoneSelectedTransform == null)
        {
            m_stoneSelectedTransform = gameObject.GetComponentsInChildren<Transform>(true)[1];
        }
        m_stoneSelected = false;
        // Debug.Log(gameObject.GetComponentsInParent<StoneSelect>(true)); //Empty
        StoneSelect[] stoneSiblingSelectable =
            gameObject.GetComponentInParent<RiddleDialog>().transform.GetComponentsInChildren<StoneSelect>();
        foreach (StoneSelect varTransform in stoneSiblingSelectable)
        {
            if (varTransform != this)
            {
                m_stoneSiblingSelectable = m_stoneSiblingSelectable.Concat(new[] { varTransform }).ToArray();
            }
        }
        // GameSession.Instance.m_selectedViewEnum = GameSession.ViewClick.None; // to avoid run three times, handle
        // this in summon
        base.Start();
    }

    private void OnEnable()
    {
        if (m_stoneSelectedTransform == null)
        {
            m_stoneSelectedTransform = gameObject.GetComponentsInChildren<Transform>(true)[1];
            Debug.Log("When processing OnEnable, I also register the m_stoneSelectedTransform.");
        }
        m_stoneSelected = m_stoneSelected;
    }

    override protected bool onClickUpLeftDelegate()
    {
        m_stoneSelected = !m_stoneSelected;
        return false;
    }

    private void VoidOtherSelected()
    {
        foreach (StoneSelect stone in m_stoneSiblingSelectable)
        {
            stone.m_stoneSelected = false;
        }
    }
}