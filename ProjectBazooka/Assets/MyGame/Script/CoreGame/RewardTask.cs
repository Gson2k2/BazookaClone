using TMPro;
using UnityEngine;

namespace MyGame.Script.CoreGame
{
    public class RewardTask : MonoBehaviour
    {
        [SerializeField] private TMP_Text moneyReward;
        [SerializeField] private TMP_Text taskDetail;

        public void OnRewardTaskConfig(string rewardMoney,string rewardDetailText)
        {
            moneyReward.text = rewardMoney;
            taskDetail.text = rewardDetailText;
        }
    }
}
