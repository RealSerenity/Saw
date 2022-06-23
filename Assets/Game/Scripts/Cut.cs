using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using System;

public class Cut : MonoBehaviour
{
    public Material mat;
    public GameObject cutObject;
    private float hiz;
    private Rotate rotate;
    private float donmehýzý;
    public GameObject saw;
    public Mesh brokenMesh;
    public Material cutMat;
    public PlayerController pC;
    public GameObject player;
    private Mesh realMesh;
    public ParticleSystem woody;
    public bool isStarted = false;
    public Material[] brokenMaterials;
    private Animator anim;
    public GameObject[] pieces;
    public ParticleSystem finishParticles;
    public GameObject smokeParticle;
    public GameObject smokeTrail;

   // private Cinemachine cam;
    private void Start()
    {
        anim = GetComponent<Animator>();
        rotate = GetComponent<Rotate>();
        donmehýzý = rotate.z;
        hiz = GameManager.Instance.forwardSpeed;
        pC = player.GetComponent<PlayerController>();
        realMesh = saw.GetComponent<MeshFilter>().mesh;
        GameManager.onLoseEvent += dead;
        InputManager.Instance.onTouchStart += touchStart;
        PlayerController.onSmokeOpen += openSmoke;
        PlayerController.onSmokeClose += closeSmoke;
        GameManager.onWinEvent += onWin;
    }

    private void onWin()
    {
        pC.sparkle.SetActive(false);
        pC.drill.SetActive(false);
    }

    private void closeSmoke()
    {
        smokeTrail.SetActive(false);
    }

    private void openSmoke()
    {
        smokeTrail.SetActive(true);
    }

    private void touchStart()
    {
        if (!isStarted)
        {
            rotate.z = 1000;
            pC.drill.SetActive(true);
            pC.sparkle.SetActive(true);
            isStarted = true;
        }
    }

    private void OnDisable()
    {
        GameManager.onLoseEvent -= dead;
        InputManager.Instance.onTouchStart -= touchStart;
        PlayerController.onSmokeOpen -= openSmoke;
        PlayerController.onSmokeClose -= closeSmoke;
        GameManager.onWinEvent -= onWin;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("JCut"))
        {
                woody.Play();
                Ignorebol(other.gameObject, 100, 60);
                
        }

