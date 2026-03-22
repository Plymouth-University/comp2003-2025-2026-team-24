UNITY SETUP QUICK GUIDE

1. Import all scripts into your Unity project.

2. Create TileData assets:
   Right click → Create → Game → Tile
   Create 5 test tiles:
   - Path (weight 10, score 5)
   - Cross (weight 8, score 6)
   - RopeSingle (weight 5, score 10)
   - RopeDouble (weight 3, score 15)
   - Zipline (weight 2, score 20)

3. Create TileDatabase:
   Right click → Create → Game → Tile Database
   Add all 5 TileData into list.

4. Scene setup:
   - Empty GameObject → GameHandler (assign GameManager + TileHandSystem)
   - Empty GameObject → GameManager
   - Empty GameObject → TileHandSystem (assign TileDatabase)

5. Create Tile Prefab:
   - Sprite → add BoxCollider2D + Tile.cs + SpriteRenderer

6. Assign prefab to each TileData.

7. Camera:
   - Orthographic
   - Position (10,10,-10)

Press Play:
You will randomly draw 3 tiles and place them.
