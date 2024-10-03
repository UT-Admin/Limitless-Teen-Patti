using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TP
{
    public class FriendsPrivateInviteRow : MonoBehaviour
    {

        [SerializeField] private Image playerAvatar;
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text playerAmount;
        [SerializeField] private Button invitePlayerBtn;
 


        List<string> profileDetails = new List<string>() { StaticStrings.AvatarIndexKey }; //, StaticStrings.SilverValKey

        


        public void InvitePlayers(string friendId)
        {
            PlayerManager.localPlayer.InvitePlayerToGame(friendId, GameController.Instance.privateRoomCode);
 //           PlayerManager.localPlayer.CmdInviteToGame(friendId, GameController.Instance.privateRoomCode);
        }
        public void FetchUpdatedFriendList()
        {
       
        }
    }

}
