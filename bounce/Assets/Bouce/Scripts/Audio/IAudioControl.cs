namespace Bounce
{
    /// <summary>
    /// Interface for any class that controls audio playback.
    /// </summary>
	public interface IAudioControl  
	{
        /// <summary>
        /// 
        /// </summary>
        /// <returns> If any audio device should play. </returns>
		bool ShouldPlay();

		void StopAudio();
		void ResumeAudio();
	}
}
