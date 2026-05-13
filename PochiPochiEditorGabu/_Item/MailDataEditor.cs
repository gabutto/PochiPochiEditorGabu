using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using PochiPochiEditorGabu.Constants;
using PochiPochiEditorGabu.FileReaders;
using PochiPochiEditorGabu.Helpers;
using PochiPochiEditorGabu.Managers;

namespace PochiPochiEditorGabu._Item
{
    public partial class MailDataEditor : Form
    {
        protected byte[] _romData;
        protected IniFileReader _config;
        protected TblFileReader _tblReader;
        protected ReservationManager _reservationManager;

        private UIStateManager _uiStateManager;

        private EntryManager<MailDataWordEntry> _mailDataManager;
        private EntryManager<WordGroupEntry> _wordGroupManager;
        private Dictionary<int, (RadioButton, TextBox)> _mailSlots;
        private Dictionary<int, WordGroupDefinition> _wordGroups;

        private EntryManager<PokemonNameEntry> _pokemonNameManager;
        private EntryManager<MoveNameEntry> _moveNameManager;

        private bool _isUpdatingUI = false;
        private int _currentMailIdx = 0;
        private ushort[] _currentwordValues = null;

        private enum WordGroupType
        {
            Normal,
            PokemonName,
            MoveName
        }

        private class WordGroupDefinition
        {
            public ComboBox ComboBox { get; }
            public RadioButton RadioButton { get; }
            public WordGroupType GroupType { get; }

            public WordGroupDefinition(
                ComboBox comboBox,
                RadioButton radioButton,
                WordGroupType groupType)
            {
                ComboBox = comboBox;
                RadioButton = radioButton;
                GroupType = groupType;
            }
        }

