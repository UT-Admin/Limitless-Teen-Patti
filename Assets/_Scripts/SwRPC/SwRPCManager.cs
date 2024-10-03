using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using SimpleJSON;

namespace TP
{
    public class SwRPC
    {
        public string[] data;

        public SwRPC(string[] param = null)
        {
            data = param;
        }
    }

    public class SwRPCManager : MonoBehaviour
    {
        public static SwRPCManager Instance;

        private void Awake()
        {
            Instance = this;
        }


        public void SendRPC(string playerId, string action, params string[] parameters)
        {
            SwRPC rpc = new SwRPC(parameters);
            string data = JsonUtility.ToJson(rpc);
          
        }

        public void RunAllRPCWithDelay()
        {
            Invoke("RunAllMyRPC",1);
        }

        public void RunAllMyRPC()
        {
           
        }
        public void ClearRPC(int rpc_id)
        {
          
        }
        private void ExecuteRPCFunction(int id, string methodName, string[] parameterObject)
        {
            Type typeInstance = this.GetType();
            if (typeInstance != null)
            {
                MethodInfo methodInfo = typeInstance.GetMethod(methodName);
                ParameterInfo[] parameterInfo = methodInfo.GetParameters();
                object classInstance = Activator.CreateInstance(typeInstance, null);

                if (parameterInfo.Length == 0)
                {
                    var result = methodInfo.Invoke(classInstance, null);
                    ClearRPC(id);
                }
                else
                {
                    var result = methodInfo.Invoke(classInstance, parameterObject);
                    ClearRPC(id);
                }
            }
        }
        public void AddFriendToFriendList(string Id)
        {
            Debug.Log("Friend list updated");
         
        }
     
          public void RemoveFriendFromFriendList(string Id)
        {
            Debug.Log("Friend list updated");
           
        }
     


    }
}