using SPT.Reflection.Patching;
using EFT.UI;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using System;
using HarmonyLib;
using Comfort.Common;

namespace BobbyRenzobbi.CustomMenuMusic
{
    public class CustomMusicPatch : ModulePatch
    {
        private static int rndNumber = 0;
        public static Dictionary<string, AudioClip> tracks = new Dictionary<string, AudioClip>();
        private static List<AudioClip> audioClips = new List<AudioClip>();
        private static System.Random rand = new System.Random();

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GUISounds), nameof(GUISounds.method_3));
        }

        private void LoadAudioClips()
        {
            string[] musicTracks = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\BepInEx\\plugins\\CustomMenuMusic\\music");

            audioClips.Clear();
            rndNumber = rand.Next(musicTracks.Length);
            string fileDir = musicTracks[rndNumber];
            var clip = RequestAudioClip(fileDir);
            clip.name = Path.GetFileName(fileDir);
            audioClips.Add(clip);
            Logger.LogInfo(clip.name + " added to audioClips");
        }

        private AudioClip RequestAudioClip(string path)
        {
            string extension = Path.GetExtension(path);
            Dictionary<string, AudioType> audioType = new Dictionary<string, AudioType>
            {
                [".wav"] = AudioType.WAV,
                [".ogg"] = AudioType.OGGVORBIS,
                [".mp3"] = AudioType.MPEG
            };
            UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, audioType[extension]);
            UnityWebRequestAsyncOperation sendWeb = uwr.SendWebRequest();

            while (!sendWeb.isDone)

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Logger.LogError("CustomMenuMusic Mod: Failed To Fetch Audio Clip");
                return null;
            }
            AudioClip audioclip = DownloadHandlerAudioClip.GetContent(uwr);
            return audioclip;
        }

        [PatchPrefix]
        static bool Prefix()
        {
            CustomMusicPatch patch = new CustomMusicPatch();
            patch.LoadAudioClips();
            //Credit to SamSWAT for discovering that the game loads infinitely if the audioClip_0 array has only one element
            if (audioClips.Count == 1)
            {
                audioClips.Add(audioClips[0]);
            }
            Traverse.Create(Singleton<GUISounds>.Instance).Field("audioClip_0").SetValue(audioClips.ToArray());
            Logger.LogInfo("Playing " + audioClips[0].name);
            return true;
        }
    }
}