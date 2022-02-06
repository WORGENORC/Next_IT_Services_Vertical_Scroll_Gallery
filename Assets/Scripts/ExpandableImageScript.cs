using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ExpandableImageScript : MonoBehaviour
{
    public RectTransform imageToMove;
    public static RectTransform smallImageTarget;
    public static Texture2D texture;

    private bool corotineEnded = false;
    private bool expand = false;
    private float timeOfTravel = 0.25f; //time after object reach a target place
    private float currentTime = 0; // actual floting time 
    private float normalizedValue;
    private Vector2 initilaSize; // Reference to initia size of imageToMove

    private void Awake()
    {
        //Get initial size at the beginning
        initilaSize = new Vector2(imageToMove.rect.width, imageToMove.rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        if(corotineEnded && !expand) //wait for the corotine to end (only on shrink), to disable the large image
        {
            corotineEnded = false;
            smallImageTarget.gameObject.GetComponentInChildren<RawImage>().enabled = true;
            ApplicationManager.scrollComponent.vertical = true; // Enable the scroll again
            imageToMove.GetComponent<RawImage>().texture = null; // Clear the texture just in case
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        currentTime = 0f; // Reset the time so the corotine can repeat
        imageToMove.GetComponent<RawImage>().texture = texture;
        expand = true; //Expand the image
        corotineEnded = false;
        StartCoroutine(MoveImage());
    }

    public void closeImage()
    {
        currentTime = 0f;
        expand = false; //Shrink the image
        corotineEnded = false;
        StartCoroutine(MoveImage());
    }

    IEnumerator MoveImage() // use corotine for a smooth transition
    {
        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel;
            Vector2 lerpResult;

            // Check if the we should expand or shrink the image
            // Lerp between the values for a smooth transition
            if (expand)
            {
                // Set the lepResult to scale up the image and also
                // move the image from the position of the small to the center of the screen
                lerpResult = Vector2.Lerp(smallImageTarget.rect.size, new Vector2(initilaSize.x * 0.9f, initilaSize.y * 0.9f), normalizedValue);
                imageToMove.SetPositionAndRotation(Vector3.Lerp(smallImageTarget.position, new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f), normalizedValue), new Quaternion(0, 0, 0, 0));
            }
            else
            {
                // Set the lepResult to scale down the image and also
                // move the image from the center of the screen to the position of the small image
                lerpResult = Vector2.Lerp(new Vector2(initilaSize.x * 0.9f, initilaSize.y * 0.9f), smallImageTarget.rect.size, normalizedValue);
                imageToMove.SetPositionAndRotation(Vector3.Lerp(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f), smallImageTarget.position, normalizedValue), new Quaternion(0, 0, 0, 0));
            }
            // Scale the image
            imageToMove.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lerpResult.x);
            imageToMove.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lerpResult.y);
            // Keep the aspect ration of the raw image using a custom function
            imageToMove.gameObject.GetComponent<RawImage>().SizeToParent();

            yield return null;
        }
        corotineEnded = true;
    }

    #region
    //imageToMove.anchorMin = new Vector2(0f, 0f);
    //imageToMove.anchorMax = new Vector2(1.0f, 1.0f);
    //imageToMove.offsetMin = new Vector2(0f, 0f);
    //imageToMove.offsetMax = new Vector2(0f, 0f);
    //IEnumerator MoveToStartPoint() // use corotine for a smooth transition
    //{
    //    while (currentTime <= timeOfTravel)
    //    {
    //        currentTime += Time.deltaTime;
    //        normalizedValue = currentTime / timeOfTravel;

    //        //Vector2 lerpResult = Vector2.Lerp(new Vector2(applicationManager.RefImage.rect.width * 0.9f, applicationManager.RefImage.rect.height * 0.9f), target.rect.size, normalizedValue);
    //        Vector2 lerpResult = Vector2.Lerp(new Vector2(initilaSize.x * 0.9f, initilaSize.y * 0.9f), target.rect.size, normalizedValue);
    //        imageToMove.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lerpResult.x);
    //        imageToMove.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lerpResult.y);

    //        imageToMove.SetPositionAndRotation(Vector3.Lerp(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f), target.position, normalizedValue), new Quaternion(0, 0, 0, 0));
    //        imageToMove.gameObject.GetComponent<RawImage>().SizeToParent();
    //        yield return null;
    //    }

    //    corotineEnded = true;
    //}

    //IEnumerator MoveToCenter() // use corotine for a smooth transition
    //{
    //    while (currentTime <= timeOfTravel)
    //    {

    //        currentTime += Time.deltaTime;
    //        normalizedValue = currentTime / timeOfTravel;

    //        //Vector2 lerpResult = Vector2.Lerp(target.rect.size, new Vector2(applicationManager.RefImage.rect.width * 0.9f, applicationManager.RefImage.rect.height * 0.9f), normalizedValue);
    //        Vector2 lerpResult = Vector2.Lerp(target.rect.size, new Vector2(initilaSize.x * 0.9f, initilaSize.y * 0.9f), normalizedValue);
    //        imageToMove.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lerpResult.x);
    //        imageToMove.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lerpResult.y);

    //        imageToMove.SetPositionAndRotation(Vector3.Lerp(target.position, new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f), normalizedValue), new Quaternion(0, 0, 0, 0));
    //        imageToMove.gameObject.GetComponent<RawImage>().SizeToParent();

    //        yield return null;
    //    }

    //    //imageToMove.anchorMin = new Vector2(0f, 0f);
    //    //imageToMove.anchorMax = new Vector2(1.0f, 1.0f);
    //}
    #endregion
}
