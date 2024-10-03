using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TP;

public class PlayerRankDetails : MonoBehaviour
{


    [SerializeField] private TMP_Text _sno;
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private TMP_Text _cashWon;
    [SerializeField] private TMP_Text _potValue;
    private LeaderboardPlayer leaderboardPlayer;
    // Start is called before the first frame update



    public void SetLeaderBoardPlayerToUI(LeaderboardPlayer leaderboardPlayer)
    {


         this.leaderboardPlayer = leaderboardPlayer;




    }
}
