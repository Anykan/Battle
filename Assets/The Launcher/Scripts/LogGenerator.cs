/* The Launcher by Çağlayan Karagözler (a.k.a Flamacore)
 * Script for generating log files and log entries.
 * 
 * To enable clients to send logs to developers if needed or for debugging.
 * Generates the file in a folder within the app path.
 * Logs are pretty rough at the moment but you'll get the idea on what is going on if a user contacts you.
 * 
 * TODO: Better formatted logs. Auto Upload dialog on crash/fail.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Net;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Log generator class. Uses a text file to write logs.
/// </summary>
public class LogGenerator : MonoBehaviour {
	/// <summary>
	/// Generates log for uploader default log file which should be located in /LauncherLogs/log.txt
	/// </summary>
	/// <param name="log">The Log string.</param>
	public static void GenerateLog(string log)
	{
		string wholeLog = "-" + Application.companyName + "-version: " + Application.unityVersion.ToString() + "-product name:" + Application.productName + "-" +log + " |Time: " + System.DateTime.Now.ToLongTimeString () + " " + System.DateTime.Now.ToLongDateString ();
		string filePath = Application.dataPath + "/LauncherLogs/log"+System.DateTime.Now.ToShortDateString ()+".txt";
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

		string filePath = Application.dataPath + "/log"+System.DateTime.Now.Month.ToString()+"-"+System.DateTime.Now.Day.ToString()+ ".txt";
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


}
