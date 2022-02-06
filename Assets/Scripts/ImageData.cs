// Json has an a array of images
[System.Serializable]
public class ImagesData
{
    public ImageItem[] images;
}

// Each image in the json is in this format
[System.Serializable]
public class ImageItem
{
    public string url;
    //Width and height are not used, but they can be usefull in the future
    public float width;
    public float height; 
}