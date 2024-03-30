using System;
using System.Collections.Generic;

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
			// 각 점으로부터 벡터를 만들고 외적을 사용하여 삼각형의 노멀을 구함
			Vector3 u = vertex2 - vertex1;
			Vector3 v = vertex3 - vertex1;
			Vector3 w = point - vertex1;
			Vector3 n = Vector3.Cross(u, v);

			// 외적이 0이면 삼각형이 아님
			if(n == Vector3.zero)
				return false;

			// 각 점에서의 삼각형의 면과의 거리를 구함
			float gamma = Vector3.Dot(Vector3.Cross(u, w), n) / n.sqrMagnitude;
			float beta = Vector3.Dot(Vector3.Cross(w, v), n) / n.sqrMagnitude;
			float alpha = 1.0f - gamma - beta;

			// 모든 거리가 양수이면 점은 삼각형 내부에 있음
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

		public static LinkRayTriangle.LinkType CheckSpaceBetweenTriangle(Triangle A, Triangle B, float minimumDistance, Func<Vector3, Vector3, bool> raycast)
		{
			if(raycast is null) return LinkRayTriangle.LinkType.NothingLink;

			int count = 0;
			count += (Vector3.Distance(A.vertex1, B.vertex1) <= minimumDistance || !raycast(A.vertex1, B.vertex1)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex1, B.vertex2) <= minimumDistance || !raycast(A.vertex1, B.vertex2)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex1, B.vertex3) <= minimumDistance || !raycast(A.vertex1, B.vertex3)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex2, B.vertex1) <= minimumDistance || !raycast(A.vertex2, B.vertex1)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex2, B.vertex2) <= minimumDistance || !raycast(A.vertex2, B.vertex2)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex2, B.vertex3) <= minimumDistance || !raycast(A.vertex2, B.vertex3)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex3, B.vertex1) <= minimumDistance || !raycast(A.vertex3, B.vertex1)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex3, B.vertex2) <= minimumDistance || !raycast(A.vertex3, B.vertex2)) ? 1 : 0;
			count += (Vector3.Distance(A.vertex3, B.vertex3) <= minimumDistance || !raycast(A.vertex3, B.vertex3)) ? 1 : 0;

			if(count == 0) return LinkRayTriangle.LinkType.NothingLink;
			if(count < 9) return LinkRayTriangle.LinkType.PartialLink;

			List<Vector3> dividedPointsA = new List<Vector3>();
			for(int i = 0 ; i < 10 ; i++)
			{
				float t = (float)i / 10;
				Vector3 point = Vector3.Lerp(A.vertex1 , A.vertex2, t);
				dividedPointsA.Add(point);
				point = Vector3.Lerp(A.vertex2, A.vertex3, t);
				dividedPointsA.Add(point);
				point = Vector3.Lerp(A.vertex3, A.vertex1, t);
				dividedPointsA.Add(point);
			}
			List<Vector3> dividedPointsB = new List<Vector3>();
			for(int i = 0 ; i < 10 ; i++)
			{
				float t = (float)i / 10;
				Vector3 point = Vector3.Lerp(B.vertex1 , B.vertex2, t);
				dividedPointsB.Add(point);
				point = Vector3.Lerp(B.vertex2, B.vertex3, t);
				dividedPointsB.Add(point);
				point = Vector3.Lerp(B.vertex3, B.vertex1, t);
				dividedPointsB.Add(point);
			}

			int countA = dividedPointsA.Count;
			int countB = dividedPointsB.Count;
			for(int i = 0 ; i < countA ; i++)
			{
				Vector3 pointA = dividedPointsA[i];
				for(int ii = 0 ; ii < countB ; ii++)
				{
					Vector3 pointB = dividedPointsB[i];

					if(Vector3.Distance(pointA, pointB) <= minimumDistance || raycast(pointA, pointB))
					{
						return LinkRayTriangle.LinkType.PartialLink;
					}
				}
			}

			return LinkRayTriangle.LinkType.PerfectLink;
		}
	}
	public struct LinkRayTriangle
	{
		public enum LinkType
		{
			NothingLink,
			PartialLink,
			PerfectLink,
		}
		public LinkType linkType;
		public Triangle linkTriangleA;
		public Triangle linkTriangleB;

		public LinkRayTriangle(LinkType linkType, Triangle linkTriangleA, Triangle linkTriangleB)
		{
			this.linkType = linkType;
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
			Debug.Log($"LinkRayTriangle : {linkTriangleA.Index} == {linkTriangleB.Index} : Link ({linkType})");
		}
		public bool IsPerfectLink => linkType == LinkType.PerfectLink;
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
