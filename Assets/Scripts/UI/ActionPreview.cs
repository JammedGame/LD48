using UnityEngine;

public class ActionPreview : MonoBehaviour
{
    public class MyData
    {
        public Vector3 Position;
        public Texture Texture;
        public bool IsValid;

        public MyData(Vector3 position, Texture texture, bool isValid)
        {
            Position = position;
            Texture = texture;
            IsValid = isValid;
        }
    }

    public MeshRenderer meshRenderer;

    public void Show(MyData data)
    {
        transform.position = data.Position;
        meshRenderer.material.mainTexture = data.Texture;
        meshRenderer.material.color = data.IsValid ? Color.green : Color.red;
        gameObject.SetActive(true);
    }

	internal void Hide()
	{
        gameObject.SetActive(false);
	}
}