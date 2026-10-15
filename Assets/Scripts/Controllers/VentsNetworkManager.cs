using System.Collections.Generic;
using UnityEngine;

public class VentsNetworkManager : MonoBehaviour
{
    public static VentsNetworkManager Instance { get; private set; }

    private readonly List<Vent> _registeredVents = new();
    public IReadOnlyList<Vent> RegisteredVents => _registeredVents;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError(
                "Multiple VentNetworkManagers found in this Scene!",
                this
            );
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ValidateConnections();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void RegisterVent(Vent vent)
    {
        if (vent == null ||
        _registeredVents.Contains(vent))
            return;

        _registeredVents.Add(vent);
    }

    public void UnregisterVent(Vent vent)
    {
        if (vent == null) return;

        _registeredVents.Remove(vent);
    }

    public IReadOnlyList<Vent> GetConnectedVents(
        Vent sourceVent)
    {
        if (sourceVent == null)
            return System.Array.Empty<Vent>();

        return sourceVent.ConnectedVents;
    }

    public void ValidateConnections()
    {
        foreach (Vent vent in _registeredVents)
        {
            if (vent == null) continue;

            HashSet<Vent> uniqueConnections = new();

            foreach (Vent connectedVent in vent.ConnectedVents)
            {
                if (connectedVent == null)
                {
                    Debug.LogWarning(
                        $"Vent '{vent.name}' has an empty connection slot.",
                        vent
                    );
                    continue;
                }
                if (connectedVent == vent)
                {
                    Debug.LogWarning(
                        $"Vent '{vent.name}' cannot connect to itself.",
                        vent
                    );
                    continue;
                }

                if (!uniqueConnections.Add(connectedVent))
                    Debug.LogWarning(
                        $"Vent '{vent.name}' contains duplicate connection to {connectedVent.name}",
                        vent
                    );
            }
        }
    }
}
