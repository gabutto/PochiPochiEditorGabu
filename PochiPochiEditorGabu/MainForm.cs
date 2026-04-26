using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using PochiPochiEditorGabu.Constants;
using PochiPochiEditorGabu.FileReaders;
using PochiPochiEditorGabu.Helpers;
using PochiPochiEditorGabu.Managers;

namespace PochiPochiEditorGabu
{
    public partial class MainForm : Form
    {
        private byte[] _romData = Array.Empty<byte>();
        private bool _isRomLoaded = false;
        private string _romPath = string.Empty;
        private string _iniPath = Path.Combine(Application.StartupPath, "config", "roms.ini");
        private string _tblPath = Path.Combine(Application.StartupPath, "config", "charmap.tbl");

        private IniFileReader _config;
        private TblFileReader _charmap;
        private ReservationManager _reservationManager;
        private Form _editorForm;

        private const string ButtonPrefix = "btn";
        private const string EditorSuffix = "Editor";
        private const string NamespacePrefix = "._";

        public MainForm()
        {
            InitializeComponent();
            InitializeEventHandlers();
            InitializeControls();
            InitializeUIStates();

            _config = new IniFileReader(cmbConfig, _iniPath);
            _charmap = new TblFileReader(_tblPath);
            _reservationManager = new ReservationManager();
        }

        private void InitializeEventHandlers()
        {
            btnLoadRom.Click += btnLoadRom_Click;
            btnSaveRom.Click += btnSaveRom_Click;
            btnUnloadRom.Click += btnUnloadRom_Click;
            btnFsfSearch.Click += btnSearch_Click;

            foreach (Button btn in grpSelectEditor.Controls)
            {
                btn.Click += EditorButton_Click;
            }
        }

        private void InitializeControls()
        {
            ControlHelper.AttachAddressAutoFormat(txtFsfStartAddr);

            string imagePath = Path.Combine(Application.StartupPath, "img", "poochyena_image.png");
            picFormPokemon.Image = Image.FromFile(imagePath);
        }

        private void InitializeUIStates()
        {
            _isRomLoaded = false;
            UpdataUIStates();
            ControlHelper.ResetControls(this);
        }

        private void UpdataUIStates()
        {
            bool isEditorOpen = _editorForm != null && !_editorForm.IsDisposed;

            // When ROM is not loaded AND editor is not open
            bool canLoadConfig = !_isRomLoaded && !isEditorOpen;
            btnLoadRom.Enabled = canLoadConfig;
            lblConfig.Enabled = canLoadConfig;
            cmbConfig.Enabled = canLoadConfig;

            // When ROM is loaded AND editor is not open
            bool canOpenEditor = _isRomLoaded && !isEditorOpen;
            btnSaveRom.Enabled = canOpenEditor;
            btnUnloadRom.Enabled = canOpenEditor;
            ControlHelper.SetControlsEnabled(grpSelectEditor, canOpenEditor);

            // Free space finder when ROM is loaded
            ControlHelper.SetControlsEnabled(grpFreeSpaceFinder, _isRomLoaded);
        }

        private void btnLoadRom_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = GbaConstants.RomFileFilter;
                openFileDialog.Title = GbaConstants.RomFileTitle;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _romPath = openFileDialog.FileName;
                    _romData = File.ReadAllBytes(_romPath);

                    string selectedConfig = cmbConfig.SelectedItem?.ToString() ?? string.Empty;
                    _config.LoadConfig(selectedConfig, _romData);

                    int addr = _config.GetInt("FreeSpaceFinderAddress");
                    txtFsfStartAddr.Text = addr.ToString("X8");
                    nudFsfByteAmount.Value = _config.GetInt("FreeSpaceByteAmount");

