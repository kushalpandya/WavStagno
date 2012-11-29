using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using WavStagno.Media;

namespace WavStagno
{
    public partial class frmMain : Form
    {
        private WaveAudio file;
        private StagnoHelper sh;

        public frmMain()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            dlgBrowseFile.ShowDialog();
        }

        private void dlgBrowseFile_FileOk(object sender, CancelEventArgs e)
        {
            txtFilePath.Text = dlgBrowseFile.FileName;
            if (txtFilePath.Text.Trim() != "")
            {
                btnExtract.Enabled = true;
                btnHide.Enabled = true;
                file = new WaveAudio(new FileStream(txtFilePath.Text, FileMode.Open, FileAccess.Read));
                sh = new StagnoHelper(file);
            }
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            string message = "";
            new Thread(() => {
                message = sh.ExtractMessage();
            }).Start();
            txtMessage.Text = message;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtMessage.Text = "";
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            string message = txtMessage.Text.Trim();
            if (message == "")
                MessageBox.Show(this, "Write Message to Hide!", "WavStagno 1.0",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            else
            {
                new Thread(() =>
                {
                    sh.HideMessage(message);
                    dlgSaveFile.ShowDialog();
                }).Start();
            }
        }

        private void dlgSaveFile_FileOk(object sender, CancelEventArgs e)
        {
            file.WriteFile(dlgSaveFile.FileName);
            file = new WaveAudio(new FileStream(txtFilePath.Text, FileMode.Open, FileAccess.Read));
            sh = new StagnoHelper(file);
        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
            lblMessageLength.Text = txtMessage.TextLength.ToString();            
        }
    }
}
