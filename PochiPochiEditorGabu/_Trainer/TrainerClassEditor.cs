using System;
using System.Linq;
using System.Windows.Forms;

using PochiPochiEditorGabu.FileReaders;
using PochiPochiEditorGabu.Helpers;
using PochiPochiEditorGabu.Managers;

namespace PochiPochiEditorGabu._Trainer
{
    public partial class TrainerClassEditor : Form
    {
        protected byte[] _romData;
        protected IniFileReader _config;
        protected TblFileReader _charmap;
        protected ReservationManager _reservationManager;

        private UIStateManager _uiStateManager;

        private EntryManager<TrainerClassNameEntry> _nameManager;
        private EntryManager<TrainerClassPrizeMultiplierEntry> _prizeMultiManager;
        private EntryManager<TrainerClassEncounterMusicEntry> _encounterMusicManager;
        private EntryManager<TrainerClassBattleMusicEntry> _battleMusicManager;
        private EntryManager<TrainerClassPokeBallEntry> _pokeBallManager;
        private EntryManager<TrainerClassBaseIVEntry> _baseIvManager;

        private bool _isUpdatingUI;
        private int _currentClassNameIdx = 0;

        public TrainerClassEditor(
            byte[] romData, 
            IniFileReader config,
            TblFileReader charmap, 
            ReservationManager reservationManager)
        {
            InitializeComponent();
            _romData = romData;
            _config = config;
            _charmap = charmap;
            _reservationManager = reservationManager;

            InitializeManagers();
            InitializeEventHandlers();
            InitializeControls();
            InitializeUIStates();

            LoadDataToUI(_currentClassNameIdx);
        }

        private void InitializeManagers()
        {
            _nameManager = EntryManager<TrainerClassNameEntry>.Create(
               _romData, _charmap, _config, "TrainerClassNameTableAddress", "TrainerClassNameCount");
            _prizeMultiManager = EntryManager<TrainerClassPrizeMultiplierEntry>.Create(
                _romData, _charmap, _config, "TrainerClassPrizeMultiplierTableAddress", "TrainerClassPrizeMultiplierCount");

            if (_config.GetBool("EnableTrainerClassEncounterMusic"))
            {
                _encounterMusicManager = EntryManager<TrainerClassEncounterMusicEntry>.Create(
                    _romData, _charmap, _config, "TrainerClassEncounterMusicTableAddress", "TrainerClassNameCount");
            }

            if (_config.GetBool("EnableTrainerClassBattleMusic"))
            {
                _battleMusicManager = EntryManager<TrainerClassBattleMusicEntry>.Create(
                    _romData, _charmap, _config, "TrainerClassBattleMusicTableAddress", "TrainerClassNameCount");
            }

            if (_config.GetBool("EnableTrainerClassPokeBall"))
            {
                _pokeBallManager = EntryManager<TrainerClassPokeBallEntry>.Create(
                    _romData, _charmap, _config, "TrainerClassPokeBallTableAddress", "TrainerClassNameCount");
            }

            if (_config.GetBool("EnableTrainerClassBaseIV"))
            {
                _baseIvManager = EntryManager<TrainerClassBaseIVEntry>.Create(
                    _romData, _charmap, _config, "TrainerClassBaseIVTableAddress", "TrainerClassNameCount");
            }
        }

        private void InitializeEventHandlers()
        {
            btnSave.Click += btnSave_Click;
            this.FormClosing += TrainerClassEditor_FormClosing;

            cmbClassName.SelectedIndexChanged += cmbClassName_SelectedIndexChanged;
            txtClassName.TextChanged += txtClassName_TextChanged;
        }

        private void InitializeControls()
        {
            // cmbClassName
            var classNames = _nameManager.Original
                             .Select(entry => entry._ClassName)
                             .ToArray();
            cmbClassName.Items.AddRange(classNames);
        }

        private void InitializeUIStates()
        {
            btnSave.Enabled = false;
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);

            _uiStateManager.AddControlsRecursive(
                grpClassData, 
                grpClassDataExtra);

            if (_config.GetBool("IsAppliedCFRU"))
            {
                if (!_config.GetBool("EnableTrainerClassEncounterMusic"))
                {
                    lblEncounterMusic.Enabled = false;
                    nudEncounterMusicIndex.Enabled = false;
                }

                if (!_config.GetBool("EnableTrainerClassBattleMusic"))
                {
                    lblBattleMusic.Enabled = false;
                    nudBattleMusicIndex.Enabled = false;
                }

                if (!_config.GetBool("EnableTrainerClassPokeBall"))
                {
                    lblPokeBall.Enabled = false;
                    nudPokeBallIndex.Enabled = false;
                }

                if (!_config.GetBool("EnableTrainerClassBaseIV"))
                {
                    lblBaseIv.Enabled = false;
                    nudBaseIv.Enabled = false;
                }
            }
            else
            {
                ControlHelper.SetControlsEnabled(grpClassDataExtra, false);
            }
        }

