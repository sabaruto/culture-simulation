using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Color[] beliefColors;

    private BackgroundManager bgManager;
    private PlayerManager playerManager;
    public int NumberOfBeliefs => beliefColors.Length;

    private void Awake()
    {
        bgManager = GetComponent<BackgroundManager>();
        playerManager = GetComponent<PlayerManager>();
        Beliefs.Initialize(beliefColors);
    }

    private void Start()
    {
        playerManager.Create();
    }
}
