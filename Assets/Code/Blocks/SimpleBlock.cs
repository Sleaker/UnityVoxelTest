using UnityEngine;

namespace Voxel.Blocks
{
    public abstract class SimpleBlock : Block
    {
        public override void Placed(Vector3 worldLocation)
        {

        }

        public override void Destroyed(Vector3 worldLocation)
        {
            //var blockItem = new BlockItem 
            //{
            //    BlockToPlace = this, 
            //    TextureName = Data.TextureNames[0],
            //    MaxStackCount = 30,
            //};
            //var item = Item.Create(worldLocation + Vector3.up / 3, blockItem);
            //var c = item.AddComponent<MovingItemBehavior>();
            //c.Item = blockItem;
            //item.name = "BlockItem: " + Data.DisplayName;            
        }
    }
}
