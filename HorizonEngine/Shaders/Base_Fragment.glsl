#version 410

in vec3 OutVertexColor;

out vec4 FinalColor;

uniform float Saturation = 1.0;

void main()
{
	vec3 Color = mix(vec3(1.0), OutVertexColor, Saturation);


	FinalColor = vec4(Color, 1.0);
}