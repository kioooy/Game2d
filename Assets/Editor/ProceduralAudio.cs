using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class ProceduralAudio
{
    private const int SampleRate = 44100;

    public static byte[] GenerateShoot()
    {
        float duration = 0.2f;
        int samples = (int)(SampleRate * duration);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float freq = Mathf.Lerp(880, 220, t); // Pitch slide down
            data[i] = Mathf.Sin(2 * Mathf.PI * freq * ((float)i / SampleRate)) * (1 - t); // Fade out
        }
        return EncodeWav(data);
    }

    public static byte[] GenerateDeath()
    {
        float duration = 0.4f;
        int samples = (int)(SampleRate * duration);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            // Mixed groan/death sound
            float freq = Mathf.Lerp(150, 80, t); 
            float wave = Mathf.Sin(2 * Mathf.PI * freq * ((float)i / SampleRate));
            float noise = (Random.value * 2 - 1) * 0.2f;
            data[i] = (wave + noise) * (1 - t);
        }
        return EncodeWav(data);
    }

    public static byte[] GenerateSpend()
    {
        float duration = 0.3f;
        int samples = (int)(SampleRate * duration);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            // Double metallic ping for "Spend"
            float freq = (t < 0.1f) ? 1500 : (t < 0.2f) ? 1000 : 2000;
            data[i] = Mathf.Sin(2 * Mathf.PI * freq * ((float)i / SampleRate)) * 0.5f * (1 - t);
        }
        return EncodeWav(data);
    }

    public static byte[] GenerateClick()
    {
        float duration = 0.05f;
        int samples = (int)(SampleRate * duration);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            data[i] = Mathf.Sin(2 * Mathf.PI * 1000 * ((float)i / SampleRate)) * (1 - t);
        }
        return EncodeWav(data);
    }

    public static byte[] GenerateWin()
    {
        float duration = 0.8f;
        int samples = (int)(SampleRate * duration);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float freq = 440;
            if (t > 0.25f) freq = 554;
            if (t > 0.5f) freq = 659;
            if (t > 0.75f) freq = 880;
            data[i] = Mathf.Sin(2 * Mathf.PI * freq * ((float)i / SampleRate)) * 0.5f;
            if (t > 0.9f) data[i] *= (1 - (t - 0.9f) / 0.1f);
        }
        return EncodeWav(data);
    }

    public static byte[] GenerateLose()
    {
        float duration = 1.0f;
        int samples = (int)(SampleRate * duration);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float freq = Mathf.Lerp(220, 110, t);
            data[i] = (Mathf.Sin(2 * Mathf.PI * freq * ((float)i / SampleRate)) + (Random.value * 0.1f)) * (1 - t);
        }
        return EncodeWav(data);
    }

    public static byte[] GenerateCoin()
    {
        float duration = 0.3f;
        int samples = (int)(SampleRate * duration);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float freq = (t < 0.5f) ? 987 : 1318; // B5 to E6
            data[i] = Mathf.Sin(2 * Mathf.PI * freq * ((float)i / SampleRate)) * 0.5f * (1 - t);
        }
        return EncodeWav(data);
    }

    private static byte[] EncodeWav(float[] samples)
    {
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write("RIFF".ToCharArray());
            writer.Write(36 + samples.Length * 2);
            writer.Write("WAVE".ToCharArray());
            writer.Write("fmt ".ToCharArray());
            writer.Write(16);
            writer.Write((ushort)1);
            writer.Write((ushort)1);
            writer.Write(SampleRate);
            writer.Write(SampleRate * 2);
            writer.Write((ushort)2);
            writer.Write((ushort)16);
            writer.Write("data".ToCharArray());
            writer.Write(samples.Length * 2);

            foreach (float sample in samples)
            {
                writer.Write((short)(sample * short.MaxValue));
            }

            return stream.ToArray();
        }
    }

    public static void SaveWav(string path, byte[] bytes)
    {
        File.WriteAllBytes(path, bytes);
    }
}
