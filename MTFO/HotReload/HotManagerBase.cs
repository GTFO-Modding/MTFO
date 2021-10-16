namespace MTFO.HotReload
{
    public abstract class HotManagerBase
    {
        public HotManagerBase()
        {
            HotReloader.Current.AddOnReloadListener(this);
        }
        public abstract void Reload(int id);
    }
}
