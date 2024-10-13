using System;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text queueStatusText;
    [SerializeField] private TMP_Text queueTimerText;
    [SerializeField] private TMP_Text findMatchButtonText;
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private Toggle teamToggle;
    [SerializeField] private Toggle privateToggle;

    private bool _isMatchmaking;
    private bool _isCancelling;
    private bool _isBusy;
    private float _timeInQueue;

    private void Start()
    {
        if (Client.Instance == null) { return; }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        queueStatusText.text = string.Empty;
        queueTimerText.text = string.Empty;
    }

    private void Update()
    {
        if (_isMatchmaking)
        {
            _timeInQueue += Time.deltaTime;
            TimeSpan ts = TimeSpan.FromSeconds(_timeInQueue);
            queueTimerText.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
        }
    }

    public async void FindMatchPressed()
    {
        if (_isCancelling) { return; }

        if (_isMatchmaking)
        {
            queueStatusText.text = "Cancelling...";
            _isCancelling = true;
            await Client.Instance.GameManager.CancelMatchmaking();
            _isCancelling = false;
            _isMatchmaking = false;
            _isBusy = false;
            findMatchButtonText.text = "Find Match";
            queueStatusText.text = string.Empty;
            queueTimerText.text = string.Empty;
            return;
        }

        if (_isBusy) { return; }

        Client.Instance.GameManager.MatchmakeAsync(teamToggle.isOn, OnMatchMade);
        findMatchButtonText.text = "Cancel";
        queueStatusText.text = "Searching...";
        _timeInQueue = 0f;
        _isMatchmaking = true;
        _isBusy = true;
    }

    private void OnMatchMade(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                queueStatusText.text = "Connecting...";
                break;
            case MatchmakerPollingResult.TicketCreationError:
                queueStatusText.text = "TicketCreationError";
                break;
            case MatchmakerPollingResult.TicketCancellationError:
                queueStatusText.text = "TicketCancellationError";
                break;
            case MatchmakerPollingResult.TicketRetrievalError:
                queueStatusText.text = "TicketRetrievalError";
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                queueStatusText.text = "MatchAssignmentError";
                break;
        }
    }

    public async void StartHost()
    {
        if (_isBusy)
            return;

        _isBusy = true;

        await Host.Instance.GameManager.StartHostAsync(privateToggle.isOn);

        _isBusy = false;
    }

    public async void StartClient()
    {
        if (_isBusy)
            return;

        _isBusy = true;

        await Client.Instance.GameManager.StartClientAsync(joinCodeField.text);

        _isBusy = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (_isBusy) { return; }

        _isBusy = true;

        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await Client.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        _isBusy = false;
    }
}
