using System;
using System.Linq;
using System.Windows.Forms;

using PochiPochiEditorGabu.FileReaders;
using PochiPochiEditorGabu.Helpers;
using PochiPochiEditorGabu.Managers;

namespace PochiPochiEditorGabu._Move
{
    public partial class TmHmTutorEditor : Form
    {
        protected byte[] _romData;
        protected IniFileReader _config;
        protected TblFileReader _tblReader;
        protected ReservationManager _reservationManager;

        private UIStateManager _uiStateManager;

        private EntryManager<MoveNameEntry> _moveNameManager;
        private EntryManager<TmHmMoveEntry> _tmHmListManager;
        private EntryManager<TutorMoveEntry> _tutorListManager;

        private bool _isUpdatingUI = false;

        public TmHmTutorEditor(
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
            InitializeControls();
            InitializeUIStates();
            InitializeLists();
            InitializeEventHandlers();
        }

        private void InitializeManagers()
        {
            // move name
            _moveNameManager = EntryManager<MoveNameEntry>.Create(
                _romData, _tblReader, _config, "MoveNameTableAddress", "MoveNameCount");

            // tm hm
            _tmHmListManager = EntryManager<TmHmMoveEntry>.Create(
                _romData, _tblReader, _config, "TmHmListTableAddress", "TmHmCount");

            // tutor
            _tutorListManager = EntryManager<TutorMoveEntry>.Create(
                _romData, _tblReader, _config, "TutorListTableAddress", "TutorCount");
        }

        private void InitializeControls()
        {
            // move for cmb
            var moveNames = _moveNameManager.Original
                             .Select(entry => entry._MoveName)
                             .ToArray();

            ControlHelper.SetupComboBoxItems(cmbMove1, -1, moveNames);
            ControlHelper.SetupComboBoxItems(cmbMove2, -1, moveNames);
        }

        private void InitializeUIStates()
        {
            btnSave.Enabled = false;
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);

            _uiStateManager.AddBinaries(
                ("TmHm", GetTmHmBinary()),
                ("Tutor", GetTutorBinary())
            );
        }

        private void InitializeLists()
        {
            _isUpdatingUI = true;

            int tmCount = _config.GetInt("TmCount");

            // tm hm
            lstTmHm.BeginUpdate();
            lstTmHm.Items.Clear();
            for (int i = 0; i < _tmHmListManager.Working.Count; i++)
            {
                lstTmHm.Items.Add(GetTmHmDisplayString(i, _tmHmListManager.Working[i]._MoveIdx, tmCount));
            }
            lstTmHm.EndUpdate();

            // tutor
            lstTutor.BeginUpdate();
            lstTutor.Items.Clear();
            for (int i = 0; i < _tutorListManager.Working.Count; i++)
            {
                lstTutor.Items.Add(GetTutorDisplayString(i, _tutorListManager.Working[i]._MoveIdx));
            }
            lstTutor.EndUpdate();

            if (lstTmHm.Items.Count > 0)
            {
                lstTmHm.SelectedIndex = 0;
                cmbMove1.SelectedIndex = _tmHmListManager.Working[0]._MoveIdx;
            }

            if (lstTutor.Items.Count > 0)
            {
                lstTutor.SelectedIndex = 0;
                cmbMove2.SelectedIndex = _tutorListManager.Working[0]._MoveIdx;
            }

            _isUpdatingUI = false;
        }

        private void InitializeEventHandlers()
        {
            lstTmHm.SelectedIndexChanged += LstTmHm_SelectedIndexChanged;
            cmbMove1.SelectedIndexChanged += CmbMove1_SelectedIndexChanged;

            lstTutor.SelectedIndexChanged += LstTutor_SelectedIndexChanged;
            cmbMove2.SelectedIndexChanged += CmbMove2_SelectedIndexChanged;

            btnSave.Click += BtnSave_Click;
            this.FormClosing += TmHmTutorEditor_FormClosing;
        }

        private void LstTmHm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI || lstTmHm.SelectedIndex < 0) return;

            _isUpdatingUI = true;
            cmbMove1.SelectedIndex = _tmHmListManager.Working[lstTmHm.SelectedIndex]._MoveIdx;
            _isUpdatingUI = false;
        }

        private void CmbMove1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI || lstTmHm.SelectedIndex < 0 || cmbMove1.SelectedIndex < 0) return;

            int index = lstTmHm.SelectedIndex;
            ushort newMoveIdx = (ushort)cmbMove1.SelectedIndex;

            if (_tmHmListManager.Working[index]._MoveIdx != newMoveIdx)
            {
                _tmHmListManager.Working[index]._MoveIdx = newMoveIdx;

                _isUpdatingUI = true;
                lstTmHm.Items[index] = GetTmHmDisplayString(index, newMoveIdx, _config.GetInt("TmCount"));
                _isUpdatingUI = false;

                _uiStateManager.UpdateBinary("TmHm", GetTmHmBinary());
            }
        }

        private void LstTutor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI || lstTutor.SelectedIndex < 0) return;

            _isUpdatingUI = true;
            cmbMove2.SelectedIndex = _tutorListManager.Working[lstTutor.SelectedIndex]._MoveIdx;
            _isUpdatingUI = false;
        }

        private void CmbMove2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI || lstTutor.SelectedIndex < 0 || cmbMove2.SelectedIndex < 0) return;

            int index = lstTutor.SelectedIndex;
            ushort newMoveIdx = (ushort)cmbMove2.SelectedIndex;

            if (_tutorListManager.Working[index]._MoveIdx != newMoveIdx)
            {
                _tutorListManager.Working[index]._MoveIdx = newMoveIdx;

                _isUpdatingUI = true;
                lstTutor.Items[index] = GetTutorDisplayString(index, newMoveIdx);
                _isUpdatingUI = false;

                _uiStateManager.UpdateBinary("Tutor", GetTutorBinary());
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        private void TmHmTutorEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                ControlHelper.HandleUnsavedChanges(
                    saveAction: () => SaveChanges(),
                    proceedAction: () => { },
                    cancelAction: () => e.Cancel = true
                );
            }
        }

        private void SaveChanges()
        {
            for (int i = 0; i < _tmHmListManager.Working.Count; i++)
            {
                _tmHmListManager.Save(i, appendTerminator: false);
            }

            for (int i = 0; i < _tutorListManager.Working.Count; i++)
            {
                _tutorListManager.Save(i, appendTerminator: false);
            }

            _uiStateManager.UpdateInitialValues();
        }

        private string GetTmHmDisplayString(int index, int moveIdx, int tmCount)
        {
            string moveName = moveIdx < _moveNameManager.Original.Count
                ? _moveNameManager.Original[moveIdx]._MoveName
                : "???";

            if (index < tmCount)
            {
                return $"TM{index + 1:D2} - {moveName}";
            }
            else
            {
                return $"HM{index - tmCount + 1:D2} - {moveName}";
            }
        }

        private string GetTutorDisplayString(int index, int moveIdx)
        {
            string moveName = moveIdx < _moveNameManager.Original.Count
                ? _moveNameManager.Original[moveIdx]._MoveName
                : "???";

            return $"No.{index + 1:D2} - {moveName}";
        }

        private byte[] GetTmHmBinary()
        {
            return _tmHmListManager.Working
                .SelectMany(x => BitConverter.GetBytes(x._MoveIdx))
                .ToArray();
        }

        private byte[] GetTutorBinary()
        {
            return _tutorListManager.Working
                .SelectMany(x => BitConverter.GetBytes(x._MoveIdx))
                .ToArray();
        }
    }
}
