using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{

    public GameObject saw;
    public GameObject drill;
    public GameObject sparkle;
    private Touch touch;
    public bool isVertical = true;
    public Cut cut;

    public static Action onSmokeOpen;
    public static Action onSmokeClose;

    void Start()
    {
        InputManager.Instance.onTouchStart += ProcessPlayerSwere;
        InputManager.Instance.onTouchMove += ProcessPlayerSwere;        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Deadzone")
        {
            GameManager.Instance.invokeLose();
            cut.onDead();
        }
    }

    private void OnDisable()
    {
        InputManager.Instance.onTouchStart -= ProcessPlayerSwere;
        InputManager.Instance.onTouchMove -= ProcessPlayerSwere;
    }
   

    private void ProcessPlayerSwere()   
    {

        if (GameManager.Instance.currentState == GameManager.GameState.Normal)
        {
            GetComponent<Mover>().MoveTo(new Vector3(
                -InputManager.Instance.GetDirection().x * GameManager.Instance.horizontalSpeed * 2, 0f, 0f));
        }
  
    }


    private void Update()
    {
        ProcessPlayerForwardMovement();
        PlayerRotate();
    }

    private void PlayerRotate()
    {
        if (GameManager.Instance.currentState == GameManager.GameState.Normal)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                    isVertical = true;
                    drill.SetActive(true);
                    sparkle.SetActive(true);
                saw.transform.DORotate(new Vector3(0, 0, 0), 0.2f);
                    onSmokeOpen?.Invoke();


            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                isVertical = false;
                drill.SetActive(false);
                sparkle.SetActive(false);
                saw.transform.DORotate(new Vector3(0, 0, -90), 0.2f);
                onSmokeClose?.Invoke();
            }

            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    if (touch.deltaPosition.y > 100)
                    {
                        isVertical = true;
                        drill.SetActive(true);
                        sparkle.SetActive(true);
                        saw.transform.DORotate(new Vector3(0, 0, 0), 0.2f);
                        //saw.transform.DOMoveY(1.010512f, 0.2f);
                        onSmokeOpen?.Invoke();
                    }
                    else if (touch.deltaPosition.y < -100)
                    {
                        isVertical = false;
                        drill.SetActive(false);
                        sparkle.SetActive(false);
                        saw.transform.DORotate(new Vector3(0, 0, 90), 0.2f);
                        //saw.transform.DOMoveY(1.2f, 0.2f);
                        onSmokeClose?.Invoke();
                    }
                }
            }
        }
    }

    private void ProcessPlayerForwardMovement()
    {
        if (GameManager.Instance.currentState == GameManager.GameState.Normal)
        {
            GetComponent<Mover>().MoveTo(new Vector3(
                0f,
                0f,
                GameManager.Instance.forwardSpeed/3.5f));
        }
    }

   
}
