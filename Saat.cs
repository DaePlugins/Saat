using System.Collections.Generic;
using System.Reflection;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using HarmonyLib;

namespace DaeSaat
{
    public class Saat : RocketPlugin<SaatYapılandırma>
    {
        public static Saat Örnek { get; private set; }
        private Harmony _harmony;

        private string _ayıraç;

        public bool AyıraçVar { get; set; }

        public List<ulong> ArayüzEfektiniAlmayacaklar { get; } = new List<ulong>();

        protected override void Load()
        {
            Örnek = this;

            _harmony = new Harmony("dae.saat");
            _harmony.PatchAll();

            AyıraçVar = true;

            _ayıraç = $"<color=#{Configuration.Instance.AyıraçRengi}>{Configuration.Instance.Ayıraç}</color>";

            if (Configuration.Instance.Analog)
            {
                foreach (var steamOyuncu in Provider.clients)
                {
                    if (ArayüzEfektiniAlmayacaklar.Contains(steamOyuncu.playerID.steamID.m_SteamID))
                    {
                        continue;
                    }

                    EffectManager.sendUIEffect(Configuration.Instance.AnalogSaatEfektIdsi, 15980, steamOyuncu.playerID.steamID, true);
                }
            }

            U.Events.OnPlayerConnected += OyuncuBağlandığında;
        }

        protected override void Unload()
        {
            Örnek = null;

            _harmony.UnpatchAll("dae.saat");
            _harmony = null;

            foreach (var steamOyuncu in Provider.clients)
            {
                if (Configuration.Instance.Analog)
                {
                    var sonYelkovanEfektIdsi = Saat.Örnek.Configuration.Instance.YelkovanEfektIdBaşlangıcı + 59;

                    for (var id = Configuration.Instance.AnalogSaatEfektIdsi; id <= sonYelkovanEfektIdsi; id++)
                    {
                        EffectManager.askEffectClearByID(id, steamOyuncu.playerID.steamID);
                    }
                }
                else
                {
                    EffectManager.askEffectClearByID(Configuration.Instance.DijitalSaatEfektIdsi, steamOyuncu.playerID.steamID);
                }
            }

            U.Events.OnPlayerConnected -= OyuncuBağlandığında;
        }

        private void OyuncuBağlandığında(UnturnedPlayer oyuncu)
        {
            if (Configuration.Instance.Analog && !ArayüzEfektiniAlmayacaklar.Contains(oyuncu.CSteamID.m_SteamID))
            {
                EffectManager.sendUIEffect(Configuration.Instance.AnalogSaatEfektIdsi, 15980, oyuncu.CSteamID, true);
            }
        }

        public KeyValuePair<byte, byte> OyunSaatindenGerçekSaatiAl()
        {
            var saatBaşınaBirim = (decimal)LightingManager.cycle / 24;
            var dakikaBaşınaBirim = (decimal)LightingManager.cycle / 1440;

            var normalZaman = LightingManager.time - (decimal)LightingManager.cycle / 36;

            var saat = (normalZaman / saatBaşınaBirim + 5) % 24;
            var dakika = (normalZaman % saatBaşınaBirim / dakikaBaşınaBirim + 60) % 60;

            return new KeyValuePair<byte, byte>((byte)saat, (byte)dakika);
        }

        public uint GerçekSaattenOyunSaatiAl(byte saat, byte dakika)
        {
            var saatBaşınaBirim = (decimal)LightingManager.cycle / 24;
            var dakikaBaşınaBirim = (decimal)LightingManager.cycle / 1440;

            return (uint)((saat + 19) % 24 * saatBaşınaBirim + dakika * dakikaBaşınaBirim + 101);
        }
        
        public void EfektleriGönder()
        {
            var zaman = OyunSaatindenGerçekSaatiAl();

            if (Configuration.Instance.AyıraçYanıpSönsün)
            {
                AyıraçVar = !AyıraçVar;
            }
            
            if (Configuration.Instance.Analog)
            {
                var akrepIdsi = (ushort)(Configuration.Instance.AkrepEfektIdBaşlangıcı + zaman.Key % 12);
                var yelkovanIdsi = (ushort)(Configuration.Instance.YelkovanEfektIdBaşlangıcı + zaman.Value);

                foreach (var steamOyuncu in Provider.clients)
                {
                    var steamId = steamOyuncu.playerID.steamID;

                    if (ArayüzEfektiniAlmayacaklar.Contains(steamId.m_SteamID))
                    {
                        continue;
                    }

                    EffectManager.sendUIEffect(akrepIdsi, 15981, steamId, true);
                    EffectManager.sendUIEffect(yelkovanIdsi, 15993, steamId, true);
                }
            }
            else
            {
                foreach (var steamOyuncu in Provider.clients)
                {
                    var steamId = steamOyuncu.playerID.steamID;

                    if (ArayüzEfektiniAlmayacaklar.Contains(steamId.m_SteamID))
                    {
                        continue;
                    }

                    EffectManager.sendUIEffect(Configuration.Instance.DijitalSaatEfektIdsi, 15979, steamId, true,
                        $"<color=#{Configuration.Instance.SaatRengi}>{zaman.Key:00}</color>{(AyıraçVar ? _ayıraç : " ")}<color=#{Configuration.Instance.DakikaRengi}>{zaman.Value:00}</color>");
                }
            }
        }

        public override TranslationList DefaultTranslations => new TranslationList
        {
            { "Format", "{0}{1}{2}" },
            { "SaatGeçersiz", "Girdiğiniz saat geçersiz." },
            { "DakikaGeçersiz", "Girdiğiniz dakika geçersiz." }
        };
    }
}