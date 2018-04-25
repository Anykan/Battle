/* The Launcher by Çağlayan Karagözler (a.k.a Flamacore)
 * The script used for localizing the launcher. The editor uploader part is not localized.
 * 
 * Very simple and crude way for localization right now but it works.
 * 
 * TODO: XML formatted, cooler and tidier localization file system.
 * 
 */
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationManager : MonoBehaviour {
	public static string[] LangStrings;
	public string[] LanguageStrings;
	void Awake () {
		LangStrings = LanguageStrings; 
	}
}