        private void LoadDataToUI(int idx)
        {
            _isUpdatingUI = true;

            _currentClassNameIdx = idx;
            cmbClassName.SelectedIndex = idx;
            nudClassName.Value = idx;

            // 肩書き名
            txtClassName.Text = _nameManager.Original[idx]._ClassName;

            // 賞金倍率
            var prizeEntry = _prizeMultiManager.Original.FirstOrDefault(e => e._ClassNameIndex == idx);
            if (prizeEntry == null)
            {
                prizeEntry = _prizeMultiManager.Original.FirstOrDefault(e => e._ClassNameIndex == 0xFF);
            }
            nudPrizeMulti.Value = prizeEntry?._PrizeMultiplier ?? nudPrizeMulti.Minimum;

            // 追加データ
            if (_config.GetBool("IsAppliedCFRU"))
            {
                if (_config.GetBool("EnableTrainerClassEncounterMusic"))
                {
                    DataBindingHelper.BindObjectToControls(this, _encounterMusicManager.Original[idx]);
                }

                if (_config.GetBool("EnableTrainerClassBattleMusic"))
                {
                    DataBindingHelper.BindObjectToControls(this, _battleMusicManager.Original[idx]);
                }

                if (_config.GetBool("EnableTrainerClassPokeBall"))
                {
                    DataBindingHelper.BindObjectToControls(this, _pokeBallManager.Original[idx]);
                }

                if (_config.GetBool("EnableTrainerClassBaseIV"))
                {
                    DataBindingHelper.BindObjectToControls(this, _baseIvManager.Original[idx]);
                }
            }

            _isUpdatingUI = false;
            _uiStateManager.UpdateInitialValues();
        }

        private void cmbClassName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int newIndex = cmbClassName.SelectedIndex;
            if (newIndex == _currentClassNameIdx) return;

            if (btnSave.Enabled)
            {
                ControlHelper.HandleUnsavedChanges(
                    saveAction: () =>
                    {
                        SaveCurrentData(_currentClassNameIdx);
                        LoadDataToUI(newIndex);
                    },
                    discardAction: () =>
                    {
                        RestoreData(_currentClassNameIdx);
                        LoadDataToUI(newIndex);
                    },
                    cancelAction: () =>
                    {
                        cmbClassName.SelectedIndex = _currentClassNameIdx;
                    }

                );
            }
            else
            {
                LoadDataToUI(newIndex);
            }
        }

        private void txtClassName_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int classNameEntryLength = _config.GetInt("TrainerClassNameEntryLength");
            int maxAllowedBytes = classNameEntryLength - 1;
            string currentText = txtClassName.Text;
            byte[] currentBytes = _charmap.StringToBytes(currentText, false);

            if (currentBytes.Length > maxAllowedBytes)
            {
                _isUpdatingUI = true;

                while (currentText.Length > 0)
                {
                    currentBytes = _charmap.StringToBytes(currentText, false);
                    if (currentBytes.Length <= maxAllowedBytes) break;

                    currentText = currentText.Substring(0, currentText.Length - 1);
                }

                int savedSelectionStart = txtClassName.SelectionStart;
                txtClassName.Text = currentText;
                txtClassName.SelectionStart = Math.Min(savedSelectionStart, currentText.Length);

                _isUpdatingUI = false;
            }

            string validName = _charmap.BytesToString(currentBytes, 0, currentBytes.Length);

            _isUpdatingUI = true;
            cmbClassName.Items[_currentClassNameIdx] = validName;
            _isUpdatingUI = false;
        }

        private void RestoreData(int idx)
        {
            cmbClassName.Items[idx] = _nameManager.Original[idx]._ClassName;
        }

        private void SaveCurrentData(int idx)
        {
            // 肩書き名
            _nameManager.Working[idx]._ClassName = cmbClassName.Items[idx].ToString();
            _nameManager.Save(idx);

            // 賞金倍率
            var prizeEntry = _prizeMultiManager.Working.FirstOrDefault(e => e._ClassNameIndex == idx);
            int prizeEntryIndex = -1;

            if (prizeEntry != null)
            {
                prizeEntryIndex = _prizeMultiManager.Working.IndexOf(prizeEntry);
            }
            else
            {
                prizeEntry = _prizeMultiManager.Working.FirstOrDefault(e => e._ClassNameIndex == 0xFF);
                prizeEntryIndex = _prizeMultiManager.Working.IndexOf(prizeEntry);
            }

            prizeEntry._PrizeMultiplier = (byte)nudPrizeMulti.Value;
            _prizeMultiManager.Save(prizeEntryIndex);

            // 追加データ
            if (_config.GetBool("IsAppliedCFRU"))
            {
                if (_config.GetBool("EnableTrainerClassEncounterMusic"))
                {
                    DataBindingHelper.BindControlsToObject(this, _encounterMusicManager.Working[idx]);
                    _encounterMusicManager.Save(idx);
                }

                if (_config.GetBool("EnableTrainerClassBattleMusic"))
                {
                    DataBindingHelper.BindControlsToObject(this, _battleMusicManager.Working[idx]);
                    _battleMusicManager.Save(idx);
                }

                if (_config.GetBool("EnableTrainerClassPokeBall"))
                {
                    DataBindingHelper.BindControlsToObject(this, _pokeBallManager.Working[idx]);
                    _pokeBallManager.Save(idx);
                }

                if (_config.GetBool("EnableTrainerClassBaseIV"))
                {
                    DataBindingHelper.BindControlsToObject(this, _baseIvManager.Working[idx]);
                    _baseIvManager.Save(idx);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentData(_currentClassNameIdx);
            _uiStateManager.UpdateInitialValues();
        }

        private void TrainerClassEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveCurrentData(_currentClassNameIdx);
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
