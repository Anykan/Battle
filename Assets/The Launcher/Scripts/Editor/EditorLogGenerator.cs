/* The Launcher by Çağlayan Karagözler (a.k.a Flamacore)
 * Script for generating log files, log entries, Version file and file list container file.
 * 
 * This log generator is for the developer only and used by the Uploader.
 * Generates the file in a folder within the app path.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Net;
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// Log generator class. Uses a text file to write logs.
/// </summary>
[ExecuteInEditMode]
public class EditorLogGenerator : MonoBehaviour {
	
	/// <summary>
	/// Generates log for uploader default log file which should be located in /LauncherLogs/log.txt
	/// </summary>
	/// <param name="log">The Log string.</param>
	public static void GenerateLog(string log)
	{
		string wholeLog = "-" + Application.companyName + "-version: " + Application.unityVersion.ToString() + "-product name:" + Application.productName + "-" +log + " |Time: " + System.DateTime.Now.ToLongTimeString () + " " + System.DateTime.Now.ToLongDateString ();
		string filePath = Application.dataPath + "/LauncherLogs/log.txt";
		try {
			StreamWriter writer = new StreamWriter (filePath, true);
			writer.WriteLine (wholeLog);
			writer.Close ();
		} catch (DirectoryNotFoundException) {
			Debug.LogWarning ("LOG FILE DOES NOT EXIST! Attempting to create...");
			System.IO.Directory.CreateDirectory(Application.dataPath + "/LauncherLogs/");
			Debug.Log ("Log Folder Created");
			GenerateLog (wholeLog);
		}
		AssetDatabase.ImportAsset ("Assets/LauncherLogs/",ImportAssetOptions.ForceUpdate);
		AssetDatabase.Refresh ();
	}

	/// <summary>
	/// Generates log for the end user log file which should be located right near the launcher.
	/// </summary>
	/// <param name="log">The Log string.</param>
	public static void GenerateDownloadLog(string log)
	{
		string wholeLog = 
			"-OS:" + System.Environment.OSVersion.Platform 
			+ "-version: " + System.Environment.OSVersion.VersionString 
			+ "-Graphics Card:" + SystemInfo.graphicsDeviceName
			+ "-Graphics Size:" + SystemInfo.graphicsMemorySize
			+ "-Processor:" + SystemInfo.processorType
			+ "-Memory:" + SystemInfo.systemMemorySize
			+ "-Log: " +log 
			+ " |Time: " + System.DateTime.Now.ToLongTimeString () + " " + System.DateTime.Now.ToLongDateString ();
		
		string filePath = Application.dataPath + "log.txt";
		try {
			StreamWriter writer = new StreamWriter (filePath, true);
			writer.WriteLine (wholeLog);
			writer.Close ();
		} catch (AccessViolationException) {
			GameObject err = GameObject.FindGameObjectWithTag ("ErrorWindow");
			//err.GetComponent<Text>().text =
			err.SetActive (true);
		}
		catch (UnauthorizedAccessException) {
			GameObject err = GameObject.FindGameObjectWithTag ("ErrorWindow");
			err.SetActive (true);
		}
	}

	/// <summary>
	/// Generates the version file.
	/// </summary>
	/// <param name="ver">Version.</param>
	public static void GenerateVersionFile(string ver)
	{
		string filePath = Application.dataPath + "/../TheLauncher/version/version.flamacore";
		try {
			StreamWriter writer = new StreamWriter (filePath, false);
			writer.WriteLine (ver);
			writer.Close ();
		} catch (DirectoryNotFoundException) {
			Debug.LogWarning ("VERSION FILE DOES NOT EXIST! Attempting to create...");
			System.IO.Directory.CreateDirectory(Application.dataPath + "/../TheLauncher/version/version.flamacore");
			Debug.Log ("Version File Created");
			GenerateVersionFile (ver);
		}
	}

	/// <summary>
	/// Generates the FileList.
	/// </summary>
	/// <param name="ver">Version.</param>
	public static void GenerateFileList(string DirectoryPath)
	{
		string filePath = Application.dataPath + "/../TheLauncher/version/v.flamacore";
		string[] Files = Directory.GetFiles (DirectoryPath,"*.*",SearchOption.AllDirectories); 
		try{
		FileStream fs = new	FileStream (filePath, FileMode.Truncate, FileAccess.Write);
		fs.Close();
		}
		catch(FileNotFoundException) {
			StreamWriter writer3 = new StreamWriter (filePath, true);
			foreach (string file in Files) {
				writer3.WriteLine (Encrypt.EncryptRJ256(file.Substring(DirectoryPath.Length+1)), true);
			}
			writer3.Close ();
			return;
		}
		StreamWriter writer = new StreamWriter (filePath, true);
		foreach (string file in Files) {
			writer.WriteLine (Encrypt.EncryptRJ256(file.Substring(DirectoryPath.Length+1)), true);
		}
		writer.Close ();
	}
}
