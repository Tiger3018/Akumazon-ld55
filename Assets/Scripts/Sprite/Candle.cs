using UnityEngine;

public class Candle : ClickDelegate
{
    [SerializeField]
    private int _m_candleId;
    [SerializeField]
    private bool _m_candleFired = true;
    [SerializeField]
    private int _m_candleColor = 0;
    private Transform m_candleFireTransform;
    private AudioSource m_audioSource;
    public int m_candleId
    {
        get => _m_candleId;
        set {
            if (value < 0 || value > 5)
            {
                Debug.LogError("Invalid candle id: " + value);
                return;
            }
            _m_candleId = value;
        }
    }
    public bool m_candleFired
    {
        get => _m_candleFired;
        set {
            if (_m_candleFired != value)
            {
                _m_candleFired = value;
                ChangeFired();
                GameSession.Instance.m_selectedCandle[_m_candleId] = _m_candleFired;
                if (_m_candleFired)
                {
                    m_audioSource.Play();
                }
            }
        }
    }
    public int m_candleColor
    {
        get => _m_candleColor;
        set {
            if (value < 0 || value > 3)
            {
                Debug.LogError("Invalid candle color: " + value);
                return;
            }
            _m_candleColor = value;
            ChangeColor();
        }
    }

    private void Start()
    {
        base.Start();
        m_candleColor = _m_candleColor;
        m_candleFired = _m_candleFired;
        m_candleId = _m_candleId;
        m_candleFireTransform = gameObject.GetComponentsInChildren<Transform>()[1];
        m_audioSource = GetComponent<AudioSource>();
    }

    override protected bool onClickUpLeftDelegate()
    {
        m_candleColor = (m_candleColor + 1) % 4;
        return true;
    }

    override protected bool onClickUpRightDelegate()
    {
        m_candleFired = !m_candleFired;
        return true;
    }

    private void ChangeColor()
    {
        Debug.Log("Candle color TODO changed");
    }

    private void ChangeFired()
    {
        m_candleFireTransform.gameObject.SetActive(_m_candleFired);
    }
}
