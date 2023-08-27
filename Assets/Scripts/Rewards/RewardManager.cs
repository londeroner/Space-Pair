using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager instance;

    public List<Transform> RewardWindowPoints = new List<Transform>();

    public List<GameObject> RewardWindowList = new List<GameObject>();

    public GameObject RewardPrefab;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            gameObject.SetActive(false);
        }
        else Destroy(this);
    }

    public void SetupRewards()
    {
        var rewardPool = GameSettings.instance.RewardPool;

        for (int i = 0; i < RewardWindowPoints.Count; i++)
        {
            var rndIndex = Random.Range(0, rewardPool.Count);

            var go = Instantiate(RewardPrefab, RewardWindowPoints[i]);
            RewardWindowList.Add(go);

            if (rewardPool[rndIndex] is RewardStatModifierSO reward)
            {
                go.GetComponent<RewardStatModifierCard>().SetupReward(reward);
            }
        }
    }

    public void DestroyRewardWindow()
    {
        foreach (var rewardWindow in RewardWindowList)
        {
            Destroy(rewardWindow);
        }
        RewardWindowList.Clear();

        gameObject.SetActive(false);
    }
}
