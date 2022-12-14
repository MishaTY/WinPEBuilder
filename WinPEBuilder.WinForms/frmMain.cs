using System;
using System.Diagnostics;
using System.Windows.Forms;
using WinPEBuilder.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WinPEBuilder.WinForms
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            tabControl1.TabPages.Remove(ProgressTab);
            lblVersion.Text = "Version: " + Builder.Version;

            if(Debugger.IsAttached&& Environment.UserName.ToLower() == "misha")
            {
                //Debug code
                txtIsoPath.Text = @"D:\1Misha\Downloads\25252.1010_amd64_en-us_professional_0ec350c5_convert\25252.1010.221122-1933.RS_PRERELEASE_FLT_CLIENTPRO_OEMRET_X64FRE_EN-US.ISO";
                txtOutFile.Text = @"D:\winpegen.vhd";
            }
        }

        private void chkDWM_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDWM.Checked)
            {
                chkUWP.Enabled = true;
                chkExplorer.Enabled = true;
                chkLogonUI.Enabled = true;
            }
            else
            {
                chkUWP.Enabled = false;
                chkExplorer.Enabled = false;
                chkLogonUI.Enabled = false;

                //uncheck
                chkUWP.Checked = false;
                chkExplorer.Checked = false;
                chkLogonUI.Checked = false;
            }
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtIsoPath.Text))
            {
                MessageBox.Show("You must select an ISO in the sources tab", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(txtOutFile.Text))
            {
                MessageBox.Show("You must select an output type/file in the output tab.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            tabControl1.TabPages.Add(ProgressTab);
            tabControl1.SelectedTab=ProgressTab;
            HideTabs();
            btnBuild.Visible = false;

            //create options
            var options = new BuilderOptions();

            if (radVHD.Checked)
            {
                options.OutputType = BuilderOptionsOutputType.VHD;
                options.Output = txtOutFile.Text;
            }
            else
            {
                throw new NotImplementedException("ISO file output not implemented");
            }

            var builder = new Builder(options, txtIsoPath.Text, Application.StartupPath + @"work\");


            builder.OnComplete += Builder_OnComplete;
            builder.OnProgress += Builder_OnProgress;
            builder.Start();
        }

        private void Builder_OnComplete(object? sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                Invoke(OnCompleteHandler);
            }
            else
            {
                OnCompleteHandler();
            }
        }


        private void Builder_OnProgress(bool error, int progress, string message)
        {
            if (this.InvokeRequired)
            {
                Invoke(ProgressHandler, error, progress, message);
            }
            else
            {
                ProgressHandler(error, progress, message);
            }
        }
        private void ProgressHandler(bool error, int progress, string message)
        {
            lblProgress.Text = message;
            progressBar1.Value = progress;
            if (error)
            {
                //the builder should have stopped
                MessageBox.Show("Error occured while building: " + message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ShowTabs();
                tabControl1.TabPages.Remove(ProgressTab);
                btnBuild.Visible = true;
            }
        }
        private void OnCompleteHandler()
        {
            ShowTabs();
            tabControl1.TabPages.Remove(ProgressTab);
            btnBuild.Visible = true;
            MessageBox.Show("Build completed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void HideTabs()
        {
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;
        }
        private void ShowTabs()
        {
            tabControl1.Appearance = TabAppearance.Normal;
            tabControl1.ItemSize = new Size(48, 18);
            tabControl1.SizeMode = TabSizeMode.Normal;
        }

        private void btnSelectVHDOutput_Click(object sender, EventArgs e)
        {
            if (VHDDialog.ShowDialog() == DialogResult.OK)
            {
                txtOutFile.Text = VHDDialog.FileName;
            }
        }

        private void btnSelectISO_Click(object sender, EventArgs e)
        {
            if (SelISODialog.ShowDialog() == DialogResult.OK)
            {
                txtIsoPath.Text = SelISODialog.FileName;
            }
        }
    }
}
