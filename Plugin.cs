using BepInEx;
using BepInEx.Logging;
using System.IO;
using System;
using System.Linq;

namespace BobbyRenzobbi.CustomMenuMusic
{
    [BepInPlugin("BobbyRenzobbi.CustomMenuMusic", "CustomMenuMusic", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        CustomMusicPatch patch = new CustomMusicPatch();
        internal static ManualLogSource LogSource;
        internal static string[] clips;
        public static string[] GetTrack()
        {
            return Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\BepInEx\\plugins\\CustomMenuMusic\\music").Select(file => Path.GetFileName(file)).ToArray();
        }
        private void Awake()
        {
            clips = GetTrack();
            LogSource = Logger;
            new CustomMusicPatch().Enable();
        }
    }
}