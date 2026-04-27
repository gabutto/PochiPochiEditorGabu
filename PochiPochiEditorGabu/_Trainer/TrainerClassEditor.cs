using System;
using System.Collections.Generic;
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
        protected TblFileReader _tblReader;
        protected ReservationManager _reservationManager;
        protected UIStateManager _uiStateManager;

        private EntryManager<TrainerClassNameEntry> _nameManager;
        private EntryManager<TrainerClassPrizeMultiplierEntry> _prizeMultiManager;
        private EntryManager<TrainerClassEncounterMusicEntry> _encounterMusicManager;
        private EntryManager<TrainerClassBattleMusicEntry> _battleMusicManager;
        private EntryManager<TrainerClassPokeBallEntry> _pokeBallManager;
        private EntryManager<TrainerClassBaseIVEntry> _baseIvManager;

        private bool _isUpdatingUI;
        private int _currentClassNameIdx = 0;

        public TrainerClassEditor(byte[] romData, IniFileReader config, TblFileReader tblReader, ReservationManager reservationManager)
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

            LoadDataToUI(_currentClassNameIdx);
        }

        private void InitializeManagers()
        {
            uint? classNameTableAddr = _config.GetAddr("TrainerClassNameTableAddress");
            int classNameEntryLength = _config.GetInt("TrainerClassNameEntryLength");
            int classNameCount = _config.GetInt("TrainerClassNameCount");
            var lengths = new Dictionary<string, int> { { "TrainerClassNameEntryLength", classNameEntryLength } };
            _nameManager = new EntryManager<TrainerClassNameEntry>(_romData, _tblReader, lengths);
            _nameManager.Load(classNameTableAddr, classNameCount);

            uint? classMultiTableAddr = _config.GetAddr("TrainerClassPrizeMultiplierTableAddress");
            int classMultiCount = _config.GetInt("TrainerClassPrizeMultiplierCount");
            _prizeMultiManager = new EntryManager<TrainerClassPrizeMultiplierEntry>(_romData, _tblReader);
            _prizeMultiManager.Load(classMultiTableAddr, classMultiCount);

            if (_config.GetBool("EnableTrainerClassEncounterMusic"))
            {
                uint? encounterMusicTableAddr = _config.GetAddr("TrainerClassEncounterMusicTableAddress");
                _encounterMusicManager = new EntryManager<TrainerClassEncounterMusicEntry>(_romData, _tblReader);
                _encounterMusicManager.Load(encounterMusicTableAddr, classNameCount);
            }

            if (_config.GetBool("EnableTrainerClassBattleMusic"))
            {
                uint? battleMusicTableAddr = _config.GetAddr("TrainerClassBattleMusicTableAddress");
                _battleMusicManager = new EntryManager<TrainerClassBattleMusicEntry>(_romData, _tblReader);
                _battleMusicManager.Load(battleMusicTableAddr, classNameCount);
            }

            if (_config.GetBool("EnableTrainerClassPokeBall"))
            {
                uint? pokeBallTableAddr = _config.GetAddr("TrainerClassPokeBallTableAddress");
                _pokeBallManager = new EntryManager<TrainerClassPokeBallEntry>(_romData, _tblReader);
                _pokeBallManager.Load(pokeBallTableAddr, classNameCount);
            }

            if (_config.GetBool("EnableTrainerClassBaseIV"))
            {
                uint? baseIvTableAddr = _config.GetAddr("TrainerClassBaseIVTableAddress");
                _baseIvManager = new EntryManager<TrainerClassBaseIVEntry>(_romData, _tblReader);
                _baseIvManager.Load(baseIvTableAddr, classNameCount);
            }
        }

        private void InitializeEventHandlers()
        {
            cmbClassName.SelectedIndexChanged += cmbClassName_SelectedIndexChanged;
            txtClassName.TextChanged += txtClassName_TextChanged;
            btnSave.Click += btnSave_Click;
            this.FormClosing += TrainerClassEditor_FormClosing;
        }

        private void InitializeControls()
        {
            // cmbClassName
            var classNames = _nameManager.Working
                             .Select(entry => entry._ClassName)
                             .ToArray();
            cmbClassName.Items.AddRange(classNames);
        }

        private void InitializeUIStates()
        {
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

            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);
            btnSave.Enabled = false;
            _uiStateManager.AddControls(
                txtClassName, nudPrizeMulti,
                nudEncounterMusicIndex, nudBattleMusicIndex, nudPokeBallIndex, nudBaseIv);
        }

        private void LoadDataToUI(int idx)
        {
            _isUpdatingUI = true;

            _currentClassNameIdx = idx;
            if (cmbClassName.SelectedIndex != idx)
            {
                cmbClassName.SelectedIndex = idx;
            }
            nudClassName.Value = idx;

            // Load class name
            txtClassName.Text = _nameManager.Working[idx]._ClassName;

            // Load class prize multi
            var prizeEntry = _prizeMultiManager.Working.FirstOrDefault(e => e._ClassNameIndex == idx);
            if (prizeEntry == null)
            {
                prizeEntry = _prizeMultiManager.Working.FirstOrDefault(e => e._ClassNameIndex == 0xFF);
            }
            nudPrizeMulti.Value = prizeEntry?._PrizeMultiplier ?? nudPrizeMulti.Minimum;

            // extra data
            if (_config.GetBool("IsAppliedCFRU"))
            {
                if (_config.GetBool("EnableTrainerClassEncounterMusic"))
                {
                    DataBindingHelper.BindObjectToControls(this, _encounterMusicManager.Working[idx]);
                }

                if (_config.GetBool("EnableTrainerClassBattleMusic"))
                {
                    DataBindingHelper.BindObjectToControls(this, _battleMusicManager.Working[idx]);
                }

                if (_config.GetBool("EnableTrainerClassPokeBall"))
                {
                    DataBindingHelper.BindObjectToControls(this, _pokeBallManager.Working[idx]);
                }

                if (_config.GetBool("EnableTrainerClassBaseIV"))
                {
                    DataBindingHelper.BindObjectToControls(this, _baseIvManager.Working[idx]);
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
                _isUpdatingUI = true;

                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveCurrentData(_currentClassNameIdx);
                        LoadDataToUI(newIndex);
                    },
                    () =>
                    {
                        RestoreClassName(_currentClassNameIdx);
                        LoadDataToUI(newIndex);
                    },
                    () =>
                    {
                        cmbClassName.SelectedIndex = _currentClassNameIdx;
                    }

                );

                _isUpdatingUI = false;
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
            byte[] currentBytes = _tblReader.StringToBytes(currentText, false);

            if (currentBytes.Length > maxAllowedBytes)
            {
                _isUpdatingUI = true;

                while (currentText.Length > 0)
                {
                    currentBytes = _tblReader.StringToBytes(currentText, false);
                    if (currentBytes.Length <= maxAllowedBytes) break;

                    currentText = currentText.Substring(0, currentText.Length - 1);
                }

                int savedSelectionStart = txtClassName.SelectionStart;
                txtClassName.Text = currentText;
                txtClassName.SelectionStart = Math.Min(savedSelectionStart, currentText.Length);

                _isUpdatingUI = false;
            }

            string validName = _tblReader.BytesToString(currentBytes, 0, currentBytes.Length);

            _isUpdatingUI = true;

            int idx = _currentClassNameIdx;
            cmbClassName.Items[idx] = validName;
            _nameManager.Working[idx]._ClassName = validName;

            _isUpdatingUI = false;
        }

        private void SaveCurrentData(int idx)
        {
            _nameManager.Save(idx);

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

            // extra data
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

        private void RestoreClassName(int idx)
        {
            var originalEntry = _nameManager.Original[idx];
            var workingEntry = _nameManager.Working[idx];

            var restoredEntry = CloneHelper.Clone(originalEntry);
            workingEntry._ClassName = restoredEntry._ClassName;

            cmbClassName.Items[idx] = originalEntry._ClassName;
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
                _isUpdatingUI = true;

                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveCurrentData(_currentClassNameIdx);
                    },
                    () =>
                    {
                        // unnecessary
                    },
                    () =>
                    {
                        e.Cancel = true;
                    }
                );

                _isUpdatingUI = false;
            }
        }
    }
}
