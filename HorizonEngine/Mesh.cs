using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Mathematics;

namespace HorizonEngine
{
	public class Mesh
	{
		// Accessible properties of Mesh class
		public Vector3 Position;
		public Vector3 Rotation;
		public Vector3 Scale;
		public Matrix4 Transform;
		public float[] Vertices;

		// Constructor
		public Mesh(Vector3 _Position, Vector3 _Rotation, Vector3 _Scale, float[] _Vertices)
		{
			Position = _Position;
			Rotation = _Rotation;
			Scale = _Scale;

			// Generate the transform from the new position, rotation and scale
			Transform = Utility.CreateTransform(Position, Rotation, Scale);

			Vertices = _Vertices;
		}
	}
}