        private class WordItem
        {
            public string Text { get; set; }
            public int Index { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        public MailDataEditor(
            byte[] romData,
            IniFileReader config,
            TblFileReader tblReader,
            ReservationManager reservationManager)
        {
            InitializeComponent();
            _romData = romData;
            _config = config;
            _tblReader = tblReader;
            _reservationManager = reservationManager;

            InitializeManagers();
            InitializeEventHandlers();
            InitializeControls();
            InitializeUIStates();

            LoadMailData(_currentMailIdx);
        }

        private void InitializeManagers()
        {
            _mailDataManager = EntryManager<MailDataWordEntry>.Create(
                _romData, _tblReader, _config, "MailDataTableAddress", "MailDataCount");

            _wordGroupManager = EntryManager<WordGroupEntry>.Create(
                _romData, _tblReader, _config, "WordGroupTableAddress", "WordGroupCount");

            // pokemon name
            _pokemonNameManager = EntryManager<PokemonNameEntry>.Create(
                _romData, _tblReader, _config, "PokemonNameTableAddress", "PokemonNameCount");

            // move name
            _moveNameManager = EntryManager<MoveNameEntry>.Create(
                _romData, _tblReader, _config, "MoveNameTableAddress", "MoveNameCount");

            // mail slot mapping
            _mailSlots = new Dictionary<int, (RadioButton, TextBox)>
            {
                [1] = (rbDataWord1, txtDataWord1),
                [2] = (rbDataWord2, txtDataWord2),
                [3] = (rbDataWord3, txtDataWord3),
                [4] = (rbDataWord4, txtDataWord4),
                [5] = (rbDataWord5, txtDataWord5),
                [6] = (rbDataWord6, txtDataWord6),
                [7] = (rbDataWord7, txtDataWord7),
                [8] = (rbDataWord8, txtDataWord8),
                [9] = (rbDataWord9, txtDataWord9),
                [10] = (rbDataWord10, txtDataWord10)
            };

            // group mapping
            _wordGroups = new Dictionary<int, WordGroupDefinition>
            {
                [GbaConstants.WordGroupPokemon1] = new WordGroupDefinition(
                    cmbWordPokemon1,
                    rbWordPokemon1,
                    WordGroupType.PokemonName),
                [GbaConstants.WordGroupPokemon2] = new WordGroupDefinition(
                    cmbWordPokemon2,
                    rbWordPokemon2,
                    WordGroupType.PokemonName),
                [GbaConstants.WordGroupMove1] = new WordGroupDefinition(
                    cmbWordMove1,
                    rbWordMove1,
                    WordGroupType.MoveName),
                [GbaConstants.WordGroupMove2] = new WordGroupDefinition(
                    cmbWordMove2,
                    rbWordMove2,
                    WordGroupType.MoveName),
                [GbaConstants.WordGroupTrainer] = new WordGroupDefinition(
                    cmbWordTrainer,
                    rbWordTrainer,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupStatus] = new WordGroupDefinition(
                    cmbWordStatus,
                    rbWordStatus,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupBattle] = new WordGroupDefinition(
                    cmbWordBattle,
                    rbWordBattle,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupGreeting] = new WordGroupDefinition(
                    cmbWordGreeting,
                    rbWordGreeting,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupPeople] = new WordGroupDefinition(
                    cmbWordPeople,
                    rbWordPeople,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupVoice] = new WordGroupDefinition(
                    cmbWordVoice,
                    rbWordVoice,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupSpeech] = new WordGroupDefinition(
                    cmbWordSpeech,
                    rbWordSpeech,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupEnding] = new WordGroupDefinition(
                    cmbWordEnding,
                    rbWordEnding,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupFeeling] = new WordGroupDefinition(
                    cmbWordFeeling,
                    rbWordFeeling,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupCondition] = new WordGroupDefinition(
                    cmbWordCondition,
                    rbWordCondition,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupAction] = new WordGroupDefinition(
                    cmbWordAction,
                    rbWordAction,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupLifestyle] = new WordGroupDefinition(
                    cmbWordLifestyle,
                    rbWordLifestyle,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupHobby] = new WordGroupDefinition(
                    cmbWordHobby,
                    rbWordHobby,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupTime] = new WordGroupDefinition(
                    cmbWordTime,
                    rbWordTime,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupMisc] = new WordGroupDefinition(
                    cmbWordMisc,
                    rbWordMisc,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupAdjective] = new WordGroupDefinition(
                    cmbWordAdjective,
                    rbWordAdjective,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupEvent] = new WordGroupDefinition(
                    cmbWordEvent,
                    rbWordEvent,
                    WordGroupType.Normal),
                [GbaConstants.WordGroupTrendy] = new WordGroupDefinition(
                    cmbWordTrendy,
                    rbWordTrendy,
                    WordGroupType.Normal)
            };
        }

        private void InitializeEventHandlers()
        {
            btnSave.Click += btnSave_Click;
            this.FormClosing += MailDataEditor_FormClosing;

            nudDataIdx.ValueChanged += nudDataIdx_ValueChanged;
            nudDataWordCount.ValueChanged += nudDataWordCount_ValueChanged;
            btnRepalceWord.Click += btnRepalceWord_Click;
        }

        private void InitializeControls()
        {
            int mailCount = _config.GetInt("MailDataCount");
            nudDataIdx.Maximum = mailCount - 1;

            ControlHelper.AttachNumericUpDownNavigators(nudDataIdx, btnDataIdxPrev, btnDataIdxNext);

            // mail word slots focus
            foreach (var (radio, textBox) in _mailSlots.Values)
            {
                ControlHelper.AttachEnterEvent(radio, textBox);
            }

            // word combo
            foreach (var kvp in _wordGroups)
            {
                int groupIdx = kvp.Key;
                var def = kvp.Value;

                uint? tableAddr = _wordGroupManager.Original[groupIdx].pWordTextEntry - GbaConstants.BaseAddr;
                int entryCount = _wordGroupManager.Original[groupIdx]._Count1;

                def.ComboBox.BeginUpdate();
                def.ComboBox.Items.Clear();

                switch (def.GroupType)
                {
                    case WordGroupType.PokemonName:
                        {
                            EntryManager<WordNameEntry> manager = new EntryManager<WordNameEntry>(_romData, _tblReader);
                            manager.Load(tableAddr, entryCount);
                            for (int i = 0; i < entryCount; i++)
                            {
                                int idx = manager.Original[i]._Idx;
                                def.ComboBox.Items.Add(new WordItem
                                {
                                    Text = _pokemonNameManager.Original[idx]._PokemonName,
                                    Index = idx
                                });
                            }
                        }
                        break;

                    case WordGroupType.MoveName:
                        {
                            EntryManager<WordNameEntry> manager = new EntryManager<WordNameEntry>(_romData, _tblReader);
                            manager.Load(tableAddr, entryCount);
                            for (int i = 0; i < entryCount; i++)
                            {
                                int idx = manager.Original[i]._Idx;
                                def.ComboBox.Items.Add(new WordItem
                                {
                                    Text = _moveNameManager.Original[idx]._MoveName,
                                    Index = idx
                                });
                            }
                        }
                        break;

                    case WordGroupType.Normal:
                        {
                            EntryManager<WordTextEntry> manager = new EntryManager<WordTextEntry>(_romData, _tblReader);
                            manager.Load(tableAddr, entryCount);
                            for (int i = 0; i < entryCount; i++)
                            {
                                uint textAddr = manager.Original[i].pTextAddr - GbaConstants.BaseAddr;
                                string text = _tblReader.BytesToString(_romData, (int)textAddr, 16);
                                int wordIdx = manager.Original[i]._Idx;

                                // trainer, event
                                if (def.ComboBox == cmbWordTrainer || def.ComboBox == cmbWordEvent)
                                {
                                    wordIdx = i;
                                }

                                def.ComboBox.Items.Add(new WordItem { Text = text, Index = wordIdx });
                            }
                        }
                        break;
                }

                def.ComboBox.EndUpdate();
                if (def.ComboBox.Items.Count > 0)
                {
                    def.ComboBox.SelectedIndex = 0;
                }

                ControlHelper.AttachEnterEvent(def.RadioButton, def.ComboBox);
            }
        }

        private void InitializeUIStates()
        {
            btnSave.Enabled = false;
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);

            _uiStateManager.AddControls(nudDataWordCount);

            int entrySize = _mailDataManager.GetEntrySize();
            _uiStateManager.AddBinaries(("MailWords", new byte[entrySize]));
        }

