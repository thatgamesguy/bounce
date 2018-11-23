using UnityEngine;

namespace Bounce
{
    /// <summary>
    /// Interface for an audio event.
    /// </summary>
    public interface IAudioEvent
	{
		AudioClip audio { get; }
        Vector3? position { get; }
    }
}
