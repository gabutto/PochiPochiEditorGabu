
namespace PochiPochiEditorGabu
{
    partial class QuickInputPopup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuickInputPopup));
            this.lblInputAddress = new System.Windows.Forms.Label();
            this.txtInputAddress = new System.Windows.Forms.TextBox();
            this.cmbInputDataType = new System.Windows.Forms.ComboBox();
            this.lblInputDataType = new System.Windows.Forms.Label();
            this.nudEntryCount = new System.Windows.Forms.NumericUpDown();
            this.lblEntryCount = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudEntryCount)).BeginInit();
            this.SuspendLayout();
            // 
            // lblInputAddress
            // 
            this.lblInputAddress.AutoSize = true;
            this.lblInputAddress.Location = new System.Drawing.Point(20, 20);
            this.lblInputAddress.Margin = new System.Windows.Forms.Padding(0);
            this.lblInputAddress.Name = "lblInputAddress";
            this.lblInputAddress.Size = new System.Drawing.Size(83, 12);
            this.lblInputAddress.TabIndex = 0;
            this.lblInputAddress.Text = "生成先アドレス :";
            // 
            // txtInputAddress
            // 
            this.txtInputAddress.Location = new System.Drawing.Point(120, 16);
            this.txtInputAddress.Margin = new System.Windows.Forms.Padding(0);
            this.txtInputAddress.Name = "txtInputAddress";
            this.txtInputAddress.Size = new System.Drawing.Size(80, 19);
            this.txtInputAddress.TabIndex = 1;
            // 
            // cmbInputDataType
            // 
            this.cmbInputDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInputDataType.FormattingEnabled = true;
            this.cmbInputDataType.Location = new System.Drawing.Point(120, 40);
            this.cmbInputDataType.Margin = new System.Windows.Forms.Padding(0);
            this.cmbInputDataType.Name = "cmbInputDataType";
            this.cmbInputDataType.Size = new System.Drawing.Size(120, 20);
            this.cmbInputDataType.TabIndex = 2;
            // 
            // lblInputDataType
            // 
            this.lblInputDataType.AutoSize = true;
            this.lblInputDataType.Location = new System.Drawing.Point(20, 44);
            this.lblInputDataType.Margin = new System.Windows.Forms.Padding(0);
            this.lblInputDataType.Name = "lblInputDataType";
            this.lblInputDataType.Size = new System.Drawing.Size(65, 12);
            this.lblInputDataType.TabIndex = 0;
            this.lblInputDataType.Text = "データタイプ :";
            // 
            // nudEntryCount
            // 
            this.nudEntryCount.Location = new System.Drawing.Point(120, 64);
            this.nudEntryCount.Margin = new System.Windows.Forms.Padding(0);
            this.nudEntryCount.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudEntryCount.Name = "nudEntryCount";
            this.nudEntryCount.Size = new System.Drawing.Size(56, 19);
            this.nudEntryCount.TabIndex = 3;
            // 
            // lblEntryCount
            // 
            this.lblEntryCount.AutoSize = true;
            this.lblEntryCount.Location = new System.Drawing.Point(20, 68);
            this.lblEntryCount.Margin = new System.Windows.Forms.Padding(0);
            this.lblEntryCount.Name = "lblEntryCount";
            this.lblEntryCount.Size = new System.Drawing.Size(66, 12);
            this.lblEntryCount.TabIndex = 0;
            this.lblEntryCount.Text = "エントリー数 :";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(244, 62);
            this.btnOK.Margin = new System.Windows.Forms.Padding(0);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "生成";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // QuickInputPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 101);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.nudEntryCount);
            this.Controls.Add(this.cmbInputDataType);
            this.Controls.Add(this.txtInputAddress);
            this.Controls.Add(this.lblEntryCount);
            this.Controls.Add(this.lblInputDataType);
            this.Controls.Add(this.lblInputAddress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "QuickInputPopup";
            this.Text = "新しいデータを作成";
            ((System.ComponentModel.ISupportInitialize)(this.nudEntryCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInputAddress;
        private System.Windows.Forms.TextBox txtInputAddress;
        private System.Windows.Forms.ComboBox cmbInputDataType;
        private System.Windows.Forms.Label lblInputDataType;
        private System.Windows.Forms.NumericUpDown nudEntryCount;
        private System.Windows.Forms.Label lblEntryCount;
        private System.Windows.Forms.Button btnOK;
    }
}