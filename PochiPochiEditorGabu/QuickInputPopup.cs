using System;
using System.ComponentModel;
using System.Windows.Forms;

using PochiPochiEditorGabu.Helpers;

namespace PochiPochiEditorGabu
{
    public partial class QuickInputPopup : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ResultAddress { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ResultDataTypeIndex { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ResultEntryCount { get; set; }

        public QuickInputPopup()
        {
            InitializeComponent();

            btnOK.Click += btnOK_Click;
            ControlHelper.AttachAddressAutoFormat(txtInputAddress);
        }

        public void Setup(string defaultAddress,
                          decimal? nudMin = null,
                          decimal? nudMax = null,
                          decimal? defaultNudValue = null,
                          string[] comboItems = null,
                          int defaultComboIndex = -1)
        {
            txtInputAddress.Text = defaultAddress;

            if (nudMin.HasValue)
                nudEntryCount.Minimum = nudMin.Value;
            if (nudMax.HasValue)
                nudEntryCount.Maximum = nudMax.Value;
            if (defaultNudValue.HasValue)
                nudEntryCount.Value = defaultNudValue.Value;

            if (comboItems != null && comboItems.Length > 0)
            {
                cmbInputDataType.Items.Clear();
                cmbInputDataType.Items.AddRange(comboItems);
                cmbInputDataType.SelectedIndex = defaultComboIndex;
                cmbInputDataType.Enabled = true;
            }
            else
            {
                cmbInputDataType.Items.Clear();
                cmbInputDataType.SelectedIndex = -1;
                cmbInputDataType.Enabled = false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!ControlHelper.ValidateAndFormatInputTextBox(txtInputAddress, out uint? dummyAddress, true)) return;

            ResultAddress = txtInputAddress.Text;
            ResultDataTypeIndex = cmbInputDataType.SelectedIndex;
            ResultEntryCount = (int)nudEntryCount.Value;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
