using Controllers.Player;
using UnityEngine;

namespace Controllers
{
	public class CursorController : Singleton<CursorController>
	{
		[SerializeField] private InputController inputController;
		[SerializeField] private float cursorSpeed = 0.05f;
		[SerializeField] private Transform frustumParent;

		private Plane[] _planes;
		
		private Vector3 _newPosition;

		private Vector2 _leftFrustumPlane;
		private Vector2 _rightFrustumPlane;
		private Vector2 _downFrustumPlane;
		private Vector2 _upFrustumPlane;

		public Vector3 CursorWorldPosition => transform.position;

		public override void Start()
		{
			base.Start();
			CalculatePlanes();
		}

		private void Update()
		{
			// _planes = GeometryUtility.CalculateFrustumPlanes(_cam);
			// // Debug.Log(inputController.Look);
			// // var msg = "";
			// for (var i = 0; i < 4; i++)
			// {
			// 	var distance = _planes[i].GetDistanceToPoint(_lastPosition);
			// 	if (distance < 0)
			// 	{
			// 		distance -= 0.2f;
			// 		switch (i)
			// 		{
			// 			case 0:
			// 				_lastPosition.x -= distance;			
			// 				break;
			// 			case 1:
			// 				_lastPosition.x += distance;
			// 				break;
			// 			case 2:
			// 				_lastPosition.y -= distance;
			// 				break;
			// 			case 3:
			// 				_lastPosition.y += distance;
			// 				break;
			// 		}	
			// 	}
			// 	
			// 	// msg += $"{i} {distance} | ";
			// 	
			// }
			// // Debug.Log(msg);
			//
			// if (GeometryUtility.TestPlanesAABB(_planes, _objCollider.bounds))
			// {
			// 	_lastPosition = transform.localPosition;
			// }

			_newPosition = transform.localPosition;
			_newPosition.x = Mathf.Clamp(_newPosition.x + (inputController.Look.x * cursorSpeed), _leftFrustumPlane.x, _rightFrustumPlane.x);
			_newPosition.y = Mathf.Clamp(_newPosition.y + (inputController.Look.y * cursorSpeed), _downFrustumPlane.y, _upFrustumPlane.y);

			transform.localPosition = _newPosition;
		
			// if (!GeometryUtility.TestPlanesAABB(_planes, _objCollider.bounds))
			// {
			// 	transform.localPosition = _lastPosition;	
			// }
		
		}

		private void CalculatePlanes()
		{
			// Calculate the planes from the main camera's view frustum
			var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
	
			// Create a "Plane" GameObject aligned to each of the calculated planes
			for (var i = 0; i < 4; ++i)
			{
				var p = frustumParent.GetChild(i);
				p.name = "Plane " + i.ToString();
				p.position = -planes[i].normal * planes[i].distance;
				p.rotation = Quaternion.FromToRotation(Vector3.up, planes[i].normal);
			}

			_leftFrustumPlane = frustumParent.GetChild(0).localPosition;
			_rightFrustumPlane = frustumParent.GetChild(1).localPosition;
			_downFrustumPlane = frustumParent.GetChild(2).localPosition;
			_upFrustumPlane = frustumParent.GetChild(3).localPosition;
		}
	}
}