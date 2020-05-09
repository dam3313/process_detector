using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// A Ajouter
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;

namespace ACC_detector
{
    public partial class Form1 : Form
    {

        public String[] cheatList_1 = new String []{ "ACC", "cheat", "TS SE Tool", "Virtual_speditor" };
        
        // Variable necessaire
        public bool[] cheatDetected;



        public Form1()
        {
            InitializeComponent();

            // Appel à l'ouverture de l'APP
            if (OpenCheck(cheatList_1, "D:\\Documents\\Euro Truck Simulator 2\\profiles\\44414D"))
            {
                // le fichier de save a ete modifie ou il n'exister pas de hash sauvegarder par l'app
                textBoxSave.Text = "oui";
            }
            else
            {
                // le fichier de save n'a pas ete modifie
                textBoxSave.Text = "non";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {


            var cheat = CloseCheck(cheatList_1, "D:\\Documents\\Euro Truck Simulator 2\\profiles\\44414D");

            // verification si des appli ont ete reperees
            if (cheat.Item1)
            {
                using (StreamWriter sw = File.AppendText("result.txt"))
                {
                    sw.WriteLine("les App suivantes ont été vues en fonctionnement :");
                    sw.WriteLine(cheat.Item2 + " ");
                }
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {


            CheatScan(cheatList_1);



            textBox1.Text = "";
            for (int i = 0; i < cheatList_1.Length; i++)
            {
                if (cheatDetected[i] == true)
                {
                    textBox1.Text += " - " + cheatList_1[i];
                }
            }
            

        }


        private bool OpenCheck(String[] cheatList, String profilePath, String savepath = "")
        {
            cheatDetected = Enumerable.Repeat<bool>(false, cheatList.Length).ToArray();

            String hash_fromPath = CreateMd5ForFolder(profilePath);

            String fileName = savepath + "AppSettings.dat";
            String hash_fromFile = "";
            //read file
            if (File.Exists(fileName))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                {
                    hash_fromFile = reader.ReadString();
                }

                if (hash_fromFile != hash_fromPath)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }


        private void CheatScan(String[] cheatList)
        {
            for (int i = 0; i < cheatList.Length; i++)
            {
                if (Process.GetProcesses().Where(x => x.ProcessName.ToLower().StartsWith(cheatList[i].ToLower())).ToList().LongCount() > 0)
                {
                    cheatDetected[i] = true;
                }
            }
        }

        private Tuple<bool, string> CloseCheck(String[] cheatList, String profilePath, String savepath = "")
        {
            string result = "";
            bool cheat = false;

            for (int i = 0; i < cheatList.Length; i++)
            {
                if (cheatDetected[i] == true)
                {
                    cheat = true;
                    result += cheatList[i];
                }
            }

            String hash = CreateMd5ForFolder(profilePath);

            String fileName = savepath + "AppSettings.dat";
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                writer.Write(hash);
            }
            
            return Tuple.Create(cheat, result);
        }

        public static string CreateMd5ForFolder(string path)
        {
            // assuming you want to include nested folders
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                 .OrderBy(p => p).ToList();

            MD5 md5 = MD5.Create();

            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];

                // hash path
                string relativePath = file.Substring(path.Length + 1);
                byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());
                md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                // hash contents
                byte[] contentBytes = File.ReadAllBytes(file);
                if (i == files.Count - 1)
                    md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                else
                    md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
            }

            return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
        }

        
    }
}
