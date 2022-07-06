using System;
using System.IO;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Input;

namespace HorizonEngine
{
	public class Game
	{
		public static GameWindow Window;
		public static ShaderProgram Program = new ShaderProgram() { ID = 0 };
		public static double FrameSecond = 0;
		public static int Frames = 0;

		public static int VAO;
		public static int Vertices;



		// Render settings
		public static bool UsePerspective = true;
		public static float FOV = 70f;
		public static float FarClipPlaneDistance = 10000f;
		public static float OrthographicScale = 5f;
		public static Vector3 CameraPosition = new Vector3(0, 0, 0);



		// Vertex data (Temporary, for testing)
		public static float[] Verts =
		{
				// Front face
				-0.5f, -0.5f,  0.5f,
				-0.5f,  0.5f,  0.5f,
				 0.5f,  0.5f,  0.5f,
				-0.5f, -0.5f,  0.5f,
				 0.5f,  0.5f,  0.5f,
				 0.5f, -0.5f,  0.5f,
				// Right face
				 0.5f, -0.5f,  0.5f,
				 0.5f,  0.5f,  0.5f,
				 0.5f,  0.5f, -0.5f,
				 0.5f, -0.5f,  0.5f,
				 0.5f,  0.5f, -0.5f,
				 0.5f, -0.5f, -0.5f,
				// Left face
				-0.5f, -0.5f,  0.5f,
				-0.5f,  0.5f,  0.5f,
				-0.5f,  0.5f, -0.5f,
				-0.5f, -0.5f,  0.5f,
				-0.5f,  0.5f, -0.5f,
				-0.5f, -0.5f, -0.5f,
				// Back face
				-0.5f, -0.5f, -0.5f,
				-0.5f,  0.5f, -0.5f,
				 0.5f,  0.5f, -0.5f,
				-0.5f, -0.5f, -0.5f,
				 0.5f,  0.5f, -0.5f,
				 0.5f, -0.5f, -0.5f,
				 // Top face
				-0.5f,  0.5f, -0.5f,
				-0.5f,  0.5f,  0.5f,
				 0.5f,  0.5f,  0.5f,
				-0.5f,  0.5f, -0.5f,
				 0.5f,  0.5f,  0.5f,
				 0.5f,  0.5f, -0.5f,
				 // Bottom face
				-0.5f, -0.5f, -0.5f,
				-0.5f, -0.5f,  0.5f,
				 0.5f, -0.5f,  0.5f,
				-0.5f, -0.5f, -0.5f,
				 0.5f, -0.5f,  0.5f,
				 0.5f, -0.5f, -0.5f,
		};



		// Rendering data
		public static Queue<Mesh> MeshQueue = new Queue<Mesh>();



		// GLSL Uniforms
		public static int ModelUniform;
		public static int ProjectionUniform;
		public static int ViewUniform;


		public static void Main(string[] args)
		{
			// Print title
			ConsoleLogger.PrintMessage(ConsoleLogger.MessageType.Highlight, "Horizon Engine - No Limits");

			// Use default settings
			GameWindowSettings WindowSettings = GameWindowSettings.Default;
			WindowSettings.IsMultiThreaded = false;	// I'm honestly not sure about the performance difference it makes, need to study it better
			WindowSettings.RenderFrequency = 0;
			WindowSettings.UpdateFrequency = 100;
				
			NativeWindowSettings NativeSettings = NativeWindowSettings.Default;
			NativeSettings.APIVersion = Version.Parse("4.1");
			NativeSettings.Size = new Vector2i(1280, 720);
			NativeSettings.Title = "Horizon Engine - No Limits";
			// NativeSettings.Icon; - CHANGE LATER

			// Create a window with above settings
			Window = new GameWindow(WindowSettings, NativeSettings);

			// Disable VSync
			Window.VSync = VSyncMode.Off;


			// Bind functions to window events
			Window.Load	       += OnWindowLoad;
			Window.UpdateFrame += OnUpdateFrame;
			Window.RenderFrame += OnRenderFrame;
			Window.Resize      += OnWindowResize;
			Window.Unload      += OnWindowUnload;



			// Run the window
			Window.Run();
		}
		
