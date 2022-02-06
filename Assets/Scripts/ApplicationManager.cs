using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ApplicationManager : MonoBehaviour
{
    //Content to put the images in
    public VerticalLayoutGroup scrollContent;
    //The scroll itself, its static for easy use in other scripts
    public static ScrollRect scrollComponent;
    //The image that 
    public GameObject displayImage;
    //Prefab of the small images / "template"
    public GameObject ImageHolder;

    //public CanvasScaler canvasScaler;

    private void Awake()
    {
        scrollComponent = FindObjectOfType<ScrollRect>();


        // Im not sure of this is usefull for now, but im keeping it just in case
        //if(Screen.orientation == ScreenOrientation.Landscape)
        //{
        //    canvasScaler.referenceResolution = new Vector2(800f, 600f);
        //}
        //else if (Screen.orientation == ScreenOrientation.Portrait)
        //{
        //    canvasScaler.referenceResolution = new Vector2(600f, 800f);
        //}
    }
    // Start is called before the first frame update
    void Start()
    {
        // Read all url links from json file
        // I use streaming assets for easy file changes, especially json
        string dataAsJson = File.ReadAllText(Application.streamingAssetsPath + "/images.json");
        ImagesData imagesData = JsonUtility.FromJson<ImagesData>(dataAsJson);

        for (int i = 0; i < imagesData.images.Length; i++)
        {
            DownloadImages(imagesData.images[i].url);
        }
    }

    public void ExpandImage(RectTransform rectTransform, Texture2D texture)
    {
        //Disable scroll and tell Expandable script the texture to load and the scroll element to work with
        scrollComponent.vertical = false;
        ExpandableImageScript.smallImageTarget = rectTransform;
        ExpandableImageScript.texture = texture;
        rectTransform.gameObject.GetComponentInChildren<RawImage>().enabled = false;
        displayImage.SetActive(true);
    }

    public void DownloadImages(string url) // Function for downloading image from a web request with url
    {
        StartCoroutine(ImageRequest(url, (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError) // Check for any network or http errors (for example 404 or 403)
            {
                Debug.LogError($"{req.error}: {req.downloadHandler.text}");
            }
            else
            {
                // Create new game object from prefab "template" and place it as a child element of the scroll view's content
                GameObject go = Instantiate(ImageHolder, scrollContent.transform);
                // Get the downloaded texture and assign it to the game object's raw image component
                Texture2D texture = DownloadHandlerTexture.GetContent(req);  
                go.GetComponent<RawImage>().texture = texture;
                // Keep the aspect ration of the raw image using a custom function
                go.GetComponent<RawImage>().SizeToParent();
                // Ad click events to the button for the expand function
                go.GetComponentInChildren<Button>().onClick.AddListener(delegate { ExpandImage(go.GetComponent<RectTransform>(), texture); });
            }
        }));
    }

    IEnumerator ImageRequest(string url, Action<UnityWebRequest> callback) // Corotine for a web request
    {
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(url))
        {
            yield return req.SendWebRequest();
            callback(req);
        }
    }
}

//Used to preserve aspect ration on raw images
static class CanvasExtensions
{
    public static Vector2 SizeToParent(this RawImage image, float padding = 0)
    {
        float w = 0, h = 0;
        var parent = image.GetComponentInParent<RectTransform>();
        var imageTransform = image.GetComponent<RectTransform>();

        // check if there is something to do
        if (image.texture != null)
        {
            if (!parent) { return imageTransform.sizeDelta; } //if we don't have a parent, just return our current width;
            padding = 1 - padding;
            float ratio = image.texture.width / (float)image.texture.height;
            var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
            if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
            {
                //Invert the bounds if the image is rotated
                bounds.size = new Vector2(bounds.height, bounds.width);
            }
            //Size by height first
            h = bounds.height * padding;
            w = h * ratio;
            if (w > bounds.width * padding)
            { //If it doesn't fit, fallback to width;
                w = bounds.width * padding;
                h = w / ratio;
            }
        }
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        return imageTransform.sizeDelta;
    }
}
