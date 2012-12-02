using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using WavStagno.Media;

/// <summary>
/// WavStagno Main Namespace.
/// </summary>
namespace WavStagno
{
    /// <summary>
    /// Exception class, thrown when Message length exceeds audio channel stream length.
    /// </summary>
    [Serializable]
    public class MessageSizeExceededException : Exception
    {
        public MessageSizeExceededException()
            : base() { }

        public MessageSizeExceededException(string message)
            : base() { }

        public MessageSizeExceededException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public MessageSizeExceededException(string message, Exception innerException)
            : base(message, innerException) { }

        public MessageSizeExceededException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected MessageSizeExceededException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

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

            if (bufferLength > channelLength) //Check if message stream length is greater than a channel's audio stream length.
                throw new MessageSizeExceededException("Message size is too large for target Audio stream."); //Throw an Exception.

            /*Store message length info in first elements of left and right streams*/
            leftStream[0] = (short)(bufferLength / 32767); //Store Quotient of actual size in first element of audio stream.
            rightStream[0] = (short)(bufferLength % 32767); //Store Remainder of actual size in first element of audio stream.
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
            int messageLengthQuotient = leftStream[0]; //Get stored Quotient of message length.
            int messageLengthRemainder = rightStream[0]; //Get stored Remainder of message length.
            int channelLength = leftStream.Count; //Get audio channel length.
            
            int bufferLength = 32767 * messageLengthQuotient + messageLengthRemainder; //Compute original message length from remaider and quotient obtained from audio streams.
            int storageBlock = (int)Math.Ceiling((double)bufferLength / (channelLength * 2)); //Get original storage block range used based and audio stream length and message length.

            byte[] bufferMessage = new byte[bufferLength]; //Create message byte[] from message size obtained.
            for (int i = 1; i < leftStream.Count; i++) //Iterate over length of audio channel stream.
            {
                if (bufferIndex < bufferLength && i % 8 > 7 - storageBlock && i % 8 <= 7) //Condition to target elements from the last position of every 8 bit block of audio stream (calculated based on storageBlock).
                {
                    /*Get message bits from left and right channel streams and store in message byte[]*/
                    bufferMessage[bufferIndex++] = (byte)leftStream[i];
                    if(bufferIndex < bufferLength) //Check if bufferIndex has exceeded total length.
                        bufferMessage[bufferIndex++] = (byte)rightStream[i];
                }
            }

            return System.Text.Encoding.UTF8.GetString(bufferMessage); //Convert message byte[] into string and return.
        }
    }
}