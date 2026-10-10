using MelonLoader;

[assembly: MelonInfo(typeof(WarningRemovals.Core), "WarningRemovals", "1.0.0", "maddi", null)]
[assembly: MelonGame("Blue Meridian", "Prehistoric Kingdom")]

namespace WarningRemovals
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }
    }
}
