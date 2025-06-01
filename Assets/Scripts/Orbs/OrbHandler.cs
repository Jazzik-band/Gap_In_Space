using UnityEngine;

public class OrbHandler : MonoBehaviour
{
    [SerializeField]private int necessaryOrbs;
    public static int CurrentOrbs;
    
    public int GetNecessaryOrbs()
    {
        return necessaryOrbs;
    }

    public int GetCurrentOrbs()
    {
        return CurrentOrbs;
    }

    public void ResetOrbs()
    {
        CurrentOrbs = 0;
    }

    public void IncreaseOrbs()
    {
        CurrentOrbs += 1;
    }
}
