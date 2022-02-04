using System;
using Controllers.Player;
using Managers;
using UnityEngine;

namespace Controllers
{
	public class CursorController : Singleton<CursorController>
	{
		private InputController _inputController;
		[SerializeField] private float cursorSpeed = 0.05f;
		[SerializeField] private float cursorRadius = 3.5f;
		[SerializeField] private Vector3 cursorCircleCenterOffset = new Vector3(0,1,0);
		[SerializeField] private Transform frustumParent;
		[SerializeField] public Material cursorMaterial;

		private Plane[] _planes;
		
		private Vector3 _newPosition;
		private Vector3 _unitCirclePosition;

		private Vector2 _leftFrustumPlane;
		private Vector2 _rightFrustumPlane;
		private Vector2 _downFrustumPlane;
		private Vector2 _upFrustumPlane;
		
		private Transform _leftFrustumPlaneTransform;
		private Transform _rightFrustumPlaneTransform;
		private Transform _downFrustumPlaneTransform;
		private Transform _upFrustumPlaneTransform;

		private Action _cursorAction;
		
		public Vector3 CursorWorldPosition => transform.position;
		
		
		protected override void OnAwakeEvent()
		{
			base.OnAwakeEvent();
			StopCursor();
			_inputController = InputController.Instance;
			
		}

		private void OnEnable()
		{
			GameManager.LevelStarted += StartCursor;
			GameManager.GameResumed += StartCursor;
			GameManager.GamePaused += StopCursor;
			GameManager.LevelFailed += StopCursor;
			GameManager.LevelCompleted += StopCursor;
			
		}
		
		private void StartCursor()
		{
			_cursorAction = UpdateCursor;
		}
		
		private void StopCursor()
		{
			_cursorAction = () => { };
		}


		public override void Start()
		{
			base.Start();
			CalculatePlanes();
		}

		private void Update()
		{
			_cursorAction();
		}

		private void UpdateCursor()
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
			_unitCirclePosition.x += _inputController.Look.x * cursorSpeed;
			_unitCirclePosition.y += _inputController.Look.y * cursorSpeed;
			_unitCirclePosition.z = 0;
			
			_unitCirclePosition.Normalize();

			_newPosition = _unitCirclePosition * cursorRadius;
			
			_newPosition = _inputController.transform.position + _newPosition + cursorCircleCenterOffset;
			
			// _newPosition.z = transform.position.z;

			

			// _newPosition.x = Mathf.Clamp(_newPosition.x + (_inputController.Look.x * cursorSpeed), _leftFrustumPlane.x, _rightFrustumPlane.x);
			// _newPosition.y = Mathf.Clamp(_newPosition.y + (_inputController.Look.y * cursorSpeed), _downFrustumPlane.y, _upFrustumPlane.y);
			
			
			/////-------------------- ScreenClamping Start -------------------------
			// _newPosition.x = Mathf.Clamp(_newPosition.x , _leftFrustumPlaneTransform.position.x, _rightFrustumPlaneTransform.position.x);
			// _newPosition.y = Mathf.Clamp(_newPosition.y , _downFrustumPlaneTransform.position.y, _upFrustumPlaneTransform.position.y);
			/////-------------------- ScreenClamping End ---------------------------
			transform.position = _newPosition;
		
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
			
			_leftFrustumPlaneTransform = frustumParent.GetChild(0);
			_rightFrustumPlaneTransform = frustumParent.GetChild(1);
			_downFrustumPlaneTransform = frustumParent.GetChild(2);
			_upFrustumPlaneTransform = frustumParent.GetChild(3);

			_leftFrustumPlane = _leftFrustumPlaneTransform.localPosition;
			_rightFrustumPlane = _rightFrustumPlaneTransform.localPosition;
			_downFrustumPlane = _downFrustumPlaneTransform.localPosition;
			_upFrustumPlane = _upFrustumPlaneTransform.localPosition;
		}
	}
}