		private static void OnWindowLoad()
		{
			// Print debug info
			ConsoleLogger.PrintMessage(ConsoleLogger.MessageType.Info, "\n" + "Renderer: " + GL.GetString(StringName.Renderer) + "\n" + "Vendor:   " + GL.GetString(StringName.Vendor));

			ConsoleLogger.PrintMessage(ConsoleLogger.MessageType.Info, "Window Loaded");

			if(UsePerspective)
				ConsoleLogger.PrintMessage(ConsoleLogger.MessageType.Info, "Using Perspective Projection With " + FOV + " FOV");
			else
				ConsoleLogger.PrintMessage(ConsoleLogger.MessageType.Info, "Using Orthographic Projection With Scale Of " + OrthographicScale);

			// Load base shader program
			Program = LoadShaderProgram("../../../../Shaders/Base_Vertex.glsl", "../../../../Shaders/Base_Fragment.glsl");

			GL.Viewport(0, 0, Window.Size.X, Window.Size.Y);

			// Set uniform locations
			ModelUniform = GL.GetUniformLocation(Program.ID, "ModelMatrix");
			ProjectionUniform = GL.GetUniformLocation(Program.ID, "ProjectionMatrix");
			ViewUniform = GL.GetUniformLocation(Program.ID, "ViewMatrix");


			// VAO Generation
			VAO = GL.GenVertexArray();
			Vertices = GL.GenBuffer();
			GL.BindVertexArray(VAO);
			GL.BindBuffer(BufferTarget.ArrayBuffer, Vertices);



			// Testing for Dynamic Mesh Rendering
			for(int x = -100; x < 100; x+=2)
			{
				for (int y = -100; y < 100; y+=2)
				{
					MeshQueue.Enqueue(new Mesh(new Vector3(x, y, 30), new Vector3(0, 0, 0), new Vector3(1, 1, 1), Verts));
				}
			}

			ConsoleLogger.PrintMessage(ConsoleLogger.MessageType.Alert, "Meshes Rendering: " + MeshQueue.Count);
			ConsoleLogger.PrintMessage(ConsoleLogger.MessageType.Alert, "Total Vertices: " + MeshQueue.Count * Verts.Length);
		}
		
		private static void OnWindowUnload()
		{
			// Unbindings
			GL.BindVertexArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.DeleteBuffer(Vertices);
			GL.DeleteVertexArray(VAO);
		}

		private static void OnWindowResize(ResizeEventArgs Args)
		{
			// Make sure OpenGL updates the viewport size after a window resize
			GL.Viewport(0, 0, Window.Size.X, Window.Size.Y);
		}

		private static void OnUpdateFrame(FrameEventArgs Args)
		{
			
		}

		private static void OnRenderFrame(FrameEventArgs Args)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);

			GL.UseProgram(Program.ID);



