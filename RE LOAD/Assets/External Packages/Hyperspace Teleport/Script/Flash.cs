using UnityEngine;

namespace HyperspaceTeleport
{
	public class Flash : MonoBehaviour
	{
		public enum EDir { ED_PosX, ED_NegX, ED_Z };
		public EDir m_Dir = EDir.ED_Z;
		[ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)] public Color m_Emission = Color.yellow;
		[Range(-5f, 5f)] public float m_Dissolve = -2.45f;

		void Update()
		{
			MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
			Material mat = mr.material;
			if (m_Dir == EDir.ED_PosX)
			{
				mat.SetVector("_Direction", new Vector4(1, 0, 0, 1));
			}
			else if (m_Dir == EDir.ED_NegX)
			{
				mat.SetVector("_Direction", new Vector4(-1, 0, 0, 1));
			}
			else if (m_Dir == EDir.ED_Z)
			{
				mat.SetVector("_Direction", new Vector4(0, 0, 1, 1));
			}
			mat.SetColor("_Emission", m_Emission);
			mat.SetFloat("_Dissolve", m_Dissolve);
		}
	}
}
