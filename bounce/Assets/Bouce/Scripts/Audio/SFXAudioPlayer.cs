using UnityEngine;
using Bounce.EventManagement;

namespace Bounce
{
    /// <summary>
    /// Listens for audio events and plays corresponding audio clip.
    /// </summary>
	public class SFXAudioPlayer : MonoBehaviour, IAudioControl
	{
        /// <summary>
        /// Maximum number if clips to be queued.
        /// </summary>
		public int maxPending = 30;

		private IAudioEvent[] m_Pending;
		private int m_Head;
		private int m_Tail;
		private AudioSource m_2DAudioSource;
		private bool m_ShouldPlay;

		void Awake ()
		{
            var sourceObj = new GameObject("Audio Effect 2D Source");
            sourceObj.transform.SetParent(transform);
            m_2DAudioSource = sourceObj.AddComponent<AudioSource>();
		}

		void Start()
		{
			m_2DAudioSource.spatialBlend = 0f;
		}

		void OnEnable ()
		{
			m_Head = m_Tail = 0;
			
			m_Pending = new IAudioEvent [maxPending];
			
			ResumeAudio();
		}

		void OnDisable ()
		{
			StopAudio();
		}

		public bool ShouldPlay()
		{
			return m_ShouldPlay;
		}

		public void StopAudio()
		{
			Events.instance.RemoveListener<SFXAudioEvent> (OnAudio);
			m_ShouldPlay = false;
		}

		public void ResumeAudio()
		{
			Events.instance.AddListener<SFXAudioEvent> (OnAudio);
			m_ShouldPlay = true;
		}

		void Update ()
		{
			if ( m_Head == m_Tail )
				return;

            if ( m_Pending[m_Head].position.HasValue )
            {
                AudioSource.PlayClipAtPoint( m_Pending[m_Head].audio, m_Pending[m_Head].position.Value) ;
            }
            else
            {
                m_2DAudioSource.PlayOneShot( m_Pending[m_Head].audio );
            }

            m_Head = ( m_Head + 1 ) % maxPending;
		}


		private void OnAudio( IAudioEvent e )
		{
			if ( e.audio == null ) 
			{
				return;
			}

			for ( int i = m_Head; i != m_Tail; i = (i + 1) % maxPending ) 
			{
				if ( m_Pending [i].audio.name.Equals( e.audio.name ) ) 
				{
					return;
				}
			}
			
			m_Pending [m_Tail] = e;
			m_Tail = (m_Tail + 1) % maxPending;
		}
		
	}
}
