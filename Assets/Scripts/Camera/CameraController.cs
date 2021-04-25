using UnityEngine;

public class CameraController : MonoBehaviour
{
	// Serialized
	public Camera Camera;
	public float PanSpeed = 20f;
	public float ZoomMin = 5;
	public float ZoomMax = 10;

	// runtime
	private Vector3 lastMousePos;
	private Vector3 posDelta;

	// edges
	private float rightEdge;
	private float leftEdge;
	private float topEdge;
	private float bottomEdge;
	private float topToBottom => topEdge - bottomEdge;
	private float rightToLeft => rightEdge - leftEdge;

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
		Drag();
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
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) pos += up * (PanSpeed * Time.deltaTime);

		// Move the camera (camera_target) Backward relative to current rotation if "S" is pressed or if the mouse moves within the borderWidth distance from the bottom edge of the screen
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) pos -= up * (PanSpeed * Time.deltaTime);

		// Move the camera (camera_target) Right relative to current rotation if "D" is pressed or if the mouse moves within the borderWidth distance from the right edge of the screen
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) pos += right * (PanSpeed * Time.deltaTime);

		// Move the camera (camera_target) Left relative to current rotation if "A" is pressed or if the mouse moves within the borderWidth distance from the left edge of the screen
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) pos -= right * (PanSpeed * Time.deltaTime);

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
		var aspectRatio = Screen.width / (float)Screen.height;

		var cameraSizeVertical = currentZoom * 2;
		var cameraSizeHorizontal = currentZoom * aspectRatio * 2;

		if (cameraSizeVertical > topToBottom) { currentZoom = topToBottom / 2; }
		if (cameraSizeHorizontal > rightToLeft) { currentZoom = rightToLeft / aspectRatio / 2; }

		Camera.orthographicSize = currentZoom;
	}

	private void Drag()
	{
		var pos = transform.position;
		var newMousePos = Input.mousePosition;
		var mouseDelta = newMousePos - lastMousePos;

		var dragSpeed = Camera.orthographicSize * 2 / Screen.height;

		if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
		{
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
		}
		else if (!Mathf.Approximately(posDelta.magnitude, 0f))
		{
			// momentum (released)
			posDelta = Vector3.Lerp(posDelta, Vector3.zero, 0.2f);
			pos -= posDelta;
		}

		ClampXY(ref pos);

		transform.position = pos;
		lastMousePos = newMousePos;
	}

	private void ClampXY(ref Vector3 pos)
	{
		var cameraSizeVertical = Camera.orthographicSize;
		var cameraSizeHorizontal = cameraSizeVertical * (Screen.width / (float)Screen.height);

		if (rightEdge - leftEdge > cameraSizeHorizontal * 2)
		{
			pos.x = Mathf.Clamp
			(
				pos.x,
				this.leftEdge + cameraSizeHorizontal,
				this.rightEdge - cameraSizeHorizontal
			);
		}
		else
		{
			pos.x = (rightEdge + leftEdge) / 2f;
		}

		if (topEdge - bottomEdge > cameraSizeVertical * 2)
		{
			pos.y = Mathf.Clamp
			(
				pos.y,
				this.bottomEdge + cameraSizeVertical,
				this.topEdge - cameraSizeVertical
			);
		}
		else
		{
			pos.y = (bottomEdge + topEdge) / 2f;
		}
	}
}