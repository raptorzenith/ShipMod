using HarmonyLib;
using Mafi;
using Mafi.Core.Mods;
using Mafi.Core.Prototypes;

namespace CoiShipMod
{
    public sealed class CoiShipMod : DataOnlyMod
    {
        private const string HarmonyId = "coishipmod.core";
        private Harmony _harmony;

        public CoiShipMod(ModManifest manifest)
            : base(manifest)
        {
            _harmony = new Harmony(HarmonyId);
            _harmony.PatchAll(typeof(CoiShipMod).Assembly);
            Log.Info($"{Manifest.DisplayName}: vaisseaux spatiaux initialisés.");
        }

        public override void RegisterPrototypes(ProtoRegistrator registrator)
        {
            registrator.RegisterData(new SpacecraftPrototypeData());
        }

        public override void Dispose()
        {
            if (_harmony != null)
            {
                _harmony.UnpatchAll(HarmonyId);
                _harmony = null;
            }

            base.Dispose();
        }
    }
}
