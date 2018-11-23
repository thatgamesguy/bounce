using UnityEngine;
using System;
using System.Collections.Generic;

namespace Bounce.AssetManagement
{
    /// <summary>
    /// Responsible for loading assets from resource folder and allows named asset recovery.
    /// </summary>
    /// <typeparam name="T">Generic asset type.</typeparam>
	public class AssetLibrary<T> where T : UnityEngine.Object
	{
		protected Dictionary<string, T> _assetLookUp = new Dictionary<string, T> ();

		protected string _resourcePreText;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileName">Name of file containing list of assets to load.</param>
		public AssetLibrary (string fileName)
		{
			string assetList = LoadFile (fileName);
			BuildLibrary (assetList);
		}

        /// <summary>
        /// Returns asset of type T if found, else returns default state of T.
        /// </summary>
        /// <param name="name">name of asset as specified in file.</param>
        /// <returns>asset of type T with name name if found.</returns>
		public virtual T GetAssetFromName (string name)
		{
			if (_assetLookUp.ContainsKey (name)) {
				return _assetLookUp [name];
			}

			return default(T);
		}

        /// <summary>
        /// Denotes whether the named asset is a group asset.
        /// </summary>
        /// <param name="name">name of asset.</param>
        /// <returns>returns true if asset is a group asset.</returns>
        public virtual bool IsGroup (string name)
		{
			return false;
		}


		protected string LoadFile (string fileName)
		{
			TextAsset txt = (TextAsset)Resources.Load (fileName, typeof(TextAsset));
			return txt.text;
		}

		protected virtual void BuildLibrary (string assetList)
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

				if (split.Length == 1) {
					AddAssetToLibrary (GetFileNameFromPath (split [0]), LoadAssetFromPath (split [0]));
				}
			}
		}

		protected void AddAssetToLibrary (string id, T asset)
		{
            if (asset == null)
            {
                return;
            }

			_assetLookUp.Add (id, asset);
		}

		protected T LoadAssetFromPath (string name)
		{
			if (_resourcePreText == null) {
				Debug.LogError ("Resource path not set");
			}

			string path = _resourcePreText + name.Trim ();
			var obj = (T)Resources.Load (path, typeof(T));


			return obj;
		}

		protected String GetFileNameFromPath (string path)
		{
			var groupNames = path.Split ('/');
			return groupNames [groupNames.Length - 1].Trim ();
		}
	}
}
