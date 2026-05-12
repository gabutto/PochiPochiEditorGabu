using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
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

        private EntryManager<PokemonNameEntry> _pokemonNameManager;
        private EntryManager<MoveNameEntry> _moveNameManager;

        private bool _isUpdatingUI = false;
        private int _currentMailIdx = 0;
        ushort[] _currentwordValues = null;
        private Dictionary<int, ComboBox> _wordGroupComboBoxMap;

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
        }

        private void InitializeEventHandlers()
        {

        }

        private void InitializeControls()
        {
            int spriteCount = _config.GetInt("MailDataCount");
            nudDataIdx.Maximum = spriteCount - 1;

            ControlHelper.AttachNumericUpDownNavigators(nudDataIdx, btnDataIdxPrev, btnDataIdxNext);
            AttachEnterEventsInGroup(grpDataWords, "rbDataWord", "txtDataWord");
            AttachEnterEventsInGroup(grpSelectWord, "rbWord", "cmbWord");

            NamesToComboBox(cmbWordPokemon1, GbaConstants.WordGroupPokemon1,
                idx => _pokemonNameManager.Original[idx]._PokemonName);
            NamesToComboBox(cmbWordPokemon2, GbaConstants.WordGroupPokemon2,
                idx => _pokemonNameManager.Original[idx]._PokemonName);
            NamesToComboBox(cmbWordMove1, GbaConstants.WordGroupMove1,
                idx => _moveNameManager.Original[idx]._MoveName);
            NamesToComboBox(cmbWordMove2, GbaConstants.WordGroupMove2,
                idx => _moveNameManager.Original[idx]._MoveName);
            WordsToComboBox(cmbWordTrainer, GbaConstants.WordGroupTrainer);
            WordsToComboBox(cmbWordStatus, GbaConstants.WordGroupStatus);
            WordsToComboBox(cmbWordBattle, GbaConstants.WordGroupBattle);
            WordsToComboBox(cmbWordGreeting, GbaConstants.WordGroupGreeting);
            WordsToComboBox(cmbWordPeople, GbaConstants.WordGroupPeople);
            WordsToComboBox(cmbWordVoice, GbaConstants.WordGroupVoice);
            WordsToComboBox(cmbWordSpeech, GbaConstants.WordGroupSpeech);
            WordsToComboBox(cmbWordEnding, GbaConstants.WordGroupEnding);
            WordsToComboBox(cmbWordFeeling, GbaConstants.WordGroupFeeling);
            WordsToComboBox(cmbWordCondition, GbaConstants.WordGroupCondition);
            WordsToComboBox(cmbWordAction, GbaConstants.WordGroupAction);
            WordsToComboBox(cmbWordLifestyle, GbaConstants.WordGroupLifestyle);
            WordsToComboBox(cmbWordHobby, GbaConstants.WordGroupHobby);
            WordsToComboBox(cmbWordTime, GbaConstants.WordGroupTime);
            WordsToComboBox(cmbWordMisc, GbaConstants.WordGroupMisc);
            WordsToComboBox(cmbWordAdjective, GbaConstants.WordGroupAdjective);
            WordsToComboBox(cmbWordEvent, GbaConstants.WordGroupEvent);
            WordsToComboBox(cmbWordTrendy, GbaConstants.WordGroupTrendy);

            _wordGroupComboBoxMap = new Dictionary<int, ComboBox>
            {
                { GbaConstants.WordGroupPokemon1,  cmbWordPokemon1 },
                { GbaConstants.WordGroupPokemon2,  cmbWordPokemon2 },
                { GbaConstants.WordGroupMove1,     cmbWordMove1 },
                { GbaConstants.WordGroupMove2,     cmbWordMove2 },
                { GbaConstants.WordGroupTrainer,   cmbWordTrainer },
                { GbaConstants.WordGroupStatus,    cmbWordStatus },
                { GbaConstants.WordGroupBattle,    cmbWordBattle },
                { GbaConstants.WordGroupGreeting,  cmbWordGreeting },
                { GbaConstants.WordGroupPeople,    cmbWordPeople },
                { GbaConstants.WordGroupVoice,     cmbWordVoice },
                { GbaConstants.WordGroupSpeech,    cmbWordSpeech },
                { GbaConstants.WordGroupEnding,    cmbWordEnding },
                { GbaConstants.WordGroupFeeling,   cmbWordFeeling },
                { GbaConstants.WordGroupCondition, cmbWordCondition },
                { GbaConstants.WordGroupAction,    cmbWordAction },
                { GbaConstants.WordGroupLifestyle, cmbWordLifestyle },
                { GbaConstants.WordGroupHobby,     cmbWordHobby },
                { GbaConstants.WordGroupTime,      cmbWordTime },
                { GbaConstants.WordGroupMisc,      cmbWordMisc },
                { GbaConstants.WordGroupAdjective, cmbWordAdjective },
                { GbaConstants.WordGroupEvent,     cmbWordEvent },
                { GbaConstants.WordGroupTrendy,    cmbWordTrendy }
            };
        }

        private void AttachEnterEventsInGroup(Control container, string radioPrefix, string targetPrefix)
        {
            foreach (var rb in container.Controls.OfType<RadioButton>())
            {
                string targetName = rb.Name.Replace(radioPrefix, targetPrefix);
                Control target = container.Controls.Find(targetName, false)
                                                   .FirstOrDefault();
                ControlHelper.AttachEnterEvent(rb, target);
            }
        }

        private void WordsToComboBox(ComboBox cmb, int grpIdx)
        {
            uint? tableAddr = _wordGroupManager.Original[grpIdx].pWordTextEntry - GbaConstants.BaseAddr;
            int entryCount = _wordGroupManager.Original[grpIdx]._Count1;
            EntryManager<WordTextEntry> _wordTextManager = new EntryManager<WordTextEntry>(_romData, _tblReader);
            _wordTextManager.Load(tableAddr, entryCount);

            cmb.BeginUpdate();
            cmb.Items.Clear();

            for (int i = 0; i < entryCount; i++)
            {
                uint textAddr = _wordTextManager.Original[i].pTextAddr - GbaConstants.BaseAddr;
                string text = _tblReader.BytesToString(_romData, (int)textAddr, 16);
                int wordIdx = _wordTextManager.Original[i]._Idx;

                if (cmb.Name == "cmbWordTrainer" || cmb.Name == "cmbWordEvent")
                {
                    wordIdx = i;
                }

                var item = new WordItem
                {
                    Text = text,
                    Index = wordIdx
                };
                cmb.Items.Add(item);
            }

            cmb.EndUpdate();
            cmb.SelectedIndex = 0;
        }

        private void NamesToComboBox(ComboBox cmb, int grpIdx, Func<int, string> nameResolver)
        {
            uint? tableAddr = _wordGroupManager.Original[grpIdx].pWordTextEntry - GbaConstants.BaseAddr;
            int entryCount = _wordGroupManager.Original[grpIdx]._Count1;
            EntryManager<WordNameEntry> _wordNameManager = new EntryManager<WordNameEntry>(_romData, _tblReader);
            _wordNameManager.Load(tableAddr, entryCount);

            cmb.BeginUpdate();
            cmb.Items.Clear();

            for (int i = 0; i < entryCount; i++)
            {
                int idx = _wordNameManager.Original[i]._Idx;
                string name = nameResolver(idx);

                var item = new WordItem
                {
                    Text = name,
                    Index = idx
                };
                cmb.Items.Add(item);
            }

            cmb.EndUpdate();
            cmb.SelectedIndex = 0;
        }

        private void InitializeUIStates()
        {
            btnSave.Enabled = false;
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);
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

            for (int i = 0; i < _currentwordValues.Length; i++)
            {
                ushort val = _currentwordValues[i];
                string wordText = string.Empty;
                bool enabled = true;

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
                    int grp = (val >> 9) & 0x7F;
                    int wIdx = val & 0x1FF;
                    wordText = GetWordTextFromGroupAndIndex(grp, wIdx);

                    if (firstValidIndex == -1)
                    {
                        firstValidIndex = i;
                    }
                }

                var txtBox = grpDataWords.Controls.Find($"txtDataWord{i + 1}", true)
                                                  .FirstOrDefault() as TextBox;
                if (txtBox != null)
                {
                    txtBox.Text = wordText;
                    txtBox.Enabled = enabled;
                }

                var radioButton = grpDataWords.Controls.Find($"rbDataWord{i + 1}", true)
                                                       .FirstOrDefault() as RadioButton;
                if (radioButton != null)
                {
                    radioButton.Enabled = enabled;
                }
            }

            if (firstValidIndex != -1)
            {
                var firstRadio = grpDataWords.Controls.Find($"rbDataWord{firstValidIndex + 1}", true)
                                                      .FirstOrDefault() as RadioButton;
                if (firstRadio != null)
                {
                    firstRadio.Checked = true;
                }
            }

            _isUpdatingUI = false;
            _uiStateManager.UpdateInitialValues();
        }

        private string GetWordTextFromGroupAndIndex(int group, int index)
        {
            if (_wordGroupComboBoxMap.TryGetValue(group, out ComboBox cmb))
            {
                foreach (WordItem item in cmb.Items)
                {
                    if (item.Index == index)
                        return item.Text;
                }
            }

            return string.Empty;
        }
    }
}