        private byte[] GetWordValuesBytes()
        {
            if (_currentwordValues == null) return new byte[0];

            byte[] bytes = new byte[_currentwordValues.Length * sizeof(ushort)];
            Buffer.BlockCopy(_currentwordValues, 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private void LoadMailData(int idx)
        {
            _isUpdatingUI = true;
            _currentMailIdx = idx;

            var mailData = _mailDataManager.Original[idx];
            _currentwordValues = new ushort[]
            {
                mailData._Word1, mailData._Word2, mailData._Word3,
                mailData._Word4, mailData._Word5, mailData._Word6,
                mailData._Word7, mailData._Word8, mailData._Word9,
                mailData._Word10
            };

            ControlHelper.ResetControls(grpDataWords);
            bool terminatorFound = false;
            int firstValidIndex = -1;
            int wordCount = 0;

            for (int i = 0; i < _currentwordValues.Length; i++)
            {
                ushort val = _currentwordValues[i];
                string wordText = string.Empty;
                bool enabled = true;
                int slot = i + 1;

                if (val == 0xFFFF)
                {
                    terminatorFound = true;
                    enabled = false;
                }
                else if (terminatorFound)
                {
                    enabled = false;
                }
                else
                {
                    int groupIdx = (val >> 9) & 0x7F;
                    int wordIdx = val & 0x1FF;
                    wordText = GetWordTextFromComboBox(groupIdx, wordIdx);
                    wordCount++;

                    if (firstValidIndex == -1)
                    {
                        firstValidIndex = i;
                    }
                }

                if (_mailSlots.TryGetValue(slot, out var controls))
                {
                    controls.Item2.Text = wordText;
                    controls.Item2.Enabled = enabled;
                    controls.Item1.Enabled = enabled;
                }
            }

            if (firstValidIndex != -1)
            {
                int firstSlot = firstValidIndex + 1;
                if (_mailSlots.TryGetValue(firstSlot, out var firstControls))
                {
                    firstControls.Item1.Checked = true;
                }
            }

            nudDataWordCount.Value = wordCount;
            _uiStateManager.UpdateBinary("MailWords", GetWordValuesBytes());

            _isUpdatingUI = false;
            _uiStateManager.UpdateInitialValues();
        }

        private string GetWordTextFromComboBox(int groupIdx, int wordIdx)
        {
            if (_wordGroups.TryGetValue(groupIdx, out var def))
            {
                foreach (var item in def.ComboBox.Items)
                {
                    var wordItem = (WordItem)item;
                    if (wordItem.Index == wordIdx)
                    {
                        return wordItem.Text;
                    }
                }
            }
            return string.Empty;
        }

        private void nudDataIdx_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int nextIdx = (int)nudDataIdx.Value;

            if (btnSave.Enabled)
            {
                ControlHelper.HandleUnsavedChanges(
                    () => SaveCurrentMailData(_currentMailIdx),
                    () => LoadMailData(nextIdx),
                    () =>
                    {
                        _isUpdatingUI = true;
                        nudDataIdx.Value = _currentMailIdx;
                        _isUpdatingUI = false;
                    }
                );
            }
            else
            {
                LoadMailData(nextIdx);
            }
        }

        private void nudDataWordCount_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int newCount = (int)nudDataWordCount.Value;

            ushort defaultWord = 0x0000;
            string defaultText = string.Empty;
            var firstGroup = _wordGroups.FirstOrDefault();
            if (firstGroup.Value != null && firstGroup.Value.ComboBox.Items.Count > 0)
            {
                var item = (WordItem)firstGroup.Value.ComboBox.Items[0];
                defaultWord = (ushort)((firstGroup.Key << 9) | (item.Index & 0x1FF));
                defaultText = item.Text;
            }

            for (int i = 0; i < _currentwordValues.Length; i++)
            {
                int slot = i + 1;
                bool isEnabled = i < newCount;

                if (_mailSlots.TryGetValue(slot, out var controls))
                {
                    controls.Item1.Enabled = isEnabled;
                    controls.Item2.Enabled = isEnabled;

                    if (!isEnabled)
                    {
                        controls.Item1.Checked = false;
                        controls.Item2.Text = string.Empty;

                        if (i == newCount)
                        {
                            _currentwordValues[i] = 0xFFFF; // terminator
                        }
                        else
                        {
                            _currentwordValues[i] = 0x0000; // padding
                        }
                    }

                    else if (_currentwordValues[i] == 0xFFFF || _currentwordValues[i] == 0x0000)
                    {
                        _currentwordValues[i] = defaultWord;
                        controls.Item2.Text = defaultText;
                    }
                }
            }

            if (newCount > 0)
            {
                bool anyChecked = false;
                for (int i = 0; i < newCount; i++)
                {
                    if (_mailSlots[i + 1].Item1.Checked)
                    {
                        anyChecked = true;
                        break;
                    }
                }
                if (!anyChecked)
                {
                    _mailSlots[1].Item1.Checked = true;
                }
            }

            _uiStateManager.UpdateBinary("MailWords", GetWordValuesBytes());
        }

