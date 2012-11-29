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
            byte[] bufferMessage = System.Text.Encoding.UTF8.GetBytes(message);
            short tempBit;
            int bufferIndex = 0, bufferLength = bufferMessage.Length, channelLength, diff;
            channelLength = leftStream.Count;
            diff = bufferLength / (channelLength * 2);
            leftStream[0] = (short) (bufferLength / 65535);
            rightStream[0] = (short) (bufferLength % 65535);
            Console.WriteLine("length  "+bufferLength+"  chnl le   "+ channelLength);
            for (int i = 1; i < leftStream.Count; i++)
            {
                if (i < leftStream.Count)
                {
                    if (bufferIndex < bufferLength && i % 8 >= 7 - diff && i % 8 <= 7)
                    {
                        tempBit = (short)bufferMessage[bufferIndex++];
                        leftStream.Insert(i, tempBit);
                    }
                }
                if (i < rightStream.Count)
                {
                    if (bufferIndex < bufferLength && i % 8 >= 7 - diff && i % 8 <= 7)
                    {
                        tempBit = (short)bufferMessage[bufferIndex++];
                        rightStream.Insert(i, tempBit);
                    }
                }
            }

            file.UpdateStreams(leftStream, rightStream);
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
            int bufferIndex = 0, bufferLength = leftStream[0], channelLength = rightStream[0], diff;
            
            bufferLength = 65536 * bufferLength + channelLength;
            leftStream.Add((short)bufferLength);
            rightStream.Add((short)channelLength);

            channelLength = leftStream.Count;
            diff = bufferLength / (channelLength * 2);

            byte[] bufferMessage = new byte[bufferLength + 1];
            for (int i = 0; i < leftStream.Count; i++)
            {
                if (bufferIndex < bufferLength && i % 8 >= 7 - diff && i % 8 <= 7)
                {
                    bufferMessage[bufferIndex++] = (byte)leftStream[i];
                    bufferMessage[bufferIndex++] = (byte)rightStream[i];
                }
            }

            return System.Text.Encoding.UTF8.GetString(bufferMessage);
        }
    }
}
