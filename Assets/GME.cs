using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GameMusicEmuSharp;
using UnityEngine;

public class GME : MonoBehaviour
{
    
    //再生用オーディオソース
    public AudioSource m_Audio;
#if UNITY_EDITOR
    // [DllImport("Dll1")]
    // private static extern float add(float x, float y);
#elif UNITY_IPHONE
    // Debug.Log("Unity iPhone");
#elif UNITY_ANDROID
    // [DllImport("native")]
    // private static extern float add(float x, float y);
#endif



    
    // [DllImport("gme")]
    // private static extern string gme_open_file(string path, out IntPtr emuOut, int sampleRate);
    //
    // [DllImport("gme")]
    // private static extern void gme_delete(IntPtr emuHandle);
    
    //https://github.com/Suzeep/audioclip_makerよりbyte配列→再生用float配列への変換処理を拝借
    readonly float RANGE_VALUE_BIT_8 = 1.0f / Mathf.Pow(2, 7);   // 1 / 128
    readonly float RANGE_VALUE_BIT_16 = 1.0f / Mathf.Pow(2, 15); // 1 / 32768
    const int BASE_CONVERT_SAMPLES = 1024 * 20;
    const int BIT_8 = 8;
    const int BIT_16 = 16;
    
    // Start is called before the first frame update
    void Start()
    {
        float x = 3;
        float y = 10;
        // Debug.Log(add(x, y));

        // IntPtr emuOut = IntPtr.Zero;
        
        var path = Application.persistentDataPath + "/" + "sample2.nsf";
        Debug.Log(path);
        GmeReader reader = new GmeReader(path);
        
        
        
        int size = reader.TrackInfo.playLength;
        // IntPtr wavPtr = AquesTalk_Synthe(ref aqtk_voice, koeSjisBytes, ref size);
        Debug.Log("size : " + size);

        //成功判定
        // if (wavPtr == IntPtr.Zero)
        // {
        //     Debug.LogError("ERROR: 音声生成に失敗しました。不正な文字が使われた可能性があります");
        // }
        
        //C#で扱えるようにマネージド側へコピー
        byte[] byte_data = new byte[size];
        reader.Read(byte_data, 0, size);
        // Marshal.Copy(wavPtr, byte_data, 0, size);
        
        //アンマネージドポインタは用が無くなった瞬間に解放
        // AquesTalk_FreeWave(wavPtr);
        
        //float配列に変換
        float[] float_data = CreateRangedRawData(byte_data, 0, size / 2, 1, BIT_16);
        
        //audioClip作成
        AudioClip audioClip = AudioClip.Create("gme", float_data.Length, 2, 44100, false);
        audioClip.SetData(float_data, 0);
        m_Audio.clip = audioClip;
        
        //再生
        m_Audio.Play();
        
    }
    
    //---------------------------------------------------------------------------
    // create rawdata( ranged 0.0 - 1.0 ) from binary wav data
    //---------------------------------------------------------------------------
    public float[] CreateRangedRawData(byte[] byte_data, int wav_buf_idx, int samples, int channels, int bit_per_sample)
    {
        float[] ranged_rawdata = new float[samples * channels];

        int step_byte = bit_per_sample / BIT_8;
        int now_idx = wav_buf_idx;

        for (int i = 0; i < (samples * channels); ++i)
        {
            ranged_rawdata[i] = convertByteToFloatData(byte_data, now_idx, bit_per_sample);

            now_idx += step_byte;
        }

        return ranged_rawdata;
    }

    //---------------------------------------------------------------------------
    // convert byte data to float data
    //---------------------------------------------------------------------------
    private float convertByteToFloatData(byte[] byte_data, int idx, int bit_per_sample)
    {
        float float_data = 0.0f;

        switch (bit_per_sample)
        {
            case BIT_8:
            {
                float_data = ((int)byte_data[idx] - 0x80) * RANGE_VALUE_BIT_8;
            }
                break;
            case BIT_16:
            {
                short sample_data = System.BitConverter.ToInt16(byte_data, idx);
                float_data = sample_data * RANGE_VALUE_BIT_16;
            }
                break;
        }

        return float_data;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
