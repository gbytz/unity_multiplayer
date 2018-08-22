using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.iOS {
	public class UnityMapsyncLibNativeInterface {
		[DllImport("__Internal")]
		private static extern IntPtr _CreateMapsyncSession(IntPtr arSession, string mapId, string userId, string developerKey, float screenHeight, float screenWidth, bool isMappingMode);

		[DllImport("__Internal")]
		private static extern void _SaveAssets(string assetJson);

		[DllImport("__Internal")]
		private static extern void _RegisterUnityCallbacks(string callbackGameObject, string assetReloadedCallback, string statusUpdatedCallback, string storePlacementCallback, string progressCallback, string objectDetectedCallback);

		[DllImport("__Internal")]
		private static extern string _MultiplayerSync(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3, float x4, float y4, float z4, string quaternion);

		[DllImport("__Internal")]
        private static extern void _Dispose();

		/// <summary>
		/// This should only be called once from MapsyncLb.cs
		/// </summary>
		public UnityMapsyncLibNativeInterface(string mapId, string userId, string developerKey, bool isMappingMode) {
			UnityARSessionNativeInterface arkit = UnityARSessionNativeInterface.GetARSessionNativeInterface ();
			IntPtr arSession = arkit.GetSession ();
			if (arSession == IntPtr.Zero) 
			{
				Debug.Log ("ARKit session is not initialized");
				return;
			}

			_CreateMapsyncSession(arSession, mapId, userId, developerKey, (float)Screen.height, (float)Screen.width, isMappingMode);

			string unityCallbackGameObject = "MapSession";
			string unityAssetLoadedCallbackFunction = "AssetReloaded";
			string unityStatusUpdatedCallback = "StatusUpdated";
			string unityStorePlacementCallback = "PlacementStored";
            string unityProgressCallback = "ProgressIncremented";
            string unityObjectDetectedCallback = "ObjectDetected";
            _RegisterUnityCallbacks (unityCallbackGameObject, unityAssetLoadedCallbackFunction, unityStatusUpdatedCallback, unityStorePlacementCallback, unityProgressCallback, unityObjectDetectedCallback);
		}

        public void Dispose() {
            _Dispose();
        }

		public MapTransform MultiplayerSync(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3, float x4, float y4, float z4, float? qx, float? qy, float? qz, float? qw) {
			string quaternion = "";
			if (qx != null) {
				quaternion = string.Format ("{0},{1},{2},{3}", qx.Value, qy.Value, qz.Value, qw.Value);
			}

			string json = _MultiplayerSync (x1, y1, z1, x2, y2, z2, x3, y3, z3, x4, y4, z4, quaternion);
			return JsonUtility.FromJson<MapTransform>(json);
		}

		public void SaveAssets(List<MapAsset> assets) {
			MapAssets mapAssets = new MapAssets () {
				Assets = assets
			};

			string assetJson = JsonUtility.ToJson (mapAssets);

			Debug.Log ("Asset json: " + assetJson);
			_SaveAssets(assetJson);
		}
	}
}
