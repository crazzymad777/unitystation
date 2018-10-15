﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct PixelPerfectRTParameter : IEquatable<PixelPerfectRTParameter>
{
	public Vector2Int units;
	public int pixelPerUnit;

	public PixelPerfectRTParameter(Vector2Int iUnits, int iPixelPerUnit)
	{
		units = iUnits;

		if (units.x < 1)
		{
			units.x = 1;
		}

		if (units.y < 1)
		{
			units.y = 1;
		}

		pixelPerUnit = iPixelPerUnit % 2 == 0 ? iPixelPerUnit : ++iPixelPerUnit;
		pixelPerUnit = Mathf.Clamp(pixelPerUnit, 1, int.MaxValue);
	}

	public Vector2Int resolution => units * pixelPerUnit;

	public float orthographicSize => units.y * 0.5f;
		
	public Vector2 GetFilteredRendererPosition(
		Vector3 iPositionToMatch,
		Vector3 iPreviousPosition,
		Vector2 iPreviousFilteredPosition)
	{
		bool _xMatchMovement = Mathf.Abs(iPreviousPosition.x - iPositionToMatch.x) > 0.00001f;
		bool _yMatchMovement = Mathf.Abs(iPreviousPosition.y - iPositionToMatch.y) > 0.00001f;
		bool _isMatchDiagonal = _xMatchMovement && _yMatchMovement;

		float _occlusionUnitPerPixel = (float)units.x / (pixelPerUnit * units.x);

		// Give Position an affinity towards one side. Helps with position noise.
		var _positionToMatch = iPositionToMatch + new Vector3(0.0001f, -0.0001f, 0);
		float _x = _occlusionUnitPerPixel * (int)(_positionToMatch.x / _occlusionUnitPerPixel);
		float _y = _occlusionUnitPerPixel * (int)(_positionToMatch.y / _occlusionUnitPerPixel);

		bool _xFilteredMovement = Mathf.Abs(iPreviousFilteredPosition.x - _x) > 0.00001f;
		bool _yFilteredMovement = Mathf.Abs(iPreviousFilteredPosition.y - _y) > 0.00001f;
		bool _isFilteredDiagonal = _xFilteredMovement && _yFilteredMovement;

		if (_isMatchDiagonal && _isFilteredDiagonal == false)
		{
			// Reject current position since it will result in orthogonal shift.
			return iPreviousFilteredPosition;
		}

		return new Vector2(_x, _y);
	}

	public static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
	{
		lineDir.Normalize();//this needs to be a unit vector
		var v = pnt - linePnt;
		var d = Vector3.Dot(v, lineDir);
		return linePnt + lineDir * d;
	}

	public static bool operator ==(PixelPerfectRTParameter iLeftHand, PixelPerfectRTParameter iRightHand)
	{
		// Equals handles case of null on right side.
		return iLeftHand.Equals(iRightHand);
	}

	public static bool operator !=(PixelPerfectRTParameter iLeftHand, PixelPerfectRTParameter iRightHand)
	{
		return !(iLeftHand == iRightHand);
	}

	public bool Equals(PixelPerfectRTParameter iParameter)
	{
		return this.units == iParameter.units &&
		       this.pixelPerUnit == iParameter.pixelPerUnit;
	}
}