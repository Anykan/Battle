/* The Launcher by Çağlayan Karagözler (a.k.a Flamacore)
 * Script for compressing the build after the developer selects the build folder.
 * 
 * Uses CompressHelper.cs for the main job.
 * The compressed file then is used by Uploader.cs
 * 
 * TODO: File extension customization.
 * 
 */
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Threading;
using System.Net;
using System;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Compress : MonoBehaviour {
	public static string pathToCompress;
	public static string pathToUpload;
	public static string CompressedFile;
	public static string compressStatus;
	public static int compressCount =0;
	public static string LastVersion;
	public static string currentFolder;
	public static int totalFiles = 0;
	public static Thread t1_;
	void Start()
	{
		LastVersion = EditorPrefs.GetString ("ver");
	}
	public static void CompressMe()
	{
		System.IO.Directory.CreateDirectory (Application.dataPath + "/../TheLauncher/");
		System.IO.Directory.CreateDirectory (Application.dataPath + "/../TheLauncher/COMPRESSED/");
		System.IO.Directory.CreateDirectory (Application.dataPath + "/../TheLauncher/version/");
		EditorLogGenerator.GenerateVersionFile (LastVersion);
		EditorLogGenerator.GenerateFileList (pathToCompress);
		currentFolder = Application.dataPath;
		t1_ = new Thread(() => CompressHelper.CompressDirectory(pathToCompress, currentFolder + "/../TheLauncher/COMPRESSED/upload_" + LastVersion + ".flamacore"));
		if(!t1_.IsAlive)
		t1_.Start ();
		EditorLogGenerator.GenerateLog ("Compression Thread Launched");
		pathToUpload = Application.dataPath + "/../TheLauncher/COMPRESSED/upload_" + LastVersion + ".flamacore";
	}


}
