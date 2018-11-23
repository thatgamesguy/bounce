using UnityEngine;
using Bounce.EventManagement;

namespace Bounce
{
    /// <summary>
    /// Raised when an audioclip should be played.
    /// </summary>
    public class SFXAudioEvent : GameEvent, IAudioEvent
	{
        /// <summary>
        /// The audioclip to play.
        /// </summary>
		public AudioClip audio { get; private set; }

        /// <summary>
        /// Position of audioclip. Null if 2D.
        /// </summary>
        public Vector3? position { get; private set; }

		public SFXAudioEvent (AudioClip audio, Vector3? position = null)
		{
            this.audio = audio;
            this.position = position;
		}
	
	}
}
