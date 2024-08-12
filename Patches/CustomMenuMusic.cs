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
using System.Linq;

namespace BobbyRenzobbi.CustomMenuMusic
{
    public class CustomMusicPatch : ModulePatch
    {
        private static int rndNumber = 0;
        private static string track = "";
        private static string trackPath = "";
        private static List<AudioClip> audioClips = new List<AudioClip>();
        private static System.Random rand = new System.Random();
        internal static List<string> trackList = new List<string>();
        private static List<string> unPlayedTrackList = new List<string>();
        private static string lastTrack = "";

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GUISounds), nameof(GUISounds.method_3));
        }

        private void LoadNextTrack()
        {
            if (trackList == null || trackList.Count == 0)
            {
                Logger.LogInfo("No music found in the 'sounds' folder");
                return;
            }
            if (unPlayedTrackList.IsNullOrEmpty())
            {
                unPlayedTrackList.AddRange(trackList);
            }
            audioClips.Clear();

            do
            {
                rndNumber = rand.Next(unPlayedTrackList.Count);
                track = unPlayedTrackList[rndNumber];
            }
            while ((track == lastTrack) && trackList.Count > 1);

            unPlayedTrackList.Remove(track);
            lastTrack = track;
            var clip = RequestAudioClip(track);
            trackPath = Path.GetFileName(track);
            audioClips.Add(clip);
            Logger.LogInfo(trackPath + " added to audioClips");
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
            patch.LoadNextTrack();
            //Credit to SamSWAT for discovering that the game loads infinitely if the audioClip_0 array has only one element
            if (audioClips.Count == 0)
            {
                return true;
            }
            if (audioClips.Count == 1)
            {
                audioClips.Add(audioClips[0]);
            }
            Traverse.Create(Singleton<GUISounds>.Instance).Field("audioClip_0").SetValue(audioClips.ToArray());
            Logger.LogInfo("Playing " + trackPath);
            return true;
        }
    }
}