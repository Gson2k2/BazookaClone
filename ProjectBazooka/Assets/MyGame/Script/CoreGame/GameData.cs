using System;
using System.Collections.Generic;

namespace MyGame.Script.CoreGame
{
    public class GameData
    {
        public SystemData systemData;
        public ItemData itemData;
        public PlayerResources playerResources;
        
        [Serializable]
        public class SystemData
        {
            public int AppTargetFramerate;
            public int AudioVolumeLevel;
            public bool ShowADS;
            public bool SystemVibrate;
        }

        [Serializable]
        public class ItemData
        {
            public List<int> itemID;
        }
        [Serializable]
        public class PlayerResources
        {
            public int PlayerCash;
            public int PlayerCurrentLevel;
            public string PlayerCurrentStage;
            public int PlayerCurrentGunEquipmentID;
            public int PlayerCurrentClothEquipmentID;
        }

        public GameData()
        {
            systemData = new SystemData();
            itemData = new ItemData();
            playerResources = new PlayerResources();
        }
    }
}
