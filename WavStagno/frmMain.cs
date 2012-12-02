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
        private string message;
        private bool HIDE_ERROR;

        public frmMain()
        {
            InitializeComponent();
            HIDE_ERROR = false;
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
            message = "";
            message = sh.ExtractMessage();
            txtMessage.Text = message;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtMessage.Text = "";
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            message = txtMessage.Text.Trim();
            if (message == "")
                MessageBox.Show(this, "Write Message to Hide!", "WavStagno 1.0", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
            {
                btnHide.Enabled = false;
                btnExtract.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
                comWorker.RunWorkerAsync();
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

        private void comWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                sh.HideMessage(message);
            }
            catch (MessageSizeExceededException ex)
            {
                HIDE_ERROR = true;
            }
        }

        private void comWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!HIDE_ERROR)
            {
                btnHide.Enabled = true;
                btnExtract.Enabled = true;
                this.Cursor = Cursors.Default;
                dlgSaveFile.ShowDialog();
            }
            else
                MessageBox.Show(this, "Message size is too large!", "WavStagno 1.0", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
