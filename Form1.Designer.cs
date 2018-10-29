namespace ITCdata
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SelectList = new System.Windows.Forms.ImageList(this.components);
            this.SelectButton = new System.Windows.Forms.PictureBox();
            this.CalculateButton = new System.Windows.Forms.PictureBox();
            this.CalculateList = new System.Windows.Forms.ImageList(this.components);
            this.CloseButton = new System.Windows.Forms.PictureBox();
            this.MinimizeButton = new System.Windows.Forms.PictureBox();
            this.ResultsGraph = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.results = new System.Windows.Forms.ListView();
            this.itcdata_BG = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.SelectButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CalculateButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CloseButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinimizeButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResultsGraph)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itcdata_BG)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox5
            // 
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox5.Font = new System.Drawing.Font("Palanquin Light", 12F);
            this.textBox5.Location = new System.Drawing.Point(43, 580);
            this.textBox5.Margin = new System.Windows.Forms.Padding(2);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(161, 29);
            this.textBox5.TabIndex = 6;
            // 
            // textBox4
            // 
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Font = new System.Drawing.Font("Palanquin Light", 12F);
            this.textBox4.Location = new System.Drawing.Point(43, 488);
            this.textBox4.Margin = new System.Windows.Forms.Padding(2);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(161, 29);
            this.textBox4.TabIndex = 5;
            // 
            // textBox3
            // 
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Font = new System.Drawing.Font("Palanquin Light", 12F);
            this.textBox3.Location = new System.Drawing.Point(43, 395);
            this.textBox3.Margin = new System.Windows.Forms.Padding(2);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(161, 29);
            this.textBox3.TabIndex = 4;
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.White;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Font = new System.Drawing.Font("Palanquin Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.textBox2.Location = new System.Drawing.Point(43, 301);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(161, 29);
            this.textBox2.TabIndex = 3;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Palanquin Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.textBox1.Location = new System.Drawing.Point(43, 206);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(161, 29);
            this.textBox1.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.White;
            this.label8.Font = new System.Drawing.Font("Palanquin Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.label8.Location = new System.Drawing.Point(1033, 690);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(117, 21);
            this.label8.TabIndex = 5;
            this.label8.Text = "v0.1 (2018) Robert Risti";
            // 
            // SelectList
            // 
            this.SelectList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("SelectList.ImageStream")));
            this.SelectList.TransparentColor = System.Drawing.Color.Transparent;
            this.SelectList.Images.SetKeyName(0, "BrowseButton.png");
            this.SelectList.Images.SetKeyName(1, "BrowseButtonActive.png");
            // 
            // SelectButton
            // 
            this.SelectButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.SelectButton.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.SelectButton.Location = new System.Drawing.Point(0, 103);
            this.SelectButton.Margin = new System.Windows.Forms.Padding(0);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(240, 40);
            this.SelectButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.SelectButton.TabIndex = 8;
            this.SelectButton.TabStop = false;
            // 
            // CalculateButton
            // 
            this.CalculateButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CalculateButton.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.CalculateButton.Location = new System.Drawing.Point(0, 651);
            this.CalculateButton.Margin = new System.Windows.Forms.Padding(0);
            this.CalculateButton.Name = "CalculateButton";
            this.CalculateButton.Size = new System.Drawing.Size(240, 40);
            this.CalculateButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.CalculateButton.TabIndex = 12;
            this.CalculateButton.TabStop = false;
            this.CalculateButton.Click += new System.EventHandler(this.CalculateButton_Click);
            // 
            // CalculateList
            // 
            this.CalculateList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("CalculateList.ImageStream")));
            this.CalculateList.TransparentColor = System.Drawing.Color.Transparent;
            this.CalculateList.Images.SetKeyName(0, "CalculateButton.png");
            this.CalculateList.Images.SetKeyName(1, "CalculateButtonActive.png");
            // 
            // CloseButton
            // 
            this.CloseButton.BackColor = System.Drawing.Color.Transparent;
            this.CloseButton.Image = ((System.Drawing.Image)(resources.GetObject("CloseButton.Image")));
            this.CloseButton.Location = new System.Drawing.Point(1243, 12);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(25, 25);
            this.CloseButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CloseButton.TabIndex = 13;
            this.CloseButton.TabStop = false;
            // 
            // MinimizeButton
            // 
            this.MinimizeButton.Image = ((System.Drawing.Image)(resources.GetObject("MinimizeButton.Image")));
            this.MinimizeButton.Location = new System.Drawing.Point(1212, 12);
            this.MinimizeButton.Name = "MinimizeButton";
            this.MinimizeButton.Size = new System.Drawing.Size(25, 25);
            this.MinimizeButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MinimizeButton.TabIndex = 14;
            this.MinimizeButton.TabStop = false;
            // 
            // ResultsGraph
            // 
            chartArea1.Name = "ChartArea1";
            this.ResultsGraph.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.ResultsGraph.Legends.Add(legend1);
            this.ResultsGraph.Location = new System.Drawing.Point(242, 483);
            this.ResultsGraph.Name = "ResultsGraph";
            this.ResultsGraph.Size = new System.Drawing.Size(706, 236);
            this.ResultsGraph.TabIndex = 15;
            this.ResultsGraph.Text = "Enzyme activity";
            // 
            // results
            // 
            this.results.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.results.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.results.Font = new System.Drawing.Font("Palanquin Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.results.Location = new System.Drawing.Point(241, 83);
            this.results.Margin = new System.Windows.Forms.Padding(2);
            this.results.Name = "results";
            this.results.Size = new System.Drawing.Size(1040, 398);
            this.results.TabIndex = 6;
            this.results.UseCompatibleStateImageBehavior = false;
            // 
            // itcdata_BG
            // 
            this.itcdata_BG.Image = ((System.Drawing.Image)(resources.GetObject("itcdata_BG.Image")));
            this.itcdata_BG.Location = new System.Drawing.Point(0, 0);
            this.itcdata_BG.Name = "itcdata_BG";
            this.itcdata_BG.Size = new System.Drawing.Size(1280, 720);
            this.itcdata_BG.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.itcdata_BG.TabIndex = 16;
            this.itcdata_BG.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.ResultsGraph);
            this.Controls.Add(this.MinimizeButton);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.CalculateButton);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.SelectButton);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.results);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.itcdata_BG);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "ITCdata";
            ((System.ComponentModel.ISupportInitialize)(this.SelectButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CalculateButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CloseButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinimizeButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResultsGraph)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itcdata_BG)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.ImageList SelectList;
        private System.Windows.Forms.PictureBox SelectButton;
        private System.Windows.Forms.PictureBox CalculateButton;
        private System.Windows.Forms.ImageList CalculateList;
        private System.Windows.Forms.PictureBox CloseButton;
        private System.Windows.Forms.PictureBox MinimizeButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.DataVisualization.Charting.Chart ResultsGraph;
        private System.Windows.Forms.ListView results;
        private System.Windows.Forms.PictureBox itcdata_BG;
    }
}

