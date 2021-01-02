using System;
using NAudio.Wave;
using UnityEngine;

// using NAudio.Wave;

// TODO: Fix .vgz playback. (needs zlib I think?)

namespace GameMusicEmuSharp
{
    /// <summary>
    /// Class for reading files supported by Game Music Emu
    /// </summary>
    public class GmeReader
    {
        #region Fields

        private readonly IntPtr emuHandle;

        private int track;
        private bool isPlaying;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">File to open using Game Music Emu.</param>
        /// <param name="sampleRate">Desired sample rate. Defaults to 44100Hz if not specified.</param>
        /// <param name="channels">Number of channels to use. Defaults to two if not specified.</param>
        public GmeReader(string fileName, int sampleRate = 44100, int channels = 2)
        {
            // Open the file.
            emuHandle = GmeNative.OpenFile(fileName, sampleRate);

            Debug.Log("GmeReader emuHandle");
            Debug.Log(emuHandle);
            // Enable accurate sound emulation.
            GmeNative.gme_enable_accuracy(emuHandle, true);

            // Get track info
            TrackInfo = GmeNative.GetTrackInfo(emuHandle, track);
            TrackCount = GmeNative.gme_track_count(emuHandle);
            VoiceCount = GmeNative.gme_voice_count(emuHandle);
            Equalizer = GmeNative.GetEqualizer(emuHandle);
            // Type = GmeNative.GetType(emuHandle);
            //
            // GmeType tempType = Type; // Since properties can't be used in ref arguments.
            // SupportsMultipleTracks = GmeNative.gme_type_multitrack(ref tempType);

            // Init the wave format.
            WaveFormat = new WaveFormat(sampleRate, 16, channels);
        }


        /// <summary>
        /// Destructor
        /// </summary>
        ~GmeReader()
        {
            // Free the handle acquired for GameMusicEmu.
            GmeNative.gme_delete(emuHandle);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The number of tracks in the currently loaded file.
        /// </summary>
        public int TrackCount { get; }

        /// <summary>
        /// Track info for the currently loaded track.
        /// </summary>
        public GmeTrackInfo TrackInfo { get; }

        /// <summary>
        /// The number of voices in this track.
        /// </summary>
        public int VoiceCount { get; }

        /// <summary>
        /// The current equalizer for this track.
        /// </summary>
        public GmeEqualizer Equalizer { get; }

        /// <summary>
        /// The type of file format being played/read.
        /// </summary>
        public GmeType Type { get; }

        /// <summary>
        /// Whether or not the loaded format supports
        /// multiple tracks within a single file.
        /// </summary>
        public bool SupportsMultipleTracks { get; }


        public WaveFormat WaveFormat { get; }


        public long Length
        {
            get { return TrackInfo.playLength; }
        }


        public long Position
        {
            get
            {
                return GmeNative.gme_tell_samples(emuHandle) / WaveFormat.SampleRate * WaveFormat.AverageBytesPerSecond;
            }
            set { GmeNative.gme_seek_samples(emuHandle, (int)value / WaveFormat.BlockAlign); }
        }

        #endregion

        #region Methods

        public int Read(byte[] buffer, int offset, int count)
        {
            if (!isPlaying)
            {
                GmeNative.gme_start_track(emuHandle, track++);
                isPlaying = true;
            }

            // Make a buffer and fill it.
            short[] sBuffer = new short[count / 2];
            GmeNative.gme_play(emuHandle, count / 2, sBuffer);

            // Convert the short samples to byte samples and place them
            // in the NAudio byte sample buffer.
            Buffer.BlockCopy(sBuffer, 0, buffer, 0, buffer.Length);

            return buffer.Length;
        }


        /// <summary>
        /// Sets the track to play
        /// </summary>
        /// <param name="trackNum">The track to play</param>
        public void SetTrack(int trackNum)
        {
            // Set that we aren't playing.
            isPlaying = false;
            track = trackNum;
        }


        /// <summary>
        /// Gets the name of a voice in the track.
        /// </summary>
        /// <param name="voiceIndex">Voice index of the voice to get the name of.</param>
        /// <returns>The name of the voice specified by the given index.</returns>
        public string GetVoiceName(int voiceIndex)
        {
            return GmeNative.GetVoiceName(emuHandle, voiceIndex);
        }

        #endregion
    }
}