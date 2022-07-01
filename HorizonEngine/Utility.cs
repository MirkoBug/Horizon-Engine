using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Mathematics;

namespace HorizonEngine
{
	public static class Utility
	{
		public static Matrix4 CreateTransform(Vector3 Position, Vector3 Rotation, Vector3 Scale)
		{
			return Matrix4.CreateTranslation(Position) * (Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X)) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y)) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z))) * Matrix4.CreateScale(Scale);
		}
	}
}
