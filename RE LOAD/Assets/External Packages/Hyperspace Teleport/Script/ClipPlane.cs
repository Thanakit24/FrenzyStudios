using UnityEngine;

namespace HyperspaceTeleport
{
	public class ClipPlane : MonoBehaviour
	{
		public GameObject m_ClipPlane;
		public enum EDir { ED_X, ED_Y, ED_Z };
		public EDir m_Dir = EDir.ED_Z;
		public enum EClipFunc { ECF_FORWARD, ECF_BACKWARD };
		public EClipFunc m_ClipFunc = EClipFunc.ECF_FORWARD;
		public bool m_UseNoise = false;
		public Color m_HaloColor = Color.yellow;
		[Range(-16f, 16f)] public float m_Clip = 1f;
		[Range(0.1f, 1f)] public float m_Halo = 0.5f;
		[Range(0.1f, 1f)] public float m_PlaneAlpha = 0.2f;
		[Range(0.8f, 3f)] public float m_Bloom = 1.5f;

		void Update()
		{
			if (m_ClipPlane)
			{
				if (m_Dir == EDir.ED_Z)
					m_Clip = m_ClipPlane.transform.position.z;
				if (m_Dir == EDir.ED_Y)
					m_Clip = m_ClipPlane.transform.position.y;
				if (m_Dir == EDir.ED_X)
					m_Clip = m_ClipPlane.transform.position.x;
				MeshRenderer mrPlane = m_ClipPlane.GetComponent<MeshRenderer>();
				mrPlane.material.SetFloat("_Alpha", m_PlaneAlpha);
			}
			MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
			if (m_Dir == EDir.ED_X)
			{
				mr.material.EnableKeyword("HT_DIR_X");
				mr.material.DisableKeyword("HT_DIR_Y");
				mr.material.DisableKeyword("HT_DIR_Z");
			}
			else if (m_Dir == EDir.ED_Y)
			{
				mr.material.DisableKeyword("HT_DIR_X");
				mr.material.EnableKeyword("HT_DIR_Y");
				mr.material.DisableKeyword("HT_DIR_Z");
			}
			else if (m_Dir == EDir.ED_Z)
			{
				mr.material.DisableKeyword("HT_DIR_X");
				mr.material.DisableKeyword("HT_DIR_Y");
				mr.material.EnableKeyword("HT_DIR_Z");
			}
			if (m_ClipFunc == EClipFunc.ECF_FORWARD)
			{
				mr.material.EnableKeyword("HT_FORWARD");
				mr.material.DisableKeyword("HT_BACKWARD");
			}
			else if (m_ClipFunc == EClipFunc.ECF_BACKWARD)
			{
				mr.material.DisableKeyword("HT_FORWARD");
				mr.material.EnableKeyword("HT_BACKWARD");
			}
			if (m_UseNoise)
				mr.material.EnableKeyword("HT_NOISE");
			else
				mr.material.DisableKeyword("HT_NOISE");
			mr.material.SetColor("_HaloColor", m_HaloColor);
			mr.material.SetFloat("_Clip", m_Clip);
			mr.material.SetFloat("_Halo", m_Halo);
			mr.material.SetFloat("_Bloom", m_Bloom);
		}
	}
}
