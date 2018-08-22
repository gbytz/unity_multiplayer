using System;
using UnityEngine;

[Serializable]
public class MapTransform
{
	public float RotationRemoteToLocalX;
	public float RotationRemoteToLocalY;
	public float RotationRemoteToLocalZ;
	public float RotationRemoteToLocalW;

	public float OffsetLocalToRemoteX;
	public float OffsetLocalToRemoteY;
	public float OffsetLocalToRemoteZ;


	public Quaternion RotationRemoteToLocal {
		get {
			return new Quaternion (RotationRemoteToLocalX, RotationRemoteToLocalY, RotationRemoteToLocalZ, RotationRemoteToLocalW);
		}
	}

	public Vector3 OffsetLocalToRemote {
		get {
			return new Vector3 (OffsetLocalToRemoteX, OffsetLocalToRemoteY, OffsetLocalToRemoteZ);
		}
	}

	public float UpdateError;

	public MapTransform (Quaternion rotationRemoteToLocal, Vector3 offsetLocalToRemote, float updateError)
	{
		RotationRemoteToLocalX = rotationRemoteToLocal.x;
		RotationRemoteToLocalY = rotationRemoteToLocal.y;
		RotationRemoteToLocalZ = rotationRemoteToLocal.z;
		RotationRemoteToLocalW = rotationRemoteToLocal.w;

		OffsetLocalToRemoteX = offsetLocalToRemote.x;
		OffsetLocalToRemoteY = offsetLocalToRemote.y;
		OffsetLocalToRemoteZ = offsetLocalToRemote.z;

		UpdateError = updateError;
	}

	public override string ToString ()
	{
		return string.Format("Error:{0}, offsetX:{1}, offsetY:{2}, offsetZ:{3}, rotationX:{4}, rotationY:{5}, rotationZ:{6}, rotationZ:{7}",
			UpdateError, OffsetLocalToRemoteX, OffsetLocalToRemoteY, OffsetLocalToRemoteZ,
			RotationRemoteToLocalX, RotationRemoteToLocalY, RotationRemoteToLocalZ, RotationRemoteToLocalW
		);
	}
}

