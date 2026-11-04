using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Comprehensive procedural audio engine for PlatformRunner3D.
/// Generates studio-quality synthesized sound effects and dynamic background beats in code,
/// requiring zero external audio files!
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    private AudioSource sfxSource;
    private AudioSource musicSource;
    private AudioSource footstepSource;

    [Header("Volume & Mute Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 0.85f;
    [Range(0f, 1f)] public float musicVolume = 0.45f;
    public bool isMuted = false;

    // Generated audio clips cache
    private AudioClip jumpClip;
    private AudioClip slideClip;
    private AudioClip hitClip;
    private AudioClip deathClip;
    private AudioClip shieldBreakClip;
    private AudioClip powerUpClip;
    private AudioClip nearMissClip;
    private AudioClip milestoneClip;
    private AudioClip footstepClip;
    private AudioClip footstepLClip;
    private AudioClip footstepRClip;
    private AudioClip landingClip;
    private bool footstepAlternator = false;
    private AudioClip gemClip;
    private AudioClip musicTrack;

    private readonly List<AudioClip> coinChimes = new List<AudioClip>();

    private const int SampleRate = 44100;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        isMuted = PlayerPrefs.GetInt("Sound_Muted", 0) == 1;

        SetupAudioSources();
        GenerateAllProceduralClips();
    }

    private void Start()
    {
        if (!isMuted && musicTrack != null)
        {
            PlayMusic();
        }
    }

    private void SetupAudioSources()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f; // 2D

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;

        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.playOnAwake = false;
        footstepSource.spatialBlend = 0f;
    }

    private void GenerateAllProceduralClips()
    {
        // 1. Coin pickups (5 rising pitch variants for combos)
        for (int i = 0; i < 6; i++)
        {
            float baseFreq = 950f + (i * 120f);
            coinChimes.Add(CreateCoinChimeClip($"Coin_{i}", baseFreq));
        }

        // 2. Rare Gem collect
        gemClip = CreateGemChimeClip();

        // 3. Jump whoosh
        jumpClip = CreateJumpClip();

        // 4. Slide swoosh
        slideClip = CreateSlideClip();

        // 5. Impact & Damage
        hitClip = CreateHitClip();

        // 6. Dramatic Death
        deathClip = CreateDeathClip();

        // 7. Shield shatter
        shieldBreakClip = CreateShieldBreakClip();

        // 8. Power-Up fanfare
        powerUpClip = CreatePowerUpClip();

        // 9. Near Miss bullet-time swoosh
        nearMissClip = CreateNearMissClip();

        // 10. Milestone celebration
        milestoneClip = CreateMilestoneClip();

        // 11. Running footsteps (Left and Right alternating sneaker taps)
        footstepLClip = CreateFootstepClip(true);
        footstepRClip = CreateFootstepClip(false);
        footstepClip = footstepLClip;

        // 12. Landing impact cushioning
        landingClip = CreateLandingClip();

        // 13. Dynamic Electronic Cyber-Beat music loop (8-bar high energy runner track)
        musicTrack = CreateElectronicMusicTrack();
        if (musicSource != null)
        {
            musicSource.clip = musicTrack;
            musicSource.volume = isMuted ? 0f : (musicVolume * masterVolume);
        }
    }

    // =========================================================================
    // PLAYBACK API
    // =========================================================================

    public void PlayCoin(int comboMultiplier = 1)
    {
        if (isMuted || sfxSource == null || coinChimes.Count == 0) return;
        int index = Mathf.Clamp(comboMultiplier - 1, 0, coinChimes.Count - 1);
        sfxSource.PlayOneShot(coinChimes[index], sfxVolume * masterVolume);
    }

    public void PlayGem()
    {
        if (isMuted || sfxSource == null || gemClip == null) return;
        sfxSource.PlayOneShot(gemClip, sfxVolume * 1.1f * masterVolume);
    }

    public void PlayJump()
    {
        if (isMuted || sfxSource == null || jumpClip == null) return;
        sfxSource.PlayOneShot(jumpClip, sfxVolume * 0.9f * masterVolume);
    }

    public void PlaySlide()
    {
        if (isMuted || sfxSource == null || slideClip == null) return;
        sfxSource.PlayOneShot(slideClip, sfxVolume * 0.8f * masterVolume);
    }

    public void PlayFootstep()
    {
        footstepAlternator = !footstepAlternator;
        PlayFootstep(footstepAlternator);
    }

    public void PlayFootstep(bool isLeft)
    {
        if (isMuted || footstepSource == null) return;
        AudioClip clip = isLeft ? (footstepLClip ?? footstepClip) : (footstepRClip ?? footstepClip);
        if (clip == null) return;
        footstepSource.pitch = UnityEngine.Random.Range(0.96f, 1.04f);
        footstepSource.PlayOneShot(clip, sfxVolume * 0.40f * masterVolume);
    }

    public void PlayLanding(float impactSpeed = 10f)
    {
        if (isMuted || sfxSource == null || landingClip == null) return;
        float vol = Mathf.Clamp(impactSpeed / 22f, 0.45f, 1.0f);
        sfxSource.pitch = UnityEngine.Random.Range(0.94f, 1.03f);
        sfxSource.PlayOneShot(landingClip, sfxVolume * vol * masterVolume);
    }

    public void PlayHit()
    {
        if (isMuted || sfxSource == null || hitClip == null) return;
        sfxSource.PlayOneShot(hitClip, sfxVolume * masterVolume);
    }

    public void PlayDeath()
    {
        if (isMuted || sfxSource == null || deathClip == null) return;
        sfxSource.PlayOneShot(deathClip, sfxVolume * masterVolume);
    }

    public void PlayShieldBreak()
    {
        if (isMuted || sfxSource == null || shieldBreakClip == null) return;
        sfxSource.PlayOneShot(shieldBreakClip, sfxVolume * masterVolume);
    }

    public void PlayPowerUp()
    {
        if (isMuted || sfxSource == null || powerUpClip == null) return;
        sfxSource.PlayOneShot(powerUpClip, sfxVolume * masterVolume);
    }

    public void PlayNearMiss()
    {
        if (isMuted || sfxSource == null || nearMissClip == null) return;
        sfxSource.PlayOneShot(nearMissClip, sfxVolume * 0.95f * masterVolume);
    }

    public void PlayMilestone()
    {
        if (isMuted || sfxSource == null || milestoneClip == null) return;
        sfxSource.PlayOneShot(milestoneClip, sfxVolume * masterVolume);
    }

    public void PlayMusic()
    {
        if (musicSource == null || musicTrack == null) return;
        musicSource.volume = isMuted ? 0f : (musicVolume * masterVolume);
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying && !isMuted)
            musicSource.UnPause();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void SetMusicPitch(float pitch)
    {
        if (musicSource != null)
            musicSource.pitch = Mathf.Clamp(pitch, 0.8f, 1.35f);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("Sound_Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();

        if (musicSource != null)
        {
            musicSource.volume = isMuted ? 0f : (musicVolume * masterVolume);
            if (!isMuted && !musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }

    // =========================================================================
    // PROCEDURAL AUDIO SYNTHESIZERS
    // =========================================================================

    private AudioClip CreateCoinChimeClip(string clipName, float startFreq)
    {
        int samples = Mathf.RoundToInt(SampleRate * 0.18f);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SampleRate;
            float progress = (float)i / samples;

            // Two-tone bright chime
            float freq = (progress < 0.35f) ? startFreq : startFreq * 1.33f;
            float envelope = Mathf.Pow(1f - progress, 2.5f);

            // Sine + overtone
            float sample = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.7f
                         + Mathf.Sin(2f * Mathf.PI * freq * 2f * t) * 0.25f
                         + Mathf.Sin(2f * Mathf.PI * freq * 3f * t) * 0.05f;

            data[i] = sample * envelope;
        }

        AudioClip clip = AudioClip.Create(clipName, samples, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateGemChimeClip()
    {
        int samples = Mathf.RoundToInt(SampleRate * 0.32f);
        float[] data = new float[samples];

        float[] notes = new float[] { 1046.50f, 1318.51f, 1567.98f, 2093.00f }; // C6, E6, G6, C7 arpeggio
        int noteSamples = samples / notes.Length;

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SampleRate;
            int noteIndex = Mathf.Clamp(i / noteSamples, 0, notes.Length - 1);
            float freq = notes[noteIndex];

            float progressInNote = (float)(i % noteSamples) / noteSamples;
            float env = (1f - progressInNote) * (1f - ((float)i / samples) * 0.5f);

            float sample = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.6f
                         + Mathf.Sin(2f * Mathf.PI * freq * 2.75f * t) * 0.25f;

            data[i] = sample * env;
        }

        AudioClip clip = AudioClip.Create("Gem_Chime", samples, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateJumpClip()
    {
        int samples = Mathf.RoundToInt(SampleRate * 0.22f);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float progress = (float)i / samples;
            float freq = Mathf.Lerp(220f, 660f, Mathf.Sqrt(progress));
            float t = (float)i / SampleRate;

            float env = Mathf.Sin(progress * Mathf.PI) * (1f - progress * 0.5f);
            float sample = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.75f;

            data[i] = sample * env;
        }

        AudioClip clip = AudioClip.Create("Jump_Whoosh", samples, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateSlideClip()
    {
        int samples = Mathf.RoundToInt(SampleRate * 0.35f);
        float[] data = new float[samples];
        System.Random rnd = new System.Random(42);

        for (int i = 0; i < samples; i++)
        {
            float progress = (float)i / samples;
            float t = (float)i / SampleRate;

            float noise = (float)(rnd.NextDouble() * 2.0 - 1.0);
            float sweepSine = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(300f, 120f, progress) * t);

            float env = Mathf.Sin(progress * Mathf.PI);
            data[i] = (noise * 0.45f + sweepSine * 0.55f) * env * 0.7f;
        }

        AudioClip clip = AudioClip.Create("Slide_Swoosh", samples, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateFootstepClip(bool isLeft)
    {
        int samples = Mathf.RoundToInt(SampleRate * 0.085f);
        float[] data = new float[samples];
        System.Random rnd = new System.Random(isLeft ? 42 : 84);

        float baseFreq = isLeft ? 125f : 140f;
        float endFreq = isLeft ? 45f : 55f;

        for (int i = 0; i < samples; i++)
        {
            float progress = (float)i / samples;
            float freq = Mathf.Lerp(baseFreq, endFreq, progress);
            float t = (float)i / SampleRate;

            float env = Mathf.Pow(1f - progress, 3.2f);
            float lowThud = Mathf.Sin(2f * Mathf.PI * freq * t);
            float friction = (float)(rnd.NextDouble() * 2.0 - 1.0) * Mathf.Exp(-progress * 22f) * 0.35f;

            data[i] = (lowThud * 0.7f + friction) * env;
        }

        AudioClip clip = AudioClip.Create(isLeft ? "Footstep_L" : "Footstep_R", samples, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateLandingClip()
    {
        int samples = Mathf.RoundToInt(SampleRate * 0.18f);
        float[] data = new float[samples];
        System.Random rnd = new System.Random(999);

        for (int i = 0; i < samples; i++)
        {
            float progress = (float)i / samples;
            float t = (float)i / SampleRate;

            // Deep cushioned low thump (90Hz down to 30Hz)
            float freq = Mathf.Lerp(90f, 30f, Mathf.Pow(progress, 0.5f));
            float lowThud = Mathf.Sin(2f * Mathf.PI * freq * t);
            float cushionWhoosh = (float)(rnd.NextDouble() * 2.0 - 1.0) * Mathf.Exp(-progress * 12f) * 0.4f;

            float env = Mathf.Pow(1f - progress, 2.2f);
            data[i] = Mathf.Clamp(lowThud * 0.75f + cushionWhoosh, -1f, 1f) * env;
        }

        AudioClip clip = AudioClip.Create("Landing_Impact", samples, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateHitClip()
    {
        int samples = Mathf.RoundToInt(SampleRate * 0.28f);
        float[] data = new float[samples];
        System.Random rnd = new System.Random(1337);

        for (int i = 0; i < samples; i++)
        {
            float progress = (float)i / samples;
            float t = (float)i / SampleRate;

            float freq = Mathf.Lerp(180f, 40f, progress);
            float lowThud = Mathf.Sin(2f * Mathf.PI * freq * t);
            float crunch = (float)(rnd.NextDouble() * 2.0 - 1.0) * Mathf.Exp(-progress * 15f);

            float env = Mathf.Pow(1f - progress, 2f);
            data[i] = Mathf.Clamp(lowThud * 0.6f + crunch * 0.8f, -1f, 1f) * env;
        }

        AudioClip clip = AudioClip.Create("Hit_Thud", samples, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateDeathClip()
    {
        int samples = Mathf.RoundToInt(SampleRate * 0.85f);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float progress = (float)i / samples;
            float t = (float)i / SampleRate;

            float freq = Mathf.Lerp(380f, 40f, Mathf.Pow(progress, 0.6f));
            float saw = (2f * ((freq * t) % 1f)) - 1f;
            float sub = Mathf.Sin(2f * Mathf.PI * (freq * 0.5f) * t);

            float env = Mathf.Pow(1f - progress, 1.5f);
            data[i] = (saw * 0.4f + sub * 0.6f) * env;
        }

        AudioClip clip = AudioClip.Create("Death_Crash", samples, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateShieldBreakClip()
    {
        int samples = Mathf.RoundToInt(SampleRate * 0.45f);
        float[] data = new float[samples];
        System.Random rnd = new System.Random(999);

        for (int i = 0; i < samples; i++)
        {
            float progress = (float)i / samples;
            float t = (float)i / SampleRate;

            float noise = (float)(rnd.NextDouble() * 2.0 - 1.0);
            float glassBell = Mathf.Sin(2f * Mathf.PI * 1800f * t) * Mathf.Exp(-progress * 10f);

            float env = Mathf.Pow(1f - progress, 2.5f);
            data[i] = (noise * 0.5f + glassBell * 0.5f) * env;
        }

        AudioClip clip = AudioClip.Create("Shield_Break", samples, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreatePowerUpClip()
    {
        int samples = Mathf.RoundToInt(SampleRate * 0.42f);
        float[] data = new float[samples];

        float[] notes = new float[] { 523.25f, 659.25f, 783.99f, 1046.50f }; // C5, E5, G5, C6
        int subLen = samples / notes.Length;

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SampleRate;
            int nIdx = Mathf.Clamp(i / subLen, 0, notes.Length - 1);
            float freq = notes[nIdx];

            float noteProg = (float)(i % subLen) / subLen;
            float env = (1f - noteProg * 0.7f);

            float wave = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.7f
                       + Mathf.Sin(2f * Mathf.PI * (freq * 2f) * t) * 0.2f;

            data[i] = wave * env;
        }

        AudioClip clip = AudioClip.Create("PowerUp_Fanfare", samples, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateNearMissClip()
    {
        int samples = Mathf.RoundToInt(SampleRate * 0.25f);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float progress = (float)i / samples;
            float t = (float)i / SampleRate;

            // Pitch rises then falls quickly (Doppler effect)
            float freq = 400f + Mathf.Sin(progress * Mathf.PI) * 500f;
            float env = Mathf.Sin(progress * Mathf.PI);

            data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * 0.75f;
        }

        AudioClip clip = AudioClip.Create("Near_Miss", samples, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateMilestoneClip()
    {
        int samples = Mathf.RoundToInt(SampleRate * 0.6f);
        float[] data = new float[samples];

        float[] chords = new float[] { 587.33f, 739.99f, 880.00f, 1174.66f }; // D major triumphant
        int sub = samples / chords.Length;

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SampleRate;
            int idx = Mathf.Clamp(i / sub, 0, chords.Length - 1);
            float freq = chords[idx];

            float progress = (float)(i % sub) / sub;
            float env = 1f - progress * 0.5f;

            data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * 0.8f;
        }

        AudioClip clip = AudioClip.Create("Milestone_Fanfare", samples, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateElectronicMusicTrack()
    {
        // 8 seconds looping cyber runner synth loop (120 BPM, 4/4 time = 4 bars)
        float duration = 8.0f;
        int samples = Mathf.RoundToInt(SampleRate * duration);
        float[] data = new float[samples];

        float bpm = 125f;
        float beatDuration = 60f / bpm;

        // Bassline frequencies (A minor synth bass: A1, C2, D2, F1)
        float[] bassNotes = new float[] { 55.0f, 65.41f, 73.42f, 43.65f };

        System.Random rnd = new System.Random(101);

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SampleRate;
            float currentBeat = t / beatDuration;
            float beatPos = currentBeat % 1.0f;

            // 1. Kick Drum on every beat
            float kickFreq = Mathf.Lerp(120f, 35f, beatPos);
            float kickEnv = Mathf.Exp(-beatPos * 12f);
            float kick = Mathf.Sin(2f * Mathf.PI * kickFreq * beatPos * beatDuration) * kickEnv * 0.6f;

            // 2. Off-beat Hi-Hat (at 0.5 beat)
            float hatPos = (currentBeat + 0.5f) % 1.0f;
            float hatEnv = Mathf.Exp(-hatPos * 25f);
            float hat = (float)(rnd.NextDouble() * 2.0 - 1.0) * hatEnv * 0.15f;

            // 3. Arpeggiated Cyber Synth
            int bar = Mathf.FloorToInt(currentBeat / 4f) % bassNotes.Length;
            float rootFreq = bassNotes[bar];

            // 16th note arpeggio
            int sixteenth = Mathf.FloorToInt(currentBeat * 4f) % 4;
            float arpMultiplier = sixteenth switch
            {
                0 => 1f,
                1 => 1.5f,
                2 => 1.25f,
                3 => 2f,
                _ => 1f
            };
            float synthFreq = rootFreq * 4f * arpMultiplier;
            float synthEnv = Mathf.Exp(-(currentBeat * 4f % 1.0f) * 6f);
            float synth = Mathf.Sin(2f * Mathf.PI * synthFreq * t) * synthEnv * 0.22f;

            // 4. Warm Sub-Bass
            float bass = Mathf.Sin(2f * Mathf.PI * rootFreq * t) * 0.3f;

            data[i] = Mathf.Clamp(kick + hat + synth + bass, -1f, 1f) * 0.7f;
        }

        AudioClip clip = AudioClip.Create("Cyber_Runner_BGM", samples, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}

