using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class Summon : ClickDelegate
{

    private string[] m_demonColor =
        new string[3] { "AOBDHSERLH", "IICODIDDAL", "CRAROMRRWE" }; // Also change in switch case
    private string[] m_demonLike = new string[3] { "11011", "10101", "01010" };
    public TMPro.TextMeshProUGUI m_keyTextUI;

    protected void Start()
    {
        base.Start();
        GameSession.Instance.m_selectedViewEnum = GameSession.ViewClick.None;
    }

    override protected bool onClickUpLeftDelegate()
    {
        if (m_keyTextUI == null)
        {
            Debug.LogError("Key Text UI is null");
            return false;
        }
        switch (GameSession.Instance.m_enumStage)
        {
        case (int)GameSession.SessionStage.Wait:
            GameSession.Instance.SetStage((int)GameSession.SessionStage.Dialogue);
            break;
        case (int)GameSession.SessionStage.Summon:
            if (GameSession.Instance.m_selectedView == -1 || m_keyTextUI.text.Length != 10)
            {
                Debug.Log("Summon Failed - May play music here");
                // play music
                m_keyTextUI.GetComponentInParent<KeyTextUI>()?.ClearKey(); // May be NULL TODO
                return false;
            }
            CheckSummonResult();
            break;
        case (int)GameSession.SessionStage.DesiredDemon:
            GameSession.Instance.SetStage((int)GameSession.SessionStage.Dialogue);
            break;
        case (int)GameSession.SessionStage.WrongDemon:
        case (int)GameSession.SessionStage.NoDemon:
            GameSession.Instance.SetStage((int)GameSession.SessionStage.Summon);
            break;
        }
        return false;
    }
    private void CheckSummonResult()
    {
        int checkValidRiddle = -1, checkValidCandle = -1, currentClient = GameSession.Instance.m_enumClient;
        string candleString = "";

        // Check for Riddle
        switch (m_keyTextUI.text[0])
        {
        case 'A':
            checkValidRiddle = m_keyTextUI.text.Equals(m_demonColor[0]) ? (int)GameSession.ColorRiddle.RedFire : -1;
            break;
        case 'I':
            checkValidRiddle = m_keyTextUI.text.Equals(m_demonColor[1]) ? (int)GameSession.ColorRiddle.GreenPlant : -1;
            break;
        case 'C':
            checkValidRiddle = m_keyTextUI.text.Equals(m_demonColor[2]) ? (int)GameSession.ColorRiddle.BlueDisease : -1;
            break;
        default:
            Debug.Log("No Summon There - Riddle Failed");
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
            return;
        }
        else if (selectedDemon != wantDemon)
        {
            Debug.Log("Summon Compare " + selectedDemon + " with desired " + wantDemon + " Failed");
            GameSession.Instance.SetStage((int)GameSession.SessionStage.WrongDemon);
            return;
        }
        else if (currentClient == 3)
        {
            Debug.Log("Summon Success End GOOD");
            GameSession.Instance.SetStage((int)GameSession.SessionStage.GoodEnd);
            return;
        }
        else
        {
            GameSession.Instance.SetEnumClient(currentClient + 1);
            GameSession.Instance.SetStage((int)GameSession.SessionStage.DesiredDemon);
        }
    }
}
