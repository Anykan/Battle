/* The Launcher by Çağlayan Karagözler (a.k.a Flamacore)
 * This is the main script for the client. 
 * 
 * Checks for the latest version from the server which you uploaded your build earlier,
 * downloads the latest file list,
 * downloads the latest version accordingly.
 * Then, decompresses the downloaded file.
 * 
 * Also, on application start, it checks file integrity by comparing the file list from the server
 * with the files on disk by creating two encrypted files to make the comparison.
 * If the files do not have equal lines, re-download requirement is stated.
 * 
 * TODO: A lot :)
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Download : MonoBehaviour {
	
	#region Private/Static Vars
	private WWW CompFileToDownload;
	private WWW versionFile;
	private WWW vFile;
	public static string DownloadStatus;
	public static string DownloadProgress;
	public static string currentVer;
	public static bool updateRequired;
	public static bool gameDownloaded = false;
	public static bool downloadInProgress;
	public static Thread t2_;
	public static int DecompedFiles;
	#endregion

	#region Public Vars
	public string HTTPGameFileLocation;
	public string versionFileUrl;
	public string FileListUrl;
	public string newsTextUrl;
	public Image progressFill;
	public GameObject startButton;
	public Text startButtonText;
	public Text statusText;
	public Text progressText;
	public Text versionText;
	public string GameName;
	public Text newsText;
	public Transform upArrow;
	#endregion

	#region Monobehaviours
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		progressFill.fillAmount = 0;
		StartCoroutine (GetVersionFile ());
		StartCoroutine (GetNewsText ());
	}

	void LateUpdate()
	{
		statusText.text = DownloadStatus;
		versionText.text = "Latest Version: " + currentVer;
		progressText.text = DownloadProgress;
		if (updateRequired) {
			startButtonText.text = LocalizationManager.LangStrings [2]; 
		}
		if (!updateRequired && gameDownloaded) {
			startButtonText.text = LocalizationManager.LangStrings [4]; 
		}
		if (!updateRequired && !gameDownloaded) {
			startButtonText.text = LocalizationManager.LangStrings [3]; 
		}
		if (DownloadProgress == LocalizationManager.LangStrings [6] || PlayerPrefs.GetString("s") == "a") {
			gameDownloaded = true;
			downloadInProgress = false;
			updateRequired = false;
		}
		if (PlayerPrefs.GetString ("s") == "s") {
			gameDownloaded = false;
		}
		if (downloadInProgress)
			startButton.GetComponent<Button> ().interactable = false;
		else
			startButton.GetComponent<Button> ().interactable = true;
	}
	#endregion

	#region functions
	/// <summary>
	/// Starts download Coroutine.
	/// </summary>
	public void StartMe()
	{
		if (startButtonText.text == LocalizationManager.LangStrings [4]) {
			#if UNITY_STANDALONE_WIN
			System.Diagnostics.Process.Start(Application.dataPath + "/" + GameName + "/" + GameName + ".exe","hac4e7kjahf93zdv" );
			#endif
			#if UNITY_STANDALONE
			System.Diagnostics.Process.Start (Application.dataPath + "/" + GameName + "/" + GameName + ".app", "hac4e7kjahf93zdv");
			#endif
		}
		if (startButtonText.text == LocalizationManager.LangStrings [3]) {
			System.IO.Directory.CreateDirectory (Application.dataPath + "/" + GameName + "/");
			LogGenerator.GenerateDownloadLog ("Begin Application");
			StartCoroutine (StartDownload ());
		}
	}

	/// <summary>
	/// Starts the file integrity check coroutine.
	/// </summary>
	public void CheckMe()
	{
		StartCoroutine (CheckFileList ());
	}

	/// <summary>
	/// Rotates the arrow. Definitely optional function for Force Re-Check button.
	/// If you have something else like an animation, please use it so.
	/// </summary>
	public void RotateArrow(){
		upArrow.Rotate (0, 0, 180);
	}

	/// <summary>
	/// Gets the bytes in a readable format.
	/// </summary>
	/// <param name="i">Bytes.</param>
	public static string GetBytesReadable(long i)
	{
		// Get absolute value
		long absolute_i = (i < 0 ? -i : i);
		// Determine the suffix and readable value
		string suffix;
		double readable;
		if (absolute_i >= 0x1000000000000000) // Exabyte
		{
			suffix = "EB";
			readable = (i >> 50);
		}
		else if (absolute_i >= 0x4000000000000) // Petabyte
		{
			suffix = "PB";
			readable = (i >> 40);
		}
		else if (absolute_i >= 0x10000000000) // Terabyte
		{
			suffix = "TB";
			readable = (i >> 30);
		}
		else if (absolute_i >= 0x40000000) // Gigabyte
		{
			suffix = "GB";
			readable = (i >> 20);
		}
		else if (absolute_i >= 0x100000) // Megabyte
		{
			suffix = "MB";
			readable = (i >> 10);
		}
		else if (absolute_i >= 0x400) // Kilobyte
		{
			suffix = "KB";
			readable = i;
		}
		else
		{
			return i.ToString("0 B"); // Byte
		}
		// Divide by 1024 to get fractional value
		readable = (readable / 1024);
		// Return formatted number with suffix
		return readable.ToString("0.### ") + suffix;
	}

	/// <summary>
	/// Compares files with the file list at the server.
	/// </summary>
	public void CompareFileList()
	{
		string filePath = Application.dataPath + "/" + GameName + "/";
		string filePath2 = Application.dataPath + "/v.flamacore";
		string[] Files = null;
		try{
		Files = Directory.GetFiles (filePath,"*.*",SearchOption.AllDirectories); 
		}
		catch(DirectoryNotFoundException) {
			return;
		}
		if (File.Exists (filePath2)) {
			FileStream fs = new	FileStream (filePath2, FileMode.Truncate, FileAccess.Write);
			fs.Close ();
		}
		StreamWriter writer = new StreamWriter (filePath2, true);
		foreach (string file in Files) {
			writer.WriteLine (Encrypt.EncryptRJ256(file.Substring(filePath.Length)), true);
		}
		writer.Close ();
		string[] lines = File.ReadAllLines (Application.dataPath + "/v.flamacore");
		string[] lines2 = File.ReadAllLines (Application.dataPath + "/vT.flamacore");
		bool filesGood = lines.SequenceEqual (lines2);
		if (!filesGood) {
			gameDownloaded = false;
			Debug.Log ("File integrity problem. Needs redownload.");
			LogGenerator.GenerateDownloadLog ("File integrity problem. Needs redownload.");
			DownloadStatus = LocalizationManager.LangStrings [11];
		} else {
			updateRequired = false;
			gameDownloaded = true;
			Debug.Log ("File Check Complete.");
			LogGenerator.GenerateDownloadLog("File Check Complete.");
			DownloadStatus = LocalizationManager.LangStrings [10];
		}
	}
	#endregion

	#region Ienumerators
	/// <summary>
	/// Gets news text from a page. Reads the WWW.text property directly.
	/// Can be formatted using Unity's rich text formatting options.
	/// </summary>
	public IEnumerator GetNewsText()
	{
		LogGenerator.GenerateDownloadLog ("Getting News Text");
		WWW news = new WWW (newsTextUrl);
		yield return news;
		if (!string.IsNullOrEmpty (news.error)) {
			LogGenerator.GenerateDownloadLog (news.text + news.error);
			Debug.Log (news.text + news.error);
			newsText.text = news.text + news.error;
		} else {
			Debug.Log ("Got the news text.");
			LogGenerator.GenerateDownloadLog ("Got the news text.");
			newsText.text = news.text;
		}

	}
	/// <summary>
	/// Gets the version file.
	/// </summary>
	public IEnumerator GetVersionFile()
	{
		if (!downloadInProgress) {
			LogGenerator.GenerateDownloadLog ("Version File Download started...");
			versionFile = new WWW (versionFileUrl);
			while (!versionFile.isDone) {
				downloadInProgress = true;
				progressFill.fillAmount = versionFile.progress;
				DownloadStatus = LocalizationManager.LangStrings [0];
				yield return null;
			}
			if (!string.IsNullOrEmpty (versionFile.error)) {
				LogGenerator.GenerateDownloadLog (versionFile.text + versionFile.error);
				Debug.Log (versionFile.text + versionFile.error);
			} else {
				downloadInProgress = false;
				Debug.Log ("Version File Downloaded.");
				LogGenerator.GenerateDownloadLog ("Version File Downloaded.");
				File.WriteAllBytes (Application.dataPath + "/version.flamacore", versionFile.bytes);
				string[] lines = File.ReadAllLines (Application.dataPath + "/version.flamacore");
				currentVer = lines [4];
				DownloadStatus = LocalizationManager.LangStrings [1];
				StartCoroutine (CheckFileList ());
				progressFill.fillAmount = 0;
			}
		}
	}
	/// <summary>
	/// Checks the file list.
	/// </summary>
	public IEnumerator CheckFileList()
	{
		if (!downloadInProgress) {
			LogGenerator.GenerateDownloadLog ("File List Download started...");
			vFile = new WWW (FileListUrl);
			while (!vFile.isDone) {
				downloadInProgress = true;
				progressFill.fillAmount = vFile.progress;
				DownloadStatus = LocalizationManager.LangStrings [8];
				yield return null;
			}
			if (!string.IsNullOrEmpty (vFile.error)) {
				LogGenerator.GenerateDownloadLog (vFile.text + vFile.error);
				Debug.Log (vFile.text + vFile.error);
				LogGenerator.GenerateDownloadLog(vFile.text + vFile.error);
				DownloadStatus = vFile.text + vFile.error;
			} else {
				downloadInProgress = false;
				Debug.Log ("File List Downloaded.");
				LogGenerator.GenerateDownloadLog ("File List Downloaded.");
				File.WriteAllBytes (Application.dataPath + "/vT.flamacore", vFile.bytes);
				DownloadStatus = LocalizationManager.LangStrings [9];
				CompareFileList ();
				progressFill.fillAmount = 0;
			}
		}
	}
	/// <summary>
	/// Starts the download.
	/// </summary>
	public IEnumerator StartDownload()
	{
		if (!downloadInProgress) {
			string totalSize = "";
			System.Net.WebRequest req = System.Net.HttpWebRequest.Create(HTTPGameFileLocation + "upload_" + currentVer + ".flamacore");
			req.Method = "HEAD";
			using (System.Net.WebResponse resp = req.GetResponse())
			{
				long ContentLength;
				if(long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
				{ 
					totalSize = GetBytesReadable (ContentLength);
				}
			}
			DownloadStatus = LocalizationManager.LangStrings [5];
			LogGenerator.GenerateDownloadLog ("Download started: " + HTTPGameFileLocation + "upload_" + currentVer + ".flamacore" );
			CompFileToDownload = new WWW (HTTPGameFileLocation + "upload_" + currentVer + ".flamacore");
			while (!CompFileToDownload.isDone) {
				downloadInProgress = true;
				progressFill.fillAmount = CompFileToDownload.progress;
				DownloadProgress = (CompFileToDownload.progress*100).ToString("F2") + "%/100% --- " + totalSize;
				yield return null;
			}
			DownloadProgress = (CompFileToDownload.progress*100).ToString("F2") + "%/100% --- " + totalSize;
			progressFill.fillAmount = CompFileToDownload.progress;
			if (!string.IsNullOrEmpty (CompFileToDownload.error)) {
				Debug.Log (CompFileToDownload.text + CompFileToDownload.error);
			} else {
				downloadInProgress = false;
				LogGenerator.GenerateDownloadLog ("Download done.");
				Debug.Log ("done");
				File.WriteAllBytes (Application.dataPath + "/upload_" + currentVer + ".flamacore", CompFileToDownload.bytes);
				downloadInProgress = true;
				progressFill.fillAmount = 0;
				string pth = Application.dataPath;
				//Decompressor.DecompressToDirectory (pth + "/upload_" + currentVer + ".flamacore", pth + "/" + GameName + "/");
				t2_ = new Thread(() => Decompressor.DecompressToDirectory (pth + "/upload_" + currentVer + ".flamacore", pth + "/" +GameName + "/"));
				if(!t2_.IsAlive)
					t2_.Start ();
				LogGenerator.GenerateLog ("Decompression Thread Launched");

			}
		}

	}

	#endregion
}
