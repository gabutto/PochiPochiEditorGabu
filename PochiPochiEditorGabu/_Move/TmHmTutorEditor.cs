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
            InitializeEventHandlers();
            InitializeControls();
            InitializeUIStates();

            InitializeLists();
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

        private void InitializeEventHandlers()
        {

        }

        private void InitializeControls()
        {
            // move for cmb
            var moveNames = _moveNameManager.Original
                             .Select(entry => entry._MoveName)
                             .ToArray();
            cmbMove1.Items.AddRange(moveNames);
            cmbMove2.Items.AddRange(moveNames);
        }

        private void InitializeUIStates()
        {
            btnSave.Enabled = false;
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);
        }

        private void InitializeLists()
        {

        }


    }
}
