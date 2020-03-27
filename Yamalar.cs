using System;
using SDG.Unturned;
using HarmonyLib;

namespace DaeSaat
{
    [HarmonyPatch(typeof(LightingManager))]
    [HarmonyPatch("updateLighting")]
    internal class YamaZaman
    {
        private static uint _sonZaman;
        private static DateTime _sonYollanma = DateTime.Now;

        [HarmonyPostfix]
        private static void ZamanGüncellendiktenSonra()
        {
            var şimdi = DateTime.Now;

            if (Saat.Örnek.Configuration.Instance.Analog && _sonZaman == LightingManager.time
                || !Saat.Örnek.Configuration.Instance.Analog && şimdi.Subtract(_sonYollanma).TotalSeconds < Saat.Örnek.Configuration.Instance.AyıraçYanıpSönmeAralığı)
            {
                return;
            }

            _sonZaman = LightingManager.time;
            _sonYollanma = şimdi;

            Saat.Örnek.EfektleriGönder();
        }
    }
}