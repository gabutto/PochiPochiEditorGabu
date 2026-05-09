
namespace PochiPochiEditorGabu._Pokemon
{
    partial class PokedexOrderEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PokedexOrderEditor));
            this.btnSave = new System.Windows.Forms.Button();
            this.lstOrder = new System.Windows.Forms.ListBox();
            this.picIcon = new System.Windows.Forms.PictureBox();
            this.lblSpecies = new System.Windows.Forms.Label();
            this.nudSpecies = new System.Windows.Forms.NumericUpDown();
            this.lblOrder = new System.Windows.Forms.Label();
            this.nudOrder = new System.Windows.Forms.NumericUpDown();
            this.txtSpeciesHex = new System.Windows.Forms.TextBox();
            this.lblSpeciesHex = new System.Windows.Forms.Label();
            this.grpNotes = new System.Windows.Forms.GroupBox();
            this.lblNote2 = new System.Windows.Forms.Label();
            this.lblNote1 = new System.Windows.Forms.Label();
            this.lstUnused = new System.Windows.Forms.ListBox();
            this.lblUnused = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpecies)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOrder)).BeginInit();
            this.grpNotes.SuspendLayout();
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
            // lstOrder
            // 
            this.lstOrder.FormattingEnabled = true;
            this.lstOrder.ItemHeight = 12;
            this.lstOrder.Location = new System.Drawing.Point(20, 52);
            this.lstOrder.Margin = new System.Windows.Forms.Padding(0);
            this.lstOrder.Name = "lstOrder";
            this.lstOrder.ScrollAlwaysVisible = true;
            this.lstOrder.Size = new System.Drawing.Size(176, 352);
            this.lstOrder.TabIndex = 1;
            // 
            // picIcon
            // 
            this.picIcon.Location = new System.Drawing.Point(212, 56);
            this.picIcon.Margin = new System.Windows.Forms.Padding(0);
            this.picIcon.Name = "picIcon";
            this.picIcon.Size = new System.Drawing.Size(32, 32);
            this.picIcon.TabIndex = 2;
            this.picIcon.TabStop = false;
            // 
            // lblSpecies
            // 
            this.lblSpecies.AutoSize = true;
            this.lblSpecies.Location = new System.Drawing.Point(258, 56);
            this.lblSpecies.Margin = new System.Windows.Forms.Padding(0);
            this.lblSpecies.Name = "lblSpecies";
            this.lblSpecies.Size = new System.Drawing.Size(75, 12);
            this.lblSpecies.TabIndex = 3;
            this.lblSpecies.Text = "ポケモンコード :";
            // 
            // nudSpecies
            // 
            this.nudSpecies.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudSpecies.Location = new System.Drawing.Point(340, 52);
            this.nudSpecies.Margin = new System.Windows.Forms.Padding(0);
            this.nudSpecies.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudSpecies.Name = "nudSpecies";
            this.nudSpecies.ReadOnly = true;
            this.nudSpecies.Size = new System.Drawing.Size(64, 19);
            this.nudSpecies.TabIndex = 4;
            // 
            // lblOrder
            // 
            this.lblOrder.AutoSize = true;
            this.lblOrder.Location = new System.Drawing.Point(258, 104);
            this.lblOrder.Margin = new System.Windows.Forms.Padding(0);
            this.lblOrder.Name = "lblOrder";
            this.lblOrder.Size = new System.Drawing.Size(59, 12);
            this.lblOrder.TabIndex = 3;
            this.lblOrder.Text = "図鑑番号 :";
            // 
            // nudOrder
            // 
            this.nudOrder.Location = new System.Drawing.Point(340, 100);
            this.nudOrder.Margin = new System.Windows.Forms.Padding(0);
            this.nudOrder.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudOrder.Name = "nudOrder";
            this.nudOrder.Size = new System.Drawing.Size(64, 19);
            this.nudOrder.TabIndex = 4;
            // 
            // txtSpeciesHex
            // 
            this.txtSpeciesHex.Location = new System.Drawing.Point(340, 76);
            this.txtSpeciesHex.Margin = new System.Windows.Forms.Padding(0);
            this.txtSpeciesHex.Name = "txtSpeciesHex";
            this.txtSpeciesHex.ReadOnly = true;
            this.txtSpeciesHex.Size = new System.Drawing.Size(64, 19);
            this.txtSpeciesHex.TabIndex = 5;
            // 
            // lblSpeciesHex
            // 
            this.lblSpeciesHex.AutoSize = true;
            this.lblSpeciesHex.Location = new System.Drawing.Point(258, 80);
            this.lblSpeciesHex.Margin = new System.Windows.Forms.Padding(0);
            this.lblSpeciesHex.Name = "lblSpeciesHex";
            this.lblSpeciesHex.Size = new System.Drawing.Size(59, 12);
            this.lblSpeciesHex.TabIndex = 3;
            this.lblSpeciesHex.Text = "（16進数） :";
            // 
            // grpNotes
            // 
            this.grpNotes.Controls.Add(this.lblNote2);
            this.grpNotes.Controls.Add(this.lblNote1);
            this.grpNotes.Location = new System.Drawing.Point(212, 130);
            this.grpNotes.Margin = new System.Windows.Forms.Padding(0);
            this.grpNotes.Name = "grpNotes";
            this.grpNotes.Padding = new System.Windows.Forms.Padding(0);
            this.grpNotes.Size = new System.Drawing.Size(170, 82);
            this.grpNotes.TabIndex = 6;
            this.grpNotes.TabStop = false;
            this.grpNotes.Text = "備考";
            // 
            // lblNote2
            // 
            this.lblNote2.AutoSize = true;
            this.lblNote2.Location = new System.Drawing.Point(20, 50);
            this.lblNote2.Margin = new System.Windows.Forms.Padding(0);
            this.lblNote2.Name = "lblNote2";
            this.lblNote2.Size = new System.Drawing.Size(123, 12);
            this.lblNote2.TabIndex = 0;
            this.lblNote2.Text = "黄色 : 図鑑番号範囲外";
            // 
            // lblNote1
            // 
            this.lblNote1.AutoSize = true;
            this.lblNote1.Location = new System.Drawing.Point(20, 28);
            this.lblNote1.Margin = new System.Windows.Forms.Padding(0);
            this.lblNote1.Name = "lblNote1";
            this.lblNote1.Size = new System.Drawing.Size(130, 12);
            this.lblNote1.TabIndex = 0;
            this.lblNote1.Text = "赤色 : 重複する図鑑番号";
            // 
            // lstUnused
            // 
            this.lstUnused.FormattingEnabled = true;
            this.lstUnused.ItemHeight = 12;
            this.lstUnused.Location = new System.Drawing.Point(212, 244);
            this.lstUnused.Margin = new System.Windows.Forms.Padding(0);
            this.lstUnused.Name = "lstUnused";
            this.lstUnused.ScrollAlwaysVisible = true;
            this.lstUnused.Size = new System.Drawing.Size(170, 160);
            this.lstUnused.TabIndex = 7;
            // 
            // lblUnused
            // 
            this.lblUnused.AutoSize = true;
            this.lblUnused.Location = new System.Drawing.Point(212, 224);
            this.lblUnused.Margin = new System.Windows.Forms.Padding(0);
            this.lblUnused.Name = "lblUnused";
            this.lblUnused.Size = new System.Drawing.Size(105, 12);
            this.lblUnused.TabIndex = 8;
            this.lblUnused.Text = "未使用の図鑑番号 :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 416);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(395, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "※「ポケモン」の図鑑タブは、図鑑番号準拠なので図鑑番号を変更する際は要修正";
            // 
            // PokedexOrderEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 441);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblUnused);
            this.Controls.Add(this.lstUnused);
            this.Controls.Add(this.grpNotes);
            this.Controls.Add(this.txtSpeciesHex);
            this.Controls.Add(this.nudOrder);
            this.Controls.Add(this.nudSpecies);
            this.Controls.Add(this.lblOrder);
            this.Controls.Add(this.lblSpeciesHex);
            this.Controls.Add(this.lblSpecies);
            this.Controls.Add(this.picIcon);
            this.Controls.Add(this.lstOrder);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PokedexOrderEditor";
            this.Text = "図鑑番号";
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpecies)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOrder)).EndInit();
            this.grpNotes.ResumeLayout(false);
            this.grpNotes.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ListBox lstOrder;
        private System.Windows.Forms.PictureBox picIcon;
        private System.Windows.Forms.Label lblSpecies;
        private System.Windows.Forms.NumericUpDown nudSpecies;
        private System.Windows.Forms.Label lblOrder;
        private System.Windows.Forms.NumericUpDown nudOrder;
        private System.Windows.Forms.TextBox txtSpeciesHex;
        private System.Windows.Forms.Label lblSpeciesHex;
        private System.Windows.Forms.GroupBox grpNotes;
        private System.Windows.Forms.Label lblNote2;
        private System.Windows.Forms.Label lblNote1;
        private System.Windows.Forms.ListBox lstUnused;
        private System.Windows.Forms.Label lblUnused;
        private System.Windows.Forms.Label label1;
    }
}