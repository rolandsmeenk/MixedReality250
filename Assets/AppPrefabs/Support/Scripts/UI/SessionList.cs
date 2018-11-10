using HoloToolkit.Examples.SharingWithUNET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Builds a list of local network session and shows them in a list.
/// Uses NetworkDiscoveryWithAnchors to detect other Unity applications.
/// </summary>
public class SessionList : MonoBehaviour
{
    public Transform contentPanel;
    public GameObject listItemPrefab;

    /// <summary>
    /// Object which controls finding sessions so we know what to put on our buttons.
    /// </summary>
    private NetworkDiscoveryWithAnchors _networkDiscovery;

    /// <summary>
    /// Current list of sessions.
    /// </summary>
    private Dictionary<string, NetworkDiscoveryWithAnchors.SessionInfo> _sessionList;

    private void Start()
    {
        _networkDiscovery = null;
    }

    public void ResetAutoconnect()
    {
        _networkDiscovery = null;
    }

    private void OnDestroy()
    {
        if (_networkDiscovery != null)
        {
            _networkDiscovery.SessionListChanged -= NetworkDiscovery_SessionListChanged;
            _networkDiscovery = null;
        }        
    }

    private void Update()
    {
        CheckNetworkDiscoveryInitialized();
    }

    private void CheckNetworkDiscoveryInitialized()
    {
        if (_networkDiscovery == null &&
            NetworkDiscoveryWithAnchors.Instance != null)
        {
            NetworkDiscoveryWithAnchors nd = NetworkDiscoveryWithAnchors.Instance;

            if (nd.running)
            {
                _networkDiscovery = nd;
            }

            if (_networkDiscovery != null)
            {
                _sessionList = _networkDiscovery.remoteSessions;
                RefreshDisplay();
                _networkDiscovery.SessionListChanged += NetworkDiscovery_SessionListChanged;
                _networkDiscovery.ConnectionStatusChanged += _networkDiscovery_ConnectionStatusChanged;
            }
        }
    }

    void RefreshDisplay()
    {
        RemoveButtons();
        AddButtons();
    }

    private void RemoveButtons()
    {
        while (contentPanel.childCount > 0)
        {
            GameObject toRemove = contentPanel.GetChild(0).gameObject;
            toRemove.transform.SetParent(null);
            Destroy(toRemove);
        }
    }

    private void AddButtons()
    {
        foreach (var session in _sessionList)
        {
            GameObject newButton = GameObject.Instantiate(listItemPrefab);
            newButton.transform.SetParent(contentPanel);

            ListItem listItem = newButton.GetComponent<ListItem>();
            if (listItem != null)
            {
                listItem.Setup(session.Value.SessionName, session.Value);
                listItem.ItemClicked += (object source, object selectedSession) =>
                {
                    if (selectedSession != null)
                    {
                        JoinSession((NetworkDiscoveryWithAnchors.SessionInfo )selectedSession);
                    };
                };
            }
        }
    }

    /// <summary>
    /// When we are connected we want to disable the UI
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">the event data</param>
    private void NetworkDiscovery_ConnectionStatusChanged(object sender, EventArgs e)
    {
        gameObject.SetActive(_networkDiscovery.running && !_networkDiscovery.isServer);
        // sets the UI to be active when not connected and disabled when connected.
        SetChildren(_networkDiscovery.running && !_networkDiscovery.isServer);
    }

    /// <summary>
    /// Called when a session is discovered
    /// </summary>
    /// <param name = "sender" > The sender of the event</param>
    /// <param name="e">the event data</param>
    private void NetworkDiscovery_SessionListChanged(object sender, EventArgs e)
    {
        _sessionList = _networkDiscovery.remoteSessions;

        RefreshDisplay();
    }


    /// <summary>
    /// Sometimes it is useful to disable rendering 
    /// </summary>
    /// <param name="Enabled"></param>
    void SetChildren(bool Enabled)
    {
        foreach (RectTransform mr in GetComponentsInChildren<RectTransform>())
        {
            mr.gameObject.SetActive(Enabled);
        }
    }

    public void JoinSession(NetworkDiscoveryWithAnchors.SessionInfo session)
    {
        if (session != null && _networkDiscovery.running)
        {
            _networkDiscovery.JoinSession(session);
        }
    }

    private void _networkDiscovery_ConnectionStatusChanged(object sender, EventArgs e)
    {
        SetChildren(_networkDiscovery.running && !_networkDiscovery.isServer);
    }
}
