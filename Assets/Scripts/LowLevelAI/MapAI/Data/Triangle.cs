using System;

using UnityEngine;

namespace BC.LowLevelAI
{
	public struct Triangle
	{
		public int Index;
		public Vector3 vertex1;
		public Vector3 vertex2;
		public Vector3 vertex3;

		public Triangle(int index, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
		{
			Index = index;
			this.vertex1=vertex1;
			this.vertex2=vertex2;
			this.vertex3=vertex3;
		}

		public Vector3 Center => (vertex1 + vertex2 + vertex3) / 3f;
		public Plane GetPlane => new Plane(vertex1, vertex2, vertex3);
		public bool IsPointInTriangle(Vector3 point)
		{
			// �� �����κ��� ���͸� ����� ������ ����Ͽ� �ﰢ���� ����� ����
			Vector3 u = vertex2 - vertex1;
			Vector3 v = vertex3 - vertex1;
			Vector3 w = point - vertex1;
			Vector3 n = Vector3.Cross(u, v);

			// ������ 0�̸� �ﰢ���� �ƴ�
			if(n == Vector3.zero)
				return false;

			// �� �������� �ﰢ���� ����� �Ÿ��� ����
			float gamma = Vector3.Dot(Vector3.Cross(u, w), n) / n.sqrMagnitude;
			float beta = Vector3.Dot(Vector3.Cross(w, v), n) / n.sqrMagnitude;
			float alpha = 1.0f - gamma - beta;

			// ��� �Ÿ��� ����̸� ���� �ﰢ�� ���ο� ����
			return alpha > 0 && beta > 0 && gamma > 0;
		}
		public void OnDrawGizmos(Vector3 offset)
		{
#if UNITY_EDITOR
			Gizmos.DrawLine(vertex1 + offset, vertex2 + offset);
			Gizmos.DrawLine(vertex2 + offset, vertex3 + offset);
			Gizmos.DrawLine(vertex3 + offset, vertex1 + offset);
#endif
		}
		public void Log()
		{
#if UNITY_EDITOR
			Debug.Log($"Triangle ({Index}) : v1({vertex1}) v2({vertex2}) v3({vertex3})");
#endif
		}

		public static int CheckSpaceBetweenTriangle(Triangle A, Triangle B, float minimumDistance, Func<Vector3, Vector3, bool> raycast)
		{
			if(raycast is null) return 0;

			int count = 0;
			count += (Vector3.Distance(A.vertex1, B.vertex1) <= minimumDistance || raycast(A.vertex1, B.vertex1)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex1, B.vertex2) <= minimumDistance || raycast(A.vertex1, B.vertex2)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex1, B.vertex3) <= minimumDistance || raycast(A.vertex1, B.vertex3)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex2, B.vertex1) <= minimumDistance || raycast(A.vertex2, B.vertex1)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex2, B.vertex2) <= minimumDistance || raycast(A.vertex2, B.vertex2)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex2, B.vertex3) <= minimumDistance || raycast(A.vertex2, B.vertex3)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex3, B.vertex1) <= minimumDistance || raycast(A.vertex3, B.vertex1)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex3, B.vertex2) <= minimumDistance || raycast(A.vertex3, B.vertex2)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex3, B.vertex3) <= minimumDistance || raycast(A.vertex3, B.vertex3)) ? 1 : 0;

			return count;
		}
	}
	public struct LinkRayTriangle
	{
		public int linkCount;
		public Triangle linkTriangleA;
		public Triangle linkTriangleB;

		public LinkRayTriangle(int linkCount, Triangle linkTriangleA, Triangle linkTriangleB)
		{
			this.linkCount = linkCount;
			this.linkTriangleA=linkTriangleA;
			this.linkTriangleB=linkTriangleB;
		}

		public bool IsThis(int A)
		{
			return (linkTriangleA.Index == A || linkTriangleB.Index == A);
		}

		public bool IsThis(int A, int B)
		{
			return A != B &&
				(linkTriangleA.Index == A || linkTriangleB.Index == A) &&
				(linkTriangleA.Index == B || linkTriangleB.Index == B);
		}
		public void Log()
		{
			Debug.Log($"LinkRayTriangle : {linkTriangleA.Index} == {linkTriangleB.Index} : Link ({linkCount})");
		}
		public bool IsPerfectLink => linkCount == 9;
		internal void OnDrawGizmos(Vector3 offset, float radius)
		{
			Vector3 a = linkTriangleA.Center + offset;
			Vector3 b = linkTriangleB.Center + offset;

			Gizmos.DrawSphere(a, radius);
			Gizmos.DrawSphere(b, radius);
			Gizmos.DrawLine(a, b);
		}

		public bool GetOther(Triangle triangle, out Triangle other)
		{
			if(linkTriangleA.Index == triangle.Index)
			{
				other = linkTriangleB;
				return true;
			}
			else if(linkTriangleB.Index == triangle.Index)
			{
				other = linkTriangleA;
				return true;
			}
			other = default;
			return false;
		}
	}
}
