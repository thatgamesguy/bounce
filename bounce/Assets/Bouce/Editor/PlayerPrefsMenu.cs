using UnityEngine;
using UnityEditor;
using System.Collections;

public class PlayerPrefsMenu : MonoBehaviour
{
	[MenuItem( "Playerprefs/Delete All" )]
	public static void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}
}
