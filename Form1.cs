using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ITCdata
{
    public partial class Form1 : Form
    {
        List<string> sFileNames = new List<string>();
        List<string> titles = new List<string>();
        List<List<double>> heat = new List<List<double>>();
        List<double> conc = new List<double>();
        List<double> averageHeat = new List<double>();
        List<Experiment> experimentList = new List<Experiment>();
        double rS;
        double intercept;
        double slope;
        double baseline;
        int initialDelay;
        int injLength;
        int peakDelay;
        int injAmount;
        double injConc;
        int injNumber;
 
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public Form1()
        {
            InitializeComponent();

            results.View = View.Details;
            results.MultiSelect = true;
            results.LabelEdit = true;
            results.Columns.Add("Slope", -2, HorizontalAlignment.Left);
            results.Columns.Add("R-squared", -2, HorizontalAlignment.Left);
            results.Columns.Add("Experiment", -2, HorizontalAlignment.Left);
            results.Columns.Add("Comments", -2, HorizontalAlignment.Left);
            results.KeyDown += results_KeyDown;
            textBox1.Validating += TextValidating;
            textBox3.Validating += TextValidating;
            textBox2.Validating += TextValidating;
            textBox4.Validating += TextValidating;
            textBox5.Validating += TextValidating;
            textBox5.Text = "40";
            peakDelay = 40;
            results.MouseClick += SelectItems;

            //Nupud
            MinimizeButton.MouseClick += MinimizeButton_Click;
            CloseButton.MouseClick += CloseButton_Click;
            SelectButton.Image = SelectList.Images[0];
            CalculateButton.MouseHover += Calculate_MouseHover;
            SelectButton.MouseHover += Select_MouseHover;
            CalculateButton.MouseLeave += Calculate_MouseLeave;
            SelectButton.MouseLeave += Select_MouseLeave;
            SelectButton.Click += OpenDialog;
            this.MouseDown += Form1_MouseDown;
            results.MouseDown += Form1_MouseDown;
            itcdata_BG.MouseDown += Form1_MouseDown;
            ResultsGraph.MouseDown += Form1_MouseDown;
            CalculateButton.Image = CalculateList.Images[0];

        }
        private void Select_MouseHover(object sender, EventArgs e)
        {
            SelectButton.Image = SelectList.Images[1];
        }
        private void Calculate_MouseHover(object sender, EventArgs e)
        {
            CalculateButton.Image = CalculateList.Images[1];
        }
        private void Select_MouseLeave(object sender, EventArgs e)
        {
            SelectButton.Image = SelectList.Images[0];
        }
        private void Calculate_MouseLeave(object sender, EventArgs e)
        {
            CalculateButton.Image = CalculateList.Images[0];
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void LinearRegression(string fileName, int fileNumber)
        {
            //flip, et saaks võrrelda vana ITC-ga
            for (int c = 0; c < heat.Count; c++)
            {
                heat[c][fileNumber] = heat[c][fileNumber] * -1;
            }

            //baseline
            baseline = 0;
            for (int a = initialDelay + peakDelay;a <= initialDelay + injLength; a++)
            {
                Console.WriteLine(heat.Count);
                baseline += heat[a][fileNumber];
            }
            baseline = baseline / (injLength - peakDelay + 1);

            //Lisan 0 konts. ja 0 heat tabelisse
            conc.Add(0);
            averageHeat.Add(0);
            for (int a = 0; a < injAmount; a++)
            {
                conc.Add(injConc * (a + 1));
            }
            //SÜstide keskmine heatrate
            for (int k = 0; k < injAmount; k++)
            {
                injNumber = k + 1;
                averageHeat.Add(0);
                for (int a = initialDelay + peakDelay + injLength * injNumber; a <= initialDelay + injLength * (injNumber + 1); a++)
                {
                    averageHeat[injNumber] += heat[a][fileNumber];
                }
            }
            //baseline maha lahutada
            for (int b = 1; b < averageHeat.Count; b++)
            {
                averageHeat[b] = averageHeat[b] / (injLength - peakDelay + 1);
                averageHeat[b] -= baseline;
            }
            

            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double sumCodeviates = 0;
            if (conc.Count != averageHeat.Count)
            {
                throw new Exception("Input values should be with the same length.");
            }
            for (var i = 0; i < conc.Count; i++)
            {
                var x = conc[i];
                var y = averageHeat[i];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }
            var ssX = sumOfXSq - ((sumOfX * sumOfX) / conc.Count);
            var ssY = sumOfYSq - ((sumOfY * sumOfY) / conc.Count);

            var rNumerator = (conc.Count * sumCodeviates) - (sumOfX * sumOfY);
            var rDenom = (conc.Count * sumOfXSq - (sumOfX * sumOfX)) * (conc.Count * sumOfYSq - (sumOfY * sumOfY));
            var sCo = sumCodeviates - ((sumOfX * sumOfY) / conc.Count);

            var meanX = sumOfX / conc.Count;
            var meanY = sumOfY / conc.Count;
            var dblR = rNumerator / Math.Sqrt(rDenom);

            rS = dblR * dblR;
            intercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;

            //Data salvestamine 
            Experiment experiment = new Experiment(titles[fileNumber],slope, intercept, injConc, averageHeat );
            experimentList.Add(experiment);

            //Andmed graafikule
            DrawPlot(fileNumber);
            //Temp listid tühjaks järgmiseks katseks
            averageHeat.Clear();
            conc.Clear();

        }//LinearRegression

        //Kontrollib, kas lahtrisse on sisestatud number
        private void TextValidating(object sender, CancelEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!ValidNumber(sender, tb.Text, out string errorMsg))
            {
                e.Cancel = true;
                tb.Select(0, tb.Text.Length);
                MessageBox.Show(errorMsg);
            }
        }
        public bool ValidNumber(object sender, string number, out string errorMsg)
        {
            int value;
            double dvalue;
            TextBox tb = (TextBox)sender;
            if (number.Length == 0 && tb != textBox1)
            {
                errorMsg = "All fields must be filled";
                return false;
            }
            if(tb == textBox1 && number.Length == 0)
            {
                errorMsg = "";
                return true;
            }
            if (tb == textBox3)
            {
                if (double.TryParse(tb.Text, out dvalue))
                {
                        errorMsg = "";
                        injConc = dvalue;
                    return true;
                }
            }
            else if (int.TryParse(tb.Text, out value))
            {
                errorMsg = "";
                return true;
            }
            errorMsg = "Insert a valid number";
            return false;
        }
        private void results_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyToClipBoard();
            }

        }
        private void CopyToClipBoard()
        {
            var builder = new StringBuilder();
            foreach (ListViewItem item in results.SelectedItems)
            {
                builder.AppendLine(item.Text);
            }
            //Kontrollib, et valik poleks tühi
            if (string.IsNullOrEmpty(builder.ToString())) { }
            else
            {
                Clipboard.SetText(builder.ToString());
            }
        }

        private void DrawPlot(int ID)
        {
            Series series1 = new Series { Name = experimentList[ID].title, ChartType = SeriesChartType.Point, MarkerSize = 7 };
            Series trend = new Series { Name = "", ChartType = SeriesChartType.Line };
            ResultsGraph.ChartAreas[0].AxisX.Minimum = 0;
            ResultsGraph.ChartAreas[0].AxisY.Minimum = 0;
            series1.Points.AddXY(0, 0);

            for (int b = 1; b < experimentList[ID].avgHeat.Count; b++)
            {
                series1.Points.AddXY(experimentList[ID].conc * b, experimentList[ID].avgHeat[b]);
            }

            trend.Points.AddXY(0, experimentList[ID].intercept);
            trend.Points.AddXY(experimentList[ID].conc * (experimentList[ID].avgHeat.Count - 1), experimentList[ID].intercept + experimentList[ID].slope * experimentList[ID].conc * (experimentList[ID].avgHeat.Count - 1));
            if(ResultsGraph.ChartAreas[0].AxisY.Maximum < Math.Round(experimentList[ID].intercept + experimentList[ID].slope * experimentList[ID].conc * (experimentList[ID].avgHeat.Count - 1) + 1, 2, MidpointRounding.AwayFromZero))
            {
                ResultsGraph.ChartAreas[0].AxisY.Maximum = Math.Round(experimentList[ID].intercept + experimentList[ID].slope * experimentList[ID].conc * (experimentList[ID].avgHeat.Count - 1) + 1, 2, MidpointRounding.AwayFromZero);
            }

            try
            {
                ResultsGraph.Series.Add(trend);
                ResultsGraph.Series.Add(series1);
            }
            catch (System.ArgumentException)
            {
                //Ignob errorit, lubab sama nimega tulemusi panna graafikule, tulevikus lisada ID lõppu nimele
            }
        }//DrawPlot

        private void SelectItems(object sender, EventArgs e) {
            ResultsGraph.Series.Clear();
            ResultsGraph.ChartAreas[0].AxisY.Maximum = 0;
            int ID;
            foreach (ListViewItem item in results.SelectedItems)
            { 
                ID = experimentList.Count - item.Index - 1;
                DrawPlot(ID);
            }
        }//SelectItems

        private void OpenDialog(object Sender, EventArgs e) {
            openFileDialog1.Filter = ".csv files|*.csv";
            openFileDialog1.Title = "Select a .csv File";
            openFileDialog1.Multiselect = true;
            DialogResult dResult = openFileDialog1.ShowDialog();
            if (dResult == DialogResult.OK) //Kui aknas vajutatakse OK (üks DialogResult valikutest)
            {
                sFileNames = openFileDialog1.FileNames.ToList();
            }
        }//OpenDialog

        private void CalculateButton_Click(object sender, EventArgs e)
        {
           ResultsGraph.Series.Clear();
            initialDelay = int.Parse(textBox1.Text) - 5;
            injLength = int.Parse(textBox2.Text);
            injAmount = int.Parse(textBox4.Text);
            peakDelay = int.Parse(textBox5.Text);
            for (int i = 0; i < sFileNames.Count; i++)
            {
                titles.Clear();
                heat.Clear();
                using (TextFieldParser parser = new TextFieldParser(sFileNames[i]))
                {
                    parser.SetDelimiters(",");
                    List<string> titlesParser = parser.ReadFields().ToList();
                    for(int x = 0; x< titlesParser.Count; x++)
                    {
                        if (x % 2 == 0)
                        {
                            titlesParser[x] = titlesParser[x].Replace("_X", "");
                            titles.Add(titlesParser[x]);
                        }
                    }
                    while (!parser.EndOfData)
                    {
                        List<string> fields = parser.ReadFields().ToList();
                        List<double> tempList = new List<double>();
                        for (int a = 1; a <= fields.Count; a++)
                        {
                            if (a % 2 == 0)
                            {
                                tempList.Add(double.Parse(fields[a-1], System.Globalization.CultureInfo.InvariantCulture)); //0 - aeg, pole vaja; 1 - soojus
                            }
                        }
                        heat.Add(tempList);
                       
                    }

                    for (int d = 0; d < titles.Count;  d++) {
                        LinearRegression(sFileNames[i], d);
                        ListViewItem item = new ListViewItem(slope.ToString("0.000"), 0);
                        item.SubItems.Add(rS.ToString("0.000"));
                        item.SubItems.Add(titles[d]);
                        item.SubItems.Add("OK");
                        results.Items.Insert(0, item);
  results.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

                    }
                }
            }
            heat.Clear();
            titles.Clear();
        }//CalculateButton_Click
    }//end class Form1
} //end namespace ITCdata
