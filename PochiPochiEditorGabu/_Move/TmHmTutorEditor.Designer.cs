
namespace PochiPochiEditorGabu._Move
{
    partial class TmHmTutorEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TmHmTutorEditor));
            this.btnSave = new System.Windows.Forms.Button();
            this.lstTmHm = new System.Windows.Forms.ListBox();
            this.lstTutor = new System.Windows.Forms.ListBox();
            this.cmbMove1 = new System.Windows.Forms.ComboBox();
            this.cmbMove2 = new System.Windows.Forms.ComboBox();
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
            // lstTmHm
            // 
            this.lstTmHm.FormattingEnabled = true;
            this.lstTmHm.ItemHeight = 12;
            this.lstTmHm.Location = new System.Drawing.Point(20, 52);
            this.lstTmHm.Margin = new System.Windows.Forms.Padding(0);
            this.lstTmHm.Name = "lstTmHm";
            this.lstTmHm.Size = new System.Drawing.Size(184, 256);
            this.lstTmHm.TabIndex = 1;
            // 
            // lstTutor
            // 
            this.lstTutor.FormattingEnabled = true;
            this.lstTutor.ItemHeight = 12;
            this.lstTutor.Location = new System.Drawing.Point(220, 52);
            this.lstTutor.Margin = new System.Windows.Forms.Padding(0);
            this.lstTutor.Name = "lstTutor";
            this.lstTutor.Size = new System.Drawing.Size(184, 256);
            this.lstTutor.TabIndex = 1;
            // 
            // cmbMove1
            // 
            this.cmbMove1.FormattingEnabled = true;
            this.cmbMove1.Location = new System.Drawing.Point(20, 316);
            this.cmbMove1.Margin = new System.Windows.Forms.Padding(0);
            this.cmbMove1.Name = "cmbMove1";
            this.cmbMove1.Size = new System.Drawing.Size(184, 20);
            this.cmbMove1.TabIndex = 2;
            // 
            // cmbMove2
            // 
            this.cmbMove2.FormattingEnabled = true;
            this.cmbMove2.Location = new System.Drawing.Point(220, 316);
            this.cmbMove2.Margin = new System.Windows.Forms.Padding(0);
            this.cmbMove2.Name = "cmbMove2";
            this.cmbMove2.Size = new System.Drawing.Size(184, 20);
            this.cmbMove2.TabIndex = 2;
            // 
            // TmHmTutorEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 353);
            this.Controls.Add(this.cmbMove2);
            this.Controls.Add(this.cmbMove1);
            this.Controls.Add(this.lstTutor);
            this.Controls.Add(this.lstTmHm);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TmHmTutorEditor";
            this.Text = "技マシン/教え技";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ListBox lstTmHm;
        private System.Windows.Forms.ListBox lstTutor;
        private System.Windows.Forms.ComboBox cmbMove1;
        private System.Windows.Forms.ComboBox cmbMove2;
    }
}