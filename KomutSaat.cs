using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace DaeSaat
{
    internal class KomutSaat : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "saat";
        public string Help => "Saat göstergesini açıp kapatır.";
        public string Syntax => "";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>{ "dae.saat.saat" };

        public void Execute(IRocketPlayer komutuÇalıştıran, string[] parametreler)
        {
            var oyuncu = (UnturnedPlayer)komutuÇalıştıran;

            if (parametreler.Length > 0 && oyuncu.HasPermission("dae.saat.ayarla"))
            {
                byte saat = 0;
                if (parametreler.Length >= 1)
                {
                    if (!byte.TryParse(parametreler[0], out saat))
                    {
                        UnturnedChat.Say(komutuÇalıştıran, Saat.Örnek.Translate("SaatGeçersiz"), Color.red);
                        return;
                    }
                }

                byte dakika = 0;
                if (parametreler.Length >= 2)
                {
                    if (!byte.TryParse(parametreler[1], out dakika))
                    {
                        UnturnedChat.Say(komutuÇalıştıran, Saat.Örnek.Translate("DakikaGeçersiz"), Color.red);
                        return;
                    }
                }

                LightingManager.time = Saat.Örnek.GerçekSaattenOyunSaatiAl(saat, dakika);

                return;
            }

            if (Saat.Örnek.ArayüzEfektiniAlmayacaklar.Contains(oyuncu.CSteamID.m_SteamID))
            {
                Saat.Örnek.ArayüzEfektiniAlmayacaklar.Remove(oyuncu.CSteamID.m_SteamID);

                if (Saat.Örnek.Configuration.Instance.Analog)
                {
                    EffectManager.sendUIEffect(Saat.Örnek.Configuration.Instance.AnalogSaatEfektIdsi, 15980, oyuncu.CSteamID, true);
                }
            }
            else
            {
                Saat.Örnek.ArayüzEfektiniAlmayacaklar.Add(oyuncu.CSteamID.m_SteamID);

                if (Saat.Örnek.Configuration.Instance.Analog)
                {
                    for (var id = Saat.Örnek.Configuration.Instance.AnalogSaatEfektIdsi; id <= 16052; id++)
                    {
                        EffectManager.askEffectClearByID(id, oyuncu.CSteamID);
                    }
                }
                else
                {
                    EffectManager.askEffectClearByID(Saat.Örnek.Configuration.Instance.DijitalSaatEfektIdsi, oyuncu.CSteamID);
                }
            }
        }
    }
}