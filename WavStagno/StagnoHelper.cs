using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WavStagno.Media;

/// <summary>
/// WavStagno Main Namespace.
/// </summary>
namespace WavStagno
{
    /// <summary>
    /// Stagnography helper class for Hiding and Extracting message in WaveAudio object.
    /// </summary>
    class StagnoHelper
    {
        private WaveAudio file;

        /// <summary>
        /// Initializes this StagnoHelper object with WaveAudio object.
        /// </summary>
        /// <param name="file">WaveAudio object to be initilized.</param>
        public StagnoHelper(WaveAudio file)
        {
            this.file = file;
        }

        /// <summary>
        /// Hides a message in WaveAudio object's Left and Right Audio channels.
        /// </summary>
        /// <param name="message">String Message to be hidden.</param>
        public void HideMessage(string message)
        {
            List<short> leftStream = file.GetLeftStream();
            List<short> rightStream = file.GetRightStream();

            //Hide Message in Streams and call file.UpdateStreams(leftStream, rightStream)
        }

        /// <summary>
        /// Extracts message from WaveAudio object's Left and Right Audio channels.
        /// </summary>
        /// <returns>String representing Extracted Message.</returns>
        public string ExtractMessage()
        {
            List<short> leftStream = file.GetLeftStream();
            List<short> rightStream = file.GetRightStream();

            //Extract Message from Streams and Return it.
            return "Hello";
        }
    }
}
