
namespace PochiPochiEditorGabu._Trainer
{
    partial class TrainerSpriteEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrainerSpriteEditor));
            this.btnSave = new System.Windows.Forms.Button();
            this.picSprite = new System.Windows.Forms.PictureBox();
            this.nudSprite = new System.Windows.Forms.NumericUpDown();
            this.btnSpritePrev = new System.Windows.Forms.Button();
            this.btnSpriteNext = new System.Windows.Forms.Button();
            this.txtSpriteImgAddr = new System.Windows.Forms.TextBox();
            this.txtSpritePalAddr = new System.Windows.Forms.TextBox();
            this.rbSpriteImgAddr = new System.Windows.Forms.RadioButton();
            this.rbSpritePalAddr = new System.Windows.Forms.RadioButton();
            this.lblSpriteYOffset = new System.Windows.Forms.Label();
            this.nudSpriteYOffset = new System.Windows.Forms.NumericUpDown();
            this.grpAnim = new System.Windows.Forms.GroupBox();
            this.txtAnimDataAddr = new System.Windows.Forms.TextBox();
            this.txtAnimPtrAddr = new System.Windows.Forms.TextBox();
            this.lblAnimDataAddr = new System.Windows.Forms.Label();
            this.lblAnimPtrAddr = new System.Windows.Forms.Label();
            this.grpImportExport = new System.Windows.Forms.GroupBox();
            this.btnSpriteExport = new System.Windows.Forms.Button();
            this.btnSpriteImport = new System.Windows.Forms.Button();
            this.txtSpriteImportAddr = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.picSprite)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSprite)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpriteYOffset)).BeginInit();
            this.grpAnim.SuspendLayout();
            this.grpImportExport.SuspendLayout();
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
            // picSprite
            // 
            this.picSprite.Location = new System.Drawing.Point(20, 48);
            this.picSprite.Margin = new System.Windows.Forms.Padding(0);
            this.picSprite.Name = "picSprite";
            this.picSprite.Size = new System.Drawing.Size(64, 64);
            this.picSprite.TabIndex = 1;
            this.picSprite.TabStop = false;
            // 
            // nudSprite
            // 
            this.nudSprite.Location = new System.Drawing.Point(20, 120);
            this.nudSprite.Margin = new System.Windows.Forms.Padding(0);
            this.nudSprite.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudSprite.Name = "nudSprite";
            this.nudSprite.Size = new System.Drawing.Size(64, 19);
            this.nudSprite.TabIndex = 2;
            // 
            // btnSpritePrev
            // 
            this.btnSpritePrev.Location = new System.Drawing.Point(20, 144);
            this.btnSpritePrev.Margin = new System.Windows.Forms.Padding(0);
            this.btnSpritePrev.Name = "btnSpritePrev";
            this.btnSpritePrev.Size = new System.Drawing.Size(30, 23);
            this.btnSpritePrev.TabIndex = 3;
            this.btnSpritePrev.Text = "<";
            this.btnSpritePrev.UseVisualStyleBackColor = true;
            // 
            // btnSpriteNext
            // 
            this.btnSpriteNext.Location = new System.Drawing.Point(54, 144);
            this.btnSpriteNext.Margin = new System.Windows.Forms.Padding(0);
            this.btnSpriteNext.Name = "btnSpriteNext";
            this.btnSpriteNext.Size = new System.Drawing.Size(30, 23);
            this.btnSpriteNext.TabIndex = 3;
            this.btnSpriteNext.Text = ">";
            this.btnSpriteNext.UseVisualStyleBackColor = true;
            // 
            // txtSpriteImgAddr
            // 
            this.txtSpriteImgAddr.Location = new System.Drawing.Point(210, 46);
            this.txtSpriteImgAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtSpriteImgAddr.Name = "txtSpriteImgAddr";
            this.txtSpriteImgAddr.Size = new System.Drawing.Size(80, 19);
            this.txtSpriteImgAddr.TabIndex = 5;
            // 
            // txtSpritePalAddr
            // 
            this.txtSpritePalAddr.Location = new System.Drawing.Point(210, 70);
            this.txtSpritePalAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtSpritePalAddr.Name = "txtSpritePalAddr";
            this.txtSpritePalAddr.Size = new System.Drawing.Size(80, 19);
            this.txtSpritePalAddr.TabIndex = 5;
            // 
            // rbSpriteImgAddr
            // 
            this.rbSpriteImgAddr.AutoSize = true;
            this.rbSpriteImgAddr.Checked = true;
            this.rbSpriteImgAddr.Location = new System.Drawing.Point(104, 48);
            this.rbSpriteImgAddr.Margin = new System.Windows.Forms.Padding(0);
            this.rbSpriteImgAddr.Name = "rbSpriteImgAddr";
            this.rbSpriteImgAddr.Size = new System.Drawing.Size(89, 16);
            this.rbSpriteImgAddr.TabIndex = 6;
            this.rbSpriteImgAddr.TabStop = true;
            this.rbSpriteImgAddr.Text = "画像アドレス :";
            this.rbSpriteImgAddr.UseVisualStyleBackColor = true;
            // 
            // rbSpritePalAddr
            // 
            this.rbSpritePalAddr.AutoSize = true;
            this.rbSpritePalAddr.Location = new System.Drawing.Point(104, 72);
            this.rbSpritePalAddr.Margin = new System.Windows.Forms.Padding(0);
            this.rbSpritePalAddr.Name = "rbSpritePalAddr";
            this.rbSpritePalAddr.Size = new System.Drawing.Size(99, 16);
            this.rbSpritePalAddr.TabIndex = 6;
            this.rbSpritePalAddr.Text = "パレットアドレス :";
            this.rbSpritePalAddr.UseVisualStyleBackColor = true;
            // 
            // lblSpriteYOffset
            // 
            this.lblSpriteYOffset.AutoSize = true;
            this.lblSpriteYOffset.Location = new System.Drawing.Point(120, 98);
            this.lblSpriteYOffset.Margin = new System.Windows.Forms.Padding(0);
            this.lblSpriteYOffset.Name = "lblSpriteYOffset";
            this.lblSpriteYOffset.Size = new System.Drawing.Size(42, 12);
            this.lblSpriteYOffset.TabIndex = 7;
            this.lblSpriteYOffset.Text = "Y座標 :";
            // 
            // nudSpriteYOffset
            // 
            this.nudSpriteYOffset.Location = new System.Drawing.Point(210, 94);
            this.nudSpriteYOffset.Margin = new System.Windows.Forms.Padding(0);
            this.nudSpriteYOffset.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudSpriteYOffset.Name = "nudSpriteYOffset";
            this.nudSpriteYOffset.Size = new System.Drawing.Size(80, 19);
            this.nudSpriteYOffset.TabIndex = 8;
            // 
            // grpAnim
            // 
            this.grpAnim.Controls.Add(this.txtAnimDataAddr);
            this.grpAnim.Controls.Add(this.txtAnimPtrAddr);
            this.grpAnim.Controls.Add(this.lblAnimDataAddr);
            this.grpAnim.Controls.Add(this.lblAnimPtrAddr);
            this.grpAnim.Location = new System.Drawing.Point(104, 120);
            this.grpAnim.Margin = new System.Windows.Forms.Padding(0);
            this.grpAnim.Name = "grpAnim";
            this.grpAnim.Padding = new System.Windows.Forms.Padding(0);
            this.grpAnim.Size = new System.Drawing.Size(186, 84);
            this.grpAnim.TabIndex = 9;
            this.grpAnim.TabStop = false;
            this.grpAnim.Text = "アニメーション";
            // 
            // txtAnimDataAddr
            // 
            this.txtAnimDataAddr.Location = new System.Drawing.Point(80, 48);
            this.txtAnimDataAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtAnimDataAddr.Name = "txtAnimDataAddr";
            this.txtAnimDataAddr.Size = new System.Drawing.Size(80, 19);
            this.txtAnimDataAddr.TabIndex = 1;
            // 
            // txtAnimPtrAddr
            // 
            this.txtAnimPtrAddr.Location = new System.Drawing.Point(80, 24);
            this.txtAnimPtrAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtAnimPtrAddr.Name = "txtAnimPtrAddr";
            this.txtAnimPtrAddr.Size = new System.Drawing.Size(80, 19);
            this.txtAnimPtrAddr.TabIndex = 1;
            // 
            // lblAnimDataAddr
            // 
            this.lblAnimDataAddr.AutoSize = true;
            this.lblAnimDataAddr.Location = new System.Drawing.Point(20, 52);
            this.lblAnimDataAddr.Margin = new System.Windows.Forms.Padding(0);
            this.lblAnimDataAddr.Name = "lblAnimDataAddr";
            this.lblAnimDataAddr.Size = new System.Drawing.Size(39, 12);
            this.lblAnimDataAddr.TabIndex = 0;
            this.lblAnimDataAddr.Text = "データ :";
            // 
            // lblAnimPtrAddr
            // 
            this.lblAnimPtrAddr.AutoSize = true;
            this.lblAnimPtrAddr.Location = new System.Drawing.Point(20, 28);
            this.lblAnimPtrAddr.Margin = new System.Windows.Forms.Padding(0);
            this.lblAnimPtrAddr.Name = "lblAnimPtrAddr";
            this.lblAnimPtrAddr.Size = new System.Drawing.Size(47, 12);
            this.lblAnimPtrAddr.TabIndex = 0;
            this.lblAnimPtrAddr.Text = "ポインタ :";
            // 
            // grpImportExport
            // 
            this.grpImportExport.Controls.Add(this.btnSpriteExport);
            this.grpImportExport.Controls.Add(this.btnSpriteImport);
            this.grpImportExport.Controls.Add(this.txtSpriteImportAddr);
            this.grpImportExport.Location = new System.Drawing.Point(104, 212);
            this.grpImportExport.Margin = new System.Windows.Forms.Padding(0);
            this.grpImportExport.Name = "grpImportExport";
            this.grpImportExport.Padding = new System.Windows.Forms.Padding(0);
            this.grpImportExport.Size = new System.Drawing.Size(210, 96);
            this.grpImportExport.TabIndex = 10;
            this.grpImportExport.TabStop = false;
            this.grpImportExport.Text = "インポート/エクスポート";
            // 
            // btnSpriteExport
            // 
            this.btnSpriteExport.Location = new System.Drawing.Point(110, 54);
            this.btnSpriteExport.Margin = new System.Windows.Forms.Padding(0);
            this.btnSpriteExport.Name = "btnSpriteExport";
            this.btnSpriteExport.Size = new System.Drawing.Size(80, 23);
            this.btnSpriteExport.TabIndex = 1;
            this.btnSpriteExport.Text = "エクスポート";
            this.btnSpriteExport.UseVisualStyleBackColor = true;
            // 
            // btnSpriteImport
            // 
            this.btnSpriteImport.Location = new System.Drawing.Point(110, 26);
            this.btnSpriteImport.Margin = new System.Windows.Forms.Padding(0);
            this.btnSpriteImport.Name = "btnSpriteImport";
            this.btnSpriteImport.Size = new System.Drawing.Size(80, 23);
            this.btnSpriteImport.TabIndex = 1;
            this.btnSpriteImport.Text = "インポート";
            this.btnSpriteImport.UseVisualStyleBackColor = true;
            // 
            // txtSpriteImportAddr
            // 
            this.txtSpriteImportAddr.Location = new System.Drawing.Point(20, 28);
            this.txtSpriteImportAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtSpriteImportAddr.Name = "txtSpriteImportAddr";
            this.txtSpriteImportAddr.Size = new System.Drawing.Size(80, 19);
            this.txtSpriteImportAddr.TabIndex = 0;
            // 
            // TrainerSpriteEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 327);
            this.Controls.Add(this.grpImportExport);
            this.Controls.Add(this.grpAnim);
            this.Controls.Add(this.nudSpriteYOffset);
            this.Controls.Add(this.lblSpriteYOffset);
            this.Controls.Add(this.rbSpritePalAddr);
            this.Controls.Add(this.rbSpriteImgAddr);
            this.Controls.Add(this.txtSpritePalAddr);
            this.Controls.Add(this.txtSpriteImgAddr);
            this.Controls.Add(this.btnSpriteNext);
            this.Controls.Add(this.btnSpritePrev);
            this.Controls.Add(this.nudSprite);
            this.Controls.Add(this.picSprite);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TrainerSpriteEditor";
            this.Text = "トレーナー画像";
            ((System.ComponentModel.ISupportInitialize)(this.picSprite)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSprite)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpriteYOffset)).EndInit();
            this.grpAnim.ResumeLayout(false);
            this.grpAnim.PerformLayout();
            this.grpImportExport.ResumeLayout(false);
            this.grpImportExport.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.PictureBox picSprite;
        private System.Windows.Forms.NumericUpDown nudSprite;
        private System.Windows.Forms.Button btnSpritePrev;
        private System.Windows.Forms.Button btnSpriteNext;
        private System.Windows.Forms.TextBox txtSpriteImgAddr;
        private System.Windows.Forms.TextBox txtSpritePalAddr;
        private System.Windows.Forms.RadioButton rbSpriteImgAddr;
        private System.Windows.Forms.RadioButton rbSpritePalAddr;
        private System.Windows.Forms.Label lblSpriteYOffset;
        private System.Windows.Forms.NumericUpDown nudSpriteYOffset;
        private System.Windows.Forms.GroupBox grpAnim;
        private System.Windows.Forms.Label lblAnimPtrAddr;
        private System.Windows.Forms.TextBox txtAnimDataAddr;
        private System.Windows.Forms.TextBox txtAnimPtrAddr;
        private System.Windows.Forms.Label lblAnimDataAddr;
        private System.Windows.Forms.GroupBox grpImportExport;
        private System.Windows.Forms.Button btnSpriteExport;
        private System.Windows.Forms.Button btnSpriteImport;
        private System.Windows.Forms.TextBox txtSpriteImportAddr;
    }
}