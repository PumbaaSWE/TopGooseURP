using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RigBuilder : MonoBehaviour
{
    public GameObject existing;
    public GameObject target;
    public float scale = 0.05f;

    CharacterJoint[] joints;
    Vector3[] anchors;
    Vector3[] connectedAnchors;

    Rigidbody[] rigidbodies;
    Collider[] colliders;

    List<GameObject> allChildren = new List<GameObject>();

    void Start()
    {
        DoTheThing();
    }

    void DoTheThing()
    {
        joints = existing.GetComponentsInChildren<CharacterJoint>(true);
        anchors = new Vector3[joints.Length];
        connectedAnchors = new Vector3[joints.Length];

        rigidbodies = existing.GetComponentsInChildren<Rigidbody>(true);
        colliders = existing.GetComponentsInChildren<Collider>(true);

        string tn = target.name;
        AddDescendants(target.transform, allChildren);

        //add all rigidbodies
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            string n = rigidbody.gameObject.name;
            //Debug.Log("Looking for " + n + " in allChildren.");
            GameObject go = allChildren.Find(gameObject => gameObject.name == n);
            if (go == null) continue;
            Transform t = go.transform;
            if (t == null)
            {
                Debug.Log("Could not find: " + n + " to attach Rigidbody");
            }
            else
            {
                t.gameObject.AddComponent(rigidbody);
            }
        }
        //add all colliders, thes should be rescaled to fit, but we need to know what part to know how to scale correctly, so we dont do that now
        foreach (Collider collider in colliders)
        {
            string n = collider.name;
            GameObject go = allChildren.Find(gameObject => gameObject.name == n);
            if (go == null) continue;
            Transform t = go.transform;
            if (t == null)
            {
                Debug.Log("Could not find: " + n + " to attach Collider");
            }
            else
            {
                Collider newCopy = t.gameObject.AddComponent(collider);

                if (newCopy is SphereCollider collider1)
                {
                    collider1.center *= scale;
                    collider1.radius *= scale;
                }
                else if (newCopy is BoxCollider collider2)
                {
                    collider2.center *= scale;
                    collider2.size *= scale;
                }
                else if (newCopy is CapsuleCollider collider3)
                {
                    collider3.center *= scale;
                    collider3.radius *= scale;
                    collider3.height *= scale;
                }
            }
        }

        foreach (CharacterJoint joint in joints)
        {
            string n = joint.name;
            GameObject go = allChildren.Find(gameObject => gameObject.name == n);
            if (go == null) continue;
            Transform t = go.transform;

            if (t == null)
            {
                Debug.Log("Could not find: " + n + " to attach CharacterJoint");
            }
            else
            {
                CharacterJoint newJoint = t.gameObject.AddComponent(joint);
                newJoint.anchor *= scale;
                string n2 = newJoint.connectedBody.transform.gameObject.name;
                GameObject reconnection = allChildren.Find(gameObject => gameObject.name == n2);
                newJoint.connectedBody = reconnection.GetComponent<Rigidbody>();
            }


            Rigidbody[] trbs = target.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rigidbody in trbs)
            {
                rigidbody.isKinematic = true;
            }

            Collider[] colliders = target.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
        }



    }

    private void AddDescendants(Transform parent, List<GameObject> list)
    {
        foreach (Transform child in parent)
        {
            list.Add(child.gameObject);
            AddDescendants(child, list);
        }
    }


}
