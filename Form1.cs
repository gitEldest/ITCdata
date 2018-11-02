using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        List<double> injStartValue = new List<double>();
        List<double> injEndValue = new List<double>();
        List<double> remainingSubstrate = new List<double>();
        List<Experiment> experimentList = new List<Experiment>();

        private double heat_STDEV;
        private double signalNoise;
        private double percentValue;
        private double refValue;
        private double injConc;
        private double totalArea;
        private int initialDelay;
        private int injLength;
        private int peakDelay;
        private int injAmount;
        private int injNumber;
        private string baselineMessage;
        private int baselineDriftTolerance;
        private int signalNoiseTolerance;
        private int mode;
        private int transformInitialDelay;
        

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public Form1()
        {
            InitializeComponent();
            mode = 0;
            ToggleMenu();
            AdvancedSettings.Hide();
            transformResults.View = View.Details;
            transformResults.MultiSelect = true;
            transformResults.LabelEdit = true;
            baselineDriftTolerance = 3;
            signalNoiseTolerance = 5;
            results.View = View.Details;
            results.MultiSelect = true;
            results.LabelEdit = true;
            results.Columns.Add("Slope", -2, HorizontalAlignment.Left);
            results.Columns.Add("%", -2, HorizontalAlignment.Left);
            results.Columns.Add("R-squared", -2, HorizontalAlignment.Left);
            results.Columns.Add("Experiment", -2, HorizontalAlignment.Left);
            results.Columns.Add("Comments", -2, HorizontalAlignment.Left);
            transformResults.Columns.Add("Remaining substrate", -2, HorizontalAlignment.Left);
            transformResults.Columns.Add("Heat rate", -2, HorizontalAlignment.Left);
            results.KeyDown += results_KeyDown;
            transformResults.KeyDown += results_KeyDown;
            textBox1.Validating += TextValidating;
            textBox3.Validating += TextValidating;
            textBox2.Validating += TextValidating;
            textBox4.Validating += TextValidating;
            refValueBox.Validating += TextValidating;
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
            itcdata_BG.MouseDown += Form1_MouseDown;
            resultsGraph.MouseDown += Form1_MouseDown;
            CalculateButton.Image = CalculateList.Images[0];
            transformationButton.Image = TransformationButtonList.Images[0];
            activityButton.Image = activityButtonList.Images[1];

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
        private void TotalTransformation(string fileName, int fileNumber)
        {

            //flip, et saaks võrrelda vana ITC-ga
            for (int c = 0; c < heat.Count; c++)
            {
                heat[c][fileNumber] = heat[c][fileNumber] * -1;
            }

            //baseline
            double baseline = 0;
            for (int a = 0; a < transformInitialDelay; a++)
            {
                baseline += heat[a][fileNumber];
            }
            baseline = baseline / transformInitialDelay;

            //baseline maha lahutada + total area + leiab max heatrate
            totalArea = 0;
            double maxValue = 0;
            int indexOfMax = 0;
            for (int a = transformInitialDelay; a < heat.Count; a++)
            {
                heat[a][fileNumber] -= baseline;
                Console.WriteLine("FILENUMBER: " + fileNumber + " LINE: " + a + " HEAT:" + heat[a][fileNumber]);
                //jätta välja negatiivsed väärtused
                if (heat[a][fileNumber] > 0)
                {
                    totalArea += heat[a][fileNumber];
                }
                if(heat[a][fileNumber] > maxValue)
                {
                    maxValue = heat[a][fileNumber];
                    indexOfMax = a;
                }
            }
            //Kogu pindala - pindala antud ajahetkeni = järelejäänud substraat
            for (int a = transformInitialDelay; a < heat.Count; a++)
            {
                double currentArea = 0;
                for(int i = transformInitialDelay; i <= a; i++ )
                    {
                    currentArea += heat[i][fileNumber];
                    }
                remainingSubstrate.Add(totalArea - currentArea);
                ListViewItem remSub = new ListViewItem((totalArea - currentArea).ToString("0.00000"), 0);
                remSub.SubItems.Add(heat[a][fileNumber].ToString("0.0000"));
                transformResults.Items.Add(remSub);
            }
            Series series1 = new Series { Name = titles[fileNumber], ChartType = SeriesChartType.Line, BorderWidth = 2 };
            transformationGraph.ChartAreas[0].AxisX.Minimum = 0;
            transformationGraph.ChartAreas[0].AxisY.Minimum = 0;

            for (int b = indexOfMax- transformInitialDelay; b < remainingSubstrate.Count; b++)
            {
                series1.Points.AddXY(remainingSubstrate[b],heat[b+ transformInitialDelay][fileNumber]);
            }
            transformationGraph.Series.Add(series1);
            remainingSubstrate.Clear();
        }
        
        private void LinearRegression(string fileName, int fileNumber)
        {
            List<double> averageHeat = new List<double>();
            List<double> conc = new List<double>();
            List<double> averageHeatDeviation = new List<double>();
            baselineMessage = "";
            //flip, et saaks võrrelda vana ITC-ga
            for (int c = 0; c < heat.Count; c++)
            {
                heat[c][fileNumber] = heat[c][fileNumber] * -1;
            }

            //baseline
            double baseline = 0;
            for (int a = initialDelay + peakDelay;a <= initialDelay + injLength; a++)
            {
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
                injStartValue.Add(heat[initialDelay + peakDelay + injLength * injNumber][fileNumber]);
                injEndValue.Add(heat[initialDelay + injLength * (injNumber + 1)][fileNumber]);
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
            //STDEV S/N jaoks
            for (int a = initialDelay + peakDelay+ injLength; a <= initialDelay + injLength*2; a++)
            {
                averageHeatDeviation.Add(Math.Pow(heat[a][fileNumber] - baseline - averageHeat[1], 2));      
               
            }
            
            for (int a = 0; a < averageHeatDeviation.Count; a++)
            {
                heat_STDEV += averageHeatDeviation[a];
            }
            heat_STDEV = heat_STDEV / averageHeatDeviation.Count();  
            heat_STDEV = Math.Sqrt(heat_STDEV);
            signalNoise = Math.Round(averageHeat[1] / heat_STDEV, 1, MidpointRounding.AwayFromZero);
            //Regression
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

            double rS = dblR * dblR;
            double intercept = meanY - ((sCo / ssX) * meanX);
            double slope = sCo / ssX;
            if (refValue > 0) {
                percentValue = (slope / refValue) * 100;
            }
            //Baseline drift evaluation
            //Hetkel jagab läbi ainult ühe STDEV väärtusega, mis on leitud esimese süsti põhjal
            int baseInj = 0;
            for (int i = 0; i < injAmount; i++)
            {
                if (Math.Abs(injEndValue[i] - injStartValue[i]) / heat_STDEV > baselineDriftTolerance && baseInj == 0)
                {
                    baselineMessage = "WARNING: Possible baseline drift for injection(s) nr: " + (i + 1).ToString();
                    baseInj++;
                }
                else if (Math.Abs(injEndValue[i] - injStartValue[i]) / heat_STDEV > baselineDriftTolerance && baseInj > 0)
                {
                    baselineMessage = baselineMessage + "," + (i + 1).ToString();
                }
            }
            baseInj = 0;
            //Data salvestamine 
            Experiment experiment = new Experiment(titles[fileNumber],slope, rS, intercept, injConc, averageHeat);
            experimentList.Add(experiment);

            //Andmed graafikule
            DrawActivityPlot(fileNumber);
            //Temp listid tühjaks järgmiseks katseks
            averageHeat.Clear();
            averageHeatDeviation.Clear();
            conc.Clear();
            heat_STDEV = 0;

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
                if (double.TryParse(tb.Text, out double dvalue))
                {
                        errorMsg = "";
                        injConc = dvalue;
                    return true;
                }
            }
            if (tb == refValueBox)
            {
                if (double.TryParse(tb.Text, out double rvalue))
                {
                    errorMsg = "";
                    refValue = rvalue;
                    return true;
                }
            }
            else if (int.TryParse(tb.Text, out int value))
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
            if (mode == 0)
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
            else if (mode == 1)
            {
                transformResults.FullRowSelect = true;
                var builder = new StringBuilder();
                foreach (ListViewItem item in transformResults.SelectedItems)
                {
                    builder.AppendLine(item.Text + "\t" + item.SubItems[1].Text);
                }
                //Kontrollib, et valik poleks tühi
                if (string.IsNullOrEmpty(builder.ToString())) { }
                else
                {
                    Clipboard.SetText(builder.ToString());
                }
            }
        }

        private void DrawActivityPlot(int ID)
        {
            Series series1 = new Series { Name = experimentList[ID].title, ChartType = SeriesChartType.Point, MarkerSize = 7 };
            Series trend = new Series { Name = "", ChartType = SeriesChartType.Line };
            resultsGraph.ChartAreas[0].AxisX.Minimum = 0;
            resultsGraph.ChartAreas[0].AxisY.Minimum = 0;
            series1.Points.AddXY(0, 0);

            for (int b = 1; b < experimentList[ID].avgHeat.Count; b++)
            {
                series1.Points.AddXY(experimentList[ID].conc * b, experimentList[ID].avgHeat[b]);
            }

            trend.Points.AddXY(0, experimentList[ID].intercept);
            trend.Points.AddXY(experimentList[ID].conc * (experimentList[ID].avgHeat.Count - 1), experimentList[ID].intercept + experimentList[ID].slope * experimentList[ID].conc * (experimentList[ID].avgHeat.Count - 1));
            if(resultsGraph.ChartAreas[0].AxisY.Maximum < Math.Round(experimentList[ID].intercept + experimentList[ID].slope * experimentList[ID].conc * (experimentList[ID].avgHeat.Count - 1) + 1, 2, MidpointRounding.AwayFromZero))
            {
                resultsGraph.ChartAreas[0].AxisY.Maximum = Math.Round(experimentList[ID].intercept + experimentList[ID].slope * experimentList[ID].conc * (experimentList[ID].avgHeat.Count - 1) + 1, 2, MidpointRounding.AwayFromZero);
            }

            try
            {
                trend.IsVisibleInLegend = false;
                resultsGraph.Series.Add(trend);
                resultsGraph.Series.Add(series1);
            }
            catch (System.ArgumentException)
            {
                //Ignob errorit, lubab sama nimega tulemusi panna graafikule, tulevikus lisada ID lõppu nimele
            }
        }//DrawActivityPlot

        private void SelectItems(object sender, EventArgs e) {
            resultsGraph.Series.Clear();
            resultsGraph.ChartAreas[0].AxisY.Maximum = 0;
            int ID;
            foreach (ListViewItem item in results.SelectedItems)
            { 
                ID = experimentList.Count - item.Index - 1;
                DrawActivityPlot(ID);
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

        private void ParseData()
        {
            if (mode == 0)
            {
                resultsGraph.Series.Clear();
                initialDelay = int.Parse(textBox1.Text) - 5;
                injLength = int.Parse(textBox2.Text);
                injAmount = int.Parse(textBox4.Text);
            }
            if (mode == 1)
            {
                transformationGraph.Series.Clear();
                transformInitialDelay = int.Parse(transformInitDelayBox.Text) - 5;

            }
            for (int i = 0; i < sFileNames.Count; i++)
            {
                titles.Clear();
                heat.Clear();
                using (TextFieldParser parser = new TextFieldParser(sFileNames[i]))
                {
                    parser.SetDelimiters(",");
                    List<string> titlesParser = parser.ReadFields().ToList();
                    for (int x = 0; x < titlesParser.Count; x++)
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
                                tempList.Add(double.Parse(fields[a - 1], System.Globalization.CultureInfo.InvariantCulture)); //0 - aeg, pole vaja; 1 - soojus
                            }
                        }
                        heat.Add(tempList);
                    }
                }
            }
        }//ParseData
        private void CalculateButton_Click(object sender, EventArgs e)
        {
            ParseData();

            switch (mode) {
                //Enzyme activity
                case 0:
            for (int i = 0; i < sFileNames.Count; i++)
            {
                for (int d = 0; d < titles.Count; d++)
                {
                    LinearRegression(sFileNames[i], d);
                    ListViewItem item = new ListViewItem(experimentList[d].slope.ToString("0.000"), 0);
                            if (refValue == 0)
                            {
                                item.SubItems.Add("No reference");
                            }
                            else if (refValue > 0)
                            {
                                item.SubItems.Add(percentValue.ToString("0.0"));
                            }
                    item.SubItems.Add(experimentList[d].rSquared.ToString("0.000"));
                    item.SubItems.Add(titles[d]);
                    item.SubItems.Add(CheckSignal() + " " + baselineMessage);
                    results.Items.Insert(0, item);
                    results.Columns[3].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                    results.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                        }
            }
                    break;
                //Total hydrolysis transformation
                case 1:
                    for (int i = 0; i < sFileNames.Count; i++)
                    {
                        for (int d = 0; d < titles.Count; d++)
                        {
                            TotalTransformation(sFileNames[i], d);
                        }
                    }
                    break;
        }
                    heat.Clear();
                    titles.Clear();
                }//CalculateButton

        private string CheckSignal() {
            if (signalNoise >= signalNoiseTolerance)
            {
                return "";
            }
            return "WARNING: S/N ratio is low: " + signalNoise.ToString("0.0");
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            results.Items.Clear();
            resultsGraph.Series.Clear();     
        }

        private void ToggleMenu()
        {
            switch (mode)
            {
                case 0:
                    results.Show();
                    resultsGraph.Show();
                    activityMenu.Show();
                    activityMenu.Enabled = true;
                    resultsGraph.Enabled = true;
                    results.Enabled = true;
                    transformResults.Enabled = false;
                    transformResults.Hide();
                    transformationGraph.Enabled = false;
                    transformationGraph.Hide();
                    transformationMenu.Hide();
                    transformationMenu.Enabled = false;
                    activityButton.Image = activityButtonList.Images[1];
                    transformationButton.Image = TransformationButtonList.Images[0];
                    break;
                case 1:
                    results.Hide();
                    resultsGraph.Hide();
                    activityMenu.Hide();
                    activityMenu.Enabled = false;
                    activityButton.Image = activityButtonList.Images[0];
                    transformationButton.Image = TransformationButtonList.Images[1];
                    transformationGraph.Enabled = true;
                    transformationGraph.Show();
                    transformResults.Show();
                    transformResults.Enabled = true;
                    transformationMenu.Show();
                    transformationMenu.Enabled = true;
                    break;
            }
        }//ToggleMenu
        private void AdvancedSettings_Click(object sender, EventArgs e)
        {
            AdvancedSettings.Show();
            AdvancedSettings.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdvancedSettings.Hide();
            AdvancedSettings.Enabled = false;
        }

        private void transformationButton_Click_1(object sender, EventArgs e)
        {
            mode = 1;
            ToggleMenu();
        }

        private void activityButton_Click(object sender, EventArgs e)
        {
            mode = 0;
            ToggleMenu();
        }
    }//end class Form1
    
} //end namespace ITCdata
