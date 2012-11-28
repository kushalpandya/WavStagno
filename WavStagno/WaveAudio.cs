using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavStagno
{
    namespace Media
    {
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
        }
    }
}
