using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

using Logger = BepInEx.Logging.Logger;

namespace ActualClownHorn.Patches
{
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class GrabbableObjectPatch
    {
        private static ManualLogSource logger = Logger.CreateLogSource(ActualClownHornBase.ModGuid + ".GrabbableObjectPatch");

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void HonkPatch(GrabbableObject __instance)
        {
            Item item = __instance.itemProperties;
            logger.LogInfo(item.name);

            if (item.name != "ClownHorn")
            {
                logger.LogWarning("not a clown horn");
                return;
            }

            logger.LogInfo("found a clown horn!");
            NoisemakerProp horn = __instance.GetComponent<NoisemakerProp>();

            AudioClip near = LoadAudio("near.mp3");
            AudioClip far = LoadAudio("far.mp3");

            if (near == null || far == null)
                return;

            horn.noiseSFX = new[] { near };
            horn.noiseSFXFar = new[] { far };
        }

        private static AudioClip LoadAudio(string fileName)
        {
#if DEBUG
            string filePath = Path.Combine(Paths.PluginPath, fileName);
#else
            string filePath = Path.Combine(Paths.PluginPath, $"{ActualClownHornBase.ModAuthor}-{ActualClownHornBase.ModGuid}", fileName);
#endif
            UnityWebRequest audioClip = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.MPEG);
            audioClip.SendWebRequest();

            logger.LogInfo("Waiting for audio...");
            while(!audioClip.isDone) { }

            if (audioClip.error != null)
            {
                logger.LogError("There was an error downloading audio: " + filePath + "\n" + audioClip.error);
                return null;
            }

            logger.LogInfo("Audio downloaded");
            AudioClip content = DownloadHandlerAudioClip.GetContent(audioClip);
            if (content != null && content.loadState == AudioDataLoadState.Loaded) 
            {
                logger.LogInfo("Loaded audio " + filePath);
                content.name = Path.GetFileName(filePath);
                return content;
            }

            logger.LogError("There was an error loading audio " + filePath);
            return null;
        }
    }
}
