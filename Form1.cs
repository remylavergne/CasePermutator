﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DictionnaryGen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private String fileContent;
        private String filePath;
        private String outputPath = "C:\\temp\\output.txt";
        private int permNumber;
        private int permDone;
        private List<String> permList = new List<string>();

        private void browseButton_Click(object sender, EventArgs e)
        {
            fileContent = string.Empty;
            filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    textBox2.Text = filePath;
                }
            }
        }

        private void loadBUtton_Click(object sender, EventArgs e)
        {
            try
            {
                var fileStream = File.OpenRead(filePath);
                toolStripProgressBar1.Enabled = true;
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    toolStripStatusLabel1.Text = "Loading file";
                    permNumber = 0;
                    permDone = 0;
                    String line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        int permPossible = countPossibilities(line.Length);
                        permNumber += permPossible;
                    }
                    fileStream.Position = 0;
                    reader.DiscardBufferedData();
                    while ((line = reader.ReadLine()) != null)
                    {
                        permute(line);
                    }
                    flushPermutations();
                    toolStripStatusLabel1.Text = "Yataa !";
                }
            }
            catch (ArgumentNullException ArgNullEx)
            {
                toolStripStatusLabel1.Text = "Incorrect file path";

            }
            catch (OutOfMemoryException OomEx)
            {
                System.Windows.Forms.MessageBox.Show("Not enough memory to perform the operation");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-ZáàâäãåçéèêëíìîïñóòôöõúùûüýÿæœÁÀÂÄÃÅÇÉÈÊËÍÌÎÏÑÓÒÔÖÕÚÙÛÜÝŸÆŒ\n\r]+", "", RegexOptions.Compiled);
        }

        private void permute(String input)
        {
            int n = input.Length;
            permDone = 0;
            // Number of permutations is 2^n 
            int max = 1 << n;

            // Converting string 
            // to lower case 
            input = input.ToLower();

            // Using all subsequences  
            // and permuting them 
            for (int i = 0; i < max; i++)
            {
                char[] combination = input.ToCharArray();

                // If j-th bit is set, we  
                // convert it to upper case 
                for (int j = 0; j < n; j++)
                {
                    if (((i >> j) & 1) == 1)
                    {
                        combination[j] = (char)(combination[j] - 32);
                    }
                }
                // Printing current combination 
                storePermutation(combination);
                permDone++;
                toolStripProgressBar1.Value = (100 * permDone) / permNumber;
            }
        }

        private void storePermutation(char[] combination)
        {
            permList.Add(new  String(combination));
            if(permList.Count == 1000)
            {
                flushPermutations();
            }
          
        }

        private void flushPermutations()
        {
            StreamWriter fs = File.AppendText(outputPath);
            foreach (String permutation in permList)
            {
                fs.WriteLine(permutation);
            }
            fs.Dispose();
            fs.Close();
            permList.Clear();
        }


        private int countPossibilities(int cLength)
        {
            return ((int)Math.Pow(2, (double)cLength));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text File|*.txt";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                StreamWriter fs = new StreamWriter(saveFileDialog1.OpenFile());
                fs.Write(textBox1.Text);
                fs.Dispose();
                fs.Close();
            }
        }
    }
}