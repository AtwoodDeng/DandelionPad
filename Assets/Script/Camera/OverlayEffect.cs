using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
public class OverlayEffect : MonoBehaviour {

	[SerializeField] Texture overlayTexture;
	[SerializeField] float resize = 0.05f;
	[SerializeField] float darkness = 1f;
	[SerializeField] float changeRate = 0.5f;
	[SerializeField] Vector2 offset;


	// public Shader shader;
	public Material material;
	public Shader CameraOverlapShader;

	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if ( material == null || material.shader != CameraOverlapShader )
		{
			material = new Material(CameraOverlapShader);
		}

		material.SetTexture("_OverTex", overlayTexture);
		material.SetFloat("_Resize" , resize);
		material.SetFloat("_Darkness" , darkness);
		material.SetFloat("_ChangeRate" , changeRate);
		material.SetFloat("_OffsetX" , offset.x);
		material.SetFloat("_OffsetY" , offset.y);
		// destination = RenderTexture.GetTemporary(source.width, source.height);
		Graphics.Blit(source, destination , material);
	}
}
