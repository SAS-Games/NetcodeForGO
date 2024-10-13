using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform m_LeaderboardEntityHolder;
    [SerializeField] private Transform m_TeamLeaderboardEntityHolder;
    [SerializeField] private GameObject m_TeamLeaderboardBackground;
    [SerializeField] private LeaderboardEntityView m_LeaderboardEntityPrefab;
    [SerializeField] private int m_EntitiesToDisplay = 8;
    [SerializeField] private Color m_OwnerColour;
    [SerializeField] private string[] m_TeamNames;
    [SerializeField] private TeamColourLookup m_TeamColourLookup;

    private NetworkList<LeaderboardEntityState> _leaderboardEntities;
    private List<LeaderboardEntityView> _entityDisplays = new List<LeaderboardEntityView>();
    private List<LeaderboardEntityView> _teamEntityDisplays = new List<LeaderboardEntityView>();

    private EventBinding<PlayerSpawnedEvent> _playerSpawnedEventBinding;
    private EventBinding<PlayerDespawnedEvent> _playerDespawnedEventBinding;

    private void Awake()
    {
        _leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            if (Client.Instance.GameManager.UserData.userGamePreferences.gameQueue == GameQueue.Team)
            {
                m_TeamLeaderboardBackground.SetActive(true);

                for (int i = 0; i < m_TeamNames.Length; i++)
                {
                    LeaderboardEntityView teamLeaderboardEntity = Instantiate(m_LeaderboardEntityPrefab, m_TeamLeaderboardEntityHolder);
                    teamLeaderboardEntity.Initialise(i, m_TeamNames[i], 0);

                    Color teamColour = m_TeamColourLookup.GetTeamColour(i);
                    teamLeaderboardEntity.SetColour(teamColour);
                    _teamEntityDisplays.Add(teamLeaderboardEntity);
                }
            }

            _leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;
            foreach (LeaderboardEntityState entity in _leaderboardEntities)
            {
                HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderboardEntityState>
                {
                    Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }

        if (IsServer)
        {
            var players = FindObjectsByType<Tank>(FindObjectsSortMode.None);
            foreach (var player in players)
                HandlePlayerSpawned((PlayerSpawnedEvent)player);

            _playerSpawnedEventBinding = new EventBinding<PlayerSpawnedEvent>(HandlePlayerSpawned);
            _playerDespawnedEventBinding = new EventBinding<PlayerDespawnedEvent>(HandlePlayerDespawned);

            //todo:
           // EventBus<PlayerSpawnedEvent>.Register(_playerSpawnedEventBinding);
           // EventBus<PlayerDespawnedEvent>.Register(_playerDespawnedEventBinding);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
            _leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged;

        if (IsServer)
        {
            EventBus<PlayerSpawnedEvent>.Deregister(_playerSpawnedEventBinding);
            EventBus<PlayerDespawnedEvent>.Deregister(_playerDespawnedEventBinding);
        }
    }

    private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
    {
        if (!gameObject.scene.isLoaded) { return; }

        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                if (!_entityDisplays.Any(x => x.ClientId == changeEvent.Value.ClientId))
                {
                    LeaderboardEntityView leaderboardEntity =
                        Instantiate(m_LeaderboardEntityPrefab, m_LeaderboardEntityHolder);
                    leaderboardEntity.Initialise(
                        changeEvent.Value.ClientId,
                        changeEvent.Value.PlayerName,
                        changeEvent.Value.Coins);
                    if (NetworkManager.Singleton.LocalClientId == changeEvent.Value.ClientId)
                        leaderboardEntity.SetColour(m_OwnerColour);

                    _entityDisplays.Add(leaderboardEntity);
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                LeaderboardEntityView displayToRemove = _entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null);
                    Destroy(displayToRemove.gameObject);
                    _entityDisplays.Remove(displayToRemove);
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                LeaderboardEntityView displayToUpdate = _entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (displayToUpdate != null)
                    displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                break;
        }

        _entityDisplays.Sort();

        for (int i = 0; i < _entityDisplays.Count; i++)
        {
            _entityDisplays[i].transform.SetSiblingIndex(i);
            _entityDisplays[i].UpdateText();
            _entityDisplays[i].gameObject.SetActive(i <= m_EntitiesToDisplay - 1);
        }

        LeaderboardEntityView myDisplay = _entityDisplays.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);

        if (myDisplay != null)
        {
            if (myDisplay.transform.GetSiblingIndex() >= m_EntitiesToDisplay)
            {
                m_LeaderboardEntityHolder.GetChild(m_EntitiesToDisplay - 1).gameObject.SetActive(false);
                myDisplay.gameObject.SetActive(true);
            }
        }

        if (!m_TeamLeaderboardBackground.activeSelf)
            return;

        LeaderboardEntityView teamDisplay = _teamEntityDisplays.FirstOrDefault(x => x.TeamIndex == changeEvent.Value.TeamIndex);

        if (teamDisplay != null)
        {
            if (changeEvent.Type == NetworkListEvent<LeaderboardEntityState>.EventType.Remove)
                teamDisplay.UpdateCoins(teamDisplay.Coins - changeEvent.Value.Coins);
            else
                teamDisplay.UpdateCoins(teamDisplay.Coins + (changeEvent.Value.Coins - changeEvent.PreviousValue.Coins));

            _teamEntityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));

            for (int i = 0; i < _teamEntityDisplays.Count; i++)
            {
                _teamEntityDisplays[i].transform.SetSiblingIndex(i);
                _teamEntityDisplays[i].UpdateText();
            }
        }
    }

    private void HandlePlayerSpawned(PlayerSpawnedEvent playarSpawnedEvent)
    {
        var player = playarSpawnedEvent.tank;
        _leaderboardEntities.Add(new LeaderboardEntityState
        {
            ClientId = player.OwnerClientId,
            PlayerName = player.playerName.Value,
            TeamIndex = player.teamIndex.Value,
            Coins = 0
        });

        player.Wallet.TotalCoins.OnValueChanged += (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
    }

    private void HandlePlayerDespawned(PlayerDespawnedEvent playarDespawnedEvent)
    {
        var player = playarDespawnedEvent.tank;
        foreach (var entity in _leaderboardEntities)
        {
            if (entity.ClientId != player.OwnerClientId)
                continue;

            _leaderboardEntities.Remove(entity);
            break;
        }

        player.Wallet.TotalCoins.OnValueChanged -= (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
    }

    private void HandleCoinsChanged(ulong clientId, int newCoins)
    {
        for (int i = 0; i < _leaderboardEntities.Count; i++)
        {
            if (_leaderboardEntities[i].ClientId != clientId) { continue; }

            _leaderboardEntities[i] = new LeaderboardEntityState
            {
                ClientId = _leaderboardEntities[i].ClientId,
                PlayerName = _leaderboardEntities[i].PlayerName,
                TeamIndex = _leaderboardEntities[i].TeamIndex,
                Coins = newCoins
            };

            return;
        }
    }
}
