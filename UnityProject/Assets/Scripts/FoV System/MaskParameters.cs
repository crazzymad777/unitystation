﻿using System;
using UnityEngine;

public struct MaskParameters : IEquatable<MaskParameters>
{
	public readonly float cameraOrthographicSize;
	public readonly Vector2Int screenSize;
	public readonly float cameraAspect;
	public readonly float maskCameraSizeAdd;

	private bool mExtendedDataCalculated;
	private float mExtendedCameraSize;
	private Vector2Int mExtendedTextureSize;
	private float lightTextureRescale;
	private Vector2Int mLightTextureSize;

	public MaskParameters(Camera iCamera, FovRenderSettings iRenderSettings)
	{
		cameraOrthographicSize = iCamera.orthographicSize;
		screenSize = new Vector2Int(Screen.width, Screen.height);
		cameraAspect = iCamera.aspect;
		maskCameraSizeAdd = iRenderSettings.maskCameraSizeAdd;
		lightTextureRescale = iRenderSettings.lightTextureRescale;

		// Set data default.
		// This will be lazy-calculated when required.
		mExtendedDataCalculated = default(bool);
		mExtendedCameraSize = default(float);
		mExtendedTextureSize = default(Vector2Int);
		mLightTextureSize = default(Vector2Int);
	}

	public Vector2Int extendedTextureSize
	{
		get
		{
			if (mExtendedDataCalculated == false)
			{
				CalculateExtendedData();
			}

			return mExtendedTextureSize;
		}
	}

	public Vector2Int lightTextureSize
	{
		get
		{
			if (mExtendedDataCalculated == false)
			{
				CalculateExtendedData();
			}

			return mLightTextureSize;
		}
	}

	public float extendedCameraSize
	{
		get
		{
			if (mExtendedDataCalculated == false)
			{
				CalculateExtendedData();
			}

			return mExtendedCameraSize;
		}
	}

	private void CalculateExtendedData()
	{
		float _widthPerSizeUnit = screenSize.x / cameraOrthographicSize;
		float _heightPerSizeUnit = screenSize.y / cameraOrthographicSize;

		mExtendedCameraSize = cameraOrthographicSize + maskCameraSizeAdd;
		mExtendedTextureSize = new Vector2Int((int)(_widthPerSizeUnit * mExtendedCameraSize), (int)(_heightPerSizeUnit * mExtendedCameraSize));

		mLightTextureSize = new Vector2Int((int)(screenSize.x * lightTextureRescale), (int)(screenSize.y * lightTextureRescale));


		mExtendedDataCalculated = true;
	}

	public static bool operator ==(MaskParameters iLeftHand, MaskParameters iRightHand)
	{
		// Equals handles case of null on right side.
		return iLeftHand.Equals(iRightHand);
	}

	public static bool operator !=(MaskParameters iLeftHand, MaskParameters iRightHand)
	{
		return !(iLeftHand == iRightHand);
	}

	public bool Equals(MaskParameters iMask)
	{
		return this.cameraOrthographicSize == iMask.cameraOrthographicSize &&
		       this.screenSize == iMask.screenSize &&
			   this.cameraAspect == iMask.cameraAspect &&
		       this.maskCameraSizeAdd == iMask.maskCameraSizeAdd &&
		       this.lightTextureRescale == iMask.lightTextureRescale;
	}
}