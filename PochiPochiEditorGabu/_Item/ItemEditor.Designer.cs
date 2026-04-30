
namespace PochiPochiEditorGabu._Item
{
    partial class ItemEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemEditor));
            this.btnSave = new System.Windows.Forms.Button();
            this.grpSelectItem = new System.Windows.Forms.GroupBox();
            this.txtItemIdHex = new System.Windows.Forms.TextBox();
            this.nudItemId = new System.Windows.Forms.NumericUpDown();
            this.lblItemIdHex = new System.Windows.Forms.Label();
            this.lblItemId = new System.Windows.Forms.Label();
            this.cmbItemName = new System.Windows.Forms.ComboBox();
            this.grpItemRename = new System.Windows.Forms.GroupBox();
            this.txtItemRename = new System.Windows.Forms.TextBox();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageInfo = new System.Windows.Forms.TabPage();
            this.grpItemEffect = new System.Windows.Forms.GroupBox();
            this.txtItemEffectAddr = new System.Windows.Forms.TextBox();
            this.lblItemEffectAddr = new System.Windows.Forms.Label();
            this.grpData = new System.Windows.Forms.GroupBox();
            this.lblNote3 = new System.Windows.Forms.Label();
            this.lblNote1 = new System.Windows.Forms.Label();
            this.nudSpecialIdx = new System.Windows.Forms.NumericUpDown();
            this.lblSpecialIdx = new System.Windows.Forms.Label();
            this.grpBattleUse = new System.Windows.Forms.GroupBox();
            this.cmbBattleUseType = new System.Windows.Forms.ComboBox();
            this.txtBattleUseAddr = new System.Windows.Forms.TextBox();
            this.lblBattleUseAddr = new System.Windows.Forms.Label();
            this.lblBattleUseType = new System.Windows.Forms.Label();
            this.grpFieldUse = new System.Windows.Forms.GroupBox();
            this.lblNote2 = new System.Windows.Forms.Label();
            this.txtFieldUseAddr = new System.Windows.Forms.TextBox();
            this.lblFieldUseAddr = new System.Windows.Forms.Label();
            this.cmbFieldUseType = new System.Windows.Forms.ComboBox();
            this.nudFieldUseType = new System.Windows.Forms.NumericUpDown();
            this.lblFieldUseType = new System.Windows.Forms.Label();
            this.cmbPocketIdx = new System.Windows.Forms.ComboBox();
            this.cmbCanHold = new System.Windows.Forms.ComboBox();
            this.cmbHoldEffectIdx = new System.Windows.Forms.ComboBox();
            this.nudUnknownValue = new System.Windows.Forms.NumericUpDown();
            this.nudEffectValue = new System.Windows.Forms.NumericUpDown();
            this.nudPrice = new System.Windows.Forms.NumericUpDown();
            this.nudIdx = new System.Windows.Forms.NumericUpDown();
            this.lblUnknownValue = new System.Windows.Forms.Label();
            this.lblPocketIdx = new System.Windows.Forms.Label();
            this.lblEffectValue = new System.Windows.Forms.Label();
            this.lblCanHold = new System.Windows.Forms.Label();
            this.lblHoldEffectIdx = new System.Windows.Forms.Label();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblIdx = new System.Windows.Forms.Label();
            this.grpDesc = new System.Windows.Forms.GroupBox();
            this.txtDescString = new System.Windows.Forms.TextBox();
            this.txtDescAddr = new System.Windows.Forms.TextBox();
            this.lblDescAddr = new System.Windows.Forms.Label();
            this.grpSprite = new System.Windows.Forms.GroupBox();
            this.btnSpriteExport = new System.Windows.Forms.Button();
            this.btnSpriteImport = new System.Windows.Forms.Button();
            this.txtSpriteImportAddr = new System.Windows.Forms.TextBox();
            this.txtSpritePalAddr = new System.Windows.Forms.TextBox();
            this.txtSpriteImgAddr = new System.Windows.Forms.TextBox();
            this.rbSpritePalAddr = new System.Windows.Forms.RadioButton();
            this.rbSpriteImgAddr = new System.Windows.Forms.RadioButton();
            this.picSprite = new System.Windows.Forms.PictureBox();
            this.grpSelectItem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemId)).BeginInit();
            this.grpItemRename.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabPageInfo.SuspendLayout();
            this.grpItemEffect.SuspendLayout();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpecialIdx)).BeginInit();
            this.grpBattleUse.SuspendLayout();
            this.grpFieldUse.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFieldUseType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUnknownValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEffectValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIdx)).BeginInit();
            this.grpDesc.SuspendLayout();
            this.grpSprite.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSprite)).BeginInit();
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
            // grpSelectItem
            // 
            this.grpSelectItem.Controls.Add(this.txtItemIdHex);
            this.grpSelectItem.Controls.Add(this.nudItemId);
            this.grpSelectItem.Controls.Add(this.lblItemIdHex);
            this.grpSelectItem.Controls.Add(this.lblItemId);
            this.grpSelectItem.Controls.Add(this.cmbItemName);
            this.grpSelectItem.Location = new System.Drawing.Point(20, 52);
            this.grpSelectItem.Margin = new System.Windows.Forms.Padding(0);
            this.grpSelectItem.Name = "grpSelectItem";
            this.grpSelectItem.Padding = new System.Windows.Forms.Padding(0);
            this.grpSelectItem.Size = new System.Drawing.Size(186, 118);
            this.grpSelectItem.TabIndex = 1;
            this.grpSelectItem.TabStop = false;
            this.grpSelectItem.Text = "アイテムを選択";
            // 
            // txtItemIdHex
            // 
            this.txtItemIdHex.Location = new System.Drawing.Point(108, 80);
            this.txtItemIdHex.Margin = new System.Windows.Forms.Padding(0);
            this.txtItemIdHex.Name = "txtItemIdHex";
            this.txtItemIdHex.ReadOnly = true;
            this.txtItemIdHex.Size = new System.Drawing.Size(56, 19);
            this.txtItemIdHex.TabIndex = 3;
            // 
            // nudItemId
            // 
            this.nudItemId.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudItemId.Location = new System.Drawing.Point(108, 56);
            this.nudItemId.Margin = new System.Windows.Forms.Padding(0);
            this.nudItemId.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudItemId.Name = "nudItemId";
            this.nudItemId.ReadOnly = true;
            this.nudItemId.Size = new System.Drawing.Size(56, 19);
            this.nudItemId.TabIndex = 2;
            // 
            // lblItemIdHex
            // 
            this.lblItemIdHex.AutoSize = true;
            this.lblItemIdHex.Location = new System.Drawing.Point(20, 84);
            this.lblItemIdHex.Margin = new System.Windows.Forms.Padding(0);
            this.lblItemIdHex.Name = "lblItemIdHex";
            this.lblItemIdHex.Size = new System.Drawing.Size(59, 12);
            this.lblItemIdHex.TabIndex = 1;
            this.lblItemIdHex.Text = "（16進数） :";
            // 
            // lblItemId
            // 
            this.lblItemId.AutoSize = true;
            this.lblItemId.Location = new System.Drawing.Point(20, 60);
            this.lblItemId.Margin = new System.Windows.Forms.Padding(0);
            this.lblItemId.Name = "lblItemId";
            this.lblItemId.Size = new System.Drawing.Size(75, 12);
            this.lblItemId.TabIndex = 1;
            this.lblItemId.Text = "アイテムコード :";
            // 
            // cmbItemName
            // 
            this.cmbItemName.FormattingEnabled = true;
            this.cmbItemName.Location = new System.Drawing.Point(20, 28);
            this.cmbItemName.Margin = new System.Windows.Forms.Padding(0);
            this.cmbItemName.Name = "cmbItemName";
            this.cmbItemName.Size = new System.Drawing.Size(144, 20);
            this.cmbItemName.TabIndex = 0;
            // 
            // grpItemRename
            // 
            this.grpItemRename.Controls.Add(this.txtItemRename);
            this.grpItemRename.Location = new System.Drawing.Point(20, 178);
            this.grpItemRename.Margin = new System.Windows.Forms.Padding(0);
            this.grpItemRename.Name = "grpItemRename";
            this.grpItemRename.Padding = new System.Windows.Forms.Padding(0);
            this.grpItemRename.Size = new System.Drawing.Size(186, 66);
            this.grpItemRename.TabIndex = 2;
            this.grpItemRename.TabStop = false;
            this.grpItemRename.Text = "名前を変更";
            // 
            // txtItemRename
            // 
            this.txtItemRename.Location = new System.Drawing.Point(20, 28);
            this.txtItemRename.Margin = new System.Windows.Forms.Padding(0);
            this.txtItemRename.Name = "txtItemRename";
            this.txtItemRename.Size = new System.Drawing.Size(144, 19);
            this.txtItemRename.TabIndex = 0;
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageInfo);
            this.tabControlMain.Location = new System.Drawing.Point(224, 36);
            this.tabControlMain.Margin = new System.Windows.Forms.Padding(0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(706, 528);
            this.tabControlMain.TabIndex = 4;
            // 
            // tabPageInfo
            // 
            this.tabPageInfo.Controls.Add(this.grpItemEffect);
            this.tabPageInfo.Controls.Add(this.grpData);
            this.tabPageInfo.Controls.Add(this.grpDesc);
            this.tabPageInfo.Controls.Add(this.grpSprite);
            this.tabPageInfo.Location = new System.Drawing.Point(4, 22);
            this.tabPageInfo.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageInfo.Name = "tabPageInfo";
            this.tabPageInfo.Size = new System.Drawing.Size(698, 502);
            this.tabPageInfo.TabIndex = 0;
            this.tabPageInfo.Text = "詳細";
            this.tabPageInfo.UseVisualStyleBackColor = true;
            // 
            // grpItemEffect
            // 
            this.grpItemEffect.Controls.Add(this.txtItemEffectAddr);
            this.grpItemEffect.Controls.Add(this.lblItemEffectAddr);
            this.grpItemEffect.Location = new System.Drawing.Point(20, 326);
            this.grpItemEffect.Margin = new System.Windows.Forms.Padding(0);
            this.grpItemEffect.Name = "grpItemEffect";
            this.grpItemEffect.Padding = new System.Windows.Forms.Padding(0);
            this.grpItemEffect.Size = new System.Drawing.Size(216, 68);
            this.grpItemEffect.TabIndex = 3;
            this.grpItemEffect.TabStop = false;
            this.grpItemEffect.Text = "アイテム効果";
            // 
            // txtItemEffectAddr
            // 
            this.txtItemEffectAddr.Location = new System.Drawing.Point(112, 28);
            this.txtItemEffectAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtItemEffectAddr.Name = "txtItemEffectAddr";
            this.txtItemEffectAddr.Size = new System.Drawing.Size(80, 19);
            this.txtItemEffectAddr.TabIndex = 5;
            // 
            // lblItemEffectAddr
            // 
            this.lblItemEffectAddr.AutoSize = true;
            this.lblItemEffectAddr.Location = new System.Drawing.Point(20, 32);
            this.lblItemEffectAddr.Margin = new System.Windows.Forms.Padding(0);
            this.lblItemEffectAddr.Name = "lblItemEffectAddr";
            this.lblItemEffectAddr.Size = new System.Drawing.Size(71, 12);
            this.lblItemEffectAddr.TabIndex = 4;
            this.lblItemEffectAddr.Text = "効果アドレス :";
            // 
            // grpData
            // 
            this.grpData.Controls.Add(this.lblNote3);
            this.grpData.Controls.Add(this.lblNote1);
            this.grpData.Controls.Add(this.nudSpecialIdx);
            this.grpData.Controls.Add(this.lblSpecialIdx);
            this.grpData.Controls.Add(this.grpBattleUse);
            this.grpData.Controls.Add(this.grpFieldUse);
            this.grpData.Controls.Add(this.cmbPocketIdx);
            this.grpData.Controls.Add(this.cmbCanHold);
            this.grpData.Controls.Add(this.cmbHoldEffectIdx);
            this.grpData.Controls.Add(this.nudUnknownValue);
            this.grpData.Controls.Add(this.nudEffectValue);
            this.grpData.Controls.Add(this.nudPrice);
            this.grpData.Controls.Add(this.nudIdx);
            this.grpData.Controls.Add(this.lblUnknownValue);
            this.grpData.Controls.Add(this.lblPocketIdx);
            this.grpData.Controls.Add(this.lblEffectValue);
            this.grpData.Controls.Add(this.lblCanHold);
            this.grpData.Controls.Add(this.lblHoldEffectIdx);
            this.grpData.Controls.Add(this.lblPrice);
            this.grpData.Controls.Add(this.lblIdx);
            this.grpData.Location = new System.Drawing.Point(326, 16);
            this.grpData.Margin = new System.Windows.Forms.Padding(0);
            this.grpData.Name = "grpData";
            this.grpData.Padding = new System.Windows.Forms.Padding(0);
            this.grpData.Size = new System.Drawing.Size(350, 466);
            this.grpData.TabIndex = 2;
            this.grpData.TabStop = false;
            this.grpData.Text = "データ";
            // 
            // lblNote3
            // 
            this.lblNote3.AutoSize = true;
            this.lblNote3.Location = new System.Drawing.Point(180, 430);
            this.lblNote3.Margin = new System.Windows.Forms.Padding(0);
            this.lblNote3.Name = "lblNote3";
            this.lblNote3.Size = new System.Drawing.Size(119, 12);
            this.lblNote3.TabIndex = 8;
            this.lblNote3.Text = "※ボール/メール/釣り竿";
            // 
            // lblNote1
            // 
            this.lblNote1.AutoSize = true;
            this.lblNote1.Location = new System.Drawing.Point(180, 156);
            this.lblNote1.Margin = new System.Windows.Forms.Padding(0);
            this.lblNote1.Name = "lblNote1";
            this.lblNote1.Size = new System.Drawing.Size(121, 12);
            this.lblNote1.TabIndex = 7;
            this.lblNote1.Text = "※木の実/技マシン番号";
            // 
            // nudSpecialIdx
            // 
            this.nudSpecialIdx.Location = new System.Drawing.Point(96, 426);
            this.nudSpecialIdx.Margin = new System.Windows.Forms.Padding(0);
            this.nudSpecialIdx.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudSpecialIdx.Name = "nudSpecialIdx";
            this.nudSpecialIdx.Size = new System.Drawing.Size(64, 19);
            this.nudSpecialIdx.TabIndex = 6;
            // 
            // lblSpecialIdx
            // 
            this.lblSpecialIdx.AutoSize = true;
            this.lblSpecialIdx.Location = new System.Drawing.Point(20, 430);
            this.lblSpecialIdx.Margin = new System.Windows.Forms.Padding(0);
            this.lblSpecialIdx.Name = "lblSpecialIdx";
            this.lblSpecialIdx.Size = new System.Drawing.Size(46, 12);
            this.lblSpecialIdx.TabIndex = 5;
            this.lblSpecialIdx.Text = "特殊ID :";
            // 
            // grpBattleUse
            // 
            this.grpBattleUse.Controls.Add(this.cmbBattleUseType);
            this.grpBattleUse.Controls.Add(this.txtBattleUseAddr);
            this.grpBattleUse.Controls.Add(this.lblBattleUseAddr);
            this.grpBattleUse.Controls.Add(this.lblBattleUseType);
            this.grpBattleUse.Location = new System.Drawing.Point(22, 328);
            this.grpBattleUse.Margin = new System.Windows.Forms.Padding(0);
            this.grpBattleUse.Name = "grpBattleUse";
            this.grpBattleUse.Padding = new System.Windows.Forms.Padding(0);
            this.grpBattleUse.Size = new System.Drawing.Size(280, 88);
            this.grpBattleUse.TabIndex = 4;
            this.grpBattleUse.TabStop = false;
            this.grpBattleUse.Text = "バトル使用";
            // 
            // cmbBattleUseType
            // 
            this.cmbBattleUseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBattleUseType.FormattingEnabled = true;
            this.cmbBattleUseType.Location = new System.Drawing.Point(74, 24);
            this.cmbBattleUseType.Margin = new System.Windows.Forms.Padding(0);
            this.cmbBattleUseType.Name = "cmbBattleUseType";
            this.cmbBattleUseType.Size = new System.Drawing.Size(184, 20);
            this.cmbBattleUseType.TabIndex = 7;
            // 
            // txtBattleUseAddr
            // 
            this.txtBattleUseAddr.Location = new System.Drawing.Point(74, 50);
            this.txtBattleUseAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtBattleUseAddr.Name = "txtBattleUseAddr";
            this.txtBattleUseAddr.Size = new System.Drawing.Size(80, 19);
            this.txtBattleUseAddr.TabIndex = 6;
            // 
            // lblBattleUseAddr
            // 
            this.lblBattleUseAddr.AutoSize = true;
            this.lblBattleUseAddr.Location = new System.Drawing.Point(16, 54);
            this.lblBattleUseAddr.Margin = new System.Windows.Forms.Padding(0);
            this.lblBattleUseAddr.Name = "lblBattleUseAddr";
            this.lblBattleUseAddr.Size = new System.Drawing.Size(47, 12);
            this.lblBattleUseAddr.TabIndex = 5;
            this.lblBattleUseAddr.Text = "アドレス :";
            // 
            // lblBattleUseType
            // 
            this.lblBattleUseType.AutoSize = true;
            this.lblBattleUseType.Location = new System.Drawing.Point(16, 28);
            this.lblBattleUseType.Margin = new System.Windows.Forms.Padding(0);
            this.lblBattleUseType.Name = "lblBattleUseType";
            this.lblBattleUseType.Size = new System.Drawing.Size(37, 12);
            this.lblBattleUseType.TabIndex = 2;
            this.lblBattleUseType.Text = "タイプ :";
            // 
            // grpFieldUse
            // 
            this.grpFieldUse.Controls.Add(this.lblNote2);
            this.grpFieldUse.Controls.Add(this.txtFieldUseAddr);
            this.grpFieldUse.Controls.Add(this.lblFieldUseAddr);
            this.grpFieldUse.Controls.Add(this.cmbFieldUseType);
            this.grpFieldUse.Controls.Add(this.nudFieldUseType);
            this.grpFieldUse.Controls.Add(this.lblFieldUseType);
            this.grpFieldUse.Location = new System.Drawing.Point(22, 206);
            this.grpFieldUse.Margin = new System.Windows.Forms.Padding(0);
            this.grpFieldUse.Name = "grpFieldUse";
            this.grpFieldUse.Padding = new System.Windows.Forms.Padding(0);
            this.grpFieldUse.Size = new System.Drawing.Size(280, 112);
            this.grpFieldUse.TabIndex = 3;
            this.grpFieldUse.TabStop = false;
            this.grpFieldUse.Text = "フィールド使用";
            // 
            // lblNote2
            // 
            this.lblNote2.AutoSize = true;
            this.lblNote2.Location = new System.Drawing.Point(158, 26);
            this.lblNote2.Margin = new System.Windows.Forms.Padding(0);
            this.lblNote2.Name = "lblNote2";
            this.lblNote2.Size = new System.Drawing.Size(58, 12);
            this.lblNote2.TabIndex = 8;
            this.lblNote2.Text = "※ボールID";
            // 
            // txtFieldUseAddr
            // 
            this.txtFieldUseAddr.Location = new System.Drawing.Point(74, 74);
            this.txtFieldUseAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtFieldUseAddr.Name = "txtFieldUseAddr";
            this.txtFieldUseAddr.Size = new System.Drawing.Size(80, 19);
            this.txtFieldUseAddr.TabIndex = 6;
            // 
            // lblFieldUseAddr
            // 
            this.lblFieldUseAddr.AutoSize = true;
            this.lblFieldUseAddr.Location = new System.Drawing.Point(16, 78);
            this.lblFieldUseAddr.Margin = new System.Windows.Forms.Padding(0);
            this.lblFieldUseAddr.Name = "lblFieldUseAddr";
            this.lblFieldUseAddr.Size = new System.Drawing.Size(47, 12);
            this.lblFieldUseAddr.TabIndex = 5;
            this.lblFieldUseAddr.Text = "アドレス :";
            // 
            // cmbFieldUseType
            // 
            this.cmbFieldUseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFieldUseType.FormattingEnabled = true;
            this.cmbFieldUseType.Location = new System.Drawing.Point(74, 48);
            this.cmbFieldUseType.Margin = new System.Windows.Forms.Padding(0);
            this.cmbFieldUseType.Name = "cmbFieldUseType";
            this.cmbFieldUseType.Size = new System.Drawing.Size(184, 20);
            this.cmbFieldUseType.TabIndex = 4;
            // 
            // nudFieldUseType
            // 
            this.nudFieldUseType.Location = new System.Drawing.Point(74, 24);
            this.nudFieldUseType.Margin = new System.Windows.Forms.Padding(0);
            this.nudFieldUseType.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudFieldUseType.Name = "nudFieldUseType";
            this.nudFieldUseType.Size = new System.Drawing.Size(64, 19);
            this.nudFieldUseType.TabIndex = 3;
            // 
            // lblFieldUseType
            // 
            this.lblFieldUseType.AutoSize = true;
            this.lblFieldUseType.Location = new System.Drawing.Point(16, 28);
            this.lblFieldUseType.Margin = new System.Windows.Forms.Padding(0);
            this.lblFieldUseType.Name = "lblFieldUseType";
            this.lblFieldUseType.Size = new System.Drawing.Size(37, 12);
            this.lblFieldUseType.TabIndex = 2;
            this.lblFieldUseType.Text = "タイプ :";
            // 
            // cmbPocketIdx
            // 
            this.cmbPocketIdx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPocketIdx.FormattingEnabled = true;
            this.cmbPocketIdx.Location = new System.Drawing.Point(96, 176);
            this.cmbPocketIdx.Margin = new System.Windows.Forms.Padding(0);
            this.cmbPocketIdx.Name = "cmbPocketIdx";
            this.cmbPocketIdx.Size = new System.Drawing.Size(152, 20);
            this.cmbPocketIdx.TabIndex = 2;
            // 
            // cmbCanHold
            // 
            this.cmbCanHold.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCanHold.FormattingEnabled = true;
            this.cmbCanHold.Location = new System.Drawing.Point(96, 126);
            this.cmbCanHold.Margin = new System.Windows.Forms.Padding(0);
            this.cmbCanHold.Name = "cmbCanHold";
            this.cmbCanHold.Size = new System.Drawing.Size(152, 20);
            this.cmbCanHold.TabIndex = 2;
            // 
            // cmbHoldEffectIdx
            // 
            this.cmbHoldEffectIdx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHoldEffectIdx.FormattingEnabled = true;
            this.cmbHoldEffectIdx.Location = new System.Drawing.Point(96, 76);
            this.cmbHoldEffectIdx.Margin = new System.Windows.Forms.Padding(0);
            this.cmbHoldEffectIdx.Name = "cmbHoldEffectIdx";
            this.cmbHoldEffectIdx.Size = new System.Drawing.Size(230, 20);
            this.cmbHoldEffectIdx.TabIndex = 2;
            // 
            // nudUnknownValue
            // 
            this.nudUnknownValue.Location = new System.Drawing.Point(96, 152);
            this.nudUnknownValue.Margin = new System.Windows.Forms.Padding(0);
            this.nudUnknownValue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudUnknownValue.Name = "nudUnknownValue";
            this.nudUnknownValue.Size = new System.Drawing.Size(64, 19);
            this.nudUnknownValue.TabIndex = 1;
            // 
            // nudEffectValue
            // 
            this.nudEffectValue.Location = new System.Drawing.Point(96, 102);
            this.nudEffectValue.Margin = new System.Windows.Forms.Padding(0);
            this.nudEffectValue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudEffectValue.Name = "nudEffectValue";
            this.nudEffectValue.Size = new System.Drawing.Size(64, 19);
            this.nudEffectValue.TabIndex = 1;
            // 
            // nudPrice
            // 
            this.nudPrice.Location = new System.Drawing.Point(96, 52);
            this.nudPrice.Margin = new System.Windows.Forms.Padding(0);
            this.nudPrice.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudPrice.Name = "nudPrice";
            this.nudPrice.Size = new System.Drawing.Size(64, 19);
            this.nudPrice.TabIndex = 1;
            // 
            // nudIdx
            // 
            this.nudIdx.Location = new System.Drawing.Point(96, 28);
            this.nudIdx.Margin = new System.Windows.Forms.Padding(0);
            this.nudIdx.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudIdx.Name = "nudIdx";
            this.nudIdx.Size = new System.Drawing.Size(64, 19);
            this.nudIdx.TabIndex = 1;
            // 
            // lblUnknownValue
            // 
            this.lblUnknownValue.AutoSize = true;
            this.lblUnknownValue.Location = new System.Drawing.Point(20, 156);
            this.lblUnknownValue.Margin = new System.Windows.Forms.Padding(0);
            this.lblUnknownValue.Name = "lblUnknownValue";
            this.lblUnknownValue.Size = new System.Drawing.Size(47, 12);
            this.lblUnknownValue.TabIndex = 0;
            this.lblUnknownValue.Text = "不明値 :";
            // 
            // lblPocketIdx
            // 
            this.lblPocketIdx.AutoSize = true;
            this.lblPocketIdx.Location = new System.Drawing.Point(20, 180);
            this.lblPocketIdx.Margin = new System.Windows.Forms.Padding(0);
            this.lblPocketIdx.Name = "lblPocketIdx";
            this.lblPocketIdx.Size = new System.Drawing.Size(56, 12);
            this.lblPocketIdx.TabIndex = 0;
            this.lblPocketIdx.Text = "ポケットID :";
            // 
            // lblEffectValue
            // 
            this.lblEffectValue.AutoSize = true;
            this.lblEffectValue.Location = new System.Drawing.Point(20, 106);
            this.lblEffectValue.Margin = new System.Windows.Forms.Padding(0);
            this.lblEffectValue.Name = "lblEffectValue";
            this.lblEffectValue.Size = new System.Drawing.Size(47, 12);
            this.lblEffectValue.TabIndex = 0;
            this.lblEffectValue.Text = "効果値 :";
            // 
            // lblCanHold
            // 
            this.lblCanHold.AutoSize = true;
            this.lblCanHold.Location = new System.Drawing.Point(20, 130);
            this.lblCanHold.Margin = new System.Windows.Forms.Padding(0);
            this.lblCanHold.Name = "lblCanHold";
            this.lblCanHold.Size = new System.Drawing.Size(59, 12);
            this.lblCanHold.TabIndex = 0;
            this.lblCanHold.Text = "所持可否 :";
            // 
            // lblHoldEffectIdx
            // 
            this.lblHoldEffectIdx.AutoSize = true;
            this.lblHoldEffectIdx.Location = new System.Drawing.Point(20, 80);
            this.lblHoldEffectIdx.Margin = new System.Windows.Forms.Padding(0);
            this.lblHoldEffectIdx.Name = "lblHoldEffectIdx";
            this.lblHoldEffectIdx.Size = new System.Drawing.Size(59, 12);
            this.lblHoldEffectIdx.TabIndex = 0;
            this.lblHoldEffectIdx.Text = "所持効果 :";
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(20, 56);
            this.lblPrice.Margin = new System.Windows.Forms.Padding(0);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(35, 12);
            this.lblPrice.TabIndex = 0;
            this.lblPrice.Text = "値段 :";
            // 
            // lblIdx
            // 
            this.lblIdx.AutoSize = true;
            this.lblIdx.Location = new System.Drawing.Point(20, 32);
            this.lblIdx.Margin = new System.Windows.Forms.Padding(0);
            this.lblIdx.Name = "lblIdx";
            this.lblIdx.Size = new System.Drawing.Size(59, 12);
            this.lblIdx.TabIndex = 0;
            this.lblIdx.Text = "アイテムID :";
            // 
            // grpDesc
            // 
            this.grpDesc.Controls.Add(this.txtDescString);
            this.grpDesc.Controls.Add(this.txtDescAddr);
            this.grpDesc.Controls.Add(this.lblDescAddr);
            this.grpDesc.Location = new System.Drawing.Point(20, 174);
            this.grpDesc.Margin = new System.Windows.Forms.Padding(0);
            this.grpDesc.Name = "grpDesc";
            this.grpDesc.Padding = new System.Windows.Forms.Padding(0);
            this.grpDesc.Size = new System.Drawing.Size(238, 142);
            this.grpDesc.TabIndex = 1;
            this.grpDesc.TabStop = false;
            this.grpDesc.Text = "説明文";
            // 
            // txtDescString
            // 
            this.txtDescString.Location = new System.Drawing.Point(20, 60);
            this.txtDescString.Margin = new System.Windows.Forms.Padding(0);
            this.txtDescString.Multiline = true;
            this.txtDescString.Name = "txtDescString";
            this.txtDescString.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescString.Size = new System.Drawing.Size(196, 60);
            this.txtDescString.TabIndex = 4;
            // 
            // txtDescAddr
            // 
            this.txtDescAddr.Location = new System.Drawing.Point(112, 28);
            this.txtDescAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtDescAddr.Name = "txtDescAddr";
            this.txtDescAddr.Size = new System.Drawing.Size(80, 19);
            this.txtDescAddr.TabIndex = 3;
            // 
            // lblDescAddr
            // 
            this.lblDescAddr.AutoSize = true;
            this.lblDescAddr.Location = new System.Drawing.Point(20, 32);
            this.lblDescAddr.Margin = new System.Windows.Forms.Padding(0);
            this.lblDescAddr.Name = "lblDescAddr";
            this.lblDescAddr.Size = new System.Drawing.Size(83, 12);
            this.lblDescAddr.TabIndex = 0;
            this.lblDescAddr.Text = "説明文アドレス :";
            // 
            // grpSprite
            // 
            this.grpSprite.Controls.Add(this.btnSpriteExport);
            this.grpSprite.Controls.Add(this.btnSpriteImport);
            this.grpSprite.Controls.Add(this.txtSpriteImportAddr);
            this.grpSprite.Controls.Add(this.txtSpritePalAddr);
            this.grpSprite.Controls.Add(this.txtSpriteImgAddr);
            this.grpSprite.Controls.Add(this.rbSpritePalAddr);
            this.grpSprite.Controls.Add(this.rbSpriteImgAddr);
            this.grpSprite.Controls.Add(this.picSprite);
            this.grpSprite.Location = new System.Drawing.Point(20, 16);
            this.grpSprite.Margin = new System.Windows.Forms.Padding(0);
            this.grpSprite.Name = "grpSprite";
            this.grpSprite.Padding = new System.Windows.Forms.Padding(0);
            this.grpSprite.Size = new System.Drawing.Size(286, 148);
            this.grpSprite.TabIndex = 0;
            this.grpSprite.TabStop = false;
            this.grpSprite.Text = "画像";
            // 
            // btnSpriteExport
            // 
            this.btnSpriteExport.Location = new System.Drawing.Point(184, 104);
            this.btnSpriteExport.Margin = new System.Windows.Forms.Padding(0);
            this.btnSpriteExport.Name = "btnSpriteExport";
            this.btnSpriteExport.Size = new System.Drawing.Size(80, 23);
            this.btnSpriteExport.TabIndex = 3;
            this.btnSpriteExport.Text = "エクスポート";
            this.btnSpriteExport.UseVisualStyleBackColor = true;
            // 
            // btnSpriteImport
            // 
            this.btnSpriteImport.Location = new System.Drawing.Point(184, 78);
            this.btnSpriteImport.Margin = new System.Windows.Forms.Padding(0);
            this.btnSpriteImport.Name = "btnSpriteImport";
            this.btnSpriteImport.Size = new System.Drawing.Size(80, 23);
            this.btnSpriteImport.TabIndex = 3;
            this.btnSpriteImport.Text = "インポート";
            this.btnSpriteImport.UseVisualStyleBackColor = true;
            // 
            // txtSpriteImportAddr
            // 
            this.txtSpriteImportAddr.Location = new System.Drawing.Point(96, 81);
            this.txtSpriteImportAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtSpriteImportAddr.Name = "txtSpriteImportAddr";
            this.txtSpriteImportAddr.Size = new System.Drawing.Size(80, 19);
            this.txtSpriteImportAddr.TabIndex = 2;
            // 
            // txtSpritePalAddr
            // 
            this.txtSpritePalAddr.Location = new System.Drawing.Point(184, 54);
            this.txtSpritePalAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtSpritePalAddr.Name = "txtSpritePalAddr";
            this.txtSpritePalAddr.Size = new System.Drawing.Size(80, 19);
            this.txtSpritePalAddr.TabIndex = 2;
            // 
            // txtSpriteImgAddr
            // 
            this.txtSpriteImgAddr.Location = new System.Drawing.Point(184, 30);
            this.txtSpriteImgAddr.Margin = new System.Windows.Forms.Padding(0);
            this.txtSpriteImgAddr.Name = "txtSpriteImgAddr";
            this.txtSpriteImgAddr.Size = new System.Drawing.Size(80, 19);
            this.txtSpriteImgAddr.TabIndex = 2;
            // 
            // rbSpritePalAddr
            // 
            this.rbSpritePalAddr.AutoSize = true;
            this.rbSpritePalAddr.Location = new System.Drawing.Point(80, 56);
            this.rbSpritePalAddr.Margin = new System.Windows.Forms.Padding(0);
            this.rbSpritePalAddr.Name = "rbSpritePalAddr";
            this.rbSpritePalAddr.Size = new System.Drawing.Size(99, 16);
            this.rbSpritePalAddr.TabIndex = 1;
            this.rbSpritePalAddr.Text = "パレットアドレス :";
            this.rbSpritePalAddr.UseVisualStyleBackColor = true;
            // 
            // rbSpriteImgAddr
            // 
            this.rbSpriteImgAddr.AutoSize = true;
            this.rbSpriteImgAddr.Checked = true;
            this.rbSpriteImgAddr.Location = new System.Drawing.Point(80, 32);
            this.rbSpriteImgAddr.Margin = new System.Windows.Forms.Padding(0);
            this.rbSpriteImgAddr.Name = "rbSpriteImgAddr";
            this.rbSpriteImgAddr.Size = new System.Drawing.Size(89, 16);
            this.rbSpriteImgAddr.TabIndex = 1;
            this.rbSpriteImgAddr.TabStop = true;
            this.rbSpriteImgAddr.Text = "画像アドレス :";
            this.rbSpriteImgAddr.UseVisualStyleBackColor = true;
            // 
            // picSprite
            // 
            this.picSprite.Location = new System.Drawing.Point(20, 28);
            this.picSprite.Margin = new System.Windows.Forms.Padding(0);
            this.picSprite.Name = "picSprite";
            this.picSprite.Size = new System.Drawing.Size(48, 48);
            this.picSprite.TabIndex = 0;
            this.picSprite.TabStop = false;
            // 
            // ItemEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 579);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.grpItemRename);
            this.Controls.Add(this.grpSelectItem);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ItemEditor";
            this.Text = "アイテム";
            this.grpSelectItem.ResumeLayout(false);
            this.grpSelectItem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemId)).EndInit();
            this.grpItemRename.ResumeLayout(false);
            this.grpItemRename.PerformLayout();
            this.tabControlMain.ResumeLayout(false);
            this.tabPageInfo.ResumeLayout(false);
            this.grpItemEffect.ResumeLayout(false);
            this.grpItemEffect.PerformLayout();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpecialIdx)).EndInit();
            this.grpBattleUse.ResumeLayout(false);
            this.grpBattleUse.PerformLayout();
            this.grpFieldUse.ResumeLayout(false);
            this.grpFieldUse.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFieldUseType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUnknownValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEffectValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIdx)).EndInit();
            this.grpDesc.ResumeLayout(false);
            this.grpDesc.PerformLayout();
            this.grpSprite.ResumeLayout(false);
            this.grpSprite.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSprite)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox grpSelectItem;
        private System.Windows.Forms.TextBox txtItemIdHex;
        private System.Windows.Forms.NumericUpDown nudItemId;
        private System.Windows.Forms.Label lblItemIdHex;
        private System.Windows.Forms.Label lblItemId;
        private System.Windows.Forms.ComboBox cmbItemName;
        private System.Windows.Forms.GroupBox grpItemRename;
        private System.Windows.Forms.TextBox txtItemRename;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageInfo;
        private System.Windows.Forms.GroupBox grpSprite;
        private System.Windows.Forms.PictureBox picSprite;
        private System.Windows.Forms.TextBox txtSpritePalAddr;
        private System.Windows.Forms.TextBox txtSpriteImgAddr;
        private System.Windows.Forms.RadioButton rbSpritePalAddr;
        private System.Windows.Forms.RadioButton rbSpriteImgAddr;
        private System.Windows.Forms.Button btnSpriteExport;
        private System.Windows.Forms.Button btnSpriteImport;
        private System.Windows.Forms.TextBox txtSpriteImportAddr;
        private System.Windows.Forms.GroupBox grpDesc;
        private System.Windows.Forms.Label lblDescAddr;
        private System.Windows.Forms.TextBox txtDescString;
        private System.Windows.Forms.TextBox txtDescAddr;
        private System.Windows.Forms.NumericUpDown nudPrice;
        private System.Windows.Forms.NumericUpDown nudIdx;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.Label lblIdx;
        private System.Windows.Forms.ComboBox cmbHoldEffectIdx;
        private System.Windows.Forms.Label lblHoldEffectIdx;
        private System.Windows.Forms.NumericUpDown nudEffectValue;
        private System.Windows.Forms.Label lblEffectValue;
        private System.Windows.Forms.ComboBox cmbPocketIdx;
        private System.Windows.Forms.ComboBox cmbCanHold;
        private System.Windows.Forms.NumericUpDown nudUnknownValue;
        private System.Windows.Forms.Label lblUnknownValue;
        private System.Windows.Forms.Label lblPocketIdx;
        private System.Windows.Forms.Label lblCanHold;
        private System.Windows.Forms.GroupBox grpFieldUse;
        private System.Windows.Forms.ComboBox cmbFieldUseType;
        private System.Windows.Forms.NumericUpDown nudFieldUseType;
        private System.Windows.Forms.Label lblFieldUseType;
        private System.Windows.Forms.TextBox txtFieldUseAddr;
        private System.Windows.Forms.Label lblFieldUseAddr;
        private System.Windows.Forms.GroupBox grpBattleUse;
        private System.Windows.Forms.ComboBox cmbBattleUseType;
        private System.Windows.Forms.TextBox txtBattleUseAddr;
        private System.Windows.Forms.Label lblBattleUseAddr;
        private System.Windows.Forms.Label lblBattleUseType;
        private System.Windows.Forms.NumericUpDown nudSpecialIdx;
        private System.Windows.Forms.Label lblSpecialIdx;
        private System.Windows.Forms.Label lblNote3;
        private System.Windows.Forms.Label lblNote1;
        private System.Windows.Forms.Label lblNote2;
        private System.Windows.Forms.GroupBox grpItemEffect;
        private System.Windows.Forms.TextBox txtItemEffectAddr;
        private System.Windows.Forms.Label lblItemEffectAddr;
        private System.Windows.Forms.GroupBox grpData;
    }
}