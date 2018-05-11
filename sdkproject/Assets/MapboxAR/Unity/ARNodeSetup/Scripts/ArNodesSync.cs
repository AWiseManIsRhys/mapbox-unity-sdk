﻿namespace Mapbox.Unity.Ar
{
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.Location;
	using Mapbox.Utils;
	using System.Linq;

	/// <summary>
	///  Generates and filters ArNodes for ARLocationManager.
	/// </summary>
	public class ArNodesSync : NodeSyncBase
	{

		[SerializeField]
		Transform _targetTransform;

		[SerializeField]
		float _minMagnitudeBetween;

		AbstractMap _map;
		CircularBuffer<Node> _nodeBuffer;

		public override void InitializeNodeBase(AbstractMap map)
		{
			_nodeBuffer = new CircularBuffer<Node>(60);
			_map = map;
			IsNodeBaseInitialized = true;
			Debug.Log("Initialized ARNodes");
		}

		public override void SaveNode()
		{
			bool saveNode = false;
			if (_nodeBuffer.Count > 1)
			{
				var previousNodePos = _map.GeoToWorldPosition(_nodeBuffer[0].LatLon, false);
				var currentMagnitude = _targetTransform.position - previousNodePos;

				if (currentMagnitude.magnitude >= _minMagnitudeBetween)
				{
					saveNode = true;
				}
			}
			else
			{
				saveNode = true;
			}
			if (saveNode == true)
			{
				Debug.Log("Saving AR Node");
				var node = new Node
				{
					LatLon = _map.WorldToGeoPosition(_targetTransform.position),
					// HACK: Save the map snapped location accuracy to here.
					Accuracy = 6
				};

				_nodeBuffer.Add(node);
			}
		}

		public override Node[] ReturnNodes()
		{
			return _nodeBuffer.GetEnumerable().ToArray();
		}

		public override Node ReturnLatestNode()
		{
			return _nodeBuffer[0];
		}
	}
}


