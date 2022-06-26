#version 410

layout(location = 0) in vec3 VertexPosition;
layout(location = 1) in vec3 VertexColor;

out vec3 OutVertexColor;

uniform mat4 ProjectionMatrix;

void main()
{
	vec4 OutVertexPosition = vec4(VertexPosition, 1.0);
	OutVertexColor = VertexColor;
	
	OutVertexPosition *= ProjectionMatrix;

	gl_Position = OutVertexPosition;
}