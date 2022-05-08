using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace wankkoree
{
    [BepInPlugin("wankkoree.TheStanleyParableUltraDeluxe_Chinese", "TheStanleyParableUltraDeluxe_Chinese", "1.0")]
    [BepInProcess("The Stanley Parable Ultra Deluxe.exe")]
    public class TheStanleyParableUltraDeluxe_Chinese : BaseUnityPlugin
    {
        public static TheStanleyParableUltraDeluxe_Chinese Inst;
        public static Font TranslateFont;
        public static TMP_FontAsset TMPTranslateFont;
        public static ConfigEntry<string> FontName;

        private void Start()
        {
            Inst = this;
            FontName = Config.Bind<string>("config", "FontName", "geetype_meiheigb_flash", "put font package to <GameName>/BepInEx/plugins/TheStanleyParableUltraDeluxe_Chinese");
            LoadFont(FontName.Value);
            Harmony.CreateAndPatchAll(typeof(TheStanleyParableUltraDeluxe_Chinese));
            Logger.LogInfo("Loaded TheStanleyParableUltraDeluxe_Chinese Plugin.");
        }

        /// <summary>
        /// 加载字体
        /// </summary>
        /// <param name="fontName">字体名称</param>
        public void LoadFont(string fontName)
        {
            try
            {
                string path = $"{Paths.PluginPath}/TheStanleyParableUltraDeluxe_Chinese/{fontName}";
                if (File.Exists(path))
                {
                    var ab = AssetBundle.LoadFromFile(path);
                    TranslateFont = ab.LoadAsset<Font>(fontName);
                    TMPTranslateFont = ab.LoadAsset<TMP_FontAsset>($"{fontName} SDF");
                    if (TranslateFont != null && TMPTranslateFont != null)
                    {
                        Logger.LogInfo($"Loaded {fontName}.");
                    }
                    else
                    {
                        Logger.LogError($"The font file is damaged. Please check the file.");
                    }
                    ab.Unload(false);
                }
                else
                {
                    Logger.LogError($"Font {fontName} not found, Please check the path: {path}");
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"Load font exception:{e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// 修改字体
        /// </summary>
        [HarmonyPostfix, HarmonyPatch(typeof(Text), "OnEnable")]
        public static void FontPatch(Text __instance)
        {
            if (__instance.font.name != TranslateFont.name)
            {
                __instance.font = TranslateFont;
            }
        }

        /// <summary>
        /// 如果有不显示的文本，则设置显示方式为溢出
        /// </summary>
        [HarmonyPostfix, HarmonyPatch(typeof(TextMeshProUGUI), "InternalUpdate")]
        public static void TMPFontPatch2(TextMeshProUGUI __instance)
        {
            if (__instance.font == TMPTranslateFont)
            {
                if (__instance.overflowMode != TextOverflowModes.Overflow)
                {
                    if (__instance.preferredWidth > 1 && __instance.bounds.extents == Vector3.zero)
                    {
                        __instance.overflowMode = TextOverflowModes.Overflow;
                    }
                }
            } else
            {
                __instance.font = TMPTranslateFont;
            }
        }
    }
}