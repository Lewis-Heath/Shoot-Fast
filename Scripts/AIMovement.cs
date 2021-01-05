using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class AIMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent m_Agent;
    [SerializeField] private ThirdPersonCharacter m_Character;
    [SerializeField] private GameObject m_Destinations;
    [SerializeField] private AIType m_State;

    private List<Transform> m_DestinationsList = new List<Transform>();
    private int m_DestinationsSize = 0;
    private Vector3 m_CurrentDestination;

    void Start()
    {
        if(m_State != AIType.Idle)
        {
            foreach (Transform t in m_Destinations.GetComponentInChildren<Transform>())
            {
                m_DestinationsSize++;
                m_DestinationsList.Add(t);
            }

            m_Agent.speed = NewSpeed();
            m_CurrentDestination = NewDestination();
            m_Agent.SetDestination(m_CurrentDestination);

            //Agent settings
            m_Agent.updateRotation = false;
        }
    }

    void Update()
    {
        if(m_State != AIType.Idle)
        {
            //Character movement
            if (m_Agent.remainingDistance > m_Agent.stoppingDistance)
            {
                m_Character.Move(m_Agent.desiredVelocity, false, false);
            }
            else
            {
                m_Agent.speed = NewSpeed();
                m_CurrentDestination = NewDestination();
                m_Agent.SetDestination(m_CurrentDestination);
            }
        } 
    }

    private Vector3 NewDestination()
    {
        bool newDestination = false;
        while (!newDestination)
        {
            Vector3 tempPosition = m_DestinationsList[Random.Range(0, m_DestinationsSize)].position;
            if(tempPosition != m_CurrentDestination)
            {
                return tempPosition;
            }
        }
        return new Vector3(0f, 0f, 0f);
    }

    private float NewSpeed()
    {
        if(m_State == AIType.Run)
            return Random.Range(0.75f, 1.0f);
        if (m_State == AIType.Walk)
            return Random.Range(0.4f, 0.6f);
        else
            return 0;
    }
}
