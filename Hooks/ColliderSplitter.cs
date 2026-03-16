using OnirismLiveSplit.Utils;
using UnityEngine;

namespace OnirismLiveSplit.Hooks;
public class ColliderSplitter : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Log.Debug("ColliderSplitter.OnTriggerEnter()");
        if (Entity.players is null) { Log.Debug("no players list"); return; }
        if (Entity.players.Count == 0) { Log.Debug("player list empty");  return; }

        var player = other.GetComponent<Entity>();
        if (!player) { Log.Debug("no entity found"); return; }
        if (Entity.players[0] != player) { Log.Debug("entity is not player"); return; }

        Log.Debug("invoking OnPlayerEnter");
        GameEvents.NotifyEvent(new (Events.EventType.SectorActivator, gameObject.name));
    }
}
