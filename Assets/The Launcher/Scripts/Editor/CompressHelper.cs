/* The Launcher by Çağlayan Karagözler (a.k.a Flamacore)
 * A Helper Script for compressing the build after the developer selects the build folder. Used by Compress.cs
 * 
 * A combination of GZipStream and FileStream classes are used for the job.
 * Probably you will not need to tamper with this :)
 * Creates a compressed file with a custom extension. 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.AccessControl;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CompressHelper : MonoBehaviour {
	public static CompressHelper instance;
	public static string DecompressStatus;
	static GZipStream strm;
	/// <summary>
	/// Compresses the file.
	/// </summary>
	public static void CompressFile(string sDir, string sRelativePath, GZipStream zipStream)
	{
		//Compress file name
		char[] chars = sRelativePath.ToCharArray ();
		zipStream.Write (BitConverter.GetBytes (chars.Length), 0, sizeof(int));
		Compress.compressStatus = "File: " + sRelativePath;
		Debug.Log ("Compressing File Name: " + sRelativePath);
		foreach (char c in chars)
			zipStream.Write (BitConverter.GetBytes (c), 0, sizeof(char));
		
		//Compress file content
		byte[] bytes = File.ReadAllBytes (Path.Combine (sDir, sRelativePath));
		zipStream.Write (BitConverter.GetBytes (bytes.Length), 0, sizeof(int));
		zipStream.Write (bytes, 0, bytes.Length);
		strm = zipStream;
	}



	/// <summary>
	/// Compresses the directory.
	/// </summary>
	public static void CompressDirectory(string sInDir, string sOutFile)
	{
		Compress.compressCount = 0;
		Compress.compressStatus = "Getting Files";
		string[] sFiles = Directory.GetFiles (sInDir, "*.*", SearchOption.AllDirectories);
		int iDirLen = sInDir [sInDir.Length - 1] == Path.DirectorySeparatorChar ? sInDir.Length : sInDir.Length + 1;
		Compress.totalFiles = sFiles.Length;
		using (FileStream outFile = new FileStream (sOutFile, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
		using (GZipStream str = new GZipStream (outFile, CompressionMode.Compress))
			foreach (string sFilePath in sFiles) {
				Compress.compressStatus = "Compressing...";
				Compress.compressCount++; 
				string sRelativePath = sFilePath.Substring (iDirLen);
				CompressFile (sInDir, sRelativePath, str);
			}
		Compress.compressStatus = "Compression Done!";
		strm.Close ();
		return;
	}


}