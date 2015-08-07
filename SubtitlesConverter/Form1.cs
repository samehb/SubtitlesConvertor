using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SubtitlesConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ConvertBtn_Click(object sender, EventArgs e)
        {
            String ArabizedLine, ProcessedLine = "", LineToProcess, SubBlocks = "";
            String[] SplittedLineToProcess;
            MessagesLbl.Text = "";
            char[] CProcessedLine, CLineToProcess, CSubBlocks;
            byte[] BProcessedLine, BSubBlocks;
            int ii = 0, SubBlocksCounter = 0;
            bool HasText = false;

            if (File.Exists(InputFileName.Text))
            {
                try
                {
                    Path.GetFullPath(OutputFileName.Text);
                }
                catch
                {
                    MessagesLbl.Text = "Invalid output path.";
                    return;
                }
            }
            else
            {
                MessagesLbl.Text = "Invalid input file.";
                return;
            }

            Arabization ArabicText = new Arabization();
            FileStream ArabicInputTextFile = new FileStream(InputFileName.Text, FileMode.Open, FileAccess.Read);
            FileStream ArabicOutputTextFile = new FileStream(OutputFileName.Text, FileMode.Create, FileAccess.ReadWrite);
            TextReader ArabicInputTextStream = new StreamReader(ArabicInputTextFile, Encoding.GetEncoding(1256));

            try
            {
                if (InputFileName.Text.EndsWith(".sub"))
                {
                    LineToProcess = ArabicInputTextStream.ReadLine();
                    CLineToProcess = LineToProcess.ToCharArray();
                    for (ii = 0; ii < CLineToProcess.Length; ii++)
                        if (CLineToProcess[ii] == '}')
                            SubBlocksCounter++;
                    if (SubBlocksCounter != 2)
                        MessagesLbl.Text = "not a valid sub file.";
                    else
                    {
                        ArabicInputTextFile.Seek(0, SeekOrigin.Begin);
                        ArabicInputTextStream = new StreamReader(ArabicInputTextFile, Encoding.GetEncoding(1256));
                        while ((LineToProcess = ArabicInputTextStream.ReadLine()) != null)
                        {
                            ii = 0;
                            SubBlocksCounter = 0;
                            SubBlocks = "";
                            CLineToProcess = LineToProcess.ToCharArray();

                            while (SubBlocksCounter < 2)
                            {
                                if (CLineToProcess[ii] == '}')
                                    SubBlocksCounter++;
                                SubBlocks += CLineToProcess[ii++];
                            }


                            SplittedLineToProcess = LineToProcess.Substring(ii).Split('|');

                            ProcessedLine = "";
                            for (int y = 0; y < SplittedLineToProcess.Length; y++)
                            {
                                ProcessedLine += ArabicText.Arabize(SplittedLineToProcess[y]);
                                if (y < (SplittedLineToProcess.Length - 1))
                                    ProcessedLine += "|";
                            }

                            CProcessedLine = ProcessedLine.ToCharArray();

                            SubBlocksCounter = 0;
                            ii = 0;

                            CSubBlocks = SubBlocks.ToCharArray();
                            BProcessedLine = new byte[CProcessedLine.Length];
                            BSubBlocks = new byte[CSubBlocks.Length];
                            for (int i = 0; i < CSubBlocks.Length; i++)
                                BSubBlocks[i] = (byte)CSubBlocks[i];
                            ArabicOutputTextFile.Write(BSubBlocks, 0, BSubBlocks.Length);
                            for (int i = 0; i < CProcessedLine.Length; i++)
                                BProcessedLine[i] = (byte)CProcessedLine[i];
                            ArabicOutputTextFile.Write(BProcessedLine, 0, BProcessedLine.Length);
                            ArabicOutputTextFile.WriteByte((byte)'\r');
                            ArabicOutputTextFile.WriteByte((byte)'\n');
                        }
                        MessagesLbl.Text = "File has been successfully converted.";
                    }
                }
                else if (InputFileName.Text.EndsWith(".srt"))
                {
                    LineToProcess = ArabicInputTextStream.ReadLine();
                    if (LineToProcess.Length == 1)
                    {
                        LineToProcess = ArabicInputTextStream.ReadLine();
                        if (!LineToProcess.Contains("-->"))
                        {
                            MessagesLbl.Text = "not a valid sub file.";
                        }
                        else
                        {
                            ArabicInputTextFile.Seek(0, SeekOrigin.Begin);
                            ArabicInputTextStream = new StreamReader(ArabicInputTextFile, Encoding.GetEncoding(1256));
                            while ((LineToProcess = ArabicInputTextStream.ReadLine()) != null)
                            {
                                ii = 0;
                                SubBlocksCounter = 0;

                                CProcessedLine = LineToProcess.ToCharArray();

                                if (HasText == true)
                                {
                                    if (LineToProcess == "")
                                    {
                                        HasText = false;
                                    }
                                    else
                                    {
                                        ArabizedLine = ArabicText.Arabize(LineToProcess.Substring(ii));
                                        CProcessedLine = ArabizedLine.ToCharArray();
                                    }

                                }


                                BProcessedLine = new byte[CProcessedLine.Length];

                                for (int i = 0; i < CProcessedLine.Length; i++)
                                    BProcessedLine[i] = (byte)CProcessedLine[i];
                                ArabicOutputTextFile.Write(BProcessedLine, 0, BProcessedLine.Length);
                                ArabicOutputTextFile.WriteByte((byte)'\r');
                                ArabicOutputTextFile.WriteByte((byte)'\n');

                                if (LineToProcess.Contains("-->"))
                                {
                                    HasText = true;
                                }
                            }
                            MessagesLbl.Text = "File has been successfully converted.";
                        }
                    }
                    else
                        MessagesLbl.Text = "not a valid sub file.";
                }
                else if (InputFileName.Text.EndsWith(".lng"))
                {
                    while ((LineToProcess = ArabicInputTextStream.ReadLine()) != null)
                    {
                        ii = 0;
                        SubBlocksCounter = 0;
                        ArabizedLine = ArabicText.Arabize(LineToProcess.Substring(ii));
                        CProcessedLine = ArabizedLine.ToCharArray();

                        BProcessedLine = new byte[CProcessedLine.Length];

                        for (int i = 0; i < CProcessedLine.Length; i++)
                            BProcessedLine[i] = (byte)CProcessedLine[i];
                        ArabicOutputTextFile.Write(BProcessedLine, 0, BProcessedLine.Length);
                        ArabicOutputTextFile.WriteByte((byte)'\r');
                        ArabicOutputTextFile.WriteByte((byte)'\n');
                    }
                    MessagesLbl.Text = "File has been successfully converted.";
                }
                else
                    MessagesLbl.Text = "Please select a valid srt or sub file.";
            }
            catch
            {
                MessagesLbl.Text = "Corrupted file. Please replace it.";
            }

            ArabicInputTextFile.Close();
            ArabicOutputTextFile.Close();
        }

        private void InputFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog.FileName = "";
            OpenFileDialog.Filter = "All supported formats (*.srt,*.sub,*.lng)|*.srt;*.sub;*.lng|SubRip Subtitles (*.srt)|*.srt|MicroDVD Subtitles (*.sub)|*.sub|SMS Language File (*.lng)|*.lng";
            OpenFileDialog.ShowDialog();
            if (OpenFileDialog.FileName != "")
                InputFileName.Text = OpenFileDialog.FileName;
        }

        private void OutputFileButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog.FileName = "";
            SaveFileDialog.Filter = "All supported formats (*.srt,*.sub,*.lng)|*.srt;*.sub;*.lng|SubRip Subtitles (*.srt)|*.srt|MicroDVD Subtitles (*.sub)|*.sub|SMS Language File (*.lng)|*.lng";
            SaveFileDialog.ShowDialog();
            if (SaveFileDialog.FileName != "")
                OutputFileName.Text = SaveFileDialog.FileName;
        }

    }

}
