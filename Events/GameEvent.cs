using System.ComponentModel;

namespace OnirismLiveSplit.Events
{
    public record GameEvent(EventType type, string name);
    public enum EventType
    {
        SceneLoad,
        SectorActivator,
        Cutscene,
        ItemCollection,
        JulieCall,
    }
}


//fixes a bug with the above constructor.
namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}