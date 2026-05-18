
namespace PochiPochiEditorGabu._Pokemon
{
    partial class PokedexHabitatEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PokedexHabitatEditor));
            this.btnSave = new System.Windows.Forms.Button();
            this.grpSelectArea = new System.Windows.Forms.GroupBox();
            this.btnCreateNewAreaData = new System.Windows.Forms.Button();
            this.nudPageCount = new System.Windows.Forms.NumericUpDown();
            this.txtAreaAddr = new System.Windows.Forms.TextBox();
            this.lblPageCount = new System.Windows.Forms.Label();
            this.lblAreaAddr = new System.Windows.Forms.Label();
            this.cmbArea = new System.Windows.Forms.ComboBox();
            this.grpSelectPage = new System.Windows.Forms.GroupBox();
            this.grpPokemon = new System.Windows.Forms.GroupBox();
            this.picPokemon4 = new System.Windows.Forms.PictureBox();
            this.picPokemon3 = new System.Windows.Forms.PictureBox();
            this.picPokemon2 = new System.Windows.Forms.PictureBox();
            this.picPokemon1 = new System.Windows.Forms.PictureBox();
            this.cmbPokemonName = new System.Windows.Forms.ComboBox();
            this.lstPage = new System.Windows.Forms.ListBox();
            this.btnCreateNewPageData = new System.Windows.Forms.Button();
            this.nudPokemonCount = new System.Windows.Forms.NumericUpDown();
            this.txtPageAddr = new System.Windows.Forms.TextBox();
            this.lblPokemonCount = new System.Windows.Forms.Label();
            this.lblPageAddr = new System.Windows.Forms.Label();
            this.grpSelectArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPageCount)).BeginInit();
            this.grpSelectPage.SuspendLayout();
            this.grpPokemon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPokemon4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPokemon3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPokemon2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPokemon1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPokemonCount)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(20, 16);
            this.btnSave.Margin = new System.Windows.Forms.Padding(0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(96, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "変更を保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // grpSelectArea
            // 
            this.grpSelectArea.Controls.Add(this.btnCreateNewAreaData);
            this.grpSelectArea.Controls.Add(this.nudPageCount);
            this.grpSelectArea.Controls.Add(this.txtAreaAddr);
            this.grpSelectArea.Controls.Add(this.lblPageCount);
            this.grpSelectArea.Controls.Add(this.lblAreaAddr);
            this.grpSelectArea.Controls.Add(this.cmbArea);
            this.grpSelectArea.Location = new System.Drawing.Point(20, 50);
            this.grpSelectArea.Margin = new System.Windows.Forms.Padding(0);
            this.grpSelectArea.Name = "grpSelectArea";
            this.grpSelectArea.Padding = new System.Windows.Forms.Padding(0);
            this.grpSelectArea.Size = new System.Drawing.Size(180, 144);
            this.grpSelectArea.TabIndex = 1;
            this.grpSelectArea.TabStop = false;
            this.grpSelectArea.Text = "エリアを選択";
            // 
            // btnCreateNewAreaData
            // 
            this.btnCreateNewAreaData.Location = new System.Drawing.Point(20, 102);
            this.btnCreateNewAreaData.Margin = new System.Windows.Forms.Padding(0);
            this.btnCreateNewAreaData.Name = "btnCreateNewAreaData";
            this.btnCreateNewAreaData.Size = new System.Drawing.Size(138, 23);
            this.btnCreateNewAreaData.TabIndex = 4;
            this.btnCreateNewAreaData.Text = "ページ数を変更";
            this.btnCreateNewAreaData.UseVisualStyleBackColor = true;
            // 
            // nudPageCount
            // 
            this.nudPageCount.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudPageCount.Location = new System.Drawing.Point(80, 78);
            this.nudPageCount.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudPageCount.Name = "nudPageCount";
            this.nudPageCount.ReadOnly = true;
            this.nudPageCount.Size = new System.Drawing.Size(78, 19);
            this.nudPageCount.TabIndex = 3;
            // 
            // txtAreaAddr
            // 
            this.txtAreaAddr.Location = new System.Drawing.Point(80, 54);
            this.txtAreaAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtAreaAddr.Name = "txtAreaAddr";
            this.txtAreaAddr.ReadOnly = true;
            this.txtAreaAddr.Size = new System.Drawing.Size(78, 19);
            this.txtAreaAddr.TabIndex = 2;
            // 
            // lblPageCount
            // 
            this.lblPageCount.AutoSize = true;
            this.lblPageCount.Location = new System.Drawing.Point(20, 82);
            this.lblPageCount.Margin = new System.Windows.Forms.Padding(0);
            this.lblPageCount.Name = "lblPageCount";
            this.lblPageCount.Size = new System.Drawing.Size(53, 12);
            this.lblPageCount.TabIndex = 1;
            this.lblPageCount.Text = "ページ数 :";
            // 
            // lblAreaAddr
            // 
            this.lblAreaAddr.AutoSize = true;
            this.lblAreaAddr.Location = new System.Drawing.Point(20, 58);
            this.lblAreaAddr.Margin = new System.Windows.Forms.Padding(0);
            this.lblAreaAddr.Name = "lblAreaAddr";
            this.lblAreaAddr.Size = new System.Drawing.Size(47, 12);
            this.lblAreaAddr.TabIndex = 1;
            this.lblAreaAddr.Text = "アドレス :";
            // 
            // cmbArea
            // 
            this.cmbArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbArea.FormattingEnabled = true;
            this.cmbArea.Location = new System.Drawing.Point(20, 28);
            this.cmbArea.Margin = new System.Windows.Forms.Padding(0);
            this.cmbArea.Name = "cmbArea";
            this.cmbArea.Size = new System.Drawing.Size(138, 20);
            this.cmbArea.TabIndex = 0;
            // 
            // grpSelectPage
            // 
            this.grpSelectPage.Controls.Add(this.grpPokemon);
            this.grpSelectPage.Controls.Add(this.lstPage);
            this.grpSelectPage.Controls.Add(this.btnCreateNewPageData);
            this.grpSelectPage.Controls.Add(this.nudPokemonCount);
            this.grpSelectPage.Controls.Add(this.txtPageAddr);
            this.grpSelectPage.Controls.Add(this.lblPokemonCount);
            this.grpSelectPage.Controls.Add(this.lblPageAddr);
            this.grpSelectPage.Location = new System.Drawing.Point(218, 50);
            this.grpSelectPage.Margin = new System.Windows.Forms.Padding(0);
            this.grpSelectPage.Name = "grpSelectPage";
            this.grpSelectPage.Padding = new System.Windows.Forms.Padding(0);
            this.grpSelectPage.Size = new System.Drawing.Size(538, 284);
            this.grpSelectPage.TabIndex = 2;
            this.grpSelectPage.TabStop = false;
            this.grpSelectPage.Text = "ページを選択";
            // 
            // grpPokemon
            // 
            this.grpPokemon.Controls.Add(this.picPokemon4);
            this.grpPokemon.Controls.Add(this.picPokemon3);
            this.grpPokemon.Controls.Add(this.picPokemon2);
            this.grpPokemon.Controls.Add(this.picPokemon1);
            this.grpPokemon.Controls.Add(this.cmbPokemonName);
            this.grpPokemon.Location = new System.Drawing.Point(156, 110);
            this.grpPokemon.Margin = new System.Windows.Forms.Padding(0);
            this.grpPokemon.Name = "grpPokemon";
            this.grpPokemon.Padding = new System.Windows.Forms.Padding(0);
            this.grpPokemon.Size = new System.Drawing.Size(360, 152);
            this.grpPokemon.TabIndex = 11;
            this.grpPokemon.TabStop = false;
            this.grpPokemon.Text = "データ";
            // 
            // picPokemon4
            // 
            this.picPokemon4.Location = new System.Drawing.Point(274, 66);
            this.picPokemon4.Margin = new System.Windows.Forms.Padding(0);
            this.picPokemon4.Name = "picPokemon4";
            this.picPokemon4.Size = new System.Drawing.Size(64, 64);
            this.picPokemon4.TabIndex = 1;
            this.picPokemon4.TabStop = false;
            // 
            // picPokemon3
            // 
            this.picPokemon3.Location = new System.Drawing.Point(188, 66);
            this.picPokemon3.Margin = new System.Windows.Forms.Padding(0);
            this.picPokemon3.Name = "picPokemon3";
            this.picPokemon3.Size = new System.Drawing.Size(64, 64);
            this.picPokemon3.TabIndex = 1;
            this.picPokemon3.TabStop = false;
            // 
            // picPokemon2
            // 
            this.picPokemon2.Location = new System.Drawing.Point(104, 66);
            this.picPokemon2.Margin = new System.Windows.Forms.Padding(0);
            this.picPokemon2.Name = "picPokemon2";
            this.picPokemon2.Size = new System.Drawing.Size(64, 64);
            this.picPokemon2.TabIndex = 1;
            this.picPokemon2.TabStop = false;
            // 
            // picPokemon1
            // 
            this.picPokemon1.Location = new System.Drawing.Point(20, 66);
            this.picPokemon1.Margin = new System.Windows.Forms.Padding(0);
            this.picPokemon1.Name = "picPokemon1";
            this.picPokemon1.Size = new System.Drawing.Size(64, 64);
            this.picPokemon1.TabIndex = 1;
            this.picPokemon1.TabStop = false;
            // 
            // cmbPokemonName
            // 
            this.cmbPokemonName.FormattingEnabled = true;
            this.cmbPokemonName.Location = new System.Drawing.Point(20, 28);
            this.cmbPokemonName.Margin = new System.Windows.Forms.Padding(0);
            this.cmbPokemonName.Name = "cmbPokemonName";
            this.cmbPokemonName.Size = new System.Drawing.Size(120, 20);
            this.cmbPokemonName.TabIndex = 0;
            // 
            // lstPage
            // 
            this.lstPage.FormattingEnabled = true;
            this.lstPage.ItemHeight = 12;
            this.lstPage.Location = new System.Drawing.Point(20, 28);
            this.lstPage.Margin = new System.Windows.Forms.Padding(0);
            this.lstPage.Name = "lstPage";
            this.lstPage.ScrollAlwaysVisible = true;
            this.lstPage.Size = new System.Drawing.Size(120, 232);
            this.lstPage.TabIndex = 10;
            // 
            // btnCreateNewPageData
            // 
            this.btnCreateNewPageData.Location = new System.Drawing.Point(156, 76);
            this.btnCreateNewPageData.Margin = new System.Windows.Forms.Padding(0);
            this.btnCreateNewPageData.Name = "btnCreateNewPageData";
            this.btnCreateNewPageData.Size = new System.Drawing.Size(146, 23);
            this.btnCreateNewPageData.TabIndex = 9;
            this.btnCreateNewPageData.Text = "ポケモン数を変更";
            this.btnCreateNewPageData.UseVisualStyleBackColor = true;
            // 
            // nudPokemonCount
            // 
            this.nudPokemonCount.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudPokemonCount.Location = new System.Drawing.Point(224, 52);
            this.nudPokemonCount.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudPokemonCount.Name = "nudPokemonCount";
            this.nudPokemonCount.ReadOnly = true;
            this.nudPokemonCount.Size = new System.Drawing.Size(78, 19);
            this.nudPokemonCount.TabIndex = 8;
            // 
            // txtPageAddr
            // 
            this.txtPageAddr.Location = new System.Drawing.Point(224, 28);
            this.txtPageAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtPageAddr.Name = "txtPageAddr";
            this.txtPageAddr.ReadOnly = true;
            this.txtPageAddr.Size = new System.Drawing.Size(78, 19);
            this.txtPageAddr.TabIndex = 7;
            // 
            // lblPokemonCount
            // 
            this.lblPokemonCount.AutoSize = true;
            this.lblPokemonCount.Location = new System.Drawing.Point(156, 56);
            this.lblPokemonCount.Margin = new System.Windows.Forms.Padding(0);
            this.lblPokemonCount.Name = "lblPokemonCount";
            this.lblPokemonCount.Size = new System.Drawing.Size(60, 12);
            this.lblPokemonCount.TabIndex = 5;
            this.lblPokemonCount.Text = "ポケモン数 :";
            // 
            // lblPageAddr
            // 
            this.lblPageAddr.AutoSize = true;
            this.lblPageAddr.Location = new System.Drawing.Point(156, 32);
            this.lblPageAddr.Margin = new System.Windows.Forms.Padding(0);
            this.lblPageAddr.Name = "lblPageAddr";
            this.lblPageAddr.Size = new System.Drawing.Size(47, 12);
            this.lblPageAddr.TabIndex = 6;
            this.lblPageAddr.Text = "アドレス :";
            // 
            // PokedexHabitatEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 353);
            this.Controls.Add(this.grpSelectPage);
            this.Controls.Add(this.grpSelectArea);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PokedexHabitatEditor";
            this.Text = "図鑑生息地";
            this.grpSelectArea.ResumeLayout(false);
            this.grpSelectArea.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPageCount)).EndInit();
            this.grpSelectPage.ResumeLayout(false);
            this.grpSelectPage.PerformLayout();
            this.grpPokemon.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPokemon4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPokemon3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPokemon2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPokemon1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPokemonCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox grpSelectArea;
        private System.Windows.Forms.NumericUpDown nudPageCount;
        private System.Windows.Forms.TextBox txtAreaAddr;
        private System.Windows.Forms.Label lblPageCount;
        private System.Windows.Forms.Label lblAreaAddr;
        private System.Windows.Forms.ComboBox cmbArea;
        private System.Windows.Forms.Button btnCreateNewAreaData;
        private System.Windows.Forms.GroupBox grpSelectPage;
        private System.Windows.Forms.ListBox lstPage;
        private System.Windows.Forms.Button btnCreateNewPageData;
        private System.Windows.Forms.NumericUpDown nudPokemonCount;
        private System.Windows.Forms.TextBox txtPageAddr;
        private System.Windows.Forms.Label lblPokemonCount;
        private System.Windows.Forms.Label lblPageAddr;
        private System.Windows.Forms.GroupBox grpPokemon;
        private System.Windows.Forms.ComboBox cmbPokemonName;
        private System.Windows.Forms.PictureBox picPokemon4;
        private System.Windows.Forms.PictureBox picPokemon3;
        private System.Windows.Forms.PictureBox picPokemon2;
        private System.Windows.Forms.PictureBox picPokemon1;
    }
}