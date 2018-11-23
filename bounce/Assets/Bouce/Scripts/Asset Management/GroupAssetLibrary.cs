using System;
using System.Collections.Generic;

namespace Bounce.AssetManagement
{
    /// <summary>
    /// Responsible for loading assets from resource folder and allows named asset recovery. Provides ability to load groups of assets i.e. a group of audioclip.
    /// When recovering an asset from a group a random asset is returned.
    /// </summary>
    /// <typeparam name="T">Generic asset type.</typeparam>
    public class GroupAssetLibrary<T> : AssetLibrary<T> where T : UnityEngine.Object
	{
		private Dictionary<string, T[]> _groupLookup = new Dictionary<string, T[]> ();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileName">Name of file containing list of assets to load.</param>
        public GroupAssetLibrary( string fileName ) : base (fileName)
		{
		}

        /// <summary>
        /// Returns asset of type T if found, else returns default state of T.
        ///  If asset is part of a group, a random asset from the group is returned.
        /// </summary>
        /// <param name="name">name of asset as specified in file.</param>
        /// <returns>asset of type T with name name if found.</returns>
        public override T GetAssetFromName (string name)
		{
			var asset = base.GetAssetFromName (name);

			return (asset == null) ? GetAssetFromGroup (name) : asset;
		}

        /// <summary>
        /// Denotes whether the named asset is a group asset.
        /// </summary>
        /// <param name="name">name of asset.</param>
        /// <returns>returns true if asset is a group asset.</returns>
        public override bool IsGroup (string name)
		{
			return _groupLookup.ContainsKey( name );
		}

        /// <summary>
        /// Returns asset group with groupName.
        /// </summary>
        /// <param name="groupName"> Name of asset group. </param>
        /// <returns></returns>
        public T[] GetGroupAssets( string groupName )
        {
            if( !IsGroup( groupName ) )
            {
                return null;
            }

            return _groupLookup[groupName];
        }

		private T GetAssetFromGroup (string groupName)
		{
			if ( IsGroup ( groupName )) {
				var assets = _groupLookup [groupName];
				return assets[ UnityEngine.Random.Range( 0, assets.Length ) ];
			}

			return null;
		}

		protected override void BuildLibrary (string assetList)
		{
			string[] lines = assetList.Split (new string[] { Environment.NewLine }, StringSplitOptions.None);

			bool firstLine = true;

			foreach (var l in lines) {
				if (l.IsComment() || l.IsNullOrWhiteSpace ()) {
					continue;
				}

				if (firstLine) {
					_resourcePreText = l.Trim () + "/";
					firstLine = false;
					continue;
				}

				var split = l.Split (',');

				if (split.Length > 1) {
					T[] assets = new T[split.Length - 1];
					for (int i = 1; i < split.Length; i++) {
						assets [i - 1] = LoadAssetFromPath (split [i]);
					}

					AddAssetToGroupLibrary (split [0].Trim (), assets);
				} else {
					AddAssetToLibrary (GetFileNameFromPath (split [0]), LoadAssetFromPath (split [0]));
				}

			}
		}

		private void AddAssetToGroupLibrary (string id, T[] asset)
		{
			_groupLookup.Add (id, asset);

		}
	}
}