			// Generate Projection Matrix
			Matrix4 ProjectionMatrix;
			if (UsePerspective)
				ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FOV * ((float)Math.PI / 180f), (float)Window.Size.X / (float)Window.Size.Y, 0.000001f, FarClipPlaneDistance);
			else
				ProjectionMatrix = Matrix4.CreateOrthographic(OrthographicScale, OrthographicScale, 0f, FarClipPlaneDistance);



			// View and Model matrix generation
			Matrix4 ViewMatrix = Matrix4.LookAt(CameraPosition, CameraPosition + Vector3.UnitZ, Vector3.UnitY).Inverted();


			// Make a Cube mesh (using the local Verts) and read its transform
			//Cube = new Mesh(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(1, 1, 1), Verts);
			//Matrix4 ModelMatrix = Cube.Transform;



			// Pass the projection and view matrices to shaders
			GL.UniformMatrix4(ProjectionUniform, false, ref ProjectionMatrix);
			GL.UniformMatrix4(ViewUniform, false, ref ViewMatrix);



			// Bind VAO and Vertex buffer and render
			GL.BindVertexArray(VAO);
			GL.BindBuffer(BufferTarget.ArrayBuffer, Vertices);
			//GL.BufferData(BufferTarget.ArrayBuffer, Verts.Length * sizeof(float), Verts, BufferUsageHint.DynamicDraw);
			GL.EnableVertexAttribArray(0);                                                                                  // Let shaders access this vertex array at layout position 0
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);                                       // Specify the number of values it can take and the data type



			// Render every mesh in the MeshQueue
			foreach (Mesh _Mesh in MeshQueue)
			{
				GL.UniformMatrix4(ModelUniform, false, ref _Mesh.Transform);

				GL.BufferData(BufferTarget.ArrayBuffer, _Mesh.Vertices.Length * sizeof(float), _Mesh.Vertices, BufferUsageHint.DynamicDraw);

				GL.DrawArrays(PrimitiveType.Triangles, 0, _Mesh.Vertices.Length / 3);

				//GL.DrawArraysIndirect(PrimitiveType.Triangles, Verts); // Might look into later
			}	



			Window.SwapBuffers();

			// Display average FPS every 5 seconds
			FrameSecond += Window.RenderTime;
			Frames++;
			if(FrameSecond >= 5.0)
			{
				FrameSecond = 0;
				ConsoleLogger.PrintMessage(ConsoleLogger.MessageType.Info, "FPS: " + Frames / 5);
				Frames = 0;
			}
		}



		// Shader-related
		#region Shader Functions

		public static Shader LoadShader(string ShaderLocation, ShaderType Type)
		{
			// Creates a shader object and returns its identifier
			int ShaderID = GL.CreateShader(Type);

			// Reads the shader's code from a file
			GL.ShaderSource(ShaderID, File.ReadAllText(ShaderLocation));

			GL.CompileShader(ShaderID);

			string InfoLog = GL.GetShaderInfoLog(ShaderID);

			if(!string.IsNullOrEmpty(InfoLog))
			{
				ConsoleLogger.PrintMessage(ConsoleLogger.MessageType.Critical, "Shader " + ShaderID + " error");

				throw new Exception(InfoLog);
			}

			ConsoleLogger.PrintMessage(ConsoleLogger.MessageType.Info, "Shader " + ShaderID + " loaded succesfully");

			// Return the shader struct with the ID
			return new Shader { ID = ShaderID };
		}

		private static ShaderProgram LoadShaderProgram(string VShaderLocation, string FShaderLocation)
		{
			// Create shader program object
			int ShaderProgramID = GL.CreateProgram();

			// Create vertex and fragment shaders
			Shader VShader = LoadShader(VShaderLocation, ShaderType.VertexShader);
			Shader FShader = LoadShader(FShaderLocation, ShaderType.FragmentShader);

			// Attach the two shaders to the shader program
			GL.AttachShader(ShaderProgramID, VShader.ID);
			GL.AttachShader(ShaderProgramID, FShader.ID);

			// Copies the shaders into memory and links them
			GL.LinkProgram(ShaderProgramID);

			// Free up memory
			GL.DetachShader(ShaderProgramID, VShader.ID);
			GL.DetachShader(ShaderProgramID, FShader.ID);
			GL.DeleteShader(VShader.ID);
			GL.DeleteShader(FShader.ID);

			string InfoLog = GL.GetProgramInfoLog(ShaderProgramID);

			if (!string.IsNullOrEmpty(InfoLog))
			{
				ConsoleLogger.PrintMessage(ConsoleLogger.MessageType.Critical, "Shader Program " + ShaderProgramID + " error");

				throw new Exception(InfoLog);
			}

			ConsoleLogger.PrintMessage(ConsoleLogger.MessageType.Info, "Shader Program " + ShaderProgramID + " loaded succesfully");

			return new ShaderProgram { ID = ShaderProgramID };
		}

		public struct Shader
		{
			public int ID;
		}

		public struct ShaderProgram
		{
			public int ID;
		}

		#endregion ShaderFunctions
	}
}
