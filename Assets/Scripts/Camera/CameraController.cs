using UnityEngine;

public class CameraController : MonoBehaviour
{
	// Serialized
	public Camera Camera;
	public float PanSpeed = 20f;
	public float ZoomMin = 5;
	public float ZoomMax = 10;
	public float ZoomSpeed = 2f;

	// runtime
	private Vector3 lastMousePos;
	private Vector3 posDelta;
	private float targetZoom;
	private Vector2 panLimit;

	public void Initialize(LevelData levelData)
	{
		panLimit = new Vector2(levelData.Width, levelData.Height);
		transform.position = levelData.TerraformerTile.TileCoordToPosition();
		targetZoom = Camera.orthographicSize;
	}

	public void CameraUpdate()
	{
		Movement();
		Drag();
		Zoom();
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
		var scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll != 0.0f)
		{
			targetZoom -= scroll * ZoomSpeed;
		}

		// smoooooooth scroll like jazz
		targetZoom = Mathf.Clamp(targetZoom, ZoomMin, ZoomMax);
		Camera.orthographicSize = Mathf.Lerp(Camera.orthographicSize, targetZoom, 0.2f);
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
		pos.y = Mathf.Clamp(pos.y, -panLimit.y + 0.5f, 0.5f);
		pos.x = Mathf.Clamp(pos.x, 0 - 0.5f, panLimit.x - 0.5f);
	}
}