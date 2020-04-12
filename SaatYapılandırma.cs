using Rocket.API;

namespace DaeSaat
{
    public class SaatYapılandırma : IRocketPluginConfiguration
    {
        public ushort DijitalSaatEfektIdsi { get; set; }
        public ushort AnalogSaatEfektIdsi { get; set; }
        public ushort AkrepEfektIdBaşlangıcı { get; set; }
        public ushort YelkovanEfektIdBaşlangıcı { get; set; }

        public bool Analog { get; set; }

        public string Ayıraç { get; set; }
        public bool AyıraçYanıpSönsün { get; set; }
        public double AyıraçYanıpSönmeAralığı { get; set; }

        public string SaatRengi { get; set; }
        public string AyıraçRengi { get; set; }
        public string DakikaRengi { get; set; }

        public void LoadDefaults()
        {
            DijitalSaatEfektIdsi = 15979;
            AnalogSaatEfektIdsi = 15980;
            AkrepEfektIdBaşlangıcı = 15981;
            YelkovanEfektIdBaşlangıcı = 15993;

            Analog = true;

            Ayıraç = ".";
            AyıraçYanıpSönsün = true;
            AyıraçYanıpSönmeAralığı = 0.75d;

            SaatRengi = "FFFFFF";
            AyıraçRengi = "FFFFFF";
            DakikaRengi = "FFFFFF";
        }
    }
}