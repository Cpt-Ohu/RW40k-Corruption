using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core
{
    public static class GenDrawExtension
    {
		public static void DrawLineBetween(Vector3 A, Vector3 B, Mesh mesh, Material mat, float width)
		{
			if (!(Mathf.Abs(A.x - B.x) < 0.01f) || !(Mathf.Abs(A.z - B.z) < 0.01f))
			{
				Vector3 pos = (A + B) / 2f;
				if (!(A == B))
				{
					A.y = B.y;
					float z = (A - B).MagnitudeHorizontal();
					Quaternion q = Quaternion.LookRotation(A - B);
					Vector3 s = new Vector3(width, 1f, z);
					Matrix4x4 matrix = default(Matrix4x4);
					matrix.SetTRS(pos, q, s);
					Graphics.DrawMesh(mesh, matrix, mat, 0);
				}
			}
		}
	}
}
