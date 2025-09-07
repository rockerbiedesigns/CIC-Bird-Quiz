using System;
using System.Collections.Generic;

[Serializable]
public class QuizCategoryItem
{
    public int QuizID;
    public string Name;       // Mapped from "Name" in JSON
    public int Length;
    public bool InBackyardBirds;
    public bool PreselectedMedia;
    public bool Locked;
    public double CreatedDate;
    public string CreatedBy;
    public string Category1;
    public int ThingID;
    public string Name_es;
    public string Category1_es;
    public string CommonName_es;

    // Convenience property to treat "Name" as QuizName in code
    public string QuizName => Name;
}

[Serializable]
public class QuizDetail
{
    public string QuizID;
    public int ThingID;
}

[Serializable]
public class QuizDetailList
{
    public List<QuizDetail> Details;
}

[Serializable]
public class MediaThing
{
    public int ThingID;
    public int MediaID;
}

[Serializable]
public class MediaThingList
{
    public List<MediaThing> Links;
}

[Serializable]
public class MediaItem
{
    public int MediaID;
    public string Path;
}

[Serializable]
public class MediaList
{
    public List<MediaItem> Media;
}

[Serializable]
public class Classification
{
    public string ThingID;
    public string CommonNameID;
}

[Serializable]
public class ClassificationList
{
    public List<Classification> Classifications;
}

[Serializable]
public class CommonNameEntry
{
    public string CommonNameID;
    public string CommonName;
    public string CommonName_es;
}

[Serializable]
public class CommonNameList
{
    public List<CommonNameEntry> Names;
}

[Serializable]
public class SimilarThing
{
    public int ThingID;
    public List<int> SimilarIDs;
}

[Serializable]
public class SimilarThingList
{
    public List<SimilarThing> SimilarThings;
}
