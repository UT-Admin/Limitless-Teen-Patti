using System;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

namespace TP
{
    public enum LocationStatusEnum
    {
        UNKNOWN,
        DISABLED_BY_USER,
        USER_NOT_AUTHORIZED,
        FAILED,
        FETCHED
    }

    public class UserDeviceInfo : SingletonMonoBehaviour<UserDeviceInfo>
    {
        LocationStatusEnum _locationStatus = LocationStatusEnum.UNKNOWN;
        public LocationStatusEnum LocationStatus { get { return _locationStatus; } }
        public string GetLatitude { get; private set; }
        public string GetLongitude { get; private set; }

        bool isLocationUpdating = false;


        public string DeviceLocationJson
        {
            get
            {
                return "{\"location\" :{\"lat\":" + GetLatitude + ",\"lng\":" + GetLongitude + "}}";
            }
        }
        public string DeviceUniqueIdentifier
        {
            get
            {
                var deviceId = "";
#if UNITY_ANDROID && !UNITY_EDITOR
                AndroidJavaClass up = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject> ("currentActivity");
                AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject> ("getContentResolver");
                AndroidJavaClass secure = new AndroidJavaClass ("android.provider.Settings$Secure");
                deviceId = secure.CallStatic<string> ("getString", contentResolver, "android_id");
#else
                deviceId = SystemInfo.deviceUniqueIdentifier;
#endif
                return deviceId;
            }
        }

        public string GetMacAddress
        {
            get
            {
                string result = "";
                result = GetIPFromServerSite().Result;
                /*foreach (NetworkInterface ninf in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ninf.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
                    if (ninf.OperationalStatus == OperationalStatus.Up)
                    {
                        result += ninf.GetPhysicalAddress().ToString();
                        break;
                    }
                }*/
                return result;
            }
        }
        public string GetIMEI
        {
            get
            {
                var deviceIMEI = "";
#if UNITY_ANDROID && !UNITY_EDITOR
                AndroidJavaObject TM = new AndroidJavaObject("android.telephony.TelephonyManager");
                deviceIMEI = TM.Call<string>("getDeviceId");
#endif
                return deviceIMEI;
            }
        }


        public void FetchLocation(Action OnComplete)
        {
            _locationStatus = LocationStatusEnum.UNKNOWN;

#if UNITY_ANDROID && !UNITY_EDITOR

            if (!isLocationEnabledByUser())
            {
                _locationStatus = LocationStatusEnum.DISABLED_BY_USER;
                OnComplete?.Invoke();
            }
            
            if (!HasUserAuthorizedLocationPermission())
            {
                _locationStatus = LocationStatusEnum.USER_NOT_AUTHORIZED;
                OnComplete?.Invoke();
            }

            if (!isLocationUpdating)
            {
                isLocationUpdating = true;
                StartCoroutine(StartLocationUpdate(OnComplete));
            }
#elif UNITY_EDITOR
            //Chennai Location
            GetLatitude = "13°04'N";
            GetLongitude = "80°17'E";

            _locationStatus = LocationStatusEnum.FETCHED;
            OnComplete?.Invoke();
#endif

        }

        IEnumerator StartLocationFromIP(Action<LocationData> Done)
        {
            using (UnityWebRequest www = UnityWebRequest.Get("http://ip-api.com/json"))
            {
                www.timeout = 45;
                yield return www.SendWebRequest();

                while (!www.isDone)
                    yield return www;

                if(www.result == UnityWebRequest.Result.Success)
                {
                    LocationData data = JsonUtility.FromJson<LocationData>(www.downloadHandler.text);
                    Done(data);
                }
                else
                {
                    Done(null);
                }
            }
        }

        IEnumerator StartLocationUpdate(Action OnComplete)
        {
            if (!Input.location.isEnabledByUser)
            {
                isLocationUpdating = false;
                yield break;
            }

            Input.location.Start(1, 0.1f);
            // Wait until service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1)
            {
                isLocationUpdating = false;
                yield break;
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                isLocationUpdating = false;

                _locationStatus = LocationStatusEnum.FAILED;
                OnComplete?.Invoke();
                yield break;
            }
            else
            {
                GetLatitude = Input.location.lastData.latitude.ToString();
                GetLongitude = Input.location.lastData.longitude.ToString();

                _locationStatus = LocationStatusEnum.FETCHED;
                OnComplete?.Invoke();
            }
            // Stop service if there is no need to query location updates continuously
            Input.location.Stop();
            isLocationUpdating = false;
        }

        bool HasUserAuthorizedLocationPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {

                return false;
            }
#endif
            return true;
        }

        bool isLocationEnabledByUser()
        {
            if (Input.location.isEnabledByUser)
            {
                return true;
            }
            return false;
        }


        private async Task<string> GetIPFromServerSite()
        {
            var webClient = new System.Net.WebClient();
            string url = "http://checkip.dyndns.org";
            string resultBody = await webClient.DownloadStringTaskAsync(url);
            resultBody = resultBody.Substring(resultBody.IndexOf(":") + 1);
            return resultBody.Substring(0, resultBody.IndexOf("<"));
        }

        /// <summary>
        /// Get the MAC of the Netowrk Interface used to connect to the specified url.
        /// </summary>
        /// <param name="allowedURL">URL to connect to.</param>
        /// <param name="port">The port to use. Default is 80.</param>
        /// <returns></returns>
        private PhysicalAddress GetCurrentMAC(string allowedURL, int port = 80)
        {
            //create tcp client
            var client = new TcpClient();

            //start connection
            client.Client.Connect(new IPEndPoint(Dns.GetHostAddresses(allowedURL)[0], port));

            //wai while connection is established
            while (!client.Connected)
            {
                Thread.Sleep(500);
            }

            //get the ip address from the connected endpoint
            var ipAddress = ((IPEndPoint)client.Client.LocalEndPoint).Address;

            //if the ip is ipv4 mapped to ipv6 then convert to ipv4
            if (ipAddress.IsIPv4MappedToIPv6)
                ipAddress = ipAddress.MapToIPv4();

            Debug.Log(ipAddress);

            //disconnect the client and free the socket
            client.Client.Disconnect(false);

            //this will dispose the client and close the connection if needed
            client.Close();

            var allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            //return early if no network interfaces found
            if (!(allNetworkInterfaces?.Length > 0))
                return null;

            foreach (var networkInterface in allNetworkInterfaces)
            {
                //get the unicast address of the network interface
                var unicastAddresses = networkInterface.GetIPProperties().UnicastAddresses;

                //skip if no unicast address found
                if (!(unicastAddresses?.Count > 0))
                    continue;

                //compare the unicast addresses to see 
                //if any match the ip address used to connect over the network
                for (var i = 0; i < unicastAddresses.Count; i++)
                {
                    var unicastAddress = unicastAddresses[i];

                    //this is unlikely but if it is null just skip
                    if (unicastAddress.Address == null)
                        continue;

                    var ipAddressToCompare = unicastAddress.Address;

                    Debug.Log(ipAddressToCompare);

                    //if the ip is ipv4 mapped to ipv6 then convert to ipv4
                    if (ipAddressToCompare.IsIPv4MappedToIPv6)
                        ipAddressToCompare = ipAddressToCompare.MapToIPv4();

                    Debug.Log(ipAddressToCompare);

                    //skip if the ip does not match
                    if (!ipAddressToCompare.Equals(ipAddress))
                        continue;

                    //return the mac address if the ip matches
                    return networkInterface.GetPhysicalAddress();
                }

            }

            //not found so return null
            return null;
        }
    }

}