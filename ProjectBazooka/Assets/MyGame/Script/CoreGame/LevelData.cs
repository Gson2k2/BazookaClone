using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGame.Script.CoreGame
{
    [CreateAssetMenu(fileName = "LevelData",menuName = "ScriptObject/LevelData",order = 3)]
    public class LevelData : ScriptableObject
    {
        [Title("First Level Reward Benefit")]
        public int levelBenefit;
        [Title("Level Reward Benefit Each Level Onward")]
        public int levelBenefitPlus;
        [Title("Level Reward First Try")]
        public int levelFirstTry;
        [Title("Level Double Kill Or More Benefit")]
        public int killingSpree;

        public List<LevelStageList> levelDataLists;

        [Serializable]
        public class LevelStageList
        {
            public string levelStageName;
            public List<LevelList> levelArr;
        }
        [Serializable]
        public class LevelList
        {
            public int level;
            public LevelController levelObject;
            public int levelCamSize;
        }
    }
}
