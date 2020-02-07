using System;
using Android.Media;
using AutoLua.Droid.LuaScript.Utils.Medias;
using AutoLua.Droid.Utils;
using NLua.Exceptions;
using static Android.Media.MediaScannerConnection;
using static AutoLua.Droid.LuaScript.Api.LuaFiles;

namespace AutoLua.Droid.LuaScript.Api
{
    /// <summary>
    /// 多媒体模块
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public class Media : Java.Lang.Object, IMediaScannerConnectionClient
    {
        private readonly MediaScannerConnection _scannerConnection;
        private MediaPlayerWrapper _mediaPlayer;

        public Media()
        {
            _scannerConnection = new MediaScannerConnection(AppUtils.GetAppContext, this);
            _scannerConnection.Connect();
        }

        public void OnMediaScannerConnected()
        {
        }

        public void OnScanCompleted(string path, Android.Net.Uri uri)
        {
        }

        /// <summary>
        /// 扫描路径path的媒体文件，将它加入媒体库中；或者如果该文件以及被删除，则通知媒体库移除该文件。
        /// 媒体库包括相册、音乐库等，因此该函数可以用于把某个图片文件加入相册。
        /// </summary>
        /// <param name="path">媒体文件路径</param>
        public void scanFile(string path)
        {
            var mimeType = MimeTypes.FromFileOr(path, null);
            var p = PFiles.Path(path);
            _scannerConnection.ScanFile(p, mimeType);
        }

        /// <summary>
        /// 播放音乐文件path。该函数不会显示任何音乐播放界面。如果文件不存在或者文件不是受支持的音乐格式，则抛出UncheckedIOException异常。
        /// </summary>
        /// <param name="path">音乐文件路径。</param>
        /// <param name="volume">播放音量，为0~1的浮点数，默认为1</param>
        /// <param name="looping">是否循环播放，如果looping为true则循环播放，默认为false。</param>
        /// <exception cref="LuaException"></exception>
        public void playMusic(string path, float volume = 1.0f, bool looping = false)
        {
            path = PFiles.Path(path);
            if (_mediaPlayer == null)
            {
                _mediaPlayer = new MediaPlayerWrapper();
            }

            _mediaPlayer.StopAndReset();
            try
            {
                _mediaPlayer.SetDataSource(path);
                _mediaPlayer.SetVolume(volume, volume);
                _mediaPlayer.Looping = looping;
                _mediaPlayer.Prepare();
            }
            catch (Exception e)
            {
                throw new LuaException(e.Message);
            }

            _mediaPlayer.Start();
        }

        /// <summary>
        /// 把当前播放进度调整到时间msec的位置。如果当前没有在播放音乐，则调用函数没有任何效果。
        /// 例如，要把音乐调到1分钟的位置，为media.musicSeekTo(60 * 1000)。
        /// </summary>
        /// <param name="msec">毫秒数，表示音乐进度</param>
        public void musicSeekTo(int msec)
        {
            _mediaPlayer?.SeekTo(msec);
        }

        /// <summary>
        /// 暂停音乐播放。如果当前没有在播放音乐，则调用函数没有任何效果。
        /// </summary>
        public void pauseMusic()
        {
            _mediaPlayer?.Prepare();
        }

        /// <summary>
        /// 继续音乐播放。如果当前没有播放过音乐，则调用该函数没有任何效果。
        /// </summary>
        public void resumeMusic()
        {
            _mediaPlayer?.Start();
        }

        /// <summary>
        /// 停止音乐播放。如果当前没有在播放音乐，则调用函数没有任何效果。
        /// </summary>
        public void stopMusic()
        {
            _mediaPlayer?.Stop();
        }

        /// <summary>
        /// 返回当前是否正在播放音乐。
        /// </summary>
        /// <returns></returns>
        public bool isMusicPlaying()
        {
            return _mediaPlayer?.IsPlaying ?? false;
        }

        /// <summary>
        /// 返回当前音乐的时长。单位毫秒。
        /// </summary>
        public int MusicDuration => _mediaPlayer?.Duration ?? 0;

        /// <summary>
        /// 返回当前音乐的播放进度(已经播放的时间)，单位毫秒。
        /// </summary>
        public int MusicCurrentPosition => _mediaPlayer?.CurrentPosition ?? -1;
    }
}