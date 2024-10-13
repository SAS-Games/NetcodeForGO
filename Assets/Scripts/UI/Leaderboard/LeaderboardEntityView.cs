using System;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class LeaderboardEntityView : MonoBehaviour, IComparable<LeaderboardEntityView>
{
    [SerializeField] private TMP_Text m_DisplayText;

    private FixedString32Bytes _displayName;

    public int TeamIndex { get; private set; }
    public ulong ClientId { get; private set; }
    public int Coins { get; private set; }

    public void Initialise(ulong clientId, FixedString32Bytes displayName, int coins)
    {
        ClientId = clientId;
        _displayName = displayName;

        UpdateCoins(coins);
    }

    public void Initialise(int teamIndex, FixedString32Bytes displayName, int coins)
    {
        TeamIndex = teamIndex;
        _displayName = displayName;

        UpdateCoins(coins);
    }

    public void SetColour(Color colour)
    {
        m_DisplayText.color = colour;
    }

    public void UpdateCoins(int coins)
    {
        Coins = coins;

        UpdateText();
    }

    public void UpdateText()
    {
        m_DisplayText.text = $"{transform.GetSiblingIndex() + 1}. {_displayName} ({Coins})";
    }

    int IComparable<LeaderboardEntityView>.CompareTo(LeaderboardEntityView other)
    {
        if (other == null) return 1;

        // Compare by Coins, descending (higher coins first)
        return other.Coins.CompareTo(Coins);
    }
}