                    _isRomLoaded = true;
                    UpdataUIStates();
                }
            }
        }

        private void btnSaveRom_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = GbaConstants.RomFileFilter;
                saveFileDialog.Title = GbaConstants.RomFileTitle;
                saveFileDialog.FileName = Path.GetFileName(_romPath);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(saveFileDialog.FileName, _romData);
                }
            }
        }

        private void btnUnloadRom_Click(object sender, EventArgs e)
        {
            _romData = Array.Empty<byte>();
            _isRomLoaded = false;
            _romPath = string.Empty;

            ControlHelper.ResetControls(this);
            UpdataUIStates();

            _config = new IniFileReader(cmbConfig, _iniPath);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (!ControlHelper.ValidateAndFormatInputTextBox(txtFsfStartAddr, out uint? startAddress))
            {
                txtFsfStartAddr.Text = string.Empty;
                return;
            }

            int neededBytes = (int)nudFsfByteAmount.Value;
            int currentAddress = (int)startAddress;
            int foundAddress = -1;

            if (currentAddress % GbaConstants.PtrSize != 0)
            {
                currentAddress = (int)(((uint)(currentAddress + (GbaConstants.PtrSize - 1))) & GbaConstants.AlignMask);
            }

            IEnumerable<ReservationManager.ReservedAreaInfo> reservedAreas = _reservationManager.GetAllReservations();

            while (currentAddress + neededBytes <= _romData.Length)
            {
                bool isFreeSpace = true;

                foreach (var res in reservedAreas)
                {
                    int resStart = (int)res.Address;
                    int resEnd = resStart + (res.Data?.Length ?? 0);
                    int curStart = currentAddress;
                    int curEnd = currentAddress + neededBytes;

                    if (curStart < resEnd && curEnd > resStart)
                    {
                        isFreeSpace = false;
                        currentAddress = (int)(((uint)(resEnd + (GbaConstants.PtrSize - 1))) & GbaConstants.AlignMask);
                        break;
                    }
                }

                if (isFreeSpace)
                {
                    for (int i = 0; i < neededBytes; i++)
                    {
                        if (_romData[currentAddress + i] != GbaConstants.FreeSpaceByte)
                        {
                            isFreeSpace = false;
                            currentAddress += (int)(((uint)(i + 1 + (GbaConstants.PtrSize - 1))) & GbaConstants.AlignMask);
                            break;
                        }
                    }
                }

                if (isFreeSpace)
                {
                    foundAddress = currentAddress;
                    break;
                }
            }

            if (foundAddress != -1)
            {
                txtFsfResultAddr.Text = foundAddress.ToString("X8");
            }
            else
            {
                MessageBox.Show("空き領域が見つかりませんでした。", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtFsfResultAddr.Text = string.Empty;
            }
        }

        private void EditorButton_Click(object sender, EventArgs e)
        {
            if (!(sender is Button btn)) return;

            string editorName = btn.Name.Substring(ButtonPrefix.Length);
            string targetClassName = $"{editorName}{EditorSuffix}";
            string baseNamespace = GetType().Namespace ?? string.Empty;

            Type formType = System.Reflection.Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .FirstOrDefault(t =>
                                t.Name == targetClassName &&
                                t.Namespace.StartsWith($"{baseNamespace}{NamespacePrefix}")
                            );

            if (formType != null)
            {
                if (Activator.CreateInstance(formType, _romData, _config, _charmap, _reservationManager) is Form newForm)
                {
                    OpenEditorForm(newForm);
                }
            }
        }

        private void OpenEditorForm(Form form)
        {
            if (_editorForm != null && !_editorForm.IsDisposed)
            {
                _editorForm.Focus();
                return;
            }

            _editorForm = form;
            _editorForm.FormClosed += EditorForm_FormClosed;

            _editorForm.Show();
            UpdataUIStates();
        }

        private void EditorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_editorForm != null)
            {
                _editorForm.FormClosed -= EditorForm_FormClosed;
                _editorForm = null;
            }

            _reservationManager.ClearAllReservations();
            UpdataUIStates();
        }
    }
}
