using UnityEngine;
using UnityEngine.Tilemaps;

namespace Coulank.Graphics
{
    class Tiles
    {
        public static Tile TileClone(Tile tile, EColor eColor = EColor.None)
        {
            Tile newTile = (Tile)ScriptableObject.CreateInstance("Tile");
            newTile.sprite = tile.sprite;
            if (eColor == EColor.None)
                newTile.color = tile.color;
            else
                newTile.color = Colors.EColor2Color(eColor);
            newTile.colliderType = tile.colliderType;
            newTile.hideFlags = tile.hideFlags;
            newTile.transform = tile.transform;
            newTile.flags = tile.flags;
            return newTile;
        }
    }
}
