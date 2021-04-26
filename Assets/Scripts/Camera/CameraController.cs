using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : BaseRaycaster,
	IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
	// Serialized
	public GameUIController UIController;
	public GameController GameController;
	public RectTransform UIRect;
	public Camera Camera;
	public int FogOfWarThreshold = 6;
	public float PanSpeed = 20f;
	public float ZoomMin = 5;
	public float ZoomMax = 10;

	// runtime
	private Vector3 posDelta;
	private bool isDragging;

	// edges
	private float rightEdge;
	private float leftEdge;
	private float topEdge;
	private float bottomEdge;
	private float topToBottom => topEdge - bottomClamp;
	private float rightToLeft => rightEdge - leftEdge;
	private float bottomClamp
	{
		get
		{
			var fogOfWarLimit = GameController.ActiveGame.ReachedDepth + FogOfWarThreshold;
			return Mathf.Max(this.bottomEdge, -fogOfWarLimit);
		}
	}

	private float leftSideScreenSizeRelative => Screen.width / (float)Screen.height;
	// chop off space for ui
	private float rightSideScreenSizeRelative => (UIRect.rect.width / UIRect.rect.height) * (UIRect.rect.width / UIRect.rect.height) / leftSideScreenSizeRelative;
	private float totalAspectRatio => (rightSideScreenSizeRelative + leftSideScreenSizeRelative) / 2;

	public override Camera eventCamera => Camera;

	public void Initialize(LevelData levelData)
	{
		transform.position = levelData.TerraformerTile.TileCoordToPosition3D();
		this.leftEdge = 0;
		this.rightEdge = levelData.Width;
		this.topEdge = 0;
		this.bottomEdge = -levelData.Height;
	}

	public void CameraUpdate()
	{
		Zoom();
		Movement();
		DragMomentum();
	}

	private void Movement()
	{
		// Local variable to hold the camera target's position during each frame
		var t = transform;
		var pos = t.position;

		// Local variable to reference the direction the camera is facing (Which is driven by the Camera target's rotation)
		var up = t.up;

		// Ensure the camera target doesn't move up and down
		up.z = 0;

		// Normalize the X, Y & Z properties of the forward vector to ensure they are between 0 & 1
		up.Normalize();

		// Local variable to reference the direction the camera is facing + 90 clockwise degrees (Which is driven by the Camera target's rotation)
		var right = transform.right;

		// Ensure the camera target doesn't move up and down
		right.z = 0;

		// Normalize the X, Y & Z properties of the right vector to ensure they are between 0 & 1
		right.Normalize();

		// Move the camera (camera_target) Forward relative to current rotation if "W" is pressed or if the mouse moves within the borderWidth distance from the top edge of the screen

		pos.x += Input.GetAxis("Horizontal") * PanSpeed * Time.deltaTime;
		pos.y += Input.GetAxis("Vertical") * PanSpeed * Time.deltaTime;

		ClampXY(ref pos);

		// Setting the camera target's position to the modified pos variable
		transform.position = pos;
	}

	private void Zoom()
	{
		// When we scroll our mouse wheel up, zoom in if the camera is not within the minimum distance (set by our zoomMin variable)
		var currentZoom = Camera.orthographicSize;
		var scroll = Input.GetAxis("Mouse ScrollWheel");
		currentZoom -= scroll * Time.deltaTime;

		if (currentZoom < ZoomMin) currentZoom = ZoomMin;
		if (currentZoom > ZoomMax) currentZoom = ZoomMax;

		// clamp to screen bounds
		var cameraSizeVertical = currentZoom * 2;
		var cameraSizeHorizontal = currentZoom * leftSideScreenSizeRelative * 2;

		if (cameraSizeVertical > topToBottom) { currentZoom = topToBottom / 2; }
		if (cameraSizeHorizontal > rightToLeft) { currentZoom = rightToLeft / leftSideScreenSizeRelative / 2; }

		Camera.orthographicSize = currentZoom;
	}

	private void DragMomentum()
	{
		if (isDragging)
		{
			return;
		}
		if (Mathf.Approximately(posDelta.magnitude, 0f))
		{
			posDelta = Vector3.zero;
			return;
		}

		var pos = transform.position;
		pos -= posDelta;
		posDelta = Vector3.Lerp(posDelta, Vector3.zero, 0.1f);
		ClampXY(ref pos);
		transform.position = pos;
	}

	private void ClampXY(ref Vector3 pos)
	{
		var cameraSizeVertical = Camera.orthographicSize;
		var cameraSizeHorizontal = cameraSizeVertical * totalAspectRatio;

		if (rightEdge - leftEdge > cameraSizeHorizontal * 2)
		{
			pos.x = Mathf.Clamp
			(
				pos.x,
				this.leftEdge + cameraSizeVertical * leftSideScreenSizeRelative,
				this.rightEdge - cameraSizeVertical * rightSideScreenSizeRelative
			);
		}
		else
		{
			pos.x = (rightEdge + leftEdge) / 2f;
		}

		if (topEdge - bottomClamp > cameraSizeVertical * 2)
		{
			pos.y = Mathf.Clamp
			(
				pos.y,
				bottomClamp + cameraSizeVertical,
				this.topEdge - cameraSizeVertical
			);
		}
		else
		{
			pos.y = (bottomClamp + topEdge) / 2f;
		}
	}

	// Unity UI interaction
	// ---------------------------

	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		resultAppendList.Add(new RaycastResult()
		{
			gameObject = gameObject,
			module = this
		});
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		isDragging = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		var pos = transform.position;
		var mouseDelta = eventData.delta;
		var dragSpeed = Camera.orthographicSize * 2 / Screen.height;

		if (!Mathf.Approximately(mouseDelta.magnitude, 0f))
		{
			// drag
			posDelta = mouseDelta * dragSpeed;
			pos -= posDelta;
		}
		else
		{
			// momentum (holding)
			posDelta = Vector3.Lerp(posDelta, Vector3.zero, 0.2f);
		}

		ClampXY(ref pos);
		transform.position = pos;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		isDragging = false;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		UIController.SelectedAction?.OnPointerDown(eventData);
	}

	// ---------------------------
}