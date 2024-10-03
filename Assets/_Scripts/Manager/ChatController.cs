using DG.Tweening;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TP
{
    public class ChatController : SingletonMonoBehaviour<ChatController>

    {
        [Header("**********ChatSystem Variables************")]
        public GameObject senderContainer;
        public GameObject receiverContainer;
        public Transform parentHolder;
        public ScrollRect scroll;
        public bool isPrivateChat =false;
        public UIHandler ChatPanelTeenPatti;
        public string[] badWords;
        string _count;
        int _valueCount;
        int _displayCountNumber;
        int _sizeCount = 70;

        [SerializeField] private TMP_InputField _inputFieldText;
        [SerializeField] private TMP_Text _displayCount;
        [SerializeField] private Image _sendImage;
        [SerializeField] private TMP_Text[] _predefinedChatMessages;


        public void ClearChatBoxText()
        {
            //======================For Chat Clone Clearing ===================
            foreach (Transform child in parentHolder)
            {
                Destroy(child.gameObject);
            }
        }

       

       

        public void InputFieldController()
        {
#if GOP || TPF 
            _inputFieldText.characterLimit = 19;
#else

            _inputFieldText.characterLimit = 70;
#endif
            _count = _inputFieldText.text;
            _valueCount = _count.Length;
            _displayCountNumber = _sizeCount - _valueCount;
            _displayCount.text = _displayCountNumber.ToString();

            //************************ImageTintColorEffect*********************************//

            Color temp = _sendImage.color;
            temp.a = 1f;
            _sendImage.color = temp;
            if (_inputFieldText.text == "")
            {
                temp.a = 0.5f;
                _sendImage.color = temp;
            }
        }
        //**********This Function called for Normal Send Button In Unity******************//
        public void SendCustomMessage()
        {

            //if (ChatHandler.InstanceChat._muteChat.isOn == false)
            //{
          
             
               
            //}



        }


      

        //**********This Function called for Normal Send Button In Unity******************//
        public void SendCustomPrivateChatMessage()
        {

            Debug.LogError("<color=cayan>Went In Check");
            //if (ChatHandler.InstanceChat._muteChat.isOn == false)
            //{
            //    Debug.LogWarning(PlayerIDVal + "Printingnasinisndfiansdiasd");
            //    bool isFriend = GameController.Instance.playerNonFriendsList.Any(x => string.Equals(x.FriendPlayFabId, PlayerIDVal));


            //    Debug.LogError( "Firnd Chat Chewck "+isFriend);
            //    if (isFriend && isPrivateChat)
            //    {
            //        if (_inputFieldText.text != "" && _inputFieldText.text.Length < 20)
            //        {

            //            SendPrivateChatMessage(PlayerIDVal, _inputFieldText.text,GameController.Instance.CurrentPlayerData.GetPlayfabID());
            //            _inputFieldText.text = "";
            //            ChatPanelTeenPatti.HideMe();
            //            Debug.LogWarning("Went in CustomPrivate");
            //        }
            //        isPrivateChat = false;
            //        _inputFieldText.text = "";
            //    }
            //    else
            //    {

            //        if (_inputFieldText.text != "" && _inputFieldText.text.Length < 20)
            //        {

            //            SendChatMessage(_inputFieldText.text);
            //            _inputFieldText.text = "";
            //            ChatPanelTeenPatti.HideMe();
            //            Debug.LogWarning("Went in CustomChat");
         
            //        }
            //        _inputFieldText.text = "";
            //    }
   

            //}



        }




        //**********This Function called for Native android  Send & Ok  Button In Unity******************//
        void Input(InputField input)
        {
            if (_inputFieldText.text != " ")
            {
                SendCustomMessage();
            }
        }
        //**********This Function called for Quick Chat  In Unity******************//
        public void SendChatMessage(int index)
        {
           
            //if (ChatHandler.InstanceChat._muteChat.isOn == false)
            //{
            //    SendChatMessage(_predefinedChatMessages[index].text);
             

            //}

        }

        //************************RPC For Custom Chat*********************************//
        public void SendChatMessage(string message)
        {
    //       viewer.RPC("ShowMessage", RpcTarget.AllViaServer, message);
            if(PlayerManager.localPlayer)
               PlayerManager.localPlayer.CmdSendChatMessageMirror(message);
        }


        //************************RPC For Private Chat*********************************//
       
    }
}
