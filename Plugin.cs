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
        private void Awake()
        {
            CustomMusicPatch.trackList.AddRange(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\BepInEx\\plugins\\Soundtrack\\sounds"));
            LogSource = Logger;
            new CustomMusicPatch().Enable();
        }
    }
}