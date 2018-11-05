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
    public partial class Main : Form
    {
        List<string> sFileNames = new List<string>();
        List<string> titles = new List<string>();
        List<List<double>> heat = new List<List<double>>();
        List<double> remainingSubstrate = new List<double>();
        List<Experiment> experimentList = new List<Experiment>();
        List<Transform> transformList = new List<Transform>();

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
        private bool exceptionThrown;

        
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public Main()
        {
            InitializeComponent();

            mode = 0;
            ToggleMenu();
            AdvancedSettings.Hide();
            transformResults.View = View.Details;
            transformResults.MultiSelect = true;
            transformResults.LabelEdit = true;
            peakBox.Text = "40";
            baselineBox.Text = "3";
            signalBox.Text = "10";
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

            transformTitlesList.Columns.Add("Filename", -2, HorizontalAlignment.Left);
            transformTitlesList.View = View.Details;
            results.KeyDown += results_KeyDown;
            transformResults.KeyDown += results_KeyDown;
            textBox1.Validating += TextValidating;
            textBox3.Validating += TextValidating;
            textBox2.Validating += TextValidating;
            textBox4.Validating += TextValidating;
            refValueBox.Validating += TextValidating;
            peakDelay = 40;
            results.MouseClick += SelectItems;
            transformTitlesList.MouseClick += SelectItems;

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
            for (int c = 0; c < heat[fileNumber].Count; c++)
            {
                heat[fileNumber][c] = heat[fileNumber][c] * -1;
            }

            //baseline
            double baseline = 0;
            try
            {
                for (int a = 0; a < transformInitialDelay; a++)
                {
                    baseline += heat[fileNumber][a];
                }
                baseline = baseline / transformInitialDelay;
            //baseline maha lahutada + total area + leiab max heatrate
            totalArea = 0;
            double maxValue = 0;
            int indexOfMax = 0;
            for (int a = transformInitialDelay; a < heat[fileNumber].Count; a++)
            {
                heat[fileNumber][a] -= baseline;
                //jätta välja negatiivsed väärtused
                if (heat[fileNumber][a] > 0)
                {
                    totalArea += heat[fileNumber][a];
                }
                if(heat[fileNumber][a] > maxValue)
                {
                    maxValue = heat[fileNumber][a];
                    indexOfMax = a;
                }
            }
            //Kogu pindala - pindala antud ajahetkeni = järelejäänud substraat

            for (int a = transformInitialDelay; a < heat[fileNumber].Count; a++){
                double currentArea = 0;
                for(int i = transformInitialDelay; i <= a; i++ ){
                    currentArea += heat[fileNumber][i];
                }
                remainingSubstrate.Add(totalArea - currentArea);               
            }

            Transform transform = new Transform(titles[fileNumber],indexOfMax, transformInitialDelay, remainingSubstrate, heat[fileNumber]);
            transformList.Add(transform);
                ListViewItem title = new ListViewItem(titles[fileNumber]);
                transformTitlesList.Items.Insert(0, title);
                transformTitlesList.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                DrawTransformPlot(fileNumber);
                remainingSubstrate.Clear();
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Enter a correct value for initial delay");
                exceptionThrown = true;
                return;
            }
        }
        
        private void LinearRegression(string fileName, int fileNumber)
        {
            List<double> averageHeat = new List<double>();
            List<double> conc = new List<double>();
            List<double> averageHeatDeviation = new List<double>();
            List<double> injStartValue = new List<double>();
            List<double> injEndValue = new List<double>();
            baselineMessage = "";
            //flip, et saaks võrrelda vana ITC-ga
            for (int c = 0; c < heat[fileNumber].Count; c++)
            {
                heat[fileNumber][c] = heat[fileNumber][c] * -1;
            }
            //baseline
            double baseline = 0;
            try
            {
                for (int a = initialDelay + peakDelay; a <= initialDelay + injLength; a++)
                {
                    baseline += heat[fileNumber][a];
                }
            }
            catch (System.ArgumentOutOfRangeException)
            {
                MessageBox.Show("Slope can't be calculated, check experiment parameters");
                exceptionThrown = true;
                return;
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
            try
            {
                for (int k = 0; k < injAmount; k++)
                {
                    injNumber = k + 1;
                    averageHeat.Add(0);
                    injStartValue.Add(heat[fileNumber][initialDelay + peakDelay + injLength * injNumber]);
                    injEndValue.Add(heat[fileNumber][initialDelay + injLength * (injNumber + 1)]);
                    for (int a = initialDelay + peakDelay + injLength * injNumber; a <= initialDelay + injLength * (injNumber + 1); a++)
                    {
                        averageHeat[injNumber] += heat[fileNumber][a];
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Slope can't be calculated, check experiment parameters");
                exceptionThrown = true;
                return;
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
                averageHeatDeviation.Add(Math.Pow(heat[fileNumber][a] - baseline - averageHeat[1], 2));      
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
        //Check textbox input
        public bool ValidNumber(object sender, string number, out string errorMsg)
        {
            TextBox tb = (TextBox)sender;
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
            if (tb == textBox4 ) {
                int.TryParse(tb.Text, out int value);
                    if (value < 2)
                {
                    errorMsg = "Must have at least 2 injections";
                    return false;
                }
                    else
                {
                    errorMsg = "";
                    return true;
                }
            }
            else if (int.TryParse(tb.Text, out int value))
            {
                errorMsg = "";
                return true;
            }
            if (tb.Text != "")
            {
                errorMsg = "Insert a valid number";
                return false;
            }
            errorMsg = "";
            return true;
            

        }
        //Ctrl + C listener - copy to clipboard
        private void results_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyToClipBoard();
            }

        }
        //Copy to clipboard
        private void CopyToClipBoard()
        {
            if (mode == 0)
            {
                var builder = new StringBuilder();
                foreach (ListViewItem item in results.SelectedItems)
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

        private void DrawActivityPlot(int ID){
            Series series1 = new Series { Name = experimentList[ID].title, ChartType = SeriesChartType.Point, MarkerSize = 7 };
            Series trend = new Series { Name = "", ChartType = SeriesChartType.Line };
            resultsGraph.ChartAreas[0].AxisX.Minimum = 0;
            resultsGraph.ChartAreas[0].AxisY.Minimum = 0;
            series1.Points.AddXY(0, 0);

            for (int b = 1; b < experimentList[ID].avgHeat.Count; b++) {
                series1.Points.AddXY(experimentList[ID].conc * b, experimentList[ID].avgHeat[b]);
            }

            trend.Points.AddXY(0, experimentList[ID].intercept);
            trend.Points.AddXY(experimentList[ID].conc * (experimentList[ID].avgHeat.Count - 1), experimentList[ID].intercept + experimentList[ID].slope * experimentList[ID].conc * (experimentList[ID].avgHeat.Count - 1));
            if(resultsGraph.ChartAreas[0].AxisY.Maximum < Math.Round(experimentList[ID].intercept + experimentList[ID].slope * experimentList[ID].conc * (experimentList[ID].avgHeat.Count - 1) + 1, 2, MidpointRounding.AwayFromZero))
            {
                resultsGraph.ChartAreas[0].AxisY.Maximum = Math.Round(experimentList[ID].intercept + experimentList[ID].slope * experimentList[ID].conc * (experimentList[ID].avgHeat.Count - 1) + 1, 2, MidpointRounding.AwayFromZero);
            }
            try{
                trend.IsVisibleInLegend = false;
                resultsGraph.Series.Add(trend);
                resultsGraph.Series.Add(series1);
            }
            catch (System.ArgumentException) {
                //Ignob errorit, lubab sama nimega tulemusi panna graafikule, tulevikus lisada ID lõppu nimele
            }
        }//DrawActivityPlot
        //Show transform data and chart from list
        private void DrawTransformPlot(int ID)
        {
            transformationGraph.Series.Clear();
            transformResults.Items.Clear();
            //transformationGraph.ChartAreas[0].AxisY.Maximum = 0;
            //transformationGraph.ChartAreas[0].AxisX.Maximum = 0;
            //Data to listview
            Console.WriteLine("DRAWING: " + ID);
                for (int a = transformList[ID].iniDelay; a < transformList[ID].heatRate.Count; a++)
                {
                    ListViewItem remSub = new ListViewItem(transformList[ID].subRemaining[a - transformInitialDelay].ToString("0.00000"), 0);
                    remSub.SubItems.Add(transformList[ID].heatRate[a].ToString("0.0000"));
                    transformResults.Items.Add(remSub);
                }
            //Draw graph
            Series transformSeries = new Series { Name = transformList[ID].title, ChartType = SeriesChartType.Point, MarkerSize = 2 };
            for (int b = transformList[ID].indexMax - transformList[ID].iniDelay; b < transformList[ID].subRemaining.Count; b++)
            {
                transformSeries.Points.AddXY(transformList[ID].subRemaining[b], transformList[ID].heatRate[b + transformList[ID].iniDelay]);
            }
            transformationGraph.Series.Add(transformSeries);

        }//DrawTransformPlot

        private void SelectItems(object sender, EventArgs e) {
            if (mode == 0)
            {
                resultsGraph.Series.Clear();
                resultsGraph.ChartAreas[0].AxisY.Maximum = 0;
                int ID;
                foreach (ListViewItem item in results.SelectedItems)
                {
                    ID = experimentList.Count - item.Index - 1;
                    DrawActivityPlot(ID);
                }
            }
            else if (mode == 1)
            {
                int ID;
                if (transformTitlesList.SelectedItems.Count > 0)
                {
                    var item = transformTitlesList.SelectedItems[0];
                    ID = transformList.Count - item.Index - 1;
                    DrawTransformPlot(ID);
                }

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
            int lastPos = 0;
            if (mode == 0)
            {
                resultsGraph.Series.Clear();
                initialDelay = int.Parse(textBox1.Text) - 5;
                injLength = int.Parse(textBox2.Text);
                injAmount = int.Parse(textBox4.Text);
                peakDelay = int.Parse(peakBox.Text);
                baselineDriftTolerance = int.Parse(baselineBox.Text);
                signalNoiseTolerance = int.Parse(signalBox.Text);
    }
            if (mode == 1)
            {
                transformationGraph.Series.Clear();
                transformInitialDelay = int.Parse(transformInitDelayBox.Text) - 5;
                peakDelay = int.Parse(peakBox.Text);
            }
            for (int i = 0; i < sFileNames.Count; i++)
            {
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
                    for (int x = heat.Count; x < titles.Count; x++)
                    {
                        List<double> tempList = new List<double>();
                        heat.Add(tempList);
                    }
                    while (!parser.EndOfData)
                    {
                        List<string> fields = parser.ReadFields().ToList();
                        int b = lastPos;
                        for (int a = 1; a <= fields.Count; a++)
                        {
                            if (a % 2 == 0)
                            {
                                double.TryParse(fields[a - 1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double heatv);
                                if (heatv != 0)
                                {
                                    heat[b].Add(heatv);
                                }
                                b++;
                            }
                        }
                    }
                    lastPos = heat.Count;
                }
            }
        }//ParseData
        private void CalculateButton_Click(object sender, EventArgs e){
            if(sFileNames.Count == 0){
                MessageBox.Show("Please select file(s)");
            }
           else{
                ParseData();
                switch (mode){
                    //Enzyme activity
                    case 0:
                        for (int d = 0; d < titles.Count; d++){
                            LinearRegression(titles[d], d);
                            if (exceptionThrown != true){
                                ListViewItem item = new ListViewItem(experimentList[experimentList.Count - 1].slope.ToString("0.000"), 0);
                                if (refValue == 0){
                                    item.SubItems.Add("");
                                }
                                else if (refValue > 0){
                                    item.SubItems.Add(percentValue.ToString("0.0"));
                                }
                                DrawActivityPlot(experimentList.Count - 1);
                                item.SubItems.Add(experimentList[experimentList.Count - 1].rSquared.ToString("0.000"));
                                item.SubItems.Add(experimentList[experimentList.Count - 1].title);
                                item.SubItems.Add(CheckSignal() + " " + baselineMessage);
                                results.Items.Insert(0, item);
                                results.Columns[3].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                                results.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                            }
                        }
                        break;
                    //Total hydrolysis transformation
                    case 1:
                        for (int d = 0; d < titles.Count; d++){
                            TotalTransformation(titles[d], d);
                        }
                        break;
                }
                heat.Clear();
                titles.Clear();
                exceptionThrown = false;
            }
                }//CalculateButton

        private string CheckSignal(){
            if (signalNoise >= signalNoiseTolerance){
                return "";
            }
            return "WARNING: S/N ratio is low: " + signalNoise.ToString("0.0");
        }
        //Clear data
        private void resetButton_Click(object sender, EventArgs e)
        {
            switch (mode) {
                case 0:
            results.Items.Clear();
            resultsGraph.Series.Clear();
                    break;
                case 1:
            transformResults.Items.Clear();
            transformationGraph.Series.Clear();
                    break;
            }
        }
        //Header menu controls
        private void ToggleMenu()
        {
            sFileNames.Clear();
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
        }

        private void transformationButton_Click_1(object sender, EventArgs e){
            mode = 1;
            ToggleMenu();
        }

        private void activityButton_Click(object sender, EventArgs e){
            mode = 0;
            ToggleMenu();
        }

        private void advancedSettingsButton_Click(object sender, EventArgs e){
            AdvancedSettings.Show();
            AdvancedSettings.Enabled = true;
        }

        private void closeSettingsButton_Click(object sender, EventArgs e){
            AdvancedSettings.Hide();
            AdvancedSettings.Enabled = false;
        }
    }//end class Form1
    
} //end namespace ITCdata
