#version 410

layout(location = 0) in vec3 VertexPosition;

uniform mat4 ModelMatrix;
uniform mat4 ProjectionMatrix;
uniform mat4 ViewMatrix;

void main()
{
	vec4 OutVertexPosition = vec4(VertexPosition, 1.0);
	
	OutVertexPosition = (ProjectionMatrix * ViewMatrix * ModelMatrix) * OutVertexPosition;

	gl_Position = OutVertexPosition;
}