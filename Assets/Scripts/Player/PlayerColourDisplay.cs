using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColourDisplay : MonoBehaviour
{
    [SerializeField] private TeamColourLookup m_TeamColourLookup;
    [SerializeField] private Tank m_Player;
    [SerializeField] private SpriteRenderer[] m_PlayerSprites;

    private void Start()
    {
        HandleTeamChanged(-1, m_Player.teamIndex.Value);
        m_Player.teamIndex.OnValueChanged += HandleTeamChanged;
    }

    private void OnDestroy()
    {
        m_Player.teamIndex.OnValueChanged -= HandleTeamChanged;
    }

    private void HandleTeamChanged(int oldTeamIndex, int newTeamIndex)
    {
        Color teamColour = m_TeamColourLookup.GetTeamColour(newTeamIndex);

        foreach (SpriteRenderer sprite in m_PlayerSprites)
            sprite.color = teamColour;
    }
}
