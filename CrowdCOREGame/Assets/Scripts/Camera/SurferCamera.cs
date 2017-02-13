using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurferCamera : MonoBehaviour
{
    // Vertical
    public float MaxYPos = 16.5f;
    public float MinYPos = 16.0f;
    public float VertLerpTime = 1.0f;
    public bool GoingUp = true;

    // Horizontal
    public float MaxXPos = -3.0f;
    public float MinXPos = -2.0f;
    public float HorLerpTime = 5.0f;
    public bool GoingRight = true;

    // Look At
    public Transform HighPoint;
    public Transform LowPoint;

    // Active Movements
    public bool MoveX;
    public bool MoveY;
    public bool MoveLook;
    public bool HoldLook;
    
    // Timers
    float verTime, horTime = 0f;
    bool verKeepLerping, horKeepLerping;

    // Camera Cuts
    public bool CameraCutting = false; // <! It's emo
    public bool CanProgressGameState = true;
    public float CameraCutLength = 2.0f;
    public List<Transform> cameraCuts;

    void Start()
    {
        CameraCutting = false;
        GoingUp = (transform.position.y < MaxYPos);
    }

    void Update ()
    {
        if(CameraCutting)
        {
            return;
        }

        // Update times
        verTime += (GoingUp) ? Time.deltaTime : -Time.deltaTime;
        horTime += (GoingRight) ? Time.deltaTime : -Time.deltaTime;

        // Check our lerp progress
        verKeepLerping = (GoingUp) ? verTime < VertLerpTime : verTime > 0;
        horKeepLerping = (GoingRight) ? horTime < HorLerpTime : horTime > 0;

        // Flip lerps if we've reached the end
        if (!verKeepLerping) GoingUp = !GoingUp;
        if (!horKeepLerping) GoingRight = !GoingRight;

        // Update the active lerps
        if (MoveY) UpdateVertical();
        if (MoveX) UpdateHorizontal();
        if (MoveLook) UpdateLookAt();
        else if (HoldLook) HoldLookAt();
    }

    public void StartCutting()
    {
        StartCoroutine(DoCameraCutting());
    }

    void UpdateVertical()
    {
        if (verKeepLerping)
        {
            float progress = verTime / VertLerpTime;
            float yPosLerp = Mathf.Lerp(MinYPos, MaxYPos, progress);
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, yPosLerp, gameObject.transform.position.z);
        }
    }

    void UpdateHorizontal()
    {
        if (horKeepLerping)
        {
            float progress = horTime / HorLerpTime;
            float xPosLerp = Mathf.Lerp(MinXPos, MaxXPos, progress);
            gameObject.transform.position = new Vector3(xPosLerp, gameObject.transform.position.y, gameObject.transform.position.z);
        }
    }

    void UpdateLookAt()
    {
        if(HighPoint != null && LowPoint != null)
        {
            if (verKeepLerping)
            {
                float progress = verTime / VertLerpTime;
                Vector3 lookLerp =  Vector3.Lerp(LowPoint.transform.position, HighPoint.position, progress);
                gameObject.transform.LookAt(lookLerp);
            }
        }
        else
        {
            Debug.LogError("Can't perform LookAt lerp without points to reference");
        }
    }

    void HoldLookAt()
    {
        if (LowPoint != null)
        {
            gameObject.transform.LookAt(LowPoint);
        }
        else
        {
            Debug.LogError("Can't perform HoldLook without LowPoint to reference");
        }
    }
    
    private IEnumerator DoCameraCutting()
    {
        int prevIndex = -1;

        // Don't perform cuts if we don't have any
        // TODO: We should probably build a separate camera class for pre/post game presentation
        if (cameraCuts.Count < 1) yield break;

        Vector3 cachedPos = transform.localPosition;
        Vector3 cachedScale = transform.localScale;
        Quaternion cachedQuat = transform.rotation;

        CanProgressGameState = false;
        CameraCutting = true;

        yield return null;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        while (CameraCutting)
        {
            int randIndex = prevIndex;
            while(randIndex == prevIndex)
            {
                randIndex = Random.Range(0, cameraCuts.Count);
            }
            prevIndex = randIndex;
            Transform rndTransform = cameraCuts[randIndex];
            transform.localPosition = rndTransform.localPosition;
            transform.localRotation = rndTransform.localRotation;
            transform.localScale = rndTransform.localScale;
            yield return new WaitForSeconds(CameraCutLength);
        }

        transform.localPosition = cachedPos;
        transform.localRotation = cachedQuat;
        transform.localScale = cachedScale;
        CanProgressGameState = true;
    }
}
