﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : State
{
    private Collider[] colliders;
    American1 american1;
    Path patrolPath;
    //Boid b;
    PathFollow pf;
    
    public override void Enter()
    {
        if (owner.gameObject.GetComponent<PathFollow>() != null)
        {
            pf.enabled = true;
        }
        else
        {
            patrolPath = GameObject.FindGameObjectWithTag("AmericanPath").GetComponent<Path>();
            pf = owner.gameObject.AddComponent<PathFollow>();
            pf.path = patrolPath;
        }
        american1 = owner.GetComponent<American1>();
        
    }
    public override void Think()
    {
        //Overlap Shpere for detecting other planes
        colliders = Physics.OverlapSphere(owner.transform.position, 5000);
        Transform nearest = null;
        int nearestRef = 0;
        string collSide;
        float nearDist = 62500f;
        if(0 < colliders.Length)
        {
            for(int i = 0; i < colliders.Length; i++)
            {
                collSide = colliders[i].gameObject.tag;
                if (collSide != american1.side)
                {
                    float thisDist = (owner.transform.position - colliders[i].transform.position).sqrMagnitude;
                    if (thisDist < nearDist)
                    {
                        nearDist = thisDist;
                        nearest = colliders[i].transform;
                        nearestRef = i;
                    }
                }
            }
        }
        if (nearest != null)
        {
            GameObject targetGO = colliders[nearestRef].gameObject;
            //Dot Product for defend or attack state check if infront
            Vector3 toTarget = (targetGO.transform.position - owner.transform.position).normalized;

            Debug.Log(Vector3.Dot(toTarget, owner.transform.forward));
            if (Vector3.Dot(toTarget, owner.transform.forward) > 0)
            {
                if(targetGO.GetComponent<Russian>().health > 0)
                {
                    owner.GetComponent<StateMachine>().ChangeState(new AttackingState(targetGO));
                }
            }
            else
            {
                if (targetGO.GetComponent<Russian>().health > 0)
                {
                    owner.GetComponent<StateMachine>().ChangeState(new DefendingState());
                }
            }
        }


    }
    public override void Exit()
    {
        pf.enabled = false;

        //Object.Destroy(pf);
    }
}

     