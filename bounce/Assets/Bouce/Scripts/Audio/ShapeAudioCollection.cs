using UnityEngine;

namespace Bounce
{
    /// <summary>
    /// COntains a group of clips to be played when a shape is hit.
    /// </summary>
    public class ShapeAudioCollection 
    {
        private AudioClip[] m_AudioClips;
        private int m_AudioIndex;
        private int m_InitialAudioIndex;

        public ShapeAudioCollection( AudioClip[] audioClips, 
            int shapeCount )
        {

            this.m_AudioClips = audioClips;

            if ( m_AudioClips.Length < shapeCount )
            {
                Debug.Log("Consumable shapes greater than number of audio clips");
                m_AudioIndex = 0;
            }
            else
            {
                m_AudioIndex = Random.Range( 0,  (m_AudioClips.Length - shapeCount) - 1 );
            }

            m_InitialAudioIndex = m_AudioIndex;

        }

        public AudioClip GetCurrentClip()
        {
            return m_AudioClips[ m_AudioIndex ];
        }

        public void IncrementIndex()
        {
            m_AudioIndex = (m_AudioIndex + 1) % m_AudioClips.Length;
        }

        public void Reset()
        {
            m_AudioIndex = m_InitialAudioIndex;
        }
    }
}
