/* The Launcher by Çağlayan Karagözler (a.k.a Flamacore)
 * Simple encryption script.
 * 
 * Using a two pair key system and the Rjindael256 alghorithm
 * two functions are used to encrypt or decrypt strings.
 * Used to encrypt the file list in order to harden the security
 * and prevent compromising file structure.
 * 
 */

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using UnityEngine;

/// <summary>
/// Encryption using simple RJ256 alghorithm. To prevent small time hackers :)
/// </summary>
public class Encrypt : MonoBehaviour {
		
	/// <summary>
	/// Decrypts the Rj256 ecnrypted string.
	/// </summary>
	/// <returns>Decrypted string.</returns>
	/// <param name="prm_text_to_decrypt">Encrypted text to decrypt.</param>
		public static string DecryptRJ256(string prm_text_to_decrypt) {
		var prm_key = "Svn1O6V06zaAHhFFbvkjbwlKkaItTiZD"; //32 chr shared ascii string (32 * 8 = 256 bit)
		var prm_iv = "741952hhreyy66#cs!9hjv887mxx7@8y"; //32 chr shared ascii string (32 * 8 = 256 bit)

			var sEncryptedString = prm_text_to_decrypt;
			
			var myRijndael = new RijndaelManaged() {
				Padding = PaddingMode.Zeros,
				Mode = CipherMode.CBC,
				KeySize = 256,
				BlockSize = 256
			};
			
			var key = Encoding.UTF8.GetBytes(prm_key);
			var IV = Encoding.UTF8.GetBytes(prm_iv);
			
			var decryptor = myRijndael.CreateDecryptor(key, IV);
			
			var sEncrypted = Convert.FromBase64String(sEncryptedString);
			
			var fromEncrypt = new byte[sEncrypted.Length];
			
			var msDecrypt = new MemoryStream(sEncrypted);
			var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
			
			csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
			
			return (Encoding.UTF8.GetString(fromEncrypt));
	}

		/// <summary>
		/// Encrypts the string given with RJ256 encryption.
		/// </summary>
		/// <returns>Encrypted string.</returns>
		/// <param name="prm_text_to_encrypt">String to encrypt.</param>
		public static string EncryptRJ256(string prm_text_to_encrypt) {
		var prm_key = "Svn1O6V06zaAHhFFbvkjbwlKkaItTiZD"; //32 chr shared ascii string (32 * 8 = 256 bit)
		var prm_iv = "741952hhreyy66#cs!9hjv887mxx7@8y"; //32 chr shared ascii string (32 * 8 = 256 bit)
		var sToEncrypt = prm_text_to_encrypt;
			
		var myRijndael = new RijndaelManaged () {
			Padding = PaddingMode.Zeros,
			Mode = CipherMode.CBC,
			KeySize = 256,
			BlockSize = 256
		};
			
		var key = Encoding.UTF8.GetBytes (prm_key);
		var IV = Encoding.UTF8.GetBytes (prm_iv);
			
		var encryptor = myRijndael.CreateEncryptor (key, IV);
			
		var msEncrypt = new MemoryStream ();
		var csEncrypt = new CryptoStream (msEncrypt, encryptor, CryptoStreamMode.Write);
			
		var toEncrypt = Encoding.UTF8.GetBytes (sToEncrypt);
			
		csEncrypt.Write (toEncrypt, 0, toEncrypt.Length);
		csEncrypt.FlushFinalBlock ();
			
		var encrypted = msEncrypt.ToArray ();
			
		return (Convert.ToBase64String (encrypted));
	}
		
}