        if (other.gameObject.tag == "DEngel")
        {
            print("destory");
            onHit(other.gameObject);
            Destroy(other.transform.parent.gameObject);
        }
        if (other.gameObject.tag == "YEngel")
        {

            onHit(other.gameObject);
            Destroy(other.transform.parent.gameObject);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("DCut"))
        {

            if (pC.isVertical)
            {
                woody.Play();
                bol(other.gameObject,100,60);
            
            }
            else
            {
                onHit(other.gameObject);
                Destroy(other.gameObject);
            }
        
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("YCut"))
        {
    
            if (!pC.isVertical)
            {
                woody.Play();
                bol(other.gameObject, 0, 120);

            }
            else
            {
                onHit(other.gameObject);
                Destroy(other.gameObject);
            }
        }
        if(other.gameObject.tag == "Repair")
        {
            print("repair");
            //repair();
        }
        if (other.gameObject.tag == "FinishLine")
        {
            print("finish");
            finishParticles.Play();
            //GameManager.Instance.forwardSpeed = 0;
            pC.drill.SetActive(false);
            rotate.z = 0;
            StartCoroutine(Finish());
            
        }
    }


    private void Ignorebol(GameObject obj, int x, int y)
    {
        int LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore");
        //mat = other.gameObject.GetComponent<MeshRenderer>().material;
        cutObject = obj;
        if (cutObject != null)
        {
            SlicedHull clipped = CutMethod(cutObject, mat);
            GameObject clippedUp = clipped.CreateUpperHull(cutObject, cutMat);

            clippedUp.AddComponent<Rigidbody>();
            clippedUp.AddComponent<MeshCollider>().convex = true;
            //clippedUp.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
            //clippedUp.GetComponent<Rigidbody>().AddExplosionForce(-300, clippedUp.transform.position, 20);
            clippedUp.GetComponent<Rigidbody>().AddForce(new Vector3(x, y, 0));
            clippedUp.layer = LayerIgnoreRaycast;

            GameObject clippedDown = clipped.CreateLowerHull(cutObject, cutMat);

            clippedDown.AddComponent<MeshCollider>().convex = true;
            clippedDown.AddComponent<Rigidbody>();
            //clippedDown.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
            //clippedDown.GetComponent<Rigidbody>().AddExplosionForce(300, clippedDown.transform.position, 20);
            clippedDown.GetComponent<Rigidbody>().AddForce(new Vector3(-x, y, 0));
            clippedDown.layer = LayerIgnoreRaycast;

            Destroy(cutObject);
            Destroy(clippedDown, 2f);
            Destroy(clippedUp, 2f);
            cutObject = null;
        }
    }

    IEnumerator Finish()
    {
        GameManager.Instance.forwardSpeed -= 0.1f;
        yield return new WaitForSeconds(0.01f);
        StartCoroutine(Finish());
        if(GameManager.Instance.forwardSpeed < 0)
        {
            StopCoroutine(Finish());
            if (GameManager.Instance.currentState != GameManager.GameState.Finish)
            {
                GameManager.Instance.currentState = GameManager.GameState.Finish;
                GameManager.onWinEvent?.Invoke();
            }
        }
    }
    private void repair()
    {
        saw.GetComponent<MeshFilter>().mesh = realMesh;
        if(GameManager.Instance.playerChances != 2)
        {
            GameManager.Instance.playerChances++;
        }

    }

    private void bol(GameObject obj, int x, int y)
    {
        //mat = other.gameObject.GetComponent<MeshRenderer>().material;
        cutObject = obj;
        if (cutObject != null)
        {
            SlicedHull clipped = CutMethod(cutObject, mat);
            GameObject clippedUp = clipped.CreateUpperHull(cutObject, cutMat);

            clippedUp.AddComponent<Rigidbody>();
            clippedUp.AddComponent<MeshCollider>().convex = true;
            //clippedUp.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
            //clippedUp.GetComponent<Rigidbody>().AddExplosionForce(-300, clippedUp.transform.position, 20);
            clippedUp.GetComponent<Rigidbody>().AddForce(new Vector3(x, y, 0));

            GameObject clippedDown = clipped.CreateLowerHull(cutObject, cutMat);

            clippedDown.AddComponent<MeshCollider>().convex = true;
            clippedDown.AddComponent<Rigidbody>();
            //clippedDown.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
            //clippedDown.GetComponent<Rigidbody>().AddExplosionForce(300, clippedDown.transform.position, 20);
            clippedDown.GetComponent<Rigidbody>().AddForce(new Vector3(-x, y, 0));

            Destroy(cutObject);
            Destroy(clippedDown,2f);
            Destroy(clippedUp,2f);
            cutObject = null;
        }
    }

    private void onHit(GameObject obj)
    {
        Instantiate(smokeParticle, obj.transform.position, Quaternion.identity);
        if (GameManager.Instance.playerChances == 1)
        {
            saw.gameObject.SetActive(false);
            for (int i = 0; i < pieces.Length; i++)
            {
                pieces[i].SetActive(true);
            }
            Collider[] colliders = Physics.OverlapSphere(pC.transform.position, 10);
            foreach(Collider nearby in colliders)
            {
                Rigidbody rigg = nearby.GetComponent<Rigidbody>();
                if(rigg != null)
                {
                    rigg.AddExplosionForce(500, pC.transform.position,1);
                }
            }
        }
        GameManager.Instance.onHitObstacle?.Invoke();
        player.GetComponent<Rigidbody>().AddForce(new Vector3(0,0,-400));
        rotate.z /= 2;
        saw.GetComponent<MeshFilter>().mesh = brokenMesh;
        saw.GetComponent<MeshRenderer>().materials = brokenMaterials;
        //anim.SetTrigger("shake");
        pC.drill.SetActive(false);
        pC.sparkle.SetActive(false);
    }

    public void onDead()
    {
        Instantiate(smokeParticle, transform.position, Quaternion.identity);
        saw.gameObject.SetActive(false);
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].SetActive(true);
        }
        Collider[] colliders = Physics.OverlapSphere(pC.transform.position, 10);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rigg = nearby.GetComponent<Rigidbody>();
            if (rigg != null)
            {
                rigg.AddExplosionForce(500, pC.transform.position, 1);
            }
        }
        player.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -600));
        saw.GetComponent<MeshFilter>().mesh = brokenMesh;
        saw.GetComponent<MeshRenderer>().materials = brokenMaterials;
        //anim.SetTrigger("shake");
        pC.drill.SetActive(false);
        pC.sparkle.SetActive(false);
    }
    void OnTriggerExit(Collider other)
    {
        //Destroy(other.transform.parent.gameObject);
        //GameManager.Instance.forwardSpeed = hiz;
        //rotate.z = donmehýzý;
    }

  


    public SlicedHull CutMethod(GameObject obj, Material crossSectionMat = null)
    {
        return obj.Slice(transform.position, transform.right, crossSectionMat);
    }

    public void dead()
    {
        rotate.z = 0;
    }

}
