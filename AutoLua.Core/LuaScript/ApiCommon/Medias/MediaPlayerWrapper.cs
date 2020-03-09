using Android.Media;
using Exception = Java.Lang.Exception;

namespace AutoLua.Core.LuaScript.ApiCommon.Medias
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class MediaPlayerWrapper : MediaPlayer
    {
        private MediaPlayerState State { get; set; } = MediaPlayerState.NotInitialized;

        public override void Prepare()
        {
            State = MediaPlayerState.Preparing;
            base.Prepare();
            State = MediaPlayerState.Prepared;
        }

        public override void PrepareAsync()
        {
        }

        public override void Start()
        {
            base.Start();
            State = MediaPlayerState.Start;
        }

        public override void Stop()
        {
            base.Stop();
            State = MediaPlayerState.Stopped;
        }

        public override void Pause()
        {
            base.Pause();
            State = MediaPlayerState.Paused;
        }

        public override void Release()
        {
            base.Release();
            State = MediaPlayerState.Released;
        }

        public override void Reset()
        {
            base.Reset();
            State = MediaPlayerState.NotInitialized;
        }

        public void StopAndReset()
        {
            try
            {
                if (State == MediaPlayerState.Start || State == MediaPlayerState.Paused)
                {
                    Stop();
                }

                if (State != MediaPlayerState.NotInitialized)
                {
                    Reset();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}