using System;
using System.IO;
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


		// Render settings
		public static bool UsePerspective = false;
		public static float FarClipPlaneDistance = 10000f;
		public static float OrthographicScale = 5f;



		// Vertex data
		public static float[] Verts =
		{
				-0.5f, -0.5f, 0.0f,
				 0.5f, -0.5f, 0.0f,
				 0.0f,  0.5f, 0.0f
		};
		public static float[] VertColors =
		{
				 1.0f,  0.0f, 0.0f,
				 0.0f,  1.0f, 0.0f,
				 0.0f,  0.0f, 1.0f
		};



		// GLSL Uniforms
		public static int OrthographicProjectionUniform;
		public static int SaturationUniform;



		public static void Main(string[] args)
		{
			// Print title
			ConsoleLogger.PrintMessage(ConsoleLogger.MessageType.Highlight, "Horizon Engine - No Limits");

			// Use default settings
			GameWindowSettings WindowSettings = GameWindowSettings.Default;
			WindowSettings.IsMultiThreaded = false;
			WindowSettings.RenderFrequency = 100;
			WindowSettings.UpdateFrequency = 100;

			NativeWindowSettings NativeSettings = NativeWindowSettings.Default;
			NativeSettings.APIVersion = Version.Parse("4.1");
			NativeSettings.Size = new Vector2i(1280, 720);
			NativeSettings.Title = "Horizon Engine - No Limits";
			// NativeSettings.Icon; - CHANGE LATER

			// Create a window with above settings
			Window = new GameWindow(WindowSettings, NativeSettings);


			// Bind functions to window events
			Window.Load += OnWindowLoad;
			Window.UpdateFrame += OnUpdateFrame;
			Window.RenderFrame += OnRenderFrame;



			// Run the window
			Window.Run();
		}

		private static void OnWindowLoad()
		{
			ConsoleLogger.PrintMessage(ConsoleLogger.MessageType.Info, "Window Loaded");

			Program = LoadShaderProgram("../../../../Shaders/Base_Vertex.glsl", "../../../../Shaders/Base_Fragment.glsl");

			// Set uniform locations
			OrthographicProjectionUniform = GL.GetUniformLocation(Program.ID, "OrthographicProjection");
			SaturationUniform = GL.GetUniformLocation(Program.ID, "Saturation");

			// GL.GetString for renderer and vendor info 
			// https://stackoverflow.com/questions/42245870/how-to-get-the-graphics-card-model-name-in-opengl-or-win32
		}

		private static void OnUpdateFrame(FrameEventArgs Args)
		{
			
		}

		private static void OnRenderFrame(FrameEventArgs Args)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);

			GL.UseProgram(Program.ID);


			// Set uniforms
			GL.Uniform1(SaturationUniform, 1.0f);

			Matrix4 OrthographicProjectionMatrix = Matrix4.CreateOrthographic(OrthographicScale, OrthographicScale, 0f, FarClipPlaneDistance);
			GL.UniformMatrix4(OrthographicProjectionUniform, false, ref OrthographicProjectionMatrix);


			// Creating and deleting VAOs and stuff every frame is inefficient, take action immediately
			int VAO = GL.GenVertexArray();
			int Vertices = GL.GenBuffer();
			int VertexColors = GL.GenBuffer();

			GL.BindVertexArray(VAO);

			GL.BindBuffer(BufferTarget.ArrayBuffer, Vertices);
			GL.BufferData(BufferTarget.ArrayBuffer, Verts.Length * sizeof(float), Verts, BufferUsageHint.DynamicDraw);
			GL.EnableVertexAttribArray(0);																					// Let shaders access this vertex array at layout position 0
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);                                       // Specify the number of values it can take and the data type

			GL.BindBuffer(BufferTarget.ArrayBuffer, VertexColors);
			GL.BufferData(BufferTarget.ArrayBuffer, VertColors.Length * sizeof(float), VertColors, BufferUsageHint.DynamicCopy);
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);

			GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

			Editor.DrawUI();

			// Unbindings
			GL.BindVertexArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.DeleteBuffer(Vertices);

			GL.BindVertexArray(1);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 1);
			GL.DeleteBuffer(VertexColors);

			GL.DeleteVertexArray(VAO);



			Window.SwapBuffers();
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
