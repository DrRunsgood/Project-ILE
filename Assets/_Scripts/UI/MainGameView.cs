using FishNet;
using TMPro;
using UnityEngine;

public class MainGameView : View
{
    private void Awake()
    {
        InstanceFinder.RegisterInstance(this);
    }

    private void OnDestroy()
    {
        InstanceFinder.UnregisterInstance<MainGameView>();
    }
    
    public override void OnShow()
    {
        
    }

    public override void OnHide()
    {
        
    }
}
