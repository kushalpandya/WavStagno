using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

/// <summary>
/// WavStagno Main Namespace.
/// </summary>
namespace WavStagno
{
    /// <summary>
    /// WavStagno Media Namespace, contains Classes for Audio representation.
    /// </summary>
    namespace Media
    {
        /// <summary>
        /// Class to represent Wave audio file, its header information and left and right channel streams.
        /// </summary>
        class WaveAudio
        {
            private byte[] riffID; // "riff"
            private uint size;  // Size
            private byte[] wavID;  // Format
            private byte[] fmtID;  // Subchunk1ID
            private uint fmtSize; // Subchunk size
            private ushort format; // format
            private ushort channels; // no of channels
            private uint sampleRate; // Samplerate
            private uint bytePerSec;
            private ushort blockSize;
            private ushort bit;
            private byte[] dataID;// "data"
            private uint dataSize;
            private List<short> leftStream;
            private List<short> rightStream;

            private FileStream fs;
            private BinaryReader br;

            /// <summary>
            /// Initializes this WaveAudio object with Wave audio file.
            /// </summary>
            /// <param name="filepath">Path of Wave Audio file.</param>
            public WaveAudio(FileStream filepath)
            {
                this.fs = filepath;
                this.br = new BinaryReader(fs);

                this.riffID = br.ReadBytes(4);
                this.size = br.ReadUInt32();
                this.wavID = br.ReadBytes(4);
                this.fmtID = br.ReadBytes(4);
                this.fmtSize = br.ReadUInt32();
                this.format = br.ReadUInt16();
                this.channels = br.ReadUInt16();
                this.sampleRate = br.ReadUInt32();
                this.bytePerSec = br.ReadUInt32();
                this.blockSize = br.ReadUInt16();
                this.bit = br.ReadUInt16();
                this.dataID = br.ReadBytes(4);
                this.dataSize = br.ReadUInt32();

                this.leftStream = new List<short>();
                this.rightStream = new List<short>();
                for (int i = 0; i < this.dataSize / this.blockSize; i++)
                {
                    leftStream.Add((short)br.ReadUInt16());
                    rightStream.Add((short)br.ReadUInt16());
                }

                br.Close();
                fs.Close();
            }

            /// <summary>
            /// Gets Left Channel Audio Stream as List of short elements.
            /// </summary>
            /// <returns>List&lt;short&gt; representing Left channel Audio Stream.</returns>
            public List<short> GetLeftStream()
            {
                return this.leftStream;
            }

            /// <summary>
            /// Gets Left Channel Audio Stream as List of short elements.
            /// </summary>
            /// <returns>List&lt;short&gt; representing Left channel Audio Stream.</returns>
            public List<short> GetRightStream()
            {
                return this.rightStream;
            }

            /// <summary>
            /// Updates Left and Right Audio Stream for this WaveAudio object.
            /// </summary>
            /// <param name="leftStream">Left Channel Audio Stream</param>
            /// <param name="rightStream">Right Channel Audio Stream</param>
            public void UpdateStreams(List<short> leftStream, List<short> rightStream)
            {
                this.leftStream = leftStream;
                this.rightStream = rightStream;
            }

            /// <summary>
            /// Writes this WaveAudio object to a file.
            /// </summary>
            /// <param name="path">Path of file to be written.</param>
            public void WriteFile(string path)
            {
                this.dataSize = (uint)Math.Max(leftStream.Count, rightStream.Count) * 4;

                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                BinaryWriter bw = new BinaryWriter(fs);

                bw.Write(this.riffID);
                bw.Write(this.size);
                bw.Write(this.wavID);
                bw.Write(this.fmtID);
                bw.Write(this.fmtSize);
                bw.Write(this.format);
                bw.Write(this.channels);
                bw.Write(this.sampleRate);
                bw.Write(this.bytePerSec);
                bw.Write(this.blockSize);
                bw.Write(this.bit);
                bw.Write(this.dataID);
                bw.Write(this.dataSize);

                for (int i = 0; i < this.dataSize / this.blockSize; i++)
                {
                    if(i < this.leftStream.Count)
                        bw.Write((ushort)this.leftStream[i]);
                    else
                        bw.Write(0);

                    if (i < this.rightStream.Count)
                        bw.Write((ushort)this.rightStream[i]);
                    else
                        bw.Write(0);
                }

                fs.Close();
                bw.Close();
            }
        }
    }
}
