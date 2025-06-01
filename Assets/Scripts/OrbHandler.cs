using UnityEngine;

public class OrbHandler : MonoBehaviour
{
    [SerializeField]private int necessaryOrbs;
    private int currentOrbs;
    
    public int GetNecessaryOrbs()
    {
        return necessaryOrbs;
    }

    public int GetCurrentOrbs()
    {
        return currentOrbs;
    }

    public void ResetOrbs()
    {
        currentOrbs = 0;
    }

    public void IncreaseOrbs()
    {
        currentOrbs += 1;
    }
}
