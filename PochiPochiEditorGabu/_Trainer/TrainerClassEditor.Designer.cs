
namespace PochiPochiEditorGabu._Trainer
{
    partial class TrainerClassEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrainerClassEditor));
            this.btnSave = new System.Windows.Forms.Button();
            this.nudClassName = new System.Windows.Forms.NumericUpDown();
            this.cmbClassName = new System.Windows.Forms.ComboBox();
            this.grpClassData = new System.Windows.Forms.GroupBox();
            this.nudPrizeMulti = new System.Windows.Forms.NumericUpDown();
            this.txtClassName = new System.Windows.Forms.TextBox();
            this.lblPrizeMulti = new System.Windows.Forms.Label();
            this.lblClassName = new System.Windows.Forms.Label();
            this.grpClassDataExtra = new System.Windows.Forms.GroupBox();
            this.nudBaseIv = new System.Windows.Forms.NumericUpDown();
            this.nudPokeBallIndex = new System.Windows.Forms.NumericUpDown();
            this.nudBattleMusicIndex = new System.Windows.Forms.NumericUpDown();
            this.nudEncounterMusicIndex = new System.Windows.Forms.NumericUpDown();
            this.lblBaseIv = new System.Windows.Forms.Label();
            this.lblPokeBall = new System.Windows.Forms.Label();
            this.lblBattleMusic = new System.Windows.Forms.Label();
            this.lblEncounterMusic = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudClassName)).BeginInit();
            this.grpClassData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrizeMulti)).BeginInit();
            this.grpClassDataExtra.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBaseIv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPokeBallIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBattleMusicIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEncounterMusicIndex)).BeginInit();
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
            // nudClassName
            // 
            this.nudClassName.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudClassName.Location = new System.Drawing.Point(20, 48);
            this.nudClassName.Margin = new System.Windows.Forms.Padding(0);
            this.nudClassName.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudClassName.Name = "nudClassName";
            this.nudClassName.ReadOnly = true;
            this.nudClassName.Size = new System.Drawing.Size(56, 19);
            this.nudClassName.TabIndex = 1;
            // 
            // cmbClassName
            // 
            this.cmbClassName.FormattingEnabled = true;
            this.cmbClassName.Location = new System.Drawing.Point(88, 47);
            this.cmbClassName.Margin = new System.Windows.Forms.Padding(0);
            this.cmbClassName.Name = "cmbClassName";
            this.cmbClassName.Size = new System.Drawing.Size(144, 20);
            this.cmbClassName.TabIndex = 2;
            // 
            // grpClassData
            // 
            this.grpClassData.Controls.Add(this.nudPrizeMulti);
            this.grpClassData.Controls.Add(this.txtClassName);
            this.grpClassData.Controls.Add(this.lblPrizeMulti);
            this.grpClassData.Controls.Add(this.lblClassName);
            this.grpClassData.Location = new System.Drawing.Point(20, 76);
            this.grpClassData.Margin = new System.Windows.Forms.Padding(0);
            this.grpClassData.Name = "grpClassData";
            this.grpClassData.Padding = new System.Windows.Forms.Padding(0);
            this.grpClassData.Size = new System.Drawing.Size(258, 84);
            this.grpClassData.TabIndex = 3;
            this.grpClassData.TabStop = false;
            this.grpClassData.Text = "肩書きデータ";
            // 
            // nudPrizeMulti
            // 
            this.nudPrizeMulti.Location = new System.Drawing.Point(92, 48);
            this.nudPrizeMulti.Margin = new System.Windows.Forms.Padding(0);
            this.nudPrizeMulti.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudPrizeMulti.Name = "nudPrizeMulti";
            this.nudPrizeMulti.Size = new System.Drawing.Size(56, 19);
            this.nudPrizeMulti.TabIndex = 2;
            // 
            // txtClassName
            // 
            this.txtClassName.Location = new System.Drawing.Point(92, 24);
            this.txtClassName.Margin = new System.Windows.Forms.Padding(0);
            this.txtClassName.Name = "txtClassName";
            this.txtClassName.Size = new System.Drawing.Size(144, 19);
            this.txtClassName.TabIndex = 1;
            // 
            // lblPrizeMulti
            // 
            this.lblPrizeMulti.AutoSize = true;
            this.lblPrizeMulti.Location = new System.Drawing.Point(20, 52);
            this.lblPrizeMulti.Margin = new System.Windows.Forms.Padding(0);
            this.lblPrizeMulti.Name = "lblPrizeMulti";
            this.lblPrizeMulti.Size = new System.Drawing.Size(59, 12);
            this.lblPrizeMulti.TabIndex = 0;
            this.lblPrizeMulti.Text = "賞金倍率 :";
            // 
            // lblClassName
            // 
            this.lblClassName.AutoSize = true;
            this.lblClassName.Location = new System.Drawing.Point(20, 28);
            this.lblClassName.Margin = new System.Windows.Forms.Padding(0);
            this.lblClassName.Name = "lblClassName";
            this.lblClassName.Size = new System.Drawing.Size(56, 12);
            this.lblClassName.TabIndex = 0;
            this.lblClassName.Text = "肩書き名 :";
            // 
            // grpClassDataExtra
            // 
            this.grpClassDataExtra.Controls.Add(this.nudBaseIv);
            this.grpClassDataExtra.Controls.Add(this.nudPokeBallIndex);
            this.grpClassDataExtra.Controls.Add(this.nudBattleMusicIndex);
            this.grpClassDataExtra.Controls.Add(this.nudEncounterMusicIndex);
            this.grpClassDataExtra.Controls.Add(this.lblBaseIv);
            this.grpClassDataExtra.Controls.Add(this.lblPokeBall);
            this.grpClassDataExtra.Controls.Add(this.lblBattleMusic);
            this.grpClassDataExtra.Controls.Add(this.lblEncounterMusic);
            this.grpClassDataExtra.Location = new System.Drawing.Point(20, 168);
            this.grpClassDataExtra.Margin = new System.Windows.Forms.Padding(0);
            this.grpClassDataExtra.Name = "grpClassDataExtra";
            this.grpClassDataExtra.Padding = new System.Windows.Forms.Padding(0);
            this.grpClassDataExtra.Size = new System.Drawing.Size(200, 134);
            this.grpClassDataExtra.TabIndex = 4;
            this.grpClassDataExtra.TabStop = false;
            this.grpClassDataExtra.Text = "追加データ";
            // 
            // nudBaseIv
            // 
            this.nudBaseIv.Location = new System.Drawing.Point(104, 96);
            this.nudBaseIv.Margin = new System.Windows.Forms.Padding(0);
            this.nudBaseIv.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBaseIv.Name = "nudBaseIv";
            this.nudBaseIv.Size = new System.Drawing.Size(56, 19);
            this.nudBaseIv.TabIndex = 1;
            // 
            // nudPokeBallIndex
            // 
            this.nudPokeBallIndex.Location = new System.Drawing.Point(104, 72);
            this.nudPokeBallIndex.Margin = new System.Windows.Forms.Padding(0);
            this.nudPokeBallIndex.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudPokeBallIndex.Name = "nudPokeBallIndex";
            this.nudPokeBallIndex.Size = new System.Drawing.Size(56, 19);
            this.nudPokeBallIndex.TabIndex = 1;
            // 
            // nudBattleMusicIndex
            // 
            this.nudBattleMusicIndex.Location = new System.Drawing.Point(104, 48);
            this.nudBattleMusicIndex.Margin = new System.Windows.Forms.Padding(0);
            this.nudBattleMusicIndex.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudBattleMusicIndex.Name = "nudBattleMusicIndex";
            this.nudBattleMusicIndex.Size = new System.Drawing.Size(72, 19);
            this.nudBattleMusicIndex.TabIndex = 1;
            // 
            // nudEncounterMusicIndex
            // 
            this.nudEncounterMusicIndex.Location = new System.Drawing.Point(104, 24);
            this.nudEncounterMusicIndex.Margin = new System.Windows.Forms.Padding(0);
            this.nudEncounterMusicIndex.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudEncounterMusicIndex.Name = "nudEncounterMusicIndex";
            this.nudEncounterMusicIndex.Size = new System.Drawing.Size(72, 19);
            this.nudEncounterMusicIndex.TabIndex = 1;
            // 
            // lblBaseIv
            // 
            this.lblBaseIv.AutoSize = true;
            this.lblBaseIv.Location = new System.Drawing.Point(20, 100);
            this.lblBaseIv.Margin = new System.Windows.Forms.Padding(0);
            this.lblBaseIv.Name = "lblBaseIv";
            this.lblBaseIv.Size = new System.Drawing.Size(71, 12);
            this.lblBaseIv.TabIndex = 0;
            this.lblBaseIv.Text = "基礎個体値 :";
            // 
            // lblPokeBall
            // 
            this.lblPokeBall.AutoSize = true;
            this.lblPokeBall.Location = new System.Drawing.Point(20, 76);
            this.lblPokeBall.Margin = new System.Windows.Forms.Padding(0);
            this.lblPokeBall.Name = "lblPokeBall";
            this.lblPokeBall.Size = new System.Drawing.Size(76, 12);
            this.lblPokeBall.TabIndex = 0;
            this.lblPokeBall.Text = "使用ボールID :";
            // 
            // lblBattleMusic
            // 
            this.lblBattleMusic.AutoSize = true;
            this.lblBattleMusic.Location = new System.Drawing.Point(20, 52);
            this.lblBattleMusic.Margin = new System.Windows.Forms.Padding(0);
            this.lblBattleMusic.Name = "lblBattleMusic";
            this.lblBattleMusic.Size = new System.Drawing.Size(72, 12);
            this.lblBattleMusic.TabIndex = 0;
            this.lblBattleMusic.Text = "戦闘中BGM :";
            // 
            // lblEncounterMusic
            // 
            this.lblEncounterMusic.AutoSize = true;
            this.lblEncounterMusic.Location = new System.Drawing.Point(20, 28);
            this.lblEncounterMusic.Margin = new System.Windows.Forms.Padding(0);
            this.lblEncounterMusic.Name = "lblEncounterMusic";
            this.lblEncounterMusic.Size = new System.Drawing.Size(72, 12);
            this.lblEncounterMusic.TabIndex = 0;
            this.lblEncounterMusic.Text = "戦闘前BGM :";
            // 
            // TrainerClassEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 319);
            this.Controls.Add(this.grpClassDataExtra);
            this.Controls.Add(this.grpClassData);
            this.Controls.Add(this.cmbClassName);
            this.Controls.Add(this.nudClassName);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TrainerClassEditor";
            this.Text = "トレーナー肩書き";
            ((System.ComponentModel.ISupportInitialize)(this.nudClassName)).EndInit();
            this.grpClassData.ResumeLayout(false);
            this.grpClassData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrizeMulti)).EndInit();
            this.grpClassDataExtra.ResumeLayout(false);
            this.grpClassDataExtra.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBaseIv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPokeBallIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBattleMusicIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEncounterMusicIndex)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.NumericUpDown nudClassName;
        private System.Windows.Forms.ComboBox cmbClassName;
        private System.Windows.Forms.GroupBox grpClassData;
        private System.Windows.Forms.NumericUpDown nudPrizeMulti;
        private System.Windows.Forms.TextBox txtClassName;
        private System.Windows.Forms.Label lblPrizeMulti;
        private System.Windows.Forms.Label lblClassName;
        private System.Windows.Forms.GroupBox grpClassDataExtra;
        private System.Windows.Forms.NumericUpDown nudBaseIv;
        private System.Windows.Forms.NumericUpDown nudPokeBallIndex;
        private System.Windows.Forms.NumericUpDown nudBattleMusicIndex;
        private System.Windows.Forms.NumericUpDown nudEncounterMusicIndex;
        private System.Windows.Forms.Label lblBaseIv;
        private System.Windows.Forms.Label lblPokeBall;
        private System.Windows.Forms.Label lblBattleMusic;
        private System.Windows.Forms.Label lblEncounterMusic;
    }
}