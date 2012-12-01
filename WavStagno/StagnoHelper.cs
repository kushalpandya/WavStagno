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
            /*Cache audio channel streams locally from WaveAudio object*/
            List<short> leftStream = file.GetLeftStream();
            List<short> rightStream = file.GetRightStream();

            /*Hide Message in Streams*/
            byte[] bufferMessage = System.Text.Encoding.UTF8.GetBytes(message); //Convert string Message into byte[]
            short tempBit;
            int bufferIndex = 0; //Set message stream index counter.
            int bufferLength = bufferMessage.Length; //Get length of message stream.
            int channelLength = leftStream.Count; //Get length of audio stream (both left and right streams have same length).
            int storageBlock = (int) Math.Ceiling((double) bufferLength / (channelLength * 2)); //Get storage block range based of length of audio channel stream and message stream.

            /*Store message length info in first elements of left and right streams*/
            leftStream[0] = (short)(bufferLength / 65535); //Store quotient of actual size in first element of audio stream.
            rightStream[0] = (short)(bufferLength % 65535); //Store Semainder of actual size in first element of audio stream.
            for (int i = 1; i < leftStream.Count; i++) //Iterate over length of audio channel stream, skip first element since it contains message length, store message bits into left and right audio streams.
            {
                if (i < leftStream.Count) //Check if storing has not exceeded audio stream length.
                {
                    if (bufferIndex < bufferLength && i % 8 > 7 - storageBlock && i % 8 <= 7) //Condition to target elements from the last position of every 8 bit block of audio stream (calculated based on storageBlock).
                    {
                        tempBit = (short)bufferMessage[bufferIndex++]; //Get message bit
                        leftStream.Insert(i, tempBit); //Replace audio data bit with message bit.
                    }
                }
                if (i < rightStream.Count)
                {
                    if (bufferIndex < bufferLength && i % 8 > 7 - storageBlock && i % 8 <= 7)
                    {
                        tempBit = (short)bufferMessage[bufferIndex++];
                        rightStream.Insert(i, tempBit);
                    }
                }
            }

            file.UpdateStreams(leftStream, rightStream); //Streams now have message hidden in it, update streams to actual WaveAudio object.
        }

        /// <summary>
        /// Extracts message from WaveAudio object's Left and Right Audio channels.
        /// </summary>
        /// <returns>String representing Extracted Message.</returns>
        public string ExtractMessage()
        {
            /*Cache audio channel streams locally from WaveAudio object*/
            List<short> leftStream = file.GetLeftStream();
            List<short> rightStream = file.GetRightStream();

            /*Extract Message from Streams and Return it.*/
            int bufferIndex = 0; //Set message stream index counter.
            int bufferLength = leftStream[0]; //Get stored Quotient to compute message length.
            int channelLength = leftStream.Count; //Get audio channel length.
            
            bufferLength = 65535 * bufferLength + channelLength; //Compute original message length from remaider and quotient obtained from audio streams.
            int storageBlock = (int)Math.Ceiling((double)bufferLength / (channelLength * 2)); //Get original storage block range used based and audio stream length and message length.

            byte[] bufferMessage = new byte[bufferLength + 1]; //Create message byte[] from message size obtained.
            for (int i = 0; i < leftStream.Count; i++) //Iterate over length of audio channel stream.
            {
                if (bufferIndex < bufferLength && i % 8 > 7 - storageBlock && i % 8 <= 7) //Condition to target elements from the last position of every 8 bit block of audio stream (calculated based on storageBlock).
                {
                    /*Get message bits from left and right channel streams and store in message byte[]*/
                    bufferMessage[bufferIndex++] = (byte)leftStream[i];
                    bufferMessage[bufferIndex++] = (byte)rightStream[i];
                }
            }

            return System.Text.Encoding.UTF8.GetString(bufferMessage); //Convert message byte[] into string and return.
        }
    }
}
