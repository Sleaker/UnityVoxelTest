using UnityEngine;

namespace Voxel.Behavior
{
    [RequireComponent(typeof(Datatable))]
    public class ModBehavior : MonoBehaviour
    {
        public Datatable Datatable
        {
            get { return GetComponent<Datatable>(); }           
        }

        public Observer Observer
        {
            get { return GetComponent<Observer>();  }
        }     
    }
}
