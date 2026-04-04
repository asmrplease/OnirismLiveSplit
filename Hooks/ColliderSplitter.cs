using OnirismLiveSplit.Events;
using OnirismLiveSplit.Utils;
using UnityEngine;

namespace OnirismLiveSplit.Hooks;
public class ColliderSplitter : MonoBehaviour
{
    static readonly LayerMask playerLayer = LayerMask.NameToLayer("Player");
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != playerLayer) return;

        GameEvent e = new(Events.EventType.SectorActivator, gameObject.name);
        Log.Debug($"ColliderSplitter.OnTriggerEnter({e})");
        GameEvents.NotifyEvent(e);
    }
}
