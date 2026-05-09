
namespace PochiPochiEditorGabu._Move
{
    partial class EggMoveEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EggMoveEditor));
            this.lstEggMoves = new System.Windows.Forms.ListBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpPokemon = new System.Windows.Forms.GroupBox();
            this.picPokemon = new System.Windows.Forms.PictureBox();
            this.cmbPokemon = new System.Windows.Forms.ComboBox();
            this.btnPokemonRemove = new System.Windows.Forms.Button();
            this.btnPokemonReplace = new System.Windows.Forms.Button();
            this.btnPokemonInsert = new System.Windows.Forms.Button();
            this.grpMove = new System.Windows.Forms.GroupBox();
            this.cmbMove = new System.Windows.Forms.ComboBox();
            this.btnMoveRemove = new System.Windows.Forms.Button();
            this.btnMoveReplace = new System.Windows.Forms.Button();
            this.btnMoveInsert = new System.Windows.Forms.Button();
            this.lblNote1 = new System.Windows.Forms.Label();
            this.grpPokemon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPokemon)).BeginInit();
            this.grpMove.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstEggMoves
            // 
            this.lstEggMoves.FormattingEnabled = true;
            this.lstEggMoves.ItemHeight = 12;
            this.lstEggMoves.Location = new System.Drawing.Point(20, 18);
            this.lstEggMoves.Margin = new System.Windows.Forms.Padding(0);
            this.lstEggMoves.Name = "lstEggMoves";
            this.lstEggMoves.Size = new System.Drawing.Size(160, 376);
            this.lstEggMoves.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(200, 18);
            this.btnSave.Margin = new System.Windows.Forms.Padding(0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(96, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "変更を保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // grpPokemon
            // 
            this.grpPokemon.Controls.Add(this.picPokemon);
            this.grpPokemon.Controls.Add(this.cmbPokemon);
            this.grpPokemon.Controls.Add(this.btnPokemonRemove);
            this.grpPokemon.Controls.Add(this.btnPokemonReplace);
            this.grpPokemon.Controls.Add(this.btnPokemonInsert);
            this.grpPokemon.Location = new System.Drawing.Point(200, 54);
            this.grpPokemon.Margin = new System.Windows.Forms.Padding(0);
            this.grpPokemon.Name = "grpPokemon";
            this.grpPokemon.Padding = new System.Windows.Forms.Padding(0);
            this.grpPokemon.Size = new System.Drawing.Size(260, 148);
            this.grpPokemon.TabIndex = 2;
            this.grpPokemon.TabStop = false;
            this.grpPokemon.Text = "ポケモン";
            // 
            // picPokemon
            // 
            this.picPokemon.Location = new System.Drawing.Point(120, 60);
            this.picPokemon.Margin = new System.Windows.Forms.Padding(0);
            this.picPokemon.Name = "picPokemon";
            this.picPokemon.Size = new System.Drawing.Size(64, 64);
            this.picPokemon.TabIndex = 2;
            this.picPokemon.TabStop = false;
            // 
            // cmbPokemon
            // 
            this.cmbPokemon.FormattingEnabled = true;
            this.cmbPokemon.Location = new System.Drawing.Point(120, 24);
            this.cmbPokemon.Margin = new System.Windows.Forms.Padding(0);
            this.cmbPokemon.Name = "cmbPokemon";
            this.cmbPokemon.Size = new System.Drawing.Size(120, 20);
            this.cmbPokemon.TabIndex = 1;
            // 
            // btnPokemonRemove
            // 
            this.btnPokemonRemove.Location = new System.Drawing.Point(20, 108);
            this.btnPokemonRemove.Margin = new System.Windows.Forms.Padding(0);
            this.btnPokemonRemove.Name = "btnPokemonRemove";
            this.btnPokemonRemove.Size = new System.Drawing.Size(80, 23);
            this.btnPokemonRemove.TabIndex = 0;
            this.btnPokemonRemove.Text = "削除";
            this.btnPokemonRemove.UseVisualStyleBackColor = true;
            // 
            // btnPokemonReplace
            // 
            this.btnPokemonReplace.Location = new System.Drawing.Point(20, 80);
            this.btnPokemonReplace.Margin = new System.Windows.Forms.Padding(0);
            this.btnPokemonReplace.Name = "btnPokemonReplace";
            this.btnPokemonReplace.Size = new System.Drawing.Size(80, 23);
            this.btnPokemonReplace.TabIndex = 0;
            this.btnPokemonReplace.Text = "置換";
            this.btnPokemonReplace.UseVisualStyleBackColor = true;
            // 
            // btnPokemonInsert
            // 
            this.btnPokemonInsert.Location = new System.Drawing.Point(20, 52);
            this.btnPokemonInsert.Margin = new System.Windows.Forms.Padding(0);
            this.btnPokemonInsert.Name = "btnPokemonInsert";
            this.btnPokemonInsert.Size = new System.Drawing.Size(80, 23);
            this.btnPokemonInsert.TabIndex = 0;
            this.btnPokemonInsert.Text = "挿入";
            this.btnPokemonInsert.UseVisualStyleBackColor = true;
            // 
            // grpMove
            // 
            this.grpMove.Controls.Add(this.cmbMove);
            this.grpMove.Controls.Add(this.btnMoveRemove);
            this.grpMove.Controls.Add(this.btnMoveReplace);
            this.grpMove.Controls.Add(this.btnMoveInsert);
            this.grpMove.Location = new System.Drawing.Point(200, 212);
            this.grpMove.Margin = new System.Windows.Forms.Padding(0);
            this.grpMove.Name = "grpMove";
            this.grpMove.Padding = new System.Windows.Forms.Padding(0);
            this.grpMove.Size = new System.Drawing.Size(260, 148);
            this.grpMove.TabIndex = 3;
            this.grpMove.TabStop = false;
            this.grpMove.Text = "技";
            // 
            // cmbMove
            // 
            this.cmbMove.FormattingEnabled = true;
            this.cmbMove.Location = new System.Drawing.Point(120, 24);
            this.cmbMove.Margin = new System.Windows.Forms.Padding(0);
            this.cmbMove.Name = "cmbMove";
            this.cmbMove.Size = new System.Drawing.Size(120, 20);
            this.cmbMove.TabIndex = 1;
            // 
            // btnMoveRemove
            // 
            this.btnMoveRemove.Location = new System.Drawing.Point(20, 108);
            this.btnMoveRemove.Margin = new System.Windows.Forms.Padding(0);
            this.btnMoveRemove.Name = "btnMoveRemove";
            this.btnMoveRemove.Size = new System.Drawing.Size(80, 23);
            this.btnMoveRemove.TabIndex = 0;
            this.btnMoveRemove.Text = "削除";
            this.btnMoveRemove.UseVisualStyleBackColor = true;
            // 
            // btnMoveReplace
            // 
            this.btnMoveReplace.Location = new System.Drawing.Point(20, 80);
            this.btnMoveReplace.Margin = new System.Windows.Forms.Padding(0);
            this.btnMoveReplace.Name = "btnMoveReplace";
            this.btnMoveReplace.Size = new System.Drawing.Size(80, 23);
            this.btnMoveReplace.TabIndex = 0;
            this.btnMoveReplace.Text = "置換";
            this.btnMoveReplace.UseVisualStyleBackColor = true;
            // 
            // btnMoveInsert
            // 
            this.btnMoveInsert.Location = new System.Drawing.Point(20, 52);
            this.btnMoveInsert.Margin = new System.Windows.Forms.Padding(0);
            this.btnMoveInsert.Name = "btnMoveInsert";
            this.btnMoveInsert.Size = new System.Drawing.Size(80, 23);
            this.btnMoveInsert.TabIndex = 0;
            this.btnMoveInsert.Text = "挿入";
            this.btnMoveInsert.UseVisualStyleBackColor = true;
            // 
            // lblNote1
            // 
            this.lblNote1.AutoSize = true;
            this.lblNote1.Location = new System.Drawing.Point(200, 372);
            this.lblNote1.Margin = new System.Windows.Forms.Padding(0);
            this.lblNote1.Name = "lblNote1";
            this.lblNote1.Size = new System.Drawing.Size(211, 12);
            this.lblNote1.TabIndex = 4;
            this.lblNote1.Text = "※タマゴ技が増える場合は、要テーブル移動";
            // 
            // EggMoveEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 413);
            this.Controls.Add(this.lblNote1);
            this.Controls.Add(this.grpMove);
            this.Controls.Add(this.grpPokemon);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lstEggMoves);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EggMoveEditor";
            this.Text = "タマゴ技";
            this.grpPokemon.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPokemon)).EndInit();
            this.grpMove.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstEggMoves;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox grpPokemon;
        private System.Windows.Forms.PictureBox picPokemon;
        private System.Windows.Forms.ComboBox cmbPokemon;
        private System.Windows.Forms.Button btnPokemonRemove;
        private System.Windows.Forms.Button btnPokemonReplace;
        private System.Windows.Forms.Button btnPokemonInsert;
        private System.Windows.Forms.GroupBox grpMove;
        private System.Windows.Forms.ComboBox cmbMove;
        private System.Windows.Forms.Button btnMoveRemove;
        private System.Windows.Forms.Button btnMoveReplace;
        private System.Windows.Forms.Button btnMoveInsert;
        private System.Windows.Forms.Label lblNote1;
    }
}