        private void btnRepalceWord_Click(object sender, EventArgs e)
        {
            int targetSlot = -1;
            foreach (var kvp in _mailSlots)
            {
                if (kvp.Value.Item1.Checked && kvp.Value.Item1.Enabled)
                {
                    targetSlot = kvp.Key;
                    break;
                }
            }

            if (targetSlot == -1)
            {
                MessageBox.Show(
                    "代入先が選択されていないか、無効です。",
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            int selectedGroupIdx = -1;
            int selectedWordIdx = -1;
            string selectedText = string.Empty;

            foreach (var kvp in _wordGroups)
            {
                if (kvp.Value.RadioButton.Checked)
                {
                    selectedGroupIdx = kvp.Key;
                    if (kvp.Value.ComboBox.SelectedItem is WordItem item)
                    {
                        selectedWordIdx = item.Index;
                        selectedText = item.Text;
                    }
                    break;
                }
            }

            if (selectedGroupIdx == -1 || selectedWordIdx == -1)
            {
                MessageBox.Show(
                    "代入元が選択されていません。",
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            int slotIdx = targetSlot - 1;
            ushort newWordValue = (ushort)((selectedGroupIdx << 9) | (selectedWordIdx & 0x1FF));

            _currentwordValues[slotIdx] = newWordValue;
            _mailSlots[targetSlot].Item2.Text = selectedText;

            _uiStateManager.UpdateBinary("MailWords", GetWordValuesBytes());
        }

        private void SaveCurrentMailData(int idx)
        {
            var mailData = _mailDataManager.Working[idx];

            mailData._Word1 = _currentwordValues[0];
            mailData._Word2 = _currentwordValues[1];
            mailData._Word3 = _currentwordValues[2];
            mailData._Word4 = _currentwordValues[3];
            mailData._Word5 = _currentwordValues[4];
            mailData._Word6 = _currentwordValues[5];
            mailData._Word7 = _currentwordValues[6];
            mailData._Word8 = _currentwordValues[7];
            mailData._Word9 = _currentwordValues[8];
            mailData._Word10 = _currentwordValues[9];

            _mailDataManager.Save(idx);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentMailData(_currentMailIdx);
            _uiStateManager.UpdateInitialValues();
        }

        private void MailDataEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveCurrentMailData(_currentMailIdx);
                    },
                    () =>
                    {
                        //
                    },
                    () =>
                    {
                        e.Cancel = true;
                    }
                );
            }
        }
    }
}
