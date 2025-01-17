using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class CVR4_Area : MonoBehaviour
{
    [System.Serializable]
    public class AgentInfo
    {
        public CVR4_Agent agent;

        [HideInInspector]
        public Vector3 startPos;

        [HideInInspector]
        public Quaternion startRot;

        [HideInInspector]
        public Rigidbody rb;
    }




    [Header("Max Environment Steps")] public int MaxEnvironmentSteps ; // 50 (per 1 second)

    public int chaserNum;
    public int runnerNum;

    List<AgentInfo> chaserList;
    List<AgentInfo> runnerList;

    private SimpleMultiAgentGroup chaserGroup;
    private SimpleMultiAgentGroup runnerGroup;

    public float agentRunSpeed = 10f;
    [HideInInspector]
    public int m_ResetTimer;
    [HideInInspector]
    public int catchedRunnerNum;

    [Header("Runner Random Spawn Position")]
    public GameObject Leftup;
    public GameObject Rightdown;

    void Start()
    {
        chaserGroup = new SimpleMultiAgentGroup();
        runnerGroup = new SimpleMultiAgentGroup();

        chaserList = new List<AgentInfo>();
        runnerList = new List<AgentInfo>();

        for (int i = 0; i < chaserNum; i++)
        {
            GameObject agent = gameObject.transform.Find("Police (" + i + ")").gameObject;
            if (agent == null)
            {
                Debug.Log($"police agent listed num : { i }");
                break;
            }

            AgentInfo agentInfo = new AgentInfo();
            agentInfo.agent = agent.GetComponent<CVR4_Agent>();
            agentInfo.startPos = agent.transform.localPosition;
            agentInfo.startRot = agent.transform.rotation;
            agentInfo.rb = agent.GetComponent<Rigidbody>();

            chaserList.Add(agentInfo);
        }

        for (int i = 0; i < runnerNum; i++)
        {
            GameObject agent = gameObject.transform.Find("Thief (" + i + ")").gameObject;
            if (agent == null)
            {
                Debug.Log($"thief agent listed num : { i }");
                break;
            }

            AgentInfo agentInfo = new AgentInfo();
            agentInfo.agent = agent.GetComponent<CVR4_Agent>();
            agentInfo.startPos = agent.transform.localPosition;
            agentInfo.startRot = agent.transform.rotation;
            agentInfo.rb = agent.GetComponent<Rigidbody>();

            runnerList.Add(agentInfo);
        }


        foreach (var item in chaserList)
        {
            item.startPos = item.agent.transform.localPosition;
            item.startRot = item.agent.transform.localRotation;
            item.rb = item.agent.GetComponent<Rigidbody>();
            chaserGroup.RegisterAgent(item.agent);
        }
        foreach (var item in runnerList)
        {
            item.startPos = item.agent.transform.localPosition;
            item.startRot = item.agent.transform.localRotation;
            item.rb = item.agent.GetComponent<Rigidbody>();
            runnerGroup.RegisterAgent(item.agent);
        }

        m_ResetTimer = 0;
        catchedRunnerNum = 0;
        ResetScene();
    }

    private void FixedUpdate()
    {
        m_ResetTimer++;

        // time penalty & advantage
        chaserGroup.AddGroupReward(-1.0f / MaxEnvironmentSteps);
        runnerGroup.AddGroupReward(2.0f / MaxEnvironmentSteps);

        // Time Over => Runner win!
        if (m_ResetTimer > MaxEnvironmentSteps)
        {
            runnerGroup.GroupEpisodeInterrupted();
            chaserGroup.GroupEpisodeInterrupted();
            ResetScene();
        }


    }


    // Runner가 잡힐 때마다 chaser 보상받음
    // 반대로 runner는 처벌
    public void RunnerIsCatched()
    {
        catchedRunnerNum++;
        chaserGroup.AddGroupReward(2.0f / runnerList.Count);
        runnerGroup.AddGroupReward(-2.0f / runnerList.Count);

        // All runners are catched => chaser win!
        if (catchedRunnerNum >= runnerList.Count)
        {
            runnerGroup.GroupEpisodeInterrupted();
            chaserGroup.GroupEpisodeInterrupted();
            ResetScene();
        }

    }





    void ResetScene()
    {

        foreach (var item in chaserList)
        {
            item.agent.transform.localPosition = new Vector3(
                Random.Range(Leftup.transform.localPosition.x, Rightdown.transform.localPosition.x),
                0,
                Random.Range(Leftup.transform.localPosition.z, Rightdown.transform.localPosition.z));
            item.agent.transform.localRotation = item.startRot;
            item.rb.velocity = Vector3.zero;
            item.rb.angularVelocity = Vector3.zero;
        }

        foreach (var item in runnerList)
        {
            if (!item.agent.gameObject.activeInHierarchy)
            {
                item.agent.gameObject.SetActive(true);
                runnerGroup.RegisterAgent(item.agent);
            }

            item.agent.transform.localPosition = new Vector3(
                Random.Range(Leftup.transform.localPosition.x, Rightdown.transform.localPosition.x),
                0,
                Random.Range(Leftup.transform.localPosition.z, Rightdown.transform.localPosition.z));
            item.agent.transform.localRotation = item.startRot;
            item.rb.velocity = Vector3.zero;
            item.rb.angularVelocity = Vector3.zero;
        }

        m_ResetTimer = 0;
        catchedRunnerNum = 0;
    }


}
