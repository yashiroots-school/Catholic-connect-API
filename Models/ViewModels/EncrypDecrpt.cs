using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ChurchAPI.Models.ViewModels;

public class EncrypDecrpt
{

  public static string encpassphrase { get; set; } = string.Empty;   
  public static string encsalt { get; set; } = string.Empty;
  public static string Decpassphrase { get; set; } = string.Empty;
  public static string Decsalt { get; set; }  = string.Empty;
  //private string encpassphrase = ConfigurationManager.AppSettings["atomtechEncrptkey"];
  //private string encsalt = ConfigurationManager.AppSettings["atomtechEncrptkey"];
  byte[] iv = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
  int iterations = 65536;
  //private string Decpassphrase = ConfigurationManager.AppSettings["atomtechDecrptkey"];
  //private string Decsalt = ConfigurationManager.AppSettings["atomtechDecrptkey"];
  public EncrypDecrpt(IConfiguration configuration)
  {
    encpassphrase = configuration["PayUSettings:atomtechEncrptkey"] ?? throw new ArgumentNullException(nameof(encpassphrase), "Encryption key is missing in configuration."); 
    encsalt = configuration["PayUSettings:atomtechEncrptkey"] ?? throw new ArgumentNullException(nameof(encpassphrase), "Encryption key is missing in configuration.");
    Decpassphrase = configuration["PayUSettings:atomtechDecrptkey"] ?? throw new ArgumentNullException(nameof(encpassphrase), "Encryption key is missing in configuration.");
    Decsalt = configuration["PayUSettings:atomtechDecrptkey"] ?? throw new ArgumentNullException(nameof(encpassphrase), "Encryption key is missing in configuration.");

    if (string.IsNullOrEmpty(encpassphrase) || string.IsNullOrEmpty(Decpassphrase))
    {
      throw new Exception("Encryption and Decryption keys must be configured in appsettings.json.");
    }
  }

  public String Encrypt(String plainText, String passphrase, String salt, Byte[] iv, int iterations)
  {
    var plainBytes = Encoding.UTF8.GetBytes(plainText);
    string data = ByteArrayToHexString(Encrypt(plainBytes, GetSymmetricAlgorithm(passphrase, salt, iv, iterations))).ToUpper();
    return data;
  }

  public String decrypt(String plainText, String passphrase, String salt, Byte[] iv, int iterations)
  {
    byte[] str = HexStringToByte(plainText);

    string data1 = Encoding.UTF8.GetString(decrypt(str, GetSymmetricAlgorithm(passphrase, salt, iv, iterations)));
    return data1;
  }
   public String Encrypt(String plainText)
  {
    var plainBytes = Encoding.UTF8.GetBytes(plainText);
    string data = ByteArrayToHexString(Encrypt(plainBytes, GetSymmetricAlgorithm(encpassphrase, encsalt, iv, iterations))).ToUpper();
    return data;
  }
  public String decrypt(String plainText)
  {
    byte[] str = HexStringToByte(plainText);

    string data1 = Encoding.UTF8.GetString(decrypt(str, GetSymmetricAlgorithm(Decpassphrase, Decsalt, iv, iterations)));
    return data1;
  }
  public byte[] Encrypt(byte[] plainBytes, SymmetricAlgorithm sa)
  {
    return sa.CreateEncryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length);

  }
  public byte[] decrypt(byte[] plainBytes, SymmetricAlgorithm sa)
  {
    return sa.CreateDecryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length);
  }
  public SymmetricAlgorithm GetSymmetricAlgorithm(String passphrase, String salt, Byte[] iv, int iterations)
  {
    var saltBytes = new byte[16];
    var ivBytes = new byte[16];
    Rfc2898DeriveBytes rfcdb = new System.Security.Cryptography.Rfc2898DeriveBytes(passphrase, Encoding.UTF8.GetBytes(salt), iterations, HashAlgorithmName.SHA512);
    saltBytes = rfcdb.GetBytes(32);
    var tempBytes = iv;
    Array.Copy(tempBytes, ivBytes, Math.Min(ivBytes.Length, tempBytes.Length));
#pragma warning disable SYSLIB0022 // Type or member is obsolete
    var rij = new RijndaelManaged
    {
      Mode = CipherMode.CBC,
      Padding = PaddingMode.PKCS7,
      FeedbackSize = 128,
      KeySize = 128,
      BlockSize = 128,
      Key = saltBytes,
      IV = ivBytes
    }; //SymmetricAlgorithm.Create();
#pragma warning restore SYSLIB0022 // Type or member is obsolete
    return rij;
  }
  protected static byte[] HexStringToByte(string hexString)
  {
    try
    {
      int bytesCount = (hexString.Length) / 2;
      byte[] bytes = new byte[bytesCount];
      for (int x = 0; x < bytesCount; ++x)
      {
        bytes[x] = Convert.ToByte(hexString.Substring(x * 2, 2), 16);
      }
      return bytes;
    }
    catch
    {
      throw;
    }
  }
  public static string ByteArrayToHexString(byte[] ba)
  {
    StringBuilder hex = new StringBuilder(ba.Length * 2);
    foreach (byte b in ba)
      hex.AppendFormat("{0:x2}", b);
    return hex.ToString();
  }
}
