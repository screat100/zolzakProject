using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ruby_goal : MonoBehaviour
{
    [SerializeField]
    StageSetting m_areaSetting;

    [System.NonSerialized]
    public bool flagGoal;// Goal 이 활성화가 됐는가?
    
    Renderer m_leftrender;
    Renderer m_rightrender;
    public Material OnMaterial;
    public Material OffMaterial;
    void Start()
    {
        flagGoal=false;
        m_leftrender=transform.GetChild(0).GetComponent<MeshRenderer>();
        m_rightrender=transform.GetChild(1).GetComponent<MeshRenderer>();
        m_areaSetting=GameObject.Find("GameArea").GetComponent<StageSetting>();
         m_leftrender.material=OffMaterial;
         m_rightrender.material=OffMaterial;
    }
    private void OnCollisionEnter(Collision other)
    {

        if(other.gameObject.tag == "thief"&&m_areaSetting.findruby)
        {
            if (other.gameObject.name == "Player")
            {
                if (other.gameObject.GetComponent<Player>().hasruby == true)
                    m_areaSetting.ScoredThief(other.gameObject);
            }
            else if (other.gameObject.GetComponent<ThiefAgent>().hasruby == true)
            {
                other.gameObject.SetActive(false);
                m_areaSetting.ScoredThief(other.gameObject);
            }
        }
    }
    public void select_finishGoal()
    {
        transform.GetChild(0).localRotation=Quaternion.Euler(new Vector3(0,90f,0));
        transform.GetChild(1).localRotation=Quaternion.Euler(new Vector3(0,-90f,0));
        flagGoal=true;
        m_leftrender.material=OnMaterial;
        m_rightrender.material=OnMaterial;
        gameObject.tag="goal";
        gameObject.layer=6; //"goal"레이어 설정
    }
    public void Goal_reset()
    {
        transform.GetChild(0).localRotation=Quaternion.Euler(new Vector3(0,0,0));
        transform.GetChild(1).localRotation=Quaternion.Euler(new Vector3(0,0,0));
        flagGoal=false;
        m_leftrender.material=OffMaterial;
        m_rightrender.material=OffMaterial;
        gameObject.tag="wall";
        gameObject.layer=0;
    }
}
