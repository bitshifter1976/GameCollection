using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Framework
{
    public class SoundManager : GameComponent
    {
        private readonly Dictionary<string, Song> songs = new();
        private readonly Dictionary<string, SoundEffect> effects = new();
        private readonly List<Tuple<string,SoundEffectInstance>> playingEffects = new();
        private Song currentSong;
        private bool isMusicPaused;

        public string CurrentSong { get; private set; }

        public float MusicVolume
        {
            get => MediaPlayer.Volume;
            set => MediaPlayer.Volume = value;
        }

        public float EffectVolume
        {
            get => SoundEffect.MasterVolume;
            set => SoundEffect.MasterVolume = value;
        }

        public bool IsSongActive => currentSong != null && MediaPlayer.State != MediaState.Stopped;

        public bool IsSongPaused => currentSong != null && isMusicPaused;

        public SoundManager(Game game) : base(game)
        {
            
        }

        public void LoadSong(string songName)
        {
            if (!songs.ContainsKey(songName))
                songs.Add(songName, Manager.Content.Load<Song>($"sound/song/{songName}"));
        }

        public void LoadEffect(string effectName)
        {
            if (!effects.ContainsKey(effectName))
                effects.Add(effectName, Manager.Content.Load<SoundEffect>($"sound/fx/{effectName}"));
        }

        public void Exit()
        {
            StopSong();
            StopAllEffects();
        }

        public void PlaySong(string songName)
        {
            PlaySong(songName, true);
        }

        public void PlaySong(string songName, bool loop)
        {
            if (CurrentSong != songName)
            {
                if (currentSong != null)
                    MediaPlayer.Stop();

                if (songs.TryGetValue(songName, out currentSong))
                {
                    CurrentSong = songName;
                    isMusicPaused = false;
                    MediaPlayer.IsRepeating = loop;
                    MediaPlayer.Play(currentSong);

                    if (!Enabled)
                        MediaPlayer.Pause();
                }
            }
        }

        public bool IsSongPlaying(string songName)
        {
            return CurrentSong == songName && MediaPlayer.State == MediaState.Playing;
        }

        public void PauseSong()
        {
            if (currentSong != null && !isMusicPaused)
            {
                if (Enabled) MediaPlayer.Pause();
                isMusicPaused = true;
            }
        }

        public void ResumeSong()
        {
            if (currentSong != null && isMusicPaused)
            {
                if (Enabled) MediaPlayer.Resume();
                isMusicPaused = false;
            }
        }

        public void StopSong()
        {
            if (currentSong != null && MediaPlayer.State != MediaState.Stopped)
            {
                MediaPlayer.Stop();
                isMusicPaused = false;
            }
        }

        public void PlayEffect(string effectName)
        {
            PlayEffect(effectName, 1.0f, 0.0f, 0.0f);
        }

        public void PlayEffect(string effectName, float volume)
        {
            PlayEffect(effectName, volume, 0.0f, 0.0f);
        }

        /// <summary>
        /// Plays the sound of the given name with the given parameters.
        /// </summary>
        /// <param name="effectName">Name of the sound</param>
        /// <param name="volume">Volume, 0.0f to 1.0f</param>
        /// <param name="pitch">Pitch, -1.0f (down one octave) to 1.0f (up one octave)</param>
        /// <param name="pan">Pan, -1.0f (full left) to 1.0f (full right)</param>
        public void PlayEffect(string effectName, float volume, float pitch, float pan)
        {
            if (effects.TryGetValue(effectName, out var sound))
            {
                var effect = sound.CreateInstance();
                effect.Volume = volume;
                effect.Pitch = pitch;
                effect.Pan = pan;
                effect.Play();
                playingEffects.Add(new Tuple<string, SoundEffectInstance>(effectName, effect));
            }
        }

        public bool IsEffectPlaying(string effectName)
        {
            return playingEffects.FirstOrDefault(e => e.Item1 == effectName) != null;
        }

        public void StopAllEffects()
        {
            playingEffects.ToList().ForEach(e =>
            {
                e.Item2.Stop(true);
                playingEffects.Remove(e);
            });
        }

        public override void Update(GameTime gameTime)
        {
            if (currentSong != null && MediaPlayer.State == MediaState.Stopped)
            {
                currentSong = null;
                CurrentSong = null;
                isMusicPaused = false;
            }
            playingEffects.ToList().ForEach(e =>
            {
                if (e.Item2.State == SoundState.Stopped)
                    playingEffects.Remove(e);
            });
            base.Update(gameTime);
        }
    }
}
