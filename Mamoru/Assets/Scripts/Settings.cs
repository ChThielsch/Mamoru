[System.Serializable]
public class Settings
{
    //Audio Settings
    public float MasterVolume=0;
    public float MusicVolume=0;
    public float OtherVolume=0;

    //Mouse Settings
    public float MouseSensibility=5;
    public bool MouseInvertX=false;
    public bool MouseInvertY=false;

    //Video Settings
    public bool Fullscreen = true;
    public int QualityIndex = 5;
    public int ResolutionIndex;

}
