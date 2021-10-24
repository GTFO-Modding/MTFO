namespace MTFO.HotReload
{
    public static class HotReloader
    {
        /// <summary>
        /// Adds a Hot Manager to the behaviour instance
        /// </summary>
        public static void AddOnReloadListener(IHotManager manager)
        {
            if (behaviour == null) Setup();
            behaviour.AddManager(manager);
        }

        /// <summary>
        /// Removes a Hot Manager from the behaviour instance
        /// </summary>
        public static void RemoveOnReloadListener(IHotManager manager)
        {
            if (behaviour == null) Setup();
            behaviour.RemoveManager(manager);
        }

        /// <summary>
        /// Create a HotReloader behaviour instance if it doesn't exist and assigns it to a singleton
        /// </summary>
        public static void Setup()
        {
            if (behaviour == null)
            {
                behaviour = HotReloaderBehaviour.Instantiate(MainMenuGuiLayer.Current.PageRundownNew);
                AddOnReloadListener(new HotGameDataManager());
                AddOnReloadListener(new HotRundownManager());
                AddOnReloadListener(new HotGearManager());
            }
        }

        private static HotReloaderBehaviour behaviour;
    }
}
