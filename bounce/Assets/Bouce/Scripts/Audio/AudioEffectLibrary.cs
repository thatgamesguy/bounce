using UnityEngine;
using Bounce.AssetManagement;

namespace Bounce
{
    /// <summary>
    /// Implementation of GroupAssetLibrary.  
    /// </summary>
    public class AudioEffectLibrary : MonoBehaviour
    {
        private GroupAssetLibrary<AudioClip> m_AudioLibrary;

        void Awake()
        {
            m_AudioLibrary = new GroupAssetLibrary<AudioClip>("sound_effects_list");
        }

        /// <summary>
        /// Wrapper for GroupAssetLibrary method.
        /// </summary>
        /// <param name="name"> Name of group asset. </param>
        /// <returns></returns>
        public AudioClip[] GetGroupWithName(string name)
        {
            return m_AudioLibrary.GetGroupAssets( name );
        }

        /// <summary>
        /// THe number of assets contained within the group. Returns -1 if no group found.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public int ClipCount(string groupName)
        {
            var group = m_AudioLibrary.GetGroupAssets(groupName);

            if (group != null)
            {
                return group.Length;
            }

            return -1;
        }
    }
}