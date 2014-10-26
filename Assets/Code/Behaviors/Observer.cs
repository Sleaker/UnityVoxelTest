namespace Voxel.Behavior
{
    /// <summary>
    /// Specifies how chunks will be loaded around this observer
    /// </summary>
    public enum LoadType
    {
        /// <summary>
        /// Loads chunks in an X by X square, where X is the LoadRange of the observer
        /// </summary>
        Square,
        /// <summary>
        /// Loads chunks in a radius X where X is the LoadRange of the observer
        /// </summary>
        Circle,
    }

    public class Observer : ModBehavior
    {
        public const int DefaultObserverRange = 8;


        public int LoadRange
        {
            get;
            set;
        }

        public LoadType LoadType
        {
            get;
            set;
        }

        void Awake()
        {
            //By default, load 8 chunks
            LoadRange = DefaultObserverRange;

            gameObject.tag = "Observer";
        }

        void OnDestroy()
        {
            gameObject.tag = "";
        }

        public void MakeNotObservable()
        {
            Destroy(this);
        }
    }
}
