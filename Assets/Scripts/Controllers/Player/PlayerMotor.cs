using System;
using Cinemachine.Utility;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Player
{
	[RequireComponent(typeof(InputController))]
	public class PlayerMotor : MonoBehaviour
	{
		private Animator _playerAnimator;
		private int _animatorX;
		private int _animatorY;
		private int _animatorIsRunning;
		private int _animatorIsJumping;
		private int _animatorIsCrouching;
		private int _animatorPlayerAltitude;
		private int _animatorVerticalVelocity;

		private Vector2 _moveInput;
		private Vector2 _animatorDirection;
		private Vector3 _moveDirection;
		private Vector3 _movementVelocity;
		private Vector3 _lookDirection;
		
		private CharacterController _controller;
		private InputController _inputController;
		private CollisionFlags _collisionFlags;

		private Camera _mainCamera;
		private Transform _mainCameraTransform;

		private Ray _groundCheckRay;
		private RaycastHit _groundRaycastHit;
		
		[SerializeField] private float speed = 3f;
		[SerializeField] private float groundDrag = 10f;
		[SerializeField] private float airDrag = 15f;
		// [SerializeField] private bool input2D = false;
		[SerializeField, Range(0f, 1f)] private float dragInputFactor = 0.5f;

		[SerializeField, Tooltip("when the horizontal movement get below this threshold, it is clamped to zero.")]
		private float minSpeedThreshold = 0.1f;
		[SerializeField] private float maxSpeed = 5f;
		[SerializeField] private float rotationSpeed = 200f;

		// [Space(10), Header("Jumping")] [SerializeField]
		// private float jumpSpeed = 10f;

		// [SerializeField, Range(0f, 1f)] private float jumpUpFactor = 0.7f;
		[SerializeField, Range(0f, 1f)] private float sneakModifier;
		[SerializeField] private float gravity = -9.8f;
		[SerializeField] private LayerMask groundLayerMask;
		[SerializeField] private Vector3 groundCheckerOffset;

		[SerializeField, Tooltip("The maximum height the player will reach when jumping.")]
		private float maxJumpHeight = 2f;
		[SerializeField, Tooltip("The maximum time the player will spend in the air while jumping from and to the same level.")]
		private float maxJumpTime = 1f;

		[SerializeField, Tooltip("The value multiplied to the gravity value when the player starts falling.")]
		private float fallMultiplier = 2f;

		[SerializeField, Tooltip("Number of jumps the player can preform without waiting for the jump cooldown.")]
		private int maxBounceJumpCount = 1;

		[SerializeField, Tooltip("The amount of time the player must wait to perform another jump, if they run out of bounce jumps.")]
		private float jumpCoolDown = 0.2f;
		
		[SerializeField, Tooltip("(Coyote Time) The amount of time in which the player can jump without after leaving the ground")]
		private float hangTime = 0.2f;
		
		[SerializeField, Tooltip("(Coyote Time) The amount of time in which the player can jump without after leaving the ground")]
		private float jumpBuffer = 0.2f;

		[Space(10), Header("Debug")] [SerializeField, ReadOnly]
		private float initialJumpingVelocity;
		
		[SerializeField, ReadOnly] private float playerAltitude;
		[SerializeField, ReadOnly] private float jumpCoolDownTimer;
		[SerializeField, ReadOnly] private float bounceJumpsCounter;
		[SerializeField, ReadOnly] private float jumpHangTimer;
		[SerializeField, ReadOnly] private float jumpBufferTimer;
		// [SerializeField, ReadOnly] private bool isJumpPressed = false;
		[SerializeField, ReadOnly] private bool isJumping = false;
		[SerializeField, ReadOnly] private bool isFalling = false;
		[SerializeField, ReadOnly] private bool jumpDownThisFrame = false;
		[SerializeField, ReadOnly] private bool jumpUpThisFrame = false;
		// [SerializeField, ReadOnly] private bool hasLanded = false;
		private bool _lastJumpFlag = false;

		private Action PlayerMovementAction;
		private Action PlayerAnimationAction;
		private Action ApplyGravityAction;


		private void Awake()
		{
			_animatorX = Animator.StringToHash("x");
			_animatorY = Animator.StringToHash("z");
			_animatorIsRunning = Animator.StringToHash("isRunning");
			_animatorIsJumping = Animator.StringToHash("isJumping");
			_animatorIsCrouching = Animator.StringToHash("isCrouching");
			_animatorPlayerAltitude = Animator.StringToHash("playerAltitude");
			_animatorVerticalVelocity = Animator.StringToHash("verticalVelocity");

			_inputController = GetComponent<InputController>();
			_controller = GetComponent<CharacterController>();
			_playerAnimator = GetComponent<Animator>();
			_mainCamera = Camera.main;
			_mainCameraTransform = _mainCamera?.gameObject.transform;

			InitializeJumpVariables();
		}
		
		private void InitializeJumpVariables()
		{
			var timeToApex = maxJumpTime / 2;
			gravity = (-2 * maxJumpHeight / Mathf.Pow(timeToApex, 2));
			initialJumpingVelocity = (2 * maxJumpHeight) / timeToApex;
			_animatorDirection = Vector2.zero;
			_moveDirection = Vector3.zero;
			_movementVelocity = Vector3.zero;
			groundCheckerOffset = Vector3.up * (_controller.radius + _controller.skinWidth + 0.05f);
			PlayerAnimationAction = UpdateAnimator;
			PlayerMovementAction = UpdateInput;
			ApplyGravityAction = ApplyGravity;
		}
		
		private void Update()
		{
			PlayerMovementAction();
			PlayerAnimationAction();
			ApplyGravityAction();
		}

		private void FixedUpdate()
		{
			GroundCheck();
		}

		private void ApplyGravity()
		{
			if (_controller.isGrounded)
			{
				//ignore vertical velocity on ground
				_movementVelocity.y = -0.2f;
				
				//Resetting the hang Timer (Coyote Timer) to its original value when grounded
				jumpHangTimer = hangTime;
				isJumping = false;
				isFalling = false;
			}
			else
			{
				var deltaTime = Time.deltaTime;
				//Decrement the handTimer (Coyote Timer) when airborne
				jumpHangTimer -= deltaTime;
				jumpBufferTimer -= deltaTime;
				
				//Checking if the player is started to fall or if the player released the jump button
				isFalling = isJumping && (_movementVelocity.y <= 0 || !_inputController.jump);
				
				//Apply Gravity
				var prevYVelocity = _movementVelocity.y;
				var newYVelocity = (( (isFalling? fallMultiplier : 1 ) *  gravity * deltaTime) + _movementVelocity.y);
				_movementVelocity.y = (prevYVelocity + newYVelocity) * 0.5f;
			}
		}

		private void GroundCheck()
		{
			var transform1 = transform;
			_groundCheckRay.origin = transform1.position + groundCheckerOffset;
			_groundCheckRay.direction = -transform1.up;
			if (Physics.SphereCast(_groundCheckRay, _controller.radius, out _groundRaycastHit, 20f, groundLayerMask,queryTriggerInteraction: QueryTriggerInteraction.Ignore))
			{
				// Debug.Log($"Jumping: {playerAltitude}");
				playerAltitude = transform.position.y - _groundRaycastHit.point.y;
			}
		}

		private void UpdateInput()
		{
			//Read the user input
			_moveInput = _inputController.Move;

			//Update the animation direction
			_animatorDirection = _moveInput;
		
			//Get Time.deltaTime
			var deltaTime = Time.deltaTime;

			//Update the movement direction
			var cameraForward = _mainCameraTransform.forward;
			var verticalVelocity = _movementVelocity.y;
			
			_moveDirection = Vector3.right * _moveInput.x;

			_moveDirection.y = 0;
			_movementVelocity.y = verticalVelocity;
			
			_moveDirection.Normalize();

			//Rotate the Player direction to movement Direction

			_lookDirection = Vector3
				.Lerp(transform.forward, _moveInput.x * Vector3.right, deltaTime * rotationSpeed * 10)
				.ProjectOntoPlane(Vector3.up)
				.normalized;
			
			if(_lookDirection != Vector3.zero)
				transform.rotation = Quaternion.LookRotation(_lookDirection, Vector3.up);
			
			HandleJump();
			
			//Apply the movement
			verticalVelocity = _movementVelocity.y;
			_movementVelocity.y = 0;
			_movementVelocity += _moveDirection * (speed * Time.deltaTime);

			//Applying Drag
			_movementVelocity.y = 0;
			_movementVelocity -= _movementVelocity.normalized *
			                     ((1 - (_moveDirection.magnitude * dragInputFactor)) *
			                      (_controller.isGrounded ? groundDrag : airDrag) * Time.deltaTime);

			//Clamping Horizontal Speed to MaxSpeed
			var currentMaxSpeed = _inputController.walk ? maxSpeed * sneakModifier : maxSpeed;
			_movementVelocity.x = Mathf.Clamp(_movementVelocity.x,-currentMaxSpeed, currentMaxSpeed);
			_movementVelocity.x = Mathf.Abs(_moveInput.x) == 0 && Mathf.Abs(_movementVelocity.x) < minSpeedThreshold ? 0 : _movementVelocity.x;
			
			_movementVelocity.y = verticalVelocity;
			_collisionFlags = _controller.Move(_movementVelocity * Time.deltaTime);
		}

		private void HandleJump()
		{
			//Update Jumping Flags
			if (_lastJumpFlag != _inputController.jump)
			{
				jumpDownThisFrame = _inputController.jump;
				jumpUpThisFrame = !_inputController.jump;
				// Debug.Log($"Down: {jumpDownThisFrame} ---- Up: {jumpUpThisFrame}");
			}
			else
			{
				jumpDownThisFrame = false;
				jumpUpThisFrame = false;
			}

			_lastJumpFlag = _inputController.jump;
			
			//Update Jump Buffer
			if (jumpDownThisFrame)
			{
				jumpBufferTimer = jumpBuffer;
			}

			if ((_controller.isGrounded && jumpBufferTimer > 0 ) || // Jump Buffer 
			    (jumpHangTimer > 0 && jumpDownThisFrame) && // Coyote Time
			    !isJumping && // Not Jumping 
			    (jumpCoolDownTimer <= 0 || bounceJumpsCounter > 0)) // Bounce jumps
			{
				// Debug.Log("Jump");
				isJumping = true;
				
				//DEBUGGING
				if(!_controller.isGrounded)
					Debug.Log($"Coyote JUMP!");
				
				if(!jumpDownThisFrame && jumpBufferTimer > 0)
					Debug.Log($"Buffered JUMP!");
					
				//Set the hang Timer (Coyote Timer) and Jump Buffer Timer to zero to prevent the player from double jumping
				jumpHangTimer = 0;
				jumpBufferTimer = 0;
					
				
				
				//Check if we jumped before the cool down was finished
				if (jumpCoolDownTimer > 0)
				{
					//Then decrement the number of bounce jumps we have
					bounceJumpsCounter--;
				}
				
				jumpCoolDownTimer = jumpCoolDown;
				_movementVelocity += Vector3.up * (initialJumpingVelocity * 0.5f);
			}
			else
			{
				//Updating the cooldown timer
				jumpCoolDownTimer = jumpCoolDownTimer > 0 ? jumpCoolDownTimer - Time.deltaTime: jumpCoolDownTimer;
				
				if (bounceJumpsCounter < maxBounceJumpCount)
				{
					//Reset Jump Counter
					bounceJumpsCounter = jumpCoolDownTimer <= 0 ? maxBounceJumpCount : bounceJumpsCounter;
				}
			}
		}

		private void UpdateAnimator()	
		{
			var deltaTime = Time.deltaTime;
			
			_playerAnimator.SetFloat(_animatorY, Mathf.Abs(_movementVelocity.x), 0.2f, deltaTime);

			_playerAnimator.SetFloat(_animatorVerticalVelocity, _movementVelocity.y, 0.2f, deltaTime);
			_playerAnimator.SetFloat(_animatorPlayerAltitude, playerAltitude);
			_playerAnimator.SetBool(_animatorIsRunning, _inputController.walk);
			_playerAnimator.SetBool(_animatorIsJumping, isJumping);
		}

		private void OnFirstPortalEnter()
		{
			// Debug.Log("First Enter");
			PlayerMovementAction = () => { };
			PlayerAnimationAction = () => { };
			ApplyGravityAction = () => { };
			// _controller.enabled = false;
		}

		private void OnSecondPortalEnter()
		{
			// Debug.Log("First Exit");
			// _controller.enabled = true;
			PlayerMovementAction = UpdateInput;
			PlayerAnimationAction = UpdateAnimator;
			ApplyGravityAction = ApplyGravity;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawRay(transform.position, _movementVelocity);
			Gizmos.DrawRay(transform.position, _movementVelocity);
		}
	}
}