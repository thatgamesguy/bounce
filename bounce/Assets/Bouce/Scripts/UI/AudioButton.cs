using UnityEngine;
using UnityEngine.UI;

namespace Bounce
{
    /// <summary>
    /// Handles audio button press events. Disables and enables the playing of audio.
    /// </summary>
	public class AudioButton : MonoBehaviour 
	{
        public Image buttonImage;
		public Sprite audioOnSprite;
		public Sprite audioOffSprite;

		private IAudioControl m_AudioController;

		void Awake()
		{
			m_AudioController = GameObject.FindGameObjectWithTag( "Audio" ).GetComponent<IAudioControl>();
		}

		public void OnAudioButtonPressed()
		{
			if( m_AudioController.ShouldPlay() )
			{
				m_AudioController.StopAudio();

				if( audioOffSprite != null )
				{
					buttonImage.sprite = audioOffSprite;
				}
		
			}
			else
			{
				m_AudioController.ResumeAudio();

				if( audioOnSprite != null )
				{
					buttonImage.sprite = audioOnSprite;
				}

			}
		}
	}
}