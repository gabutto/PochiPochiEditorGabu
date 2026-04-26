
namespace PochiPochiEditorGabu
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnLoadRom = new System.Windows.Forms.Button();
            this.btnSaveRom = new System.Windows.Forms.Button();
            this.lblConfig = new System.Windows.Forms.Label();
            this.cmbConfig = new System.Windows.Forms.ComboBox();
            this.btnUnloadRom = new System.Windows.Forms.Button();
            this.grpSelectEditor = new System.Windows.Forms.GroupBox();
            this.btnPokemon = new System.Windows.Forms.Button();
            this.btnItem = new System.Windows.Forms.Button();
            this.btnMap = new System.Windows.Forms.Button();
            this.btnPokedexOrder = new System.Windows.Forms.Button();
            this.btnTrainerClass = new System.Windows.Forms.Button();
            this.btnTileset = new System.Windows.Forms.Button();
            this.btnPokedexHabitat = new System.Windows.Forms.Button();
            this.btnPokedexSearch = new System.Windows.Forms.Button();
            this.btnTrainerSprite = new System.Windows.Forms.Button();
            this.btnTrainerList = new System.Windows.Forms.Button();
            this.btnOverworldSprite = new System.Windows.Forms.Button();
            this.btnWildEncounter = new System.Windows.Forms.Button();
            this.btnTmHmTutor = new System.Windows.Forms.Button();
            this.btnEggMove = new System.Windows.Forms.Button();
            this.btnIngameTrade = new System.Windows.Forms.Button();
            this.btnMail = new System.Windows.Forms.Button();
            this.btnBattleBackground = new System.Windows.Forms.Button();
            this.btnTownMap = new System.Windows.Forms.Button();
            this.btnExpandTable = new System.Windows.Forms.Button();
            this.btnApplyPatch = new System.Windows.Forms.Button();
            this.grpFreeSpaceFinder = new System.Windows.Forms.GroupBox();
            this.lblFsfStartAddr = new System.Windows.Forms.Label();
            this.txtFsfStartAddr = new System.Windows.Forms.TextBox();
            this.lblFsfByteAmount = new System.Windows.Forms.Label();
            this.nudFsfByteAmount = new System.Windows.Forms.NumericUpDown();
            this.btnFsfSearch = new System.Windows.Forms.Button();
            this.lblFsfResultAddr = new System.Windows.Forms.Label();
            this.txtFsfResultAddr = new System.Windows.Forms.TextBox();
            this.picFormPokemon = new System.Windows.Forms.PictureBox();
            this.grpSelectEditor.SuspendLayout();
            this.grpFreeSpaceFinder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFsfByteAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFormPokemon)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoadRom
            // 
            this.btnLoadRom.Location = new System.Drawing.Point(20, 16);
            this.btnLoadRom.Margin = new System.Windows.Forms.Padding(0);
            this.btnLoadRom.Name = "btnLoadRom";
            this.btnLoadRom.Size = new System.Drawing.Size(96, 31);
            this.btnLoadRom.TabIndex = 0;
            this.btnLoadRom.Text = "ROMを選択";
            this.btnLoadRom.UseVisualStyleBackColor = true;
            // 
            // btnSaveRom
            // 
            this.btnSaveRom.Location = new System.Drawing.Point(128, 16);
            this.btnSaveRom.Margin = new System.Windows.Forms.Padding(0);
            this.btnSaveRom.Name = "btnSaveRom";
            this.btnSaveRom.Size = new System.Drawing.Size(96, 31);
            this.btnSaveRom.TabIndex = 0;
            this.btnSaveRom.Text = "ROMを保存";
            this.btnSaveRom.UseVisualStyleBackColor = true;
            // 
            // lblConfig
            // 
            this.lblConfig.AutoSize = true;
            this.lblConfig.Location = new System.Drawing.Point(240, 20);
            this.lblConfig.Margin = new System.Windows.Forms.Padding(0);
            this.lblConfig.Name = "lblConfig";
            this.lblConfig.Size = new System.Drawing.Size(81, 12);
            this.lblConfig.TabIndex = 1;
            this.lblConfig.Text = "読み込み設定 :";
            // 
            // cmbConfig
            // 
            this.cmbConfig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConfig.FormattingEnabled = true;
            this.cmbConfig.Location = new System.Drawing.Point(328, 16);
            this.cmbConfig.Margin = new System.Windows.Forms.Padding(0);
            this.cmbConfig.Name = "cmbConfig";
            this.cmbConfig.Size = new System.Drawing.Size(96, 20);
            this.cmbConfig.TabIndex = 2;
            // 
            // btnUnloadRom
            // 
            this.btnUnloadRom.Location = new System.Drawing.Point(242, 42);
            this.btnUnloadRom.Margin = new System.Windows.Forms.Padding(0);
            this.btnUnloadRom.Name = "btnUnloadRom";
            this.btnUnloadRom.Size = new System.Drawing.Size(182, 23);
            this.btnUnloadRom.TabIndex = 3;
            this.btnUnloadRom.Text = "読み込んだROMをクリア";
            this.btnUnloadRom.UseVisualStyleBackColor = true;
            // 
            // grpSelectEditor
            // 
            this.grpSelectEditor.Controls.Add(this.btnTownMap);
            this.grpSelectEditor.Controls.Add(this.btnWildEncounter);
            this.grpSelectEditor.Controls.Add(this.btnTileset);
            this.grpSelectEditor.Controls.Add(this.btnBattleBackground);
            this.grpSelectEditor.Controls.Add(this.btnOverworldSprite);
            this.grpSelectEditor.Controls.Add(this.btnMap);
            this.grpSelectEditor.Controls.Add(this.btnApplyPatch);
            this.grpSelectEditor.Controls.Add(this.btnMail);
            this.grpSelectEditor.Controls.Add(this.btnTrainerList);
            this.grpSelectEditor.Controls.Add(this.btnTrainerClass);
            this.grpSelectEditor.Controls.Add(this.btnIngameTrade);
            this.grpSelectEditor.Controls.Add(this.btnTrainerSprite);
            this.grpSelectEditor.Controls.Add(this.btnItem);
            this.grpSelectEditor.Controls.Add(this.btnExpandTable);
            this.grpSelectEditor.Controls.Add(this.btnEggMove);
            this.grpSelectEditor.Controls.Add(this.btnPokedexSearch);
            this.grpSelectEditor.Controls.Add(this.btnPokedexOrder);
            this.grpSelectEditor.Controls.Add(this.btnTmHmTutor);
            this.grpSelectEditor.Controls.Add(this.btnPokedexHabitat);
            this.grpSelectEditor.Controls.Add(this.btnPokemon);
            this.grpSelectEditor.Location = new System.Drawing.Point(20, 72);
            this.grpSelectEditor.Margin = new System.Windows.Forms.Padding(0);
            this.grpSelectEditor.Name = "grpSelectEditor";
            this.grpSelectEditor.Size = new System.Drawing.Size(404, 252);
            this.grpSelectEditor.TabIndex = 4;
            this.grpSelectEditor.TabStop = false;
            this.grpSelectEditor.Text = "編集項目を選択";
            // 
            // btnPokemon
            // 
            this.btnPokemon.Location = new System.Drawing.Point(20, 28);
            this.btnPokemon.Margin = new System.Windows.Forms.Padding(0);
            this.btnPokemon.Name = "btnPokemon";
            this.btnPokemon.Size = new System.Drawing.Size(114, 23);
            this.btnPokemon.TabIndex = 0;
            this.btnPokemon.Text = "ポケモン";
            this.btnPokemon.UseVisualStyleBackColor = true;
            // 
            // btnItem
            // 
            this.btnItem.Location = new System.Drawing.Point(144, 28);
            this.btnItem.Margin = new System.Windows.Forms.Padding(0);
            this.btnItem.Name = "btnItem";
            this.btnItem.Size = new System.Drawing.Size(114, 23);
            this.btnItem.TabIndex = 0;
            this.btnItem.Text = "アイテム";
            this.btnItem.UseVisualStyleBackColor = true;
            // 
            // btnMap
            // 
            this.btnMap.Location = new System.Drawing.Point(268, 28);
            this.btnMap.Margin = new System.Windows.Forms.Padding(0);
            this.btnMap.Name = "btnMap";
            this.btnMap.Size = new System.Drawing.Size(114, 23);
            this.btnMap.TabIndex = 0;
            this.btnMap.Text = "マップ";
            this.btnMap.UseVisualStyleBackColor = true;
            // 
            // btnPokedexOrder
            // 
            this.btnPokedexOrder.Location = new System.Drawing.Point(20, 56);
            this.btnPokedexOrder.Margin = new System.Windows.Forms.Padding(0);
            this.btnPokedexOrder.Name = "btnPokedexOrder";
            this.btnPokedexOrder.Size = new System.Drawing.Size(114, 23);
            this.btnPokedexOrder.TabIndex = 0;
            this.btnPokedexOrder.Text = "図鑑番号";
            this.btnPokedexOrder.UseVisualStyleBackColor = true;
            // 
            // btnTrainerClass
            // 
            this.btnTrainerClass.Location = new System.Drawing.Point(144, 56);
            this.btnTrainerClass.Margin = new System.Windows.Forms.Padding(0);
            this.btnTrainerClass.Name = "btnTrainerClass";
            this.btnTrainerClass.Size = new System.Drawing.Size(114, 23);
            this.btnTrainerClass.TabIndex = 0;
            this.btnTrainerClass.Text = "トレーナー肩書き";
            this.btnTrainerClass.UseVisualStyleBackColor = true;
            // 
            // btnTileset
            // 
            this.btnTileset.Location = new System.Drawing.Point(268, 56);
            this.btnTileset.Margin = new System.Windows.Forms.Padding(0);
            this.btnTileset.Name = "btnTileset";
            this.btnTileset.Size = new System.Drawing.Size(114, 23);
            this.btnTileset.TabIndex = 0;
            this.btnTileset.Text = "タイルセット";
            this.btnTileset.UseVisualStyleBackColor = true;
            // 
            // btnPokedexHabitat
            // 
            this.btnPokedexHabitat.Location = new System.Drawing.Point(20, 84);
            this.btnPokedexHabitat.Margin = new System.Windows.Forms.Padding(0);
            this.btnPokedexHabitat.Name = "btnPokedexHabitat";
            this.btnPokedexHabitat.Size = new System.Drawing.Size(114, 23);
            this.btnPokedexHabitat.TabIndex = 0;
            this.btnPokedexHabitat.Text = "図鑑生息地";
            this.btnPokedexHabitat.UseVisualStyleBackColor = true;
            // 
            // btnPokedexSearch
            // 
            this.btnPokedexSearch.Location = new System.Drawing.Point(20, 112);
            this.btnPokedexSearch.Margin = new System.Windows.Forms.Padding(0);
            this.btnPokedexSearch.Name = "btnPokedexSearch";
            this.btnPokedexSearch.Size = new System.Drawing.Size(114, 23);
            this.btnPokedexSearch.TabIndex = 0;
            this.btnPokedexSearch.Text = "図鑑索引";
            this.btnPokedexSearch.UseVisualStyleBackColor = true;
            // 
            // btnTrainerSprite
            // 
            this.btnTrainerSprite.Location = new System.Drawing.Point(144, 84);
            this.btnTrainerSprite.Margin = new System.Windows.Forms.Padding(0);
            this.btnTrainerSprite.Name = "btnTrainerSprite";
            this.btnTrainerSprite.Size = new System.Drawing.Size(114, 23);
            this.btnTrainerSprite.TabIndex = 0;
            this.btnTrainerSprite.Text = "トレーナー画像";
            this.btnTrainerSprite.UseVisualStyleBackColor = true;
            // 
            // btnTrainerList
            // 
            this.btnTrainerList.Location = new System.Drawing.Point(144, 112);
            this.btnTrainerList.Margin = new System.Windows.Forms.Padding(0);
            this.btnTrainerList.Name = "btnTrainerList";
            this.btnTrainerList.Size = new System.Drawing.Size(114, 23);
            this.btnTrainerList.TabIndex = 0;
            this.btnTrainerList.Text = "トレーナーリスト";
            this.btnTrainerList.UseVisualStyleBackColor = true;
            // 
            // btnOverworldSprite
            // 
            this.btnOverworldSprite.Location = new System.Drawing.Point(268, 84);
            this.btnOverworldSprite.Margin = new System.Windows.Forms.Padding(0);
            this.btnOverworldSprite.Name = "btnOverworldSprite";
            this.btnOverworldSprite.Size = new System.Drawing.Size(114, 23);
            this.btnOverworldSprite.TabIndex = 0;
            this.btnOverworldSprite.Text = "歩行グラフィック";
            this.btnOverworldSprite.UseVisualStyleBackColor = true;
            // 
            // btnWildEncounter
            // 
            this.btnWildEncounter.Location = new System.Drawing.Point(268, 112);
            this.btnWildEncounter.Margin = new System.Windows.Forms.Padding(0);
            this.btnWildEncounter.Name = "btnWildEncounter";
            this.btnWildEncounter.Size = new System.Drawing.Size(114, 23);
            this.btnWildEncounter.TabIndex = 0;
            this.btnWildEncounter.Text = "野生設定";
            this.btnWildEncounter.UseVisualStyleBackColor = true;
            // 
            // btnTmHmTutor
            // 
            this.btnTmHmTutor.Location = new System.Drawing.Point(20, 140);
            this.btnTmHmTutor.Margin = new System.Windows.Forms.Padding(0);
            this.btnTmHmTutor.Name = "btnTmHmTutor";
            this.btnTmHmTutor.Size = new System.Drawing.Size(114, 23);
            this.btnTmHmTutor.TabIndex = 0;
            this.btnTmHmTutor.Text = "技マシン/教え技";
            this.btnTmHmTutor.UseVisualStyleBackColor = true;
            // 
            // btnEggMove
            // 
            this.btnEggMove.Location = new System.Drawing.Point(20, 168);
            this.btnEggMove.Margin = new System.Windows.Forms.Padding(0);
            this.btnEggMove.Name = "btnEggMove";
            this.btnEggMove.Size = new System.Drawing.Size(114, 23);
            this.btnEggMove.TabIndex = 0;
            this.btnEggMove.Text = "タマゴ技";
            this.btnEggMove.UseVisualStyleBackColor = true;
            // 
            // btnIngameTrade
            // 
            this.btnIngameTrade.Location = new System.Drawing.Point(144, 140);
            this.btnIngameTrade.Margin = new System.Windows.Forms.Padding(0);
            this.btnIngameTrade.Name = "btnIngameTrade";
            this.btnIngameTrade.Size = new System.Drawing.Size(114, 23);
            this.btnIngameTrade.TabIndex = 0;
            this.btnIngameTrade.Text = "ゲーム内交換";
            this.btnIngameTrade.UseVisualStyleBackColor = true;
            // 
            // btnMail
            // 
            this.btnMail.Location = new System.Drawing.Point(144, 168);
            this.btnMail.Margin = new System.Windows.Forms.Padding(0);
            this.btnMail.Name = "btnMail";
            this.btnMail.Size = new System.Drawing.Size(114, 23);
            this.btnMail.TabIndex = 0;
            this.btnMail.Text = "メール";
            this.btnMail.UseVisualStyleBackColor = true;
            // 
            // btnBattleBackground
            // 
            this.btnBattleBackground.Location = new System.Drawing.Point(268, 140);
            this.btnBattleBackground.Margin = new System.Windows.Forms.Padding(0);
            this.btnBattleBackground.Name = "btnBattleBackground";
            this.btnBattleBackground.Size = new System.Drawing.Size(114, 23);
            this.btnBattleBackground.TabIndex = 0;
            this.btnBattleBackground.Text = "戦闘背景";
            this.btnBattleBackground.UseVisualStyleBackColor = true;
            // 
            // btnTownMap
            // 
            this.btnTownMap.Location = new System.Drawing.Point(268, 168);
            this.btnTownMap.Margin = new System.Windows.Forms.Padding(0);
            this.btnTownMap.Name = "btnTownMap";
            this.btnTownMap.Size = new System.Drawing.Size(114, 23);
            this.btnTownMap.TabIndex = 0;
            this.btnTownMap.Text = "タウンマップ";
            this.btnTownMap.UseVisualStyleBackColor = true;
            // 
            // btnExpandTable
            // 
            this.btnExpandTable.Location = new System.Drawing.Point(20, 206);
            this.btnExpandTable.Margin = new System.Windows.Forms.Padding(0);
            this.btnExpandTable.Name = "btnExpandTable";
            this.btnExpandTable.Size = new System.Drawing.Size(114, 23);
            this.btnExpandTable.TabIndex = 0;
            this.btnExpandTable.Text = "※テーブルを拡張";
            this.btnExpandTable.UseVisualStyleBackColor = true;
            // 
            // btnApplyPatch
            // 
            this.btnApplyPatch.Location = new System.Drawing.Point(144, 206);
            this.btnApplyPatch.Margin = new System.Windows.Forms.Padding(0);
            this.btnApplyPatch.Name = "btnApplyPatch";
            this.btnApplyPatch.Size = new System.Drawing.Size(114, 23);
            this.btnApplyPatch.TabIndex = 0;
            this.btnApplyPatch.Text = "※パッチを適用";
            this.btnApplyPatch.UseVisualStyleBackColor = true;
            // 
            // grpFreeSpaceFinder
            // 
            this.grpFreeSpaceFinder.Controls.Add(this.btnFsfSearch);
            this.grpFreeSpaceFinder.Controls.Add(this.nudFsfByteAmount);
            this.grpFreeSpaceFinder.Controls.Add(this.txtFsfResultAddr);
            this.grpFreeSpaceFinder.Controls.Add(this.txtFsfStartAddr);
            this.grpFreeSpaceFinder.Controls.Add(this.lblFsfByteAmount);
            this.grpFreeSpaceFinder.Controls.Add(this.lblFsfResultAddr);
            this.grpFreeSpaceFinder.Controls.Add(this.lblFsfStartAddr);
            this.grpFreeSpaceFinder.Location = new System.Drawing.Point(440, 72);
            this.grpFreeSpaceFinder.Margin = new System.Windows.Forms.Padding(0);
            this.grpFreeSpaceFinder.Name = "grpFreeSpaceFinder";
            this.grpFreeSpaceFinder.Padding = new System.Windows.Forms.Padding(0);
            this.grpFreeSpaceFinder.Size = new System.Drawing.Size(204, 144);
            this.grpFreeSpaceFinder.TabIndex = 5;
            this.grpFreeSpaceFinder.TabStop = false;
            this.grpFreeSpaceFinder.Text = "空き領域を検索";
            // 
            // lblFsfStartAddr
            // 
            this.lblFsfStartAddr.AutoSize = true;
            this.lblFsfStartAddr.Location = new System.Drawing.Point(20, 32);
            this.lblFsfStartAddr.Margin = new System.Windows.Forms.Padding(0);
            this.lblFsfStartAddr.Name = "lblFsfStartAddr";
            this.lblFsfStartAddr.Size = new System.Drawing.Size(71, 12);
            this.lblFsfStartAddr.TabIndex = 0;
            this.lblFsfStartAddr.Text = "開始アドレス :";
            // 
            // txtFsfStartAddr
            // 
            this.txtFsfStartAddr.Location = new System.Drawing.Point(100, 28);
            this.txtFsfStartAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtFsfStartAddr.Name = "txtFsfStartAddr";
            this.txtFsfStartAddr.Size = new System.Drawing.Size(80, 19);
            this.txtFsfStartAddr.TabIndex = 1;
            // 
            // lblFsfByteAmount
            // 
            this.lblFsfByteAmount.AutoSize = true;
            this.lblFsfByteAmount.Location = new System.Drawing.Point(20, 56);
            this.lblFsfByteAmount.Margin = new System.Windows.Forms.Padding(0);
            this.lblFsfByteAmount.Name = "lblFsfByteAmount";
            this.lblFsfByteAmount.Size = new System.Drawing.Size(64, 12);
            this.lblFsfByteAmount.TabIndex = 0;
            this.lblFsfByteAmount.Text = "必要サイズ :";
            // 
            // nudFsfByteAmount
            // 
            this.nudFsfByteAmount.Location = new System.Drawing.Point(100, 52);
            this.nudFsfByteAmount.Margin = new System.Windows.Forms.Padding(0);
            this.nudFsfByteAmount.Maximum = new decimal(new int[] {
            4095,
            0,
            0,
            0});
            this.nudFsfByteAmount.Name = "nudFsfByteAmount";
            this.nudFsfByteAmount.Size = new System.Drawing.Size(80, 19);
            this.nudFsfByteAmount.TabIndex = 2;
            // 
            // btnFsfSearch
            // 
            this.btnFsfSearch.Location = new System.Drawing.Point(22, 76);
            this.btnFsfSearch.Margin = new System.Windows.Forms.Padding(0);
            this.btnFsfSearch.Name = "btnFsfSearch";
            this.btnFsfSearch.Size = new System.Drawing.Size(158, 23);
            this.btnFsfSearch.TabIndex = 3;
            this.btnFsfSearch.Text = "検索開始";
            this.btnFsfSearch.UseVisualStyleBackColor = true;
            // 
            // lblFsfResultAddr
            // 
            this.lblFsfResultAddr.AutoSize = true;
            this.lblFsfResultAddr.Location = new System.Drawing.Point(20, 108);
            this.lblFsfResultAddr.Margin = new System.Windows.Forms.Padding(0);
            this.lblFsfResultAddr.Name = "lblFsfResultAddr";
            this.lblFsfResultAddr.Size = new System.Drawing.Size(71, 12);
            this.lblFsfResultAddr.TabIndex = 0;
            this.lblFsfResultAddr.Text = "候補アドレス :";
            // 
            // txtFsfResultAddr
            // 
            this.txtFsfResultAddr.Location = new System.Drawing.Point(100, 104);
            this.txtFsfResultAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtFsfResultAddr.Name = "txtFsfResultAddr";
            this.txtFsfResultAddr.Size = new System.Drawing.Size(80, 19);
            this.txtFsfResultAddr.TabIndex = 1;
            // 
            // picFormPokemon
            // 
            this.picFormPokemon.Location = new System.Drawing.Point(462, 220);
            this.picFormPokemon.Margin = new System.Windows.Forms.Padding(0);
            this.picFormPokemon.Name = "picFormPokemon";
            this.picFormPokemon.Size = new System.Drawing.Size(136, 104);
            this.picFormPokemon.TabIndex = 6;
            this.picFormPokemon.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 343);
            this.Controls.Add(this.picFormPokemon);
            this.Controls.Add(this.grpFreeSpaceFinder);
            this.Controls.Add(this.grpSelectEditor);
            this.Controls.Add(this.btnUnloadRom);
            this.Controls.Add(this.cmbConfig);
            this.Controls.Add(this.lblConfig);
            this.Controls.Add(this.btnSaveRom);
            this.Controls.Add(this.btnLoadRom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "メイン画面";
            this.grpSelectEditor.ResumeLayout(false);
            this.grpFreeSpaceFinder.ResumeLayout(false);
            this.grpFreeSpaceFinder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFsfByteAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFormPokemon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoadRom;
        private System.Windows.Forms.Button btnSaveRom;
        private System.Windows.Forms.Label lblConfig;
        private System.Windows.Forms.ComboBox cmbConfig;
        private System.Windows.Forms.Button btnUnloadRom;
        private System.Windows.Forms.GroupBox grpSelectEditor;
        private System.Windows.Forms.Button btnTownMap;
        private System.Windows.Forms.Button btnWildEncounter;
        private System.Windows.Forms.Button btnTileset;
        private System.Windows.Forms.Button btnBattleBackground;
        private System.Windows.Forms.Button btnOverworldSprite;
        private System.Windows.Forms.Button btnMap;
        private System.Windows.Forms.Button btnMail;
        private System.Windows.Forms.Button btnTrainerList;
        private System.Windows.Forms.Button btnTrainerClass;
        private System.Windows.Forms.Button btnIngameTrade;
        private System.Windows.Forms.Button btnTrainerSprite;
        private System.Windows.Forms.Button btnItem;
        private System.Windows.Forms.Button btnEggMove;
        private System.Windows.Forms.Button btnPokedexSearch;
        private System.Windows.Forms.Button btnPokedexOrder;
        private System.Windows.Forms.Button btnTmHmTutor;
        private System.Windows.Forms.Button btnPokedexHabitat;
        private System.Windows.Forms.Button btnPokemon;
        private System.Windows.Forms.Button btnApplyPatch;
        private System.Windows.Forms.Button btnExpandTable;
        private System.Windows.Forms.GroupBox grpFreeSpaceFinder;
        private System.Windows.Forms.Button btnFsfSearch;
        private System.Windows.Forms.NumericUpDown nudFsfByteAmount;
        private System.Windows.Forms.TextBox txtFsfResultAddr;
        private System.Windows.Forms.TextBox txtFsfStartAddr;
        private System.Windows.Forms.Label lblFsfByteAmount;
        private System.Windows.Forms.Label lblFsfResultAddr;
        private System.Windows.Forms.Label lblFsfStartAddr;
        private System.Windows.Forms.PictureBox picFormPokemon;
    }
